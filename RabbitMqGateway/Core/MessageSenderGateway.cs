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

        public override RQMessage Send<TEntity>(TEntity message)
        {
            throw new System.NotImplementedException();
        }

        public override RQMessage Send<TEntity>(TEntity entity, Action<AssignApplicationIdDelegate, AssignCorrelationIdDelegate> AssignProperty)
        {
            throw new NotImplementedException();
        }

        public override RQMessage Send<TEntity>(TEntity entity, IReturnAddress<RQMessage> returnAddress)
        {
            throw new NotImplementedException();
        }

        public override RQMessage Send<TEntity>(TEntity entity, IReturnAddress<RQMessage> returnAddress, Action<AssignApplicationIdDelegate, AssignCorrelationIdDelegate, AssignPriorityDelegate> AssignProperty)
        {
            throw new NotImplementedException();
        }

        public override RQMessage SendRawMessage(RQMessage message)
        {
            message.MessageId = Guid.NewGuid().ToString();

            IBasicProperties properties = GetQueue().CreateBasicProperties();

            properties.MessageId = message.MessageId;

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

        byte[] GetMessage<TEntity>(TEntity entity)
        {
            CanonicalDataModel<TEntity> _cdm = new CanonicalDataModel<TEntity>();
            RQMessage message = _cdm.GetMessage(entity);
            string jsonMessage = JsonConvert.SerializeObject(message);

            return Encoding.UTF8.GetBytes(jsonMessage); ;
        }
    }
}
