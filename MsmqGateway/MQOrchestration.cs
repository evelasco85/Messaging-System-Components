using Messaging.Orchestration.Shared.Models;
using Messaging.Orchestration.Shared.Services;
using Messaging.Orchestration.Shared.Services.Interfaces;
using System;
using System.Messaging;
using MsmqGateway.Core;
using Messaging.Base;
using Orchestration;

namespace MsmqGateway
{
    public interface IMQOrchestration : IBaseOrchestrationRequestResponse<Message>
    {
        IClientServiceSetup CreateClient(string clientId, string groupId, string serverRequestQueue, string serverReplyQueue);
        IServerService<Message> CreateServer(string serverRequestQueue, string serverReplyQueue);
    }

    public class MQOrchestration : BaseOrchestration<Message>, IMQOrchestration
    {
        static IMQOrchestration s_instance = new MQOrchestration();

        private MQOrchestration()
        {
        }

        public static IMQOrchestration GetInstance()
        {
            return s_instance;
        }

        public IClientServiceSetup CreateClient(string clientId, string groupId, string serverRequestQueue,
            string serverReplyQueue)
        {
            return CreateClient(
                clientId,
                groupId,
                new MessageSenderGateway(serverRequestQueue),
                new MQSelectiveConsumer<ServerMessage>(
                    serverReplyQueue,
                    clientId)
                );
        }

        public override ServerMessage ConstructServerMessage(Message message)
        {
            ServerMessage response = null;

            if (message.Body is ServerMessage)
                response = (ServerMessage) message.Body;

            return response;
        }

        public IServerService<Message> CreateServer(string serverRequestQueue, string serverReplyQueue)
        {
            return CreateServer(
                new MessageReceiverGateway(
                    serverRequestQueue,
                    new XmlMessageFormatter(new Type[] {typeof(ServerRequest)})),
                new MessageSenderGateway(serverReplyQueue));
        }

        public override ServerRequest ConstrucServerRequest(Message message)
        {
            ServerRequest request = null;

            if (message.Body is ServerRequest)
                request = (ServerRequest) message.Body;

            return request;
        }

        public override void ConstructResponseSender(IMessageSender<Message> sender, ServerMessage response)
        {
            Message message = new Message(response);

            message.CorrelationId = response.ClientId;

            sender.Send(message);
        }
    }
}
