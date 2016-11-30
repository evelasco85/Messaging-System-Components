using Messaging.Base;
using Messaging.Base.System_Management.SmartProxy;
using Messaging.Orchestration.Shared.Models;

namespace Messaging.Orchestration.Shared.Services.Interfaces
{
    public delegate ServerMessage ProcessRequestDelegate(ServerRequest request);
    public delegate void SendResponseDelegate<TMessage>(IMessageSender<TMessage> sender, ServerMessage message);
    public delegate ServerRequest ServerRequestConverterDelegate<TMessage>(TMessage message);

    public interface IServerService<TMessage> : IMessageConsumer<TMessage>
    {
        void Register(
            ServerRequestConverterDelegate<TMessage> serverRequestConverter,
            SendResponseDelegate<TMessage> sendResponse,
            ProcessRequestDelegate processRequest
            );
        void SendClientMessage(ServerMessage serverMessage);
    }
}
