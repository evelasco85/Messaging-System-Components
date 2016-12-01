using System.Collections.Generic;
using System.Linq;
using Messaging.Base;
using Messaging.Orchestration.Shared.Models;

namespace Messaging.Orchestration.Shared.Services
{
    public class ClientService<TMessage> : IClientService, IClientServiceSetup, IClientService_ParameterRegistration
    {
        private ClientCommandStatus _lastClientStatus = ClientCommandStatus.Inactive;
        string _clientId;
        private string _groupId;
        InvalidRegistrationDelegate _invalidRegistrationSequence;
        StandByDelegate _standbySequence;
        StartDelegate _startSequence;
        StopDelegate _stopSequence;

        IDictionary<string, SetParameterDelegate> _serverParameterRequests = new Dictionary<string, SetParameterDelegate>();
        IMessageSender<TMessage> _serverRequestSender;
        private IMessageReceiver<TMessage> _serverReplyReceiver;

        private ConvertToServerResponseDelegate<TMessage> _serverResponseConverter;
        ClientParameterSetupCompleteDelegate _clientParameterSetupComplete;

        public ClientService(
            string clientId,
            string groupId,
            IMessageSender<TMessage> serverRequestSender,
            IMessageReceiver<TMessage> serverReplyReceiver,
            ConvertToServerResponseDelegate<TMessage> serverResponseConverter
            )
        {
            serverReplyReceiver.ReceiveMessageProcessor += ProcessMessage;

            _clientId = clientId;
            _groupId = groupId;
            _serverReplyReceiver = serverReplyReceiver;
            _serverRequestSender = serverRequestSender;
            _serverResponseConverter = serverResponseConverter;
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

        public IClientService Register(
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

            return this;
        }

        
        public void RegisterRequiredServerParameters(string name, SetParameterDelegate setValueOperator)
        {
            if (!_serverParameterRequests.ContainsKey(name))
                _serverParameterRequests.Add(name, setValueOperator);
        }

        void PerformClientRegistration(
            IDictionary<string, SetParameterDelegate> serverParametersRequest)
        {
            ServerRequest request = new ServerRequest
            {
                ClientId = _clientId,
                GroupId = _groupId,
                RequestType = ServerRequestType.Register
            };

            if (serverParametersRequest != null)
            {
                request.ParameterList = serverParametersRequest
                    .Select(kvp => kvp.Key)
                    .ToList();
            }

            _serverRequestSender.Send(request);
        }

        void ReceiveServerCommand(ServerMessage response)
        {
            ClientCommandStatus clientStatus = response.ClientStatus;

            switch(clientStatus)
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

            _lastClientStatus = clientStatus;
        }

        void SetupClientParameters(IDictionary<string, SetParameterDelegate> serverParametersRequest, List<ParameterEntry> clientParameters)
        {
            if ((serverParametersRequest == null) || (clientParameters == null))
                return;

            foreach(KeyValuePair<string, SetParameterDelegate> kvp in serverParametersRequest)
            {
                ParameterEntry entry = clientParameters
                    .DefaultIfEmpty(null)
                    .FirstOrDefault(param => (param != null) && (param.Name == kvp.Key));

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
