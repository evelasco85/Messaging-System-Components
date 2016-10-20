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
	    {
            GetQueue().MessageReadPropertyFilter.ClearAll();
            GetQueue().MessageReadPropertyFilter.AppSpecific = true;
            GetQueue().MessageReadPropertyFilter.Body = true;
            GetQueue().MessageReadPropertyFilter.CorrelationId = true;
            GetQueue().MessageReadPropertyFilter.Id = true;
            GetQueue().MessageReadPropertyFilter.ResponseQueue = true;
            GetQueue().MessageReadPropertyFilter.ArrivedTime = true;
            GetQueue().MessageReadPropertyFilter.SentTime = true;
	    }

        public MessageSenderGateway(String q) : this(new MessageQueueGateway(q))
        { }

        public MessageSenderGateway(MessageQueue queue) : this(new MessageQueueGateway(queue))
        { }

		public override void Send(Message msg){
			GetQueue().Send(msg);
		}	
	}
}
