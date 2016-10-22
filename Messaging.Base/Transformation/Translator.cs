using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.Transformation
{
    /// <summary>
    /// Translator implementation
    /// </summary>
    /// <typeparam name="TOutput">Type of output</typeparam>
    public class Translator<TOutput> : ITranslator<TOutput>
    {
        IDictionary<string, object> _translators = new Dictionary<string, object>();

        public Translator()
        {
        }

        string GetKey<TInput>()
        {
            return typeof(TInput).FullName;
        }

        public TOutput Translate<TInput>(TInput entity)
        {
            string key = GetKey<TInput>();

            if (entity == null)
                return default(TOutput);

            if (!_translators.ContainsKey(key))
                throw new ArgumentException(string.Format("Translator for input type '{0}' does not exists", key));

            TranslationDelegate<TOutput, TInput> translator = (TranslationDelegate<TOutput, TInput>)_translators[key];

            return translator(entity);
        }

        public void RegisterTranslator<TInput>(TranslationDelegate<TOutput, TInput> translator)
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
