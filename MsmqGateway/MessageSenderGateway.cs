/* Loan Broker Example with MSMQ
 * from Enterprise Integration Patterns (Addison-Wesley, ISBN 0321200683)
 * 
 * Copyright (c) 2003 Gregor Hohpe 
 *
 * This code is supplied as is. No warranties. 
 */

using Messaging.Base;
using System;
using System.Messaging;

namespace MessageGateway{
	
	public class MessageSenderGateway: SenderGateway<MessageQueue, Message>
    {
        public MessageSenderGateway(MessageQueueGateway messageQueueGateway) : base(messageQueueGateway)
        {}

        public MessageSenderGateway(String q) : this(new MessageQueueGateway(q))
        { }

        public MessageSenderGateway(MessageQueue queue) : this(new MessageQueueGateway(queue))
        { }

		public override void Send(Message msg){
			GetQueue().Send(msg);
		}	
	}
}
