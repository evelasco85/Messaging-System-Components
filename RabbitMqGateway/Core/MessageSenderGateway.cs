using System;
using System.Text;
using Messaging.Base;
using Messaging.Base.Constructions;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace RabbitMqGateway.Core
{
    public class MessageSenderGateway : SenderGateway<IModel, Message>
    {
        private IReturnAddress<Message> _returnAddress;

        public MessageSenderGateway(MessageQueueGateway messageQueueGateway)
            : base(messageQueueGateway)
        {
            _returnAddress = new MQReturnAddress(messageQueueGateway);
        }

        public override void SetupSender()
        {
        }

        public override IReturnAddress<Message> AsReturnAddress()
        {
            return _returnAddress;
        }

        public override Message Send<TEntity>(TEntity entity)
        {
            Message message = GetMessage(entity);

            return SendRawMessage(message);
        }

        public override Message Send<TEntity>(TEntity entity, Action<AssignApplicationIdDelegate, AssignCorrelationIdDelegate> AssignProperty)
        {
            Message message = GetMessage(entity);

            AssignProperty(
                (applicationId) =>
                {
                    message.AppSpecific = applicationId;
                },
                (correlationId =>
                {
                    message.CorrelationId = correlationId;
                }));

            return SendRawMessage(message);
        }

        public override Message Send<TEntity>(TEntity entity, IReturnAddress<Message> returnAddress)
        {
            Message message = GetMessage(entity);

            returnAddress.SetMessageReturnAddress(ref message);

            return SendRawMessage(message);
        }

        public override Message Send<TEntity>(TEntity entity, IReturnAddress<Message> returnAddress, Action<AssignApplicationIdDelegate, AssignCorrelationIdDelegate, AssignPriorityDelegate> AssignProperty)
        {
            Message message = GetMessage(entity);

            AssignProperty(
                (applicationId) =>
                {
                    message.AppSpecific = applicationId;
                },
                (correlationId =>
                {
                    message.CorrelationId = correlationId;
                }),
                (priority =>
                {
                    message.Priority = (byte)priority;
                }));
            returnAddress.SetMessageReturnAddress(ref message);

            return SendRawMessage(message);
        }

        public override Message SendRawMessage(Message message)
        {
            if (string.IsNullOrEmpty(message.Id))
                message.Id = Guid.NewGuid().ToString();

            IBasicProperties properties = GetQueue().CreateBasicProperties();

            properties.MessageId = message.Id;
            properties.Priority = message.Priority;
            properties.CorrelationId = message.CorrelationId;
            properties.AppId = message.AppSpecific;
            properties.ReplyTo = message.ReplyTo;

            string jsonMessage = JsonConvert.SerializeObject(message);
            
            GetQueue()
                .BasicPublish(
                    exchange: "",
                    routingKey: this.QueueName,
                    basicProperties: properties,
                    body: Encoding.UTF8.GetBytes(jsonMessage)
                );

            return message;
        }

        Message GetMessage<TEntity>(TEntity entity)
        {
            Message message = new Message
            {
                Body = entity
            };

            message.Id = Guid.NewGuid().ToString();

            return message;
        }
    }
}
