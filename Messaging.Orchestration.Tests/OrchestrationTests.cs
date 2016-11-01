using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using MessageGateway;
using Messaging.Orchestration.Shared.Models;
using Messaging.Orchestration.Shared.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MsmqGateway;
using Messaging.Orchestration.Shared.Services.Interfaces;
using System.Threading;

namespace Messaging.Orchestration.Tests
{
    [TestClass]
    public class OrchestrationTests
    {
        String ToPath(String arg)
        {
            return ".\\private$\\" + arg;
        }

        IClientService client;
        IServerService<MessageQueue, Message> server;

        [TestMethod]
        public void TestMethod1()
        {
            string requestQueue = "ServerRequestQueue";
            string replyQueue = "ServerReplyQueue";
            string clientId = @"1c4054c1-6ae4-4a3c-b540-55d768988994\123";
            string nameValue = string.Empty;

            client = new ClientService<MessageQueue, Message>(
                clientId,
                new MessageSenderGateway(ToPath(requestQueue)),
                new MQSelectiveConsumer(
                    ToPath(replyQueue),
                    new XmlMessageFormatter(new Type[] {typeof(ServerMessage)}),
                    clientId),
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
                registration.RegisterRequiredServerParameters("name", (value) => nameValue = (string)value);
            },
                errorMessage =>
                {
                    //Invalid registration
                },
                () =>
                {
                    //Client parameter setup completed
                    Assert.AreEqual("Albert", nameValue);

                    server.SendClientMessage(new ServerMessage
                    {
                        ClientId = clientId,
                        ClientStatus = ClientCommandStatus.Standby
                    });
                },
                () =>
                {
                    //Stand-by
                    Assert.IsTrue(true);

                    server.SendClientMessage(new ServerMessage
                    {
                        ClientId = clientId,
                        ClientStatus = ClientCommandStatus.Stop
                    });
                },
                () =>
                {
                    //Start
                },
                () =>
                {
                    //Stop
                    //Force stopping this process
                    client.StopReceivingMessages();
                    server.StopProcessing();

                    //Thread.CurrentThread.Join();
                });

            server = new ServerService<MessageQueue, Message>
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
                            try
                            {
                                List<ParameterEntry> paramList = request
                                .ParameterList
                               .Select(param => new ParameterEntry
                               {
                                   Name = param,
                                   Value = "Albert"
                               })
                               .ToList();

                                response = new ServerMessage
                                {
                                    ClientId = request.ClientId,
                                    ClientStatus = ClientCommandStatus.SetupClientParameters,
                                    ClientParameters = paramList
                                };
                            }
                            catch (Exception ex)
                            { throw ex; }
                            break;
                    }

                    return response;
                },
                (sender, response) =>
                {
                    Message message = new Message(response);

                    message.CorrelationId = response.ClientId;

                    sender.Send(message);
                });

            server.Process();
            client.Process();            
        }
    }
}
