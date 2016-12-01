using Messaging.Base;
using Messaging.Base.System_Management.SmartProxy;

namespace ManagementConsole
{
    public class ControlBusConsumer<TMessage> : MessageConsumer<TMessage>
    {
        public delegate void MessageReceived(TMessage message);

        private MessageReceived _messageReceivedDelegate;

        public ControlBusConsumer(IMessageReceiver<TMessage> receiver, MessageReceived messageReceivedDelegate)
            : base(receiver)
        {
            _messageReceivedDelegate = messageReceivedDelegate;
        }

        public override void ProcessMessage(TMessage message)
        {
            if (_messageReceivedDelegate != null)
                _messageReceivedDelegate(message);
        }
    }
}
