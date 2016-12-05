
using System.Messaging;
using System;
using Messaging.Base;
using Messaging.Base.Constructions;

namespace MsmqGateway.Core
{
    public class MessageReceiverGateway :
        ReceiverGateway<MessageQueue, Message>
    {
        private IReturnAddress<Message> _returnAddress;
        private MessageDelegate<Message> _receivedMessageProcessor;

        public MessageReceiverGateway(MessageQueueGateway messageQueueGateway) : base(messageQueueGateway)
        {
            _returnAddress = new MQReturnAddress(messageQueueGateway);
        }

        public override void SetupReceiver()
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

        public MessageReceiverGateway(String q) : this(new MessageQueueGateway(q))
        {
            this._receivedMessageProcessor = new MessageDelegate<Message>(NullImpl);
        }

        public MessageReceiverGateway(String path, MessageDelegate<Message> receiveMessageDelegate) : this(path)
        {
            this._receivedMessageProcessor = receiveMessageDelegate;
        }

        public MessageReceiverGateway(MessageQueue q) : this(new MessageQueueGateway(q))
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

            this._receivedMessageProcessor = new MessageDelegate<Message>(NullImpl);
        }

        public MessageReceiverGateway(String q, IMessageFormatter formatter,
            MessageDelegate<Message> receiveMessageDelegate)
            : this(q, formatter)
        {
            this._receivedMessageProcessor += receiveMessageDelegate;
        }

        public override MessageDelegate<Message> ReceiveMessageProcessor
        {
            get { return _receivedMessageProcessor; }
            set { _receivedMessageProcessor = value; }
        }

        private void NullImpl(Message msg)
        {
        }

        public override void StartReceivingMessages()
        {
            if(Started)
                return;

            if (GetQueue().Formatter == null)
                GetQueue().Formatter = new System.Messaging.XmlMessageFormatter(new String[] { "System.String,mscorlib" });

            GetQueue().ReceiveCompleted += new ReceiveCompletedEventHandler(OnReceiveCompleted);

            GetQueue().BeginReceive();

            Started = true;
        }

        public override void StopReceivingMessages()
        {
            if(!Started)
                return;

            GetQueue().ReceiveCompleted -= new ReceiveCompletedEventHandler(OnReceiveCompleted);
            GetQueue().Close();

            Started = false;
        }

        private void OnReceiveCompleted(Object source, ReceiveCompletedEventArgs asyncResult)
        {
            MessageQueue mq = (MessageQueue) source;
            Message m = mq.EndReceive(asyncResult.AsyncResult);

            if (_receivedMessageProcessor != null)
                _receivedMessageProcessor.Invoke(m);

            mq.BeginReceive();
        }

        public override IReturnAddress<Message> AsReturnAddress()
        {
            return _returnAddress;
        }
    }
}
