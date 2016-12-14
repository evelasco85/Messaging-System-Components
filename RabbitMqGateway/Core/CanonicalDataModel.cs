using System;
using Messaging.Base.Transformation;

namespace RabbitMqGateway.Core
{
    public class CanonicalDataModel<TEntity> : CanonicalDataModel<Message, TEntity>
    {
        public override void TranslateToMessage(TEntity entity, out Message output)
        {
            output = new Message()
            {
                Body = entity
            };
        }

        public override void TranslateToEntity(Message message, out TEntity output)
        {
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
