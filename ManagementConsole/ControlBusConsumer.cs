using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using MessageGateway;
using Messaging.Base.System_Management.SmartProxy;

namespace ManagementConsole
{
    public class NullJournal
    {
    }

    public class ControlBusConsumer : MessageConsumer<MessageQueue, Message, NullJournal>
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
