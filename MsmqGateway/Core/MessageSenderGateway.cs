using Messaging.Base;
using System;
using System.Collections.Generic;
using System.Messaging;
using System.Reflection;
using Messaging.Base.Constructions;

namespace MsmqGateway.Core
{
    public class MessageSenderGateway : SenderGateway<MessageQueue, Message>
    {
        private IReturnAddress<Message> _returnAddress;
        private IList<PropertyMap> _propertyMaps;


        public MessageSenderGateway(MessageQueueGateway messageQueueGateway) : base(messageQueueGateway)
        {
            _returnAddress = new MQReturnAddress(messageQueueGateway);
        }

        public MessageSenderGateway(String q)
            : this(new MessageQueueGateway(q))
        {
        }

        public MessageSenderGateway(MessageQueue queue)
            : this(new MessageQueueGateway(queue))
        {
        }

        public MessageSenderGateway(MessageQueueGateway messageQueueGateway, IList<PropertyMap> propertyMap)
            : base(messageQueueGateway)
        {
            _propertyMaps = propertyMap;
            _returnAddress = new MQReturnAddress(messageQueueGateway);
        }

        public MessageSenderGateway(String q, IList<PropertyMap> propertyMap)
            : this(new MessageQueueGateway(q), propertyMap)
        {
        }

        public MessageSenderGateway(MessageQueue queue, IList<PropertyMap> propertyMap)
            : this(new MessageQueueGateway(queue), propertyMap)
        {
        }

        public override IReturnAddress<Message> AsReturnAddress()
        {
            return _returnAddress;
        }

        public override Message Send(Message msg)
        {
            return SendMessage(msg);
        }

        public override Message Send<TEntity>(TEntity entity)
        {
            CanonicalDataModel<TEntity> _cdm = new CanonicalDataModel<TEntity>();
            Message message = _cdm.GetMessage(entity);

            return SendMessage(message);
        }

        public override Message Send<TEntity>(TEntity entity, Action<AssignSenderPropertyDelegate> AssignProperty)
        {
            CanonicalDataModel<TEntity> _cdm = new CanonicalDataModel<TEntity>();
            Message message = _cdm.GetMessage(entity);

            AssignProperty((name, value) =>
            {
                ApplyProperties(ref message, name, value);
            });

            return SendMessage(message);
        }

        public override Message Send<TEntity>(TEntity entity, IReturnAddress<Message> returnAddress)
        {
            CanonicalDataModel<TEntity> _cdm = new CanonicalDataModel<TEntity>();
            Message message = _cdm.GetMessage(entity);

            returnAddress.SetMessageReturnAddress(ref message);

            return SendMessage(message);
        }

        public override Message Send<TEntity>(TEntity entity,
            IReturnAddress<Message> returnAddress,
            Action<AssignSenderPropertyDelegate> AssignProperty)
        {
            CanonicalDataModel<TEntity> _cdm = new CanonicalDataModel<TEntity>();
            Message message = _cdm.GetMessage(entity);

            AssignProperty((name, value) =>
            {
                ApplyProperties(ref message, name, value);
            });
            returnAddress.SetMessageReturnAddress(ref message);

            return SendMessage(message);
        }

        Message SendMessage(Message message)
        {
            GetQueue().Send(message);

            return message;
        }

        void ApplyProperties(ref Message message, string name, object value)
        {
            Type propertyType = message.GetType();
            PropertyInfo propertyInfo = propertyType.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);

            propertyInfo.SetValue(message, value);
        }

        public override void SetupSender()
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
	}
}
