using System;
using System.Collections.Generic;
using System.Linq;
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
                    new XmlMessageFormatter(new Type[] {typeof(ServerMessage)}),
                    clientId.ToString()),
                (sender, request) => //Concrete sender implementation
                {
                    sender.Send(new Message(request));
                },
                message => //Concrete receiver implementation
                {
                    ServerMessage response = null;

                    if (message.Body is ServerMessage)
                        response = (ServerMessage) message.Body;

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

            IServerService<MessageQueue, Message> server = new ServerService<MessageQueue, Message>
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
                    ServerMessage response = null;

                    if (request == null)
                        return response;

                    switch (request.RequestType)
                    {
                        case ServerRequestType.Register:
                            IDictionary<string, object> paramList = request
                                .ParameterList
                                .Select(param => new KeyValuePair<string, object>(param, "hello world"))
                                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                            response = new ServerMessage
                            {
                                ClientId = request.ClientId,
                                ClientStatus = ClientCommandStatus.Standby,
                                ClientParameters = paramList
                            };
                            break;
                    }


                    return response;
                },
                (sender, response) =>
                {
                    Message message = new Message(response);

                    //MsmqMessage<ServerMessage> message = new MsmqMessage<ServerMessage>(response);
                    string corrId = response.ClientId.ToString();
                    message.CorrelationId = corrId;

                    sender.Send(message);
                });

            server.Process();
        }
    }
}
