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
        private bool _continueReceivingMessages = true;
        private IMessageFormatter _formatter = new XmlMessageFormatter(new Type[] { typeof(TEntity) });

        public MQSelectiveConsumer(
            MessageQueueGateway messageQueueGateway, 
            string correlationId
            ) : base(messageQueueGateway)
        {
            _returnAddress = new MQReturnAddress(messageQueueGateway);
            _correlationId = correlationId;
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
            : this(new MessageQueueGateway(q), correlationId)
        {
            GetQueue().Formatter = _formatter;
            this._receivedMessageProcessor = new MessageDelegate<Message>(NullImpl);
        }

        public MQSelectiveConsumer(String q, MessageDelegate<Message> receiveMessageDelegate,
            string correlationId)
            : this(q, correlationId)
        {
            this._receivedMessageProcessor += receiveMessageDelegate;
        }

        public MQSelectiveConsumer(MessageQueue q, string correlationId)
            : this(new MessageQueueGateway(q), correlationId)
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
