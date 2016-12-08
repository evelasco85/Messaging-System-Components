namespace Messaging.Base.Transformation
{
    public interface ICanonicalDataModel<TMessage, TEntity>
    {
        TMessage TranslateToMessage(TEntity entity);
        TEntity TranslateToEntity(TMessage message);
        TMessage GetMessage(TEntity entity);
        TEntity GetEntity(TMessage message);
    }
}
