using System;
using Messaging.Base.Transformation;

namespace RabbitMqGateway.Core
{
    public class CanonicalDataModel<TEntity> : CanonicalDataModel<RQMessage, TEntity>
    {
        public override void TranslateToMessage(TEntity entity, out RQMessage output)
        {
            throw new NotImplementedException();
        }

        public override void TranslateToEntity(RQMessage message, out TEntity output)
        {
            throw new NotImplementedException();
        }
    }
}
