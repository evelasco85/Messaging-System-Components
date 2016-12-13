using Messaging.Base;
using Messaging.Base.Constructions;
using RabbitMqGateway.Core;
using RabbitMQ.Client;

namespace RabbitMqGateway
{
    public class MQReturnAddress : ReturnAddress<IModel, RQMessage>
    {
        public MQReturnAddress(IMessageCore<IModel> messageReplyQueue)
            : base(messageReplyQueue,
                (IModel queue, ref RQMessage message) =>
                {
                    IBasicProperties properties = queue.CreateBasicProperties();

                    properties.ReplyTo = messageReplyQueue.QueueName;
                })
        {
        }
    }
}
