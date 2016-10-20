using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messaging.Base.Constructions;
using Messaging.Base.System_Management.SmartProxy;

namespace Messaging.Base.System_Management
{
    public abstract class TestMessage<TMessageQueue, TMessage> : MessageConsumer<TMessageQueue, TMessage>, ITestMessage<TMessageQueue, TMessage>
    {
        IMessageSender<TMessageQueue, TMessage> _controlBusQueue;
        IReturnAddress<TMessage> _monitorQueueReturnAddress;
        IMessageSender<TMessageQueue, TMessage> _serviceQueue;

        public TestMessage(
            IMessageSender<TMessageQueue, TMessage> controlBusQueue,
            IMessageSender<TMessageQueue, TMessage> serviceQueue,
            IReturnAddress<TMessage> monitorQueueReturnAddress,
            IMessageReceiver<TMessageQueue, TMessage> monitorReceiver)
            : base(monitorReceiver)
        {
            _controlBusQueue = controlBusQueue;
            _monitorQueueReturnAddress = monitorQueueReturnAddress;
            _serviceQueue = serviceQueue;
        }

        public void SendControlBusStatus(TMessage statusMessage)
        {
            _controlBusQueue.Send(statusMessage);
        }

        public void SendTestMessage(TMessage message)
        {
            _monitorQueueReturnAddress.SetMessageReturnAddress(ref message);
            _serviceQueue.Send(message);
        }

        public override void ProcessMessage(TMessage message)
        {
            ReceiveTestMessageResponse(message);
        }

        public abstract void ReceiveTestMessageResponse(TMessage message);
    }
}
