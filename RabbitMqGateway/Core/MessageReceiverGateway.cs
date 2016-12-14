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
        ReceiverGateway<IModel, Message>
    {
        private IReturnAddress<Message> _returnAddress;
        private MessageDelegate<Message> _receivedMessageProcessor;
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

        public MessageReceiverGateway(MessageQueueGateway messageQueueGateway)
            : this(messageQueueGateway, new CanonicalDataModel<TEntity>())
        {
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
            Message message = (Message)JsonConvert.DeserializeObject(jsonMessage);

            if (_receivedMessageProcessor != null)
                _receivedMessageProcessor.Invoke(message);
        }

        public override MessageDelegate<Message> ReceiveMessageProcessor
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

        public override IReturnAddress<Message> AsReturnAddress()
        {
            return _returnAddress;
        }

        public override string GetMessageApplicationId(Message message)
        {
            return message.AppSpecific;
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
