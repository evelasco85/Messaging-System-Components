using Messaging.Orchestration.Shared.Models;
using Messaging.Orchestration.Shared.Services;
using Messaging.Orchestration.Shared.Services.Interfaces;
using Messaging.Base;

namespace Orchestration
{
    public interface IBaseOrchestrationRequestResponse<TMessage>
    {
        ServerRequestConverterDelegate<TMessage> CreateServerRequestConverter();
        SendResponseDelegate<TMessage> CreateServerSendResponse();
    }

    public interface IBaseOrchestration<TMessage> : IBaseOrchestrationRequestResponse<TMessage>
    {
        IClientServiceSetup CreateClient(
            string clientId, string groupId,
            IMessageSender<TMessage> serverRequestQueue,
            IMessageReceiver<TMessage> serverReplyQueue
            );
        ServerMessage ConstructServerMessage(TMessage message);
        IServerService<TMessage> CreateServer(IMessageReceiver<TMessage> serverRequestQueue, IMessageSender<TMessage> serverReplyQueue);
        ServerRequest ConstrucServerRequest(TMessage message);
        void ConstructResponseSender(IMessageSender<TMessage> sender, ServerMessage response);
    }

    public abstract class BaseOrchestration<TMessage> : IBaseOrchestration<TMessage>
    {
        public IClientServiceSetup CreateClient(
            string clientId, string groupId,
            IMessageSender<TMessage> serverRequestQueue,
            IMessageReceiver<TMessage> serverReplyQueue
            )
        {
            IClientServiceSetup client = new ClientService<TMessage>(
                clientId,
                groupId,
                serverRequestQueue,
                serverReplyQueue,
                ConstructServerMessage//Concrete receiver implementation
                );

            return client;
        }

        public abstract ServerMessage ConstructServerMessage(TMessage message);


        public IServerService<TMessage> CreateServer(IMessageReceiver<TMessage>  serverRequestQueue, IMessageSender<TMessage> serverReplyQueue)
        {
            return new ServerService<TMessage>(serverRequestQueue, serverReplyQueue);
        }

        public ServerRequestConverterDelegate<TMessage> CreateServerRequestConverter()
        {
            return ConstrucServerRequest;
        }

        public abstract ServerRequest ConstrucServerRequest(TMessage message);

        public SendResponseDelegate<TMessage> CreateServerSendResponse()
        {
            return ConstructResponseSender;
        }

        public abstract void ConstructResponseSender(IMessageSender<TMessage> sender, ServerMessage response);
    }
}
