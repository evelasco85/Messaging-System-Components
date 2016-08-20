using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.Transformation
{
    public class Translator<TEntityTo> : ITranslator<TEntityTo>
    {
        IDictionary<string, object> _translators = new Dictionary<string, object>();

        string GetKey<TInput>()
        {
            return typeof(TInput).FullName;
        }

        public TEntityTo Translate<TInput>(TInput entity)
        {
            string key = GetKey<TInput>();

            if (entity == null)
                return default(TEntityTo);

            if (!_translators.ContainsKey(key))
                throw new ArgumentException(string.Format("Translator for input type '{0}' does not exists", key));

            TranslationDelegate<TEntityTo, TInput> translator = (TranslationDelegate<TEntityTo, TInput>)_translators[key];

            return translator(entity);
        }

        public void RegisterTranslators<TInput>(TranslationDelegate<TEntityTo, TInput> translator)
        {
            if (translator == null)
                throw new ArgumentNullException("'translator' implementation is required");

            string key = GetKey<TInput>();

            if (_translators.ContainsKey(key))
                throw new ArgumentException(string.Format("Translator for input type '{0}' already registered", key));

            _translators.Add(key, translator);
        }
    }
}
