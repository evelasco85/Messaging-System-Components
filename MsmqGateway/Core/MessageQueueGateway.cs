using System.Messaging;
using System;
using Messaging.Base;

namespace MsmqGateway.Core
{
    public class MessageQueueGateway : QueueGateway<MessageQueue>, IDisposable
    {
        MessageQueue msgQueue;

        public override string QueueName
        {
            get { return msgQueue.QueueName; }
        }

        public void Dispose()
        {
            if (msgQueue != null)
            {
                msgQueue.Dispose();
                msgQueue = null;
            }
        }

        public MessageQueueGateway(String q)
            : this(FindQueue(q))
		{
		}
		
		public MessageQueueGateway(MessageQueue queue)
		{
            SetQueue(queue);
		}
		
        
		public override MessageQueue GetQueue(){
			return msgQueue;
		}

        public override void SetQueue(MessageQueue queue)
        {
            msgQueue = queue;
        }

        private static MessageQueue FindQueue(String q)
		{
			if (!MessageQueue.Exists(q))
			{
				return MessageQueue.Create(q);
			}
			else
				return new MessageQueue(q);
		}
	}
}
