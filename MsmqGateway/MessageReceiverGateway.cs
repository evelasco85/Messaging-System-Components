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
		
		private MessageDelegate<Message> _receivedMessageProcessor;

        public MessageReceiverGateway(MessageQueueGateway messageQueueGateway) : base(messageQueueGateway)
        {
        }

        public MessageReceiverGateway(String q) : this(new MessageQueueGateway(q))
        {
            this._receivedMessageProcessor = new MessageDelegate<Message>(NullImpl);
        }

        public MessageReceiverGateway(String path, MessageDelegate<Message> receiveMessageDelegate) : this(path)
        {
            this._receivedMessageProcessor = receiveMessageDelegate;
        }

        public MessageReceiverGateway(MessageQueue q): this(new MessageQueueGateway(q))
        {
			this._receivedMessageProcessor = new MessageDelegate<Message>(NullImpl);
		}

        public MessageReceiverGateway(MessageQueue q, MessageDelegate<Message> receiveMessageDelegate) : this(q)
        {
            this._receivedMessageProcessor += receiveMessageDelegate;
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

            this._receivedMessageProcessor = new MessageDelegate<Message>(NullImpl);
        }

        public MessageReceiverGateway(String q, IMessageFormatter formatter, MessageDelegate<Message> receiveMessageDelegate)
               : this(q, formatter)
        {
            this._receivedMessageProcessor += receiveMessageDelegate;
        }

        public override MessageDelegate<Message> ReceiveMessageProcessor
        { 
			get { return _receivedMessageProcessor; }  
			set { _receivedMessageProcessor = value; }
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

			_receivedMessageProcessor.Invoke(m);
			mq.BeginReceive(); 
		}	
	}
}
