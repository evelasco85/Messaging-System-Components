using System;
using System.Messaging;
using MessageGateway;
using Messaging.Orchestration.Shared.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MsmqGateway;

namespace Messaging.Orchestration.Tests
{
    [TestClass]
    public class OrchestrationTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            Guid clientId = Guid.Parse("1c4054c1-6ae4-4a3c-b540-55d768988994");
            IClientService client = new ClientService<MessageQueue, Message>(
                clientId,
                new MessageSenderGateway(".\\private$\\ServerRequestQueue"),
                new MQSelectiveConsumer(
                    ".\\private$\\ServerReplyQueue",
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
                registration.RegisterRequiredServerParameters("name", null);
            },
                errorMessage =>
                {
                },
                () =>
                {
                },
                () => { },
                () => { }
                );
        }
    }
}
