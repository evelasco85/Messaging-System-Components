
using System.Messaging;
using System;
using Messaging.Base;
using Messaging.Base.Constructions;

namespace MsmqGateway.Core
{
    public class MessageReceiverGateway<TEntity> :
        ReceiverGateway<MessageQueue, Message>
    {
        private IReturnAddress<Message> _returnAddress;
        private MessageDelegate<Message> _receivedMessageProcessor;
        private IMessageFormatter _formatter = new XmlMessageFormatter(new Type[] {typeof(TEntity)});

        public MessageReceiverGateway(MessageQueueGateway messageQueueGateway)
            : base(messageQueueGateway)
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

        public MessageReceiverGateway(String q)
            : this(new MessageQueueGateway(q))
        {
            GetQueue().Formatter = _formatter;

            this._receivedMessageProcessor = new MessageDelegate<Message>(NullImpl);
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

            GetQueue().Formatter = _formatter;
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

            m.Formatter = _formatter;

            if (_receivedMessageProcessor != null)
                _receivedMessageProcessor.Invoke(m);

            mq.BeginReceive();
        }

        public override IReturnAddress<Message> AsReturnAddress()
        {
            return _returnAddress;
        }

        public override string GetMessageApplicationId(Message message)
        {
            return message.AppSpecific.ToString();
        }

        public override string GetMessageCorrelationId(Message message)
        {
            return message.CorrelationId;
        }

        public override string GetMessageId(Message message)
        {
            return message.Id;
        }
    }
}
