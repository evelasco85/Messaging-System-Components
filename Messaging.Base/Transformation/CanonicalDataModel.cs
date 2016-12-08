namespace Messaging.Base.Transformation
{
    public abstract class CanonicalDataModel<TMessage, TEntity> : ICanonicalDataModel<TMessage, TEntity>
    {
        class MessageTranslator : Translator<TMessage>
        {
        }

        class EntityTranslator : Translator<TEntity>
        {
        }

        EntityTranslator _entityTranslator = new EntityTranslator();
        MessageTranslator _messageTranslator = new MessageTranslator();

        public CanonicalDataModel()
        {
            _entityTranslator.RegisterTranslator(
                new TranslationDelegate<TEntity, TMessage>(TranslateToEntity)
                );

            _messageTranslator.RegisterTranslator(
                new TranslationDelegate<TMessage, TEntity>(TranslateToMessage)
                );
        }

        public abstract TMessage TranslateToMessage(TEntity entity);
        public abstract TEntity TranslateToEntity(TMessage message);

        public TMessage GetMessage(TEntity entity)
        {
            return _messageTranslator.Translate(entity);
        }

        public TEntity GetEntity(TMessage message)
        {
            return _entityTranslator.Translate(message);
        }
    }
}
