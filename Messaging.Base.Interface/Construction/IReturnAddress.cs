using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.Constructions
{
    public delegate void ReplyAddressSetupDelegate<TMessageQueue, TMessage>(TMessageQueue replyQueue, ref TMessage message);

    public interface IReturnAddress<TMessage>
    {
        void SetMessageReturnAddress(ref TMessage message);
    }
}
