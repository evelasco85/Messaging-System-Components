namespace Messaging.Base.Transformation
{
    public interface ICanonicalDataModel<TMessage, TEntity>
    {
        void TranslateToMessage(TEntity entity, out TMessage expectedMessage);
        void TranslateToEntity(TMessage message, out TEntity expectedEntity);
        TMessage GetMessage(TEntity entity);
        TEntity GetEntity(TMessage message);
    }
}
