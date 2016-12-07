using System.Messaging;
using Messaging.Base;
using Messaging.Base.Constructions;

namespace MsmqGateway
{
    public class MQReturnAddress : ReturnAddress<MessageQueue, Message>
    {
        public MQReturnAddress(IMessageCore<MessageQueue> messageReplyQueue) : base(messageReplyQueue,
            (MessageQueue queue, ref Message message) =>
            {
                message.ResponseQueue = queue;
            })
        {
        }
    }
}
