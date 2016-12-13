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

        public override void TranslateToMessage(TEntity entity, out Message output)
        {
            output = new Message(entity);
        }

        public override void TranslateToEntity(Message message, out TEntity output)
        {
            message.Formatter = _formatter;

            output = (TEntity) message.Body;
        }

        public Type GetRequestBodyType()
        {
            return typeof(TEntity);
        }

        public bool MatchedDataModel(Message message)
        {
            return message.Body is TEntity;
        }
    }
}
