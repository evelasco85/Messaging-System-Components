using Messaging.Orchestration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Orchestration.Shared.Services
{
    public enum ClientCommandStatus
    {
        Inactive = 0,
        InvalidRegistration,
        SetupClientParameters,
        Standby,
        Start,
        Stop
    }

    public class ServerResponse
    {
        public QueueTypeEnum QueueType { get; set; }
        public Guid ID { get; set; }
        public ClientCommandStatus ClientStatus { get; set; }
        public IDictionary<string, object> ClientParameters { get; set; }
        public string Message { get; set; }
    }

    public interface IClientService_ParameterRegistration
    {
        void RegisterRequiredServerParameters(string name, Action<object> setValueOperator);
    }

    public interface IClientService : IClientService_ParameterRegistration
    {

    }

    public class ClientService : IClientService
    {
        Guid _clientId;
        QueueTypeEnum _queueType;
        Action<string> _invalidRegistrationSequence;
        Action _standbySequence;
        Action _startSequence;
        Action _stopSequence;

        IDictionary<string, Action<object>> _serverParameterRequests = new Dictionary<string, Action<object>>();

        public void RegisterClient<TVersion, TObject>(
            Guid clientId, QueueTypeEnum queueType,
            Action<IClientService_ParameterRegistration> registerRequiredServerParametersSequence,
            Action<string> invalidRegistrationSequence,
            Action standbySequence,
            Action startSequence,
            Action stopSequence)
        {
            _clientId = clientId;
            _queueType = queueType;
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
            IDictionary<string, Action<object>> serverParameters)
        {
            Guid clientId = _clientId;
            QueueTypeEnum queueType = _queueType;

            //Send registration request
        }

        void ReceiveServerCommand(ServerResponse response)
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

        void SetupClientParameters(IDictionary<string, Action<object>> serverParameterRequests, IDictionary<string, object> clientParameters)
        {
            if ((serverParameterRequests == null) || (clientParameters == null))
                return;

            foreach(KeyValuePair<string, Action<object>> kvp in serverParameterRequests)
            {
                if ((kvp.Value != null) && (clientParameters.ContainsKey(kvp.Key)))
                    kvp.Value(clientParameters[kvp.Key]);
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
