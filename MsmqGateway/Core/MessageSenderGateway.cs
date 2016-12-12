using Messaging.Base;
using System;
using System.Collections.Generic;
using System.Messaging;
using System.Reflection;
using Messaging.Base.Constructions;

namespace MsmqGateway.Core
{
	public class MessageSenderGateway: SenderGateway<MessageQueue, Message>
	{
        private IReturnAddress<Message> _returnAddress;
        
	    public MessageSenderGateway(MessageQueueGateway messageQueueGateway) : base(messageQueueGateway)
	    {
            _returnAddress = new MQReturnAddress(messageQueueGateway);
	    }

	    public MessageSenderGateway(String q)
            : this(new MessageQueueGateway(q))
	    { }

        public MessageSenderGateway(MessageQueue queue)
            : this(new MessageQueueGateway(queue))
        { }

	    public override IReturnAddress<Message> AsReturnAddress()
	    {
	        return _returnAddress;
	    }

        public override Message Send(Message msg)
        {
            GetQueue().Send(msg);

            return msg;
        }

        public override Message Send<TEntity>(TEntity entity)
        {
            CanonicalDataModel<TEntity> _cdm = new CanonicalDataModel<TEntity>();
            Message message = _cdm.GetMessage(entity);

            return Send(message);
        }

        public override Message Send<TEntity>(TEntity entity, IList<SenderProperty> propertiesToSet)
        {
            CanonicalDataModel<TEntity> _cdm = new CanonicalDataModel<TEntity>();
            Message message = _cdm.GetMessage(entity);

            ApplyProperties(ref message, propertiesToSet);
            return Send(message);
        }

	    public override Message Send<TEntity>(TEntity entity, IReturnAddress<Message> returnAddress)
	    {
            CanonicalDataModel<TEntity> _cdm = new CanonicalDataModel<TEntity>();
            Message message = _cdm.GetMessage(entity);

            returnAddress.SetMessageReturnAddress(ref message);
            return Send(message);
	    }

	    public override Message Send<TEntity>(TEntity entity, IReturnAddress<Message> returnAddress, IList<SenderProperty> propertiesToSet)
        {
            CanonicalDataModel<TEntity> _cdm = new CanonicalDataModel<TEntity>();
            Message message = _cdm.GetMessage(entity);

            ApplyProperties(ref message, propertiesToSet);
            returnAddress.SetMessageReturnAddress(ref message);
            return Send(message);
        }

	    void ApplyProperties(ref Message message, IList<SenderProperty> propertiesToSet)
	    {
	        if (propertiesToSet == null)
	            return;

            Type propertyType = message.GetType();

            for(int index = 0; index < propertiesToSet.Count; index++)
            {
                SenderProperty property = propertiesToSet[index];
                PropertyInfo propertyInfo = propertyType.GetProperty(property.Name, BindingFlags.Public | BindingFlags.Instance);

                propertyInfo.SetValue(message, property.Value);
            }
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
