using Messaging.Base;
using System;
using System.Messaging;
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

        public MessageSenderGateway(String q) : this(new MessageQueueGateway(q))
        { }

        public MessageSenderGateway(MessageQueue queue) : this(new MessageQueueGateway(queue))
        { }

	    public override IReturnAddress<Message> AsReturnAddress()
	    {
	        return _returnAddress;
	    }

        public override void Send(Message msg)
        {
            GetQueue().Send(msg);
        }

        public override void Send<TEntity>(TEntity entity)
        {
            CanonicalDataModel<TEntity> _cdm = new CanonicalDataModel<TEntity>();

            Send(_cdm.GetMessage(entity));
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
