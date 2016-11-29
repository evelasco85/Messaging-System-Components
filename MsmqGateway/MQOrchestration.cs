using MessageGateway;
using Messaging.Orchestration.Shared.Models;
using Messaging.Orchestration.Shared.Services;
using Messaging.Orchestration.Shared.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace MsmqGateway
{
    public interface IMQOrchestration
    {
        IClientService CreateClient(string clientId, string groupId, string serverRequestQueue, string serverReplyQueue);
        IServerService<MessageQueue, Message> CreateServer(string serverRequestQueue, string serverReplyQueue);
        ServerRequestConverterDelegate<Message> CreateServerRequestConverter();
        SendResponseDelegate<MessageQueue, Message> CreateServerSendResponse();
    }

    public class MQOrchestration : IMQOrchestration
    {
        static IMQOrchestration s_instance = new MQOrchestration();

        private MQOrchestration()
        {
        }

        public static IMQOrchestration GetInstance()
        {
            return s_instance;
        }

        public IClientService CreateClient(string clientId, string groupId, string serverRequestQueue, string serverReplyQueue)
        {
            IClientService client = new ClientService<MessageQueue, Message>(
                clientId,
                groupId,
                new MessageSenderGateway(serverRequestQueue),
                new MQSelectiveConsumer(
                    serverReplyQueue,
                    new XmlMessageFormatter(new Type[] { typeof(ServerMessage) }),
                    clientId),
                (sender, request) => //Concrete sender implementation
                {
                    sender.Send(new Message(request));
                },
                message => //Concrete receiver implementation
                {
                    ServerMessage response = null;

                    if (message.Body is ServerMessage)
                        response = (ServerMessage)message.Body;

                    return response;
                });

            return client;
        }

        

        public IServerService<MessageQueue, Message> CreateServer(string serverRequestQueue, string serverReplyQueue)
        {
            IServerService<MessageQueue, Message> server = new ServerService<MessageQueue, Message>
                (
                new MessageReceiverGateway(
                    serverRequestQueue,
                    new XmlMessageFormatter(new Type[] { typeof(ServerRequest) })),
                new MessageSenderGateway(serverReplyQueue));

            return server;
        }

        public ServerRequestConverterDelegate<Message> CreateServerRequestConverter()
        {
            ServerRequestConverterDelegate<Message> converter = (message) =>
            {
                ServerRequest request = null;

                if (message.Body is ServerRequest)
                    request = (ServerRequest)message.Body;

                return request;
            };

            return converter;
        }

        public SendResponseDelegate<MessageQueue, Message> CreateServerSendResponse()
        {
            SendResponseDelegate<MessageQueue, Message> sendResponse = (sender, response) =>
            {
                Message message = new Message(response);

                message.CorrelationId = response.ClientId;

                sender.Send(message);
            };

            return sendResponse;
        }
    }
}
