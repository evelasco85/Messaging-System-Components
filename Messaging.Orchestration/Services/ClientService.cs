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
        GetRegistrationInfo,
        InvalidRegistration,
        Standby,
        Start,
        Stop
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
        ClientCommandStatus _currentClientStatus = ClientCommandStatus.Inactive;
        Action _invalidRegistrationSequence;
        Action _standbySequence;
        Action _startSequence;
        Action _stopSequence;

        IDictionary<string, Action<object>> _serverParameters = new Dictionary<string, Action<object>>();

        public void RegisterClient<TVersion, TObject>(
            QueueTypeEnum queueType, Guid id, TVersion version,
            Action<IClientService_ParameterRegistration> registerRequiredServerParametersSequence,
            Action invalidRegistrationSequence,
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
                PerformClientRegistration<TVersion, TObject>(queueType, id, version, _serverParameters);
            }
        }

        
        public void RegisterRequiredServerParameters(string name, Action<object> setValueOperator)
        {
            if (!_serverParameters.ContainsKey(name))
                _serverParameters.Add(name, setValueOperator);
        }

        
        void PerformClientRegistration<TVersion, TObject>(
            QueueTypeEnum queueType, Guid id, TVersion version,
            IDictionary<string, Action<object>> serverParameters)
        {
        }

        void SendServerCommand()
        {
        }

        void ReceiveServerCommand(ClientCommandStatus command)
        {
            _currentClientStatus = command;

            switch(command)
            {
                case ClientCommandStatus.InvalidRegistration:
                    SafeInvokeMethod(_invalidRegistrationSequence);
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

        void SafeInvokeMethod(Action actionToCall)
        {
            if (actionToCall != null)
                actionToCall();
        }
    }
}
