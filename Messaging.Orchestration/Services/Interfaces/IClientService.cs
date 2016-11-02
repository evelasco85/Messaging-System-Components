using Messaging.Base;
using Messaging.Orchestration.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Orchestration.Shared.Services
{
    public delegate ServerMessage ConvertToServerResponseDelegate<TMessage>(TMessage message);
    public delegate void SendRequestToServerDelegate<TMessageQueue, TMessage>(IMessageSender<TMessageQueue, TMessage> sender, ServerRequest request);
    public delegate object SetParameterDelegate(object value);
    public delegate void InvalidRegistrationDelegate(string errorMessage);
    public delegate void RegisterRequiredServerParametersDelegate(IClientService_ParameterRegistration registration);
    public delegate void ClientParameterSetupCompleteDelegate();
    public delegate void StandByDelegate();
    public delegate void StartDelegate();
    public delegate void StopDelegate();

    public interface IClientService_ParameterRegistration
    {
        void RegisterRequiredServerParameters(string name, SetParameterDelegate setValueOperator);
    }

    public interface IClientService : IClientService_ParameterRegistration
    {
        void Process();

        void Register(
            RegisterRequiredServerParametersDelegate registerRequiredServerParametersSequence,
            InvalidRegistrationDelegate invalidRegistrationSequence,
            ClientParameterSetupCompleteDelegate clientParameterSetupCompleteSequence,
            StandByDelegate standbySequence,
            StartDelegate startSequence,
            StopDelegate stopSequence
            );
        void StopReceivingMessages();
    }
}
