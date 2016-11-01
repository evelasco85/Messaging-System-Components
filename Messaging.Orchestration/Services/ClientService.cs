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
    public interface IClientService_ParameterRegistration
    {
        void RegisterRequiredServerParameters(string name, Action<object> setValueOperator);
    }

    public interface IClientService : IClientService_ParameterRegistration
    {
        void Process();

        void Register(
            Action<IClientService_ParameterRegistration> registerRequiredServerParametersSequence,
            Action<string> invalidRegistrationSequence,
            Action standbySequence,
            Action startSequence,
            Action stopSequence);
    }

    public class ClientService<TMessageQueue, TMessage> : IClientService
    {
        string _clientId;
        Action<string> _invalidRegistrationSequence;
        Action _standbySequence;
        Action _startSequence;
        Action _stopSequence;

        IDictionary<string, Action<object>> _serverParameterRequests = new Dictionary<string, Action<object>>();
        IMessageSender<TMessageQueue, TMessage> _serverRequestSender;
        private IMessageReceiver<TMessageQueue, TMessage> _serverReplyReceiver;
        private Func<TMessage, ServerMessage> _serverResponseConverter;
        private Action<IMessageSender<TMessageQueue, TMessage>, ServerRequest> _sendRequest;

        public ClientService(
            string clientId, 
            IMessageSender<TMessageQueue, TMessage> serverRequestSender,
            IMessageReceiver<TMessageQueue, TMessage> serverReplyReceiver,
            Action<IMessageSender<TMessageQueue, TMessage>, ServerRequest> sendRequest,
            Func<TMessage, ServerMessage> serverResponseConverter
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

        void ProcessMessage(TMessage message)
        {
            if(_serverResponseConverter == null)
                return;

            ServerMessage response = _serverResponseConverter(message);

            if (response != null)
                ReceiveServerCommand(response);
        }

        public void Register(
            Action<IClientService_ParameterRegistration> registerRequiredServerParametersSequence,
            Action<string> invalidRegistrationSequence,
            Action standbySequence,
            Action startSequence,
            Action stopSequence)
        {
            _invalidRegistrationSequence = invalidRegistrationSequence;
            _standbySequence = standbySequence;
            _startSequence = startSequence;
            _stopSequence = stopSequence;

            if (registerRequiredServerParametersSequence != null)
            {
                registerRequiredServerParametersSequence(this);
                PerformClientRegistration(_serverParameterRequests);
            }
        }

        
        public void RegisterRequiredServerParameters(string name, Action<object> setValueOperator)
        {
            if (!_serverParameterRequests.ContainsKey(name))
                _serverParameterRequests.Add(name, setValueOperator);
        }

        
        void PerformClientRegistration(
            IDictionary<string, Action<object>> serverParametersRequest)
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
                    break;
                case ClientCommandStatus.Standby:
                    SafeInvokeMethod(_standbySequence);
                    break;
                case ClientCommandStatus.Start:
                    SafeInvokeMethod(_startSequence);
                    break;
                case ClientCommandStatus.Stop:
                    SafeInvokeMethod(_stopSequence);
                    break;
            }
        }

        void SetupClientParameters(IDictionary<string, Action<object>> serverParametersRequest, List<ParameterEntry> clientParameters)
        {
            if ((serverParametersRequest == null) || (clientParameters == null))
                return;

            foreach(KeyValuePair<string, Action<object>> kvp in serverParametersRequest)
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
        void SafeInvokeMethod(Action actionToCall)
        {
            if (actionToCall != null)
                actionToCall();
        }
    }
}
