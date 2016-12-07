using System;
using System.Messaging;
using System.Threading;
using MsmqGateway.Core;
using Messaging.Base;
using Messaging.Base.Constructions;

namespace MsmqGateway
{
    public class MQSelectiveConsumer<TEntity> : ReceiverGateway<MessageQueue, Message>
    {
        private IReturnAddress<Message> _returnAddress;
        private MessageDelegate<Message> _receivedMessageProcessor;
        private string _correlationId;
        bool _continueReceivingMessages = true;
        private CanonicalDataModel<TEntity> _cdm;

        public MQSelectiveConsumer(
            MessageQueueGateway messageQueueGateway, 
            string correlationId,
            CanonicalDataModel<TEntity> cdm
            ) : base(messageQueueGateway)
        {
            _returnAddress = new MQReturnAddress(messageQueueGateway);
            _correlationId = correlationId;
            _cdm = cdm;
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

        public MQSelectiveConsumer(
            String q,
            string correlationId)
            : this(new MessageQueueGateway(q), correlationId, new CanonicalDataModel<TEntity>())
        {
            GetQueue().Formatter = _cdm.Formatter;
            this._receivedMessageProcessor = new MessageDelegate<Message>(NullImpl);
        }

        public MQSelectiveConsumer(String q, MessageDelegate<Message> receiveMessageDelegate,
            string correlationId)
            : this(q, correlationId)
        {
            this._receivedMessageProcessor += receiveMessageDelegate;
        }

        public MQSelectiveConsumer(MessageQueue q, string correlationId)
            : this(new MessageQueueGateway(q), correlationId, new CanonicalDataModel<TEntity>())
        {
            this._receivedMessageProcessor = new MessageDelegate<Message>(NullImpl);
        }

        public MQSelectiveConsumer(MessageQueue q, MessageDelegate<Message> receiveMessageDelegate,
            string correlationId)
            : this(q, correlationId)
        {
            this._receivedMessageProcessor += receiveMessageDelegate;
        }

        public override MessageDelegate<Message> ReceiveMessageProcessor
        {
            get { return _receivedMessageProcessor; }
            set { _receivedMessageProcessor = value; }
        }

        public CanonicalDataModel<TEntity> CanonicalDataModel
        {
            get { return _cdm; }
        }

        private void NullImpl(Message msg)
        {
        }

        public override void StartReceivingMessages()
        {
            _continueReceivingMessages = true;

            while (_continueReceivingMessages)
            {
                try
                {
                    Message message = GetQueue().ReceiveByCorrelationId(_correlationId);

                    if ((message != null) && (_receivedMessageProcessor != null))
                        _receivedMessageProcessor.Invoke(message);

                    Thread.Sleep(1000);
                }
                catch (InvalidOperationException e)
                {

                }
            }
        }

        public override void StopReceivingMessages()
        {
            _continueReceivingMessages = false;
        }

        public override IReturnAddress<Message> AsReturnAddress()
        {
            return _returnAddress;
        }
    }
}
