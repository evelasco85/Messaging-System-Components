/* Loan Broker Example with MSMQ
 * from Enterprise Integration Patterns (Addison-Wesley, ISBN 0321200683)
 * 
 * Copyright (c) 2003 Gregor Hohpe 
 *
 * This code is supplied as is. No warranties. 
 */

using System.Messaging;
using System;
using Messaging.Base.Interface;
using Messaging.Base;

namespace MessageGateway{
	public class MessageReceiverGateway :
        ReceiverGateway<MessageQueue, Message>{
		
		private OnMsgEvent<Message> receiver;

        public MessageReceiverGateway(MessageQueueGateway messageQueueGateway) : base(messageQueueGateway)
        {
        }

        public MessageReceiverGateway(String q) : this(new MessageQueueGateway(q))
        {
            this.receiver = new OnMsgEvent<Message>(NullImpl);
        }

        public MessageReceiverGateway(String path, OnMsgEvent<Message> receiver) : this(path)
        {
            this.receiver = receiver;
        }

        public MessageReceiverGateway(MessageQueue q): this(new MessageQueueGateway(q))
        {
			this.receiver = new OnMsgEvent<Message>(NullImpl);
		}

        public MessageReceiverGateway(MessageQueue q, OnMsgEvent<Message> ev) : this(q)
        {
            this.receiver += ev;
        }

        public MessageReceiverGateway(String q, IMessageFormatter formatter) : this(q)
        {
            GetQueue().Formatter = formatter;
            GetQueue().MessageReadPropertyFilter.ClearAll();
            GetQueue().MessageReadPropertyFilter.AppSpecific = true;
            GetQueue().MessageReadPropertyFilter.Body = true;
            GetQueue().MessageReadPropertyFilter.CorrelationId = true;
            GetQueue().MessageReadPropertyFilter.Id = true;
            GetQueue().MessageReadPropertyFilter.ResponseQueue = true;
            GetQueue().MessageReadPropertyFilter.ArrivedTime = true;

            this.receiver = new OnMsgEvent<Message>(NullImpl);
        }

        public MessageReceiverGateway(String q, IMessageFormatter formatter, OnMsgEvent<Message> ev)
               : this(q, formatter)
        {
            this.receiver += ev;
        }

        public override OnMsgEvent<Message> OnMessage
        { 
			get { return receiver; }  
			set { receiver = value; }
		}
		
		private void NullImpl(Message msg){}
		
        public override void StartReceivingMessages()
        {
            GetQueue().Formatter =
                new System.Messaging.XmlMessageFormatter(new String[] {"System.String,mscorlib"});
			GetQueue().ReceiveCompleted += new ReceiveCompletedEventHandler(OnReceiveCompleted);
			GetQueue().BeginReceive();
		}
		
		private void OnReceiveCompleted(Object source, ReceiveCompletedEventArgs asyncResult)
		{
			MessageQueue mq = (MessageQueue)source;
			Message m = mq.EndReceive(asyncResult.AsyncResult);
			receiver(m);
			mq.BeginReceive(); 
		}	
	}
}
