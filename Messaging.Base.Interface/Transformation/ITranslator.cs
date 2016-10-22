using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.Transformation
{
    public delegate TEntityTo TranslationDelegate<TEntityTo, TInput>(TInput entity);

    public interface ITranslator<TEntityTo>
    {
        TEntityTo Translate<TInput>(TInput entity);
        void RegisterTranslator<TInput>(TranslationDelegate<TEntityTo, TInput> translator);
    }
}
