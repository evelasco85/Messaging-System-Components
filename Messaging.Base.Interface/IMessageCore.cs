using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Base
{
    public interface IMessageCore<TMessageQueue>
    {
        TMessageQueue GetQueue();
    }
}
