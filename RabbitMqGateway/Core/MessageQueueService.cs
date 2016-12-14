using System;
using Messaging.Base;
using RabbitMQ.Client;

namespace RabbitMqGateway.Core
{
    public class MessageQueueService : QueueService<IModel, Message>
    {
        static protected readonly String InvalidMessageQueueName = "invalidMessageQueue";

        protected Type requestBodyType;

        private ConnectionFactory _factory;

        public MessageQueueService(ConnectionFactory factory, IMessageReceiver<IModel, Message> receiver)
            : base(receiver, new MessageSenderGateway(new MessageQueueGateway(factory, InvalidMessageQueueName)))
        {
            _factory = factory;
        }

        public override void SendReply(Object responseObject, Message originalRequestMessage)
        {
            if (originalRequestMessage.ReplyTo != null)
            {
                IMessageSender<IModel, Message> replyQueue = new MessageSenderGateway(new MessageQueueGateway(_factory, originalRequestMessage.ReplyTo));

                replyQueue.Send(responseObject,
                    (assignApplicationId, assignCorrelationId) =>
                    {
                        assignApplicationId(originalRequestMessage.AppSpecific.ToString());
                        assignCorrelationId(originalRequestMessage.Id);
                    });
            }
            else
            {
                this.InvalidQueue.Send(responseObject,
                    (assignApplicationId, assignCorrelationId) =>
                    {
                        assignApplicationId(originalRequestMessage.AppSpecific.ToString());
                        assignCorrelationId(originalRequestMessage.Id);
                    });
            }
        }
    }
}
