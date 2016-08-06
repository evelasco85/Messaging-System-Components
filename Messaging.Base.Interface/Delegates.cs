using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Base.Interface
{
    public delegate void OnMsgEvent<TMessage>(TMessage msg);
}
