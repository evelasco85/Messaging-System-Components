using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.Constructions
{
    public class ReturnAddress<TMessageQueue, TMessage> : IReturnAddress<TMessage>
    {
        private IMessageCore<TMessageQueue> _messageReplyQueue;
        private ReplyAddressSetupDelegate<TMessageQueue, TMessage> _replyAddressSetupDelegate;

        public ReturnAddress(IMessageCore<TMessageQueue> messageReplyQueue, ReplyAddressSetupDelegate<TMessageQueue, TMessage> replyAddressSetupDelegate)
        {
            _messageReplyQueue = messageReplyQueue;
            _replyAddressSetupDelegate = replyAddressSetupDelegate;
        }

        public void SetMessageReturnAddress(ref TMessage message)
        {
            if((_messageReplyQueue == null) || (_messageReplyQueue.GetQueue() == null) || (_replyAddressSetupDelegate == null))
                return;

            _replyAddressSetupDelegate(_messageReplyQueue.GetQueue(), ref message);
        }
    }
}
