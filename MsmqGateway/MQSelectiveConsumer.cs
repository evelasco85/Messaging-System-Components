using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessageGateway;
using Messaging.Base;
using Messaging.Base.Constructions;

namespace MsmqGateway
{
    public class MQSelectiveConsumer : ReceiverGateway<MessageQueue, Message>
    {
        private IReturnAddress<Message> _returnAddress;
        private MessageDelegate<Message> _receivedMessageProcessor;
        private string _correlationId;


        public MQSelectiveConsumer(MessageQueueGateway messageQueueGateway, 
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

        public MQSelectiveConsumer(String q,
            string correlationId)
            : this(new MessageQueueGateway(q), correlationId)
        {
            this._receivedMessageProcessor = new MessageDelegate<Message>(NullImpl);
        }

        public MQSelectiveConsumer(String path, MessageDelegate<Message> receiveMessageDelegate,
            string correlationId)
            : this(path, correlationId)
        {
            this._receivedMessageProcessor = receiveMessageDelegate;
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

        public MQSelectiveConsumer(String q, IMessageFormatter formatter,
            string correlationId)
            : this(q, correlationId)
        {
            GetQueue().Formatter = formatter;

            this._receivedMessageProcessor = new MessageDelegate<Message>(NullImpl);
        }

        public MQSelectiveConsumer(String q, IMessageFormatter formatter,
            MessageDelegate<Message> receiveMessageDelegate,
            string correlationId)
            : this(q, formatter, correlationId)
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
            if (GetQueue().Formatter == null)
                GetQueue().Formatter = new System.Messaging.XmlMessageFormatter(new String[] { "System.String,mscorlib" });

            while (true)
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

        public override IReturnAddress<Message> AsReturnAddress()
        {
            return _returnAddress;
        }
    }
}
