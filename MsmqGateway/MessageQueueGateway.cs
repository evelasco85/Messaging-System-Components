/* Loan Broker Example with MSMQ
 * from Enterprise Integration Patterns (Addison-Wesley, ISBN 0321200683)
 * 
 * Copyright (c) 2003 Gregor Hohpe 
 *
 * This code is supplied as is. No warranties. 
 */

using System.Messaging;
using System;
using Messaging.Base;

namespace MessageGateway{
    public class MessageQueueGateway : QueueGateway<MessageQueue>, IDisposable
    {
        MessageQueue msgQueue;

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
