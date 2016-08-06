using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Base.Interface
{
    public interface IMessageCore<TMessageQueue>
    {
        TMessageQueue GetQueue();
    }
}
