using System.Messaging;
using MessageGateway;
using Messaging.Base.System_Management.SmartProxy;

namespace ManagementConsole
{
    public class ControlBusConsumer : MessageConsumer<Message>
    {
        public delegate  void MessageReceived(Message message);

        private MessageReceived _messageReceivedDelegate;

        public ControlBusConsumer(string controlBusQueueName, MessageReceived messageReceivedDelegate)
            : base(new MessageReceiverGateway(controlBusQueueName))
        {
            _messageReceivedDelegate = messageReceivedDelegate;
        }

        public override void ProcessMessage(Message message)
        {
            if (_messageReceivedDelegate != null)
                _messageReceivedDelegate(message);
        }
    }
}
