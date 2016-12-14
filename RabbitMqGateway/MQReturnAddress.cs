using Messaging.Base;
using Messaging.Base.Constructions;
using RabbitMqGateway.Core;
using RabbitMQ.Client;

namespace RabbitMqGateway
{
    public class MQReturnAddress : ReturnAddress<IModel, Message>
    {
        public MQReturnAddress(IMessageCore<IModel> messageReplyQueue)
            : base(messageReplyQueue,
                (IModel queue, ref Message message) =>
                {
                    message.ReplyTo = messageReplyQueue.QueueName;
                })
        {
        }
    }
}
