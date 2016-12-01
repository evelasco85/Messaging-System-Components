using Messaging.Base;
using Messaging.Orchestration.Shared.Models;

namespace Messaging.Orchestration.Shared.Services
{
    public delegate ServerMessage ConvertToServerResponseDelegate<TMessage>(TMessage message);
    public delegate void SendRequestToServerDelegate<TMessage>(IMessageSender<TMessage> sender, ServerRequest request);
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

    public interface IClientServiceSetup
    {
        IClientService Register(
            RegisterRequiredServerParametersDelegate registerRequiredServerParametersSequence,
            InvalidRegistrationDelegate invalidRegistrationSequence,
            ClientParameterSetupCompleteDelegate clientParameterSetupCompleteSequence,
            StandByDelegate standbySequence,
            StartDelegate startSequence,
            StopDelegate stopSequence
            );
    }

    public interface IClientService
    {
        void StartService();
        
        void StopService();
    }
}
