using Messaging.Orchestration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Messaging.Base;
using Messaging.Orchestration.Shared.Models;

namespace Messaging.Orchestration.Shared.Services
{
    public class ClientService<TMessageQueue, TMessage> : IClientService
    {
        string _clientId;
        InvalidRegistrationDelegate _invalidRegistrationSequence;
        StandByDelegate _standbySequence;
        StartDelegate _startSequence;
        StopDelegate _stopSequence;

        IDictionary<string, SetParameterDelegate> _serverParameterRequests = new Dictionary<string, SetParameterDelegate>();
        IMessageSender<TMessageQueue, TMessage> _serverRequestSender;
        private IMessageReceiver<TMessageQueue, TMessage> _serverReplyReceiver;


        private ConvertToServerResponseDelegate<TMessage> _serverResponseConverter;
        private SendRequestToServerDelegate<TMessageQueue, TMessage> _sendRequest;
        ClientParameterSetupCompleteDelegate _clientParameterSetupComplete;

        public ClientService(
            string clientId, 
            IMessageSender<TMessageQueue, TMessage> serverRequestSender,
            IMessageReceiver<TMessageQueue, TMessage> serverReplyReceiver,
            SendRequestToServerDelegate<TMessageQueue, TMessage> sendRequest,
            ConvertToServerResponseDelegate<TMessage> serverResponseConverter
            )
        {
            serverReplyReceiver.ReceiveMessageProcessor += ProcessMessage;

            _clientId = clientId;
            _serverReplyReceiver = serverReplyReceiver;
            _serverRequestSender = serverRequestSender;
            _serverResponseConverter = serverResponseConverter;
            _sendRequest = sendRequest;
        }

        public void Process()
        {
            _serverReplyReceiver.StartReceivingMessages();
        }

        public void StopReceivingMessages()
        {
            _serverReplyReceiver.StopReceivingMessages();
        }

        void ProcessMessage(TMessage message)
        {
            if(_serverResponseConverter == null)
                return;

            ServerMessage response = _serverResponseConverter(message);

            if (response != null)
                ReceiveServerCommand(response);
        }

        public void Register(
            RegisterRequiredServerParametersDelegate registerRequiredServerParametersSequence,
            InvalidRegistrationDelegate invalidRegistrationSequence,
            ClientParameterSetupCompleteDelegate clientParameterSetupCompleteSequence,
            StandByDelegate standbySequence,
            StartDelegate startSequence,
            StopDelegate stopSequence)
        {
            _invalidRegistrationSequence = invalidRegistrationSequence;
            _clientParameterSetupComplete = clientParameterSetupCompleteSequence;
            _standbySequence = standbySequence;
            _startSequence = startSequence;
            _stopSequence = stopSequence;

            if (registerRequiredServerParametersSequence != null)
            {
                registerRequiredServerParametersSequence(this);
                PerformClientRegistration(_serverParameterRequests);
            }
        }

        
        public void RegisterRequiredServerParameters(string name, SetParameterDelegate setValueOperator)
        {
            if (!_serverParameterRequests.ContainsKey(name))
                _serverParameterRequests.Add(name, setValueOperator);
        }

        
        void PerformClientRegistration(
            IDictionary<string, SetParameterDelegate> serverParametersRequest)
        {
            if (_sendRequest == null)
                return;

            ServerRequest request = new ServerRequest
            {
                ClientId = _clientId,
                RequestType = ServerRequestType.Register
            };

            if (serverParametersRequest != null)
            {
                request.ParameterList = serverParametersRequest
                    .Select(kvp => kvp.Key)
                    .ToList();
            }

            _sendRequest(_serverRequestSender, request);
        }

        void ReceiveServerCommand(ServerMessage response)
        {
            switch(response.ClientStatus)
            {
                case ClientCommandStatus.InvalidRegistration:
                    InvokeInvalidRegistration(response.Message);
                    break;
                case ClientCommandStatus.SetupClientParameters:
                    SetupClientParameters(_serverParameterRequests, response.ClientParameters);

                    if (_clientParameterSetupComplete != null)
                        _clientParameterSetupComplete();
                    break;
                case ClientCommandStatus.Standby:
                    if (_standbySequence != null)
                        _standbySequence();
                    break;
                case ClientCommandStatus.Start:
                    if (_startSequence != null)
                        _startSequence();
                    break;
                case ClientCommandStatus.Stop:
                    if (_stopSequence != null)
                        _stopSequence();
                    break;
            }
        }

        void SetupClientParameters(IDictionary<string, SetParameterDelegate> serverParametersRequest, List<ParameterEntry> clientParameters)
        {
            if ((serverParametersRequest == null) || (clientParameters == null))
                return;

            foreach(KeyValuePair<string, SetParameterDelegate> kvp in serverParametersRequest)
            {
                ParameterEntry entry = clientParameters
                    .DefaultIfEmpty(null)
                    .FirstOrDefault(param => param.Name == kvp.Key);

                if ((kvp.Value != null) && (entry != null))
                    kvp.Value(entry.Value);
            }
        }

        void InvokeInvalidRegistration(string serverMessage)
        {
            if (_invalidRegistrationSequence != null)
                _invalidRegistrationSequence(serverMessage);
        }
    }
}
