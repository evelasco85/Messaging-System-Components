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
                new TranslationDelegate<TEntity, TMessage>(
                    (inputMessage) =>
                    {
                        TEntity output;

                        TranslateToEntity(inputMessage, out output);

                        return output;
                    })
                    );

            _messageTranslator.RegisterTranslator(
                new TranslationDelegate<TMessage, TEntity>(
                    (inputEntity) =>
                    {
                        TMessage output;

                        TranslateToMessage(inputEntity, out output);

                        return output;
                    })
                    );
        }

        public abstract void TranslateToMessage(TEntity entity, out TMessage expectedMessage);
        public abstract void TranslateToEntity(TMessage message, out TEntity expectedEntity);

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
