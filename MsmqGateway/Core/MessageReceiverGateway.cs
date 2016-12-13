
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
        private CanonicalDataModel<TEntity> _cdm;

        public CanonicalDataModel<TEntity> CanonicalDataModel
        {
            get { return _cdm; }
        }

        public MessageReceiverGateway(MessageQueueGateway messageQueueGateway, CanonicalDataModel<TEntity> cdm)
            : base(messageQueueGateway)
        {
            _cdm = cdm;
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

        public MessageReceiverGateway(String q, CanonicalDataModel<TEntity> cdm) : this(new MessageQueueGateway(q), cdm)
        {
            this._receivedMessageProcessor = new MessageDelegate<Message>(NullImpl);
        }

        public MessageReceiverGateway(String q, MessageDelegate<Message> receiveMessageDelegate) : this(q, new CanonicalDataModel<TEntity>())
        {
            this._receivedMessageProcessor += receiveMessageDelegate;
        }

        public MessageReceiverGateway(MessageQueue q) : this(new MessageQueueGateway(q), new CanonicalDataModel<TEntity>())
        {
            this._receivedMessageProcessor = new MessageDelegate<Message>(NullImpl);
        }

        public MessageReceiverGateway(MessageQueue q, MessageDelegate<Message> receiveMessageDelegate) : this(q)
        {
            this._receivedMessageProcessor += receiveMessageDelegate;
        }

        public MessageReceiverGateway(String q) : this(q, new CanonicalDataModel<TEntity>())
        {
            GetQueue().Formatter = _cdm.Formatter;

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

            GetQueue().Formatter = _cdm.Formatter;
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

            m.Formatter = _cdm.Formatter;

            if (_receivedMessageProcessor != null)
                _receivedMessageProcessor.Invoke(m);

            mq.BeginReceive();
        }

        public override IReturnAddress<Message> AsReturnAddress()
        {
            return _returnAddress;
        }

        public override string GetMessageAppSpecific(Message message)
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
