using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Base
{
    public delegate void MessageDelegate<TMessage>(TMessage msg);
}
