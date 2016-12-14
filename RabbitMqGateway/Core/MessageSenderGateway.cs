using System;
using System.Text;
using Messaging.Base;
using Messaging.Base.Constructions;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace RabbitMqGateway.Core
{
    public class MessageSenderGateway : SenderGateway<IModel, RQMessage>
    {
        private IReturnAddress<RQMessage> _returnAddress;

        public MessageSenderGateway(MessageQueueGateway messageQueueGateway)
            : base(messageQueueGateway)
        {
            _returnAddress = new MQReturnAddress(messageQueueGateway);
        }

        public override void SetupSender()
        {
        }

        public override IReturnAddress<RQMessage> AsReturnAddress()
        {
            return _returnAddress;
        }

        public override RQMessage Send<TEntity>(TEntity entity)
        {
            RQMessage message = GetMessage(entity);

            return SendRawMessage(message);
        }

        public override RQMessage Send<TEntity>(TEntity entity, Action<AssignApplicationIdDelegate, AssignCorrelationIdDelegate> AssignProperty)
        {
            RQMessage message = GetMessage(entity);

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

        public override RQMessage Send<TEntity>(TEntity entity, IReturnAddress<RQMessage> returnAddress)
        {
            RQMessage message = GetMessage(entity);

            returnAddress.SetMessageReturnAddress(ref message);

            return SendRawMessage(message);
        }

        public override RQMessage Send<TEntity>(TEntity entity, IReturnAddress<RQMessage> returnAddress, Action<AssignApplicationIdDelegate, AssignCorrelationIdDelegate, AssignPriorityDelegate> AssignProperty)
        {
            RQMessage message = GetMessage(entity);

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

        public override RQMessage SendRawMessage(RQMessage message)
        {
            string id = Guid.NewGuid().ToString();

            message.Id = id;

            IBasicProperties properties = GetQueue().CreateBasicProperties();

            properties.MessageId = id;
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

        RQMessage GetMessage<TEntity>(TEntity entity)
        {
            CanonicalDataModel<TEntity> _cdm = new CanonicalDataModel<TEntity>();
            RQMessage message =  _cdm.GetMessage(entity);

            message.Id = Guid.NewGuid().ToString();

            return message;
        }
    }
}
