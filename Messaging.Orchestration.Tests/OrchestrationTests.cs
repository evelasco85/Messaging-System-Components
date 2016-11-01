using System;
using System.Messaging;
using MessageGateway;
using Messaging.Orchestration.Shared.Models;
using Messaging.Orchestration.Shared.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MsmqGateway;

namespace Messaging.Orchestration.Tests
{
    [TestClass]
    public class OrchestrationTests
    {
        String ToPath(String arg)
        {
            return ".\\private$\\" + arg;
        }

        [TestMethod]
        public void TestMethod1()
        {
            string requestQueue = "ServerRequestQueue";
            string replyQueue = "ServerReplyQueue";
            Guid clientId = Guid.Parse("1c4054c1-6ae4-4a3c-b540-55d768988994");
            IClientService client = new ClientService<MessageQueue, Message>(
                clientId,
                new MessageSenderGateway(ToPath(requestQueue)),
                new MQSelectiveConsumer(
                    ToPath(replyQueue),
                    new XmlMessageFormatter(new Type[] {typeof(ServerResponse)}),
                    clientId.ToString()),
                (sender, request) => //Concrete sender implementation
                {
                    sender.Send(new Message(request));
                },
                message => //Concrete receiver implementation
                {
                    ServerResponse response = null;

                    if (message.Body is ServerResponse)
                        response = (ServerResponse) message.Body;

                    return response;
                });


            client.Register(registration =>
            {
                //Server parameter requests
                registration.RegisterRequiredServerParameters("name", null);
            },
                errorMessage =>
                {
                    //Invalid registration
                },
                () =>
                {
                    //Stand-by
                },
                () =>
                {
                    //Start
                },
                () =>
                {
                    //Stop
                });

            ServerService<MessageQueue, Message> server = new ServerService<MessageQueue, Message>
                (
                new MessageReceiverGateway(
                    ToPath(requestQueue),
                    new XmlMessageFormatter(new Type[] {typeof(ServerRequest)})),
                new MessageSenderGateway(ToPath(replyQueue)),
                message =>
                {
                    ServerRequest request = null;

                    if (message.Body is ServerRequest)
                        request = (ServerRequest) message.Body;

                    return request;
                },
                request =>
                {
                    ServerResponse response = null;

                    if (request != null)
                    {
                        response = new ServerResponse
                        {
                            ClientId = request.ClientId
                        };
                    }

                    return response;
                },
                (sender, response) =>
                {
                    sender.Send(new Message(response));
                });

            server.Process();
        }
    }
}
