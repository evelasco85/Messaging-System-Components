using System.Collections.Generic;
using Messaging.Base.Constructions;

namespace Messaging.Base
{
    public class SenderProperty
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }

    public class PropertyMap
    {
        public string SenderPropertyName { get; set; }
        public string TechnicalPropertyName { get; set; }
    }

    public delegate void AssignSenderPropertyDelegate(string name, object value);

    public interface IMessageSender
    {
        string QueueName { get; }
        void SetupSender();
    }

    public interface IMessageSender<TMessage> : IMessageSender
    {
        IReturnAddress<TMessage> AsReturnAddress();
        TMessage Send<TEntity>(TEntity message);
        TMessage Send<TEntity>(TEntity entity, IList<SenderProperty> propertiesToSet);
        TMessage Send(TMessage message);
        TMessage Send<TEntity>(TEntity entity, IReturnAddress<TMessage> returnAddress);
        TMessage Send<TEntity>(TEntity entity, IReturnAddress<TMessage> returnAddress, IList<SenderProperty> propertiesToSet);
    }

    public interface IMessageSender<TMessageQueue, TMessage> : IMessageCore<TMessageQueue>, IMessageSender<TMessage>
    {
    }
}
