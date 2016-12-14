using System;
using System.Text;
using Messaging.Base;
using Messaging.Base.Constructions;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqGateway.Core
{
    public class MessageReceiverGateway<TEntity> :
        ReceiverGateway<IModel, RQMessage>
    {
        private IReturnAddress<RQMessage> _returnAddress;
        private MessageDelegate<RQMessage> _receivedMessageProcessor;
        private CanonicalDataModel<TEntity> _cdm;
        private EventingBasicConsumer _consumer;
        private string _consumerTag = string.Empty;

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
            _consumer = new EventingBasicConsumer(GetQueue());

            _consumer.Received += _consumer_Received;
        }

        void _consumer_Received(IBasicConsumer sender, BasicDeliverEventArgs args)
        {
            byte[] messageByte = args.Body;
            string jsonMessage = Encoding.UTF8.GetString(messageByte);
            RQMessage message = (RQMessage)JsonConvert.DeserializeObject(jsonMessage);

            if (_receivedMessageProcessor != null)
                _receivedMessageProcessor.Invoke(message);
        }

        public override MessageDelegate<RQMessage> ReceiveMessageProcessor
        {
            get { return _receivedMessageProcessor; }
            set { _receivedMessageProcessor = value; }
        }

        public override void StartReceivingMessages()
        {
            if (Started)
                return;

            _consumerTag = GetQueue().BasicConsume(
                queue: QueueName,
                noAck: true,
                consumer: _consumer
                );

            Started = true;
        }

        public override void StopReceivingMessages()
        {
            if (!Started)
                return;

            GetQueue().BasicCancel(_consumerTag);

            Started = false;
        }

        public override IReturnAddress<RQMessage> AsReturnAddress()
        {
            return _returnAddress;
        }

        public override string GetMessageApplicationId(RQMessage message)
        {
            return message.AppSpecific;
        }

        public override string GetMessageCorrelationId(RQMessage message)
        {
            return message.CorrelationId;
        }

        public override string GetMessageId(RQMessage message)
        {
            return message.Id;
        }
    }
}
