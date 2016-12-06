using System;
using System.Messaging;
using Messaging.Base.Transformation;

namespace MsmqGateway.Core
{
    public class CanonicalDataModel<TEntity> : CanonicalDataModel<Message, TEntity>
    {
        IMessageFormatter _formatter = new XmlMessageFormatter(new Type[] { typeof(TEntity) });

        public IMessageFormatter Formatter
        {
            get { return _formatter; }
        }


        public override Message TranslateToMessage(TEntity entity)
        {
            return new Message(entity);
        }

        public override TEntity TranslateToEntity(Message message)
        {
            message.Formatter = _formatter;

            TEntity entity = (TEntity) message.Body;

            return entity;
        }
    }
}
