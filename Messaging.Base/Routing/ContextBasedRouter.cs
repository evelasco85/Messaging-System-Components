using Messaging.Base.System_Management.SmartProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.Routing
{
    public class ContextBasedRouter<TMessageQueue, TMessage, TInput> : MessageConsumer<TMessageQueue, TMessage>, IContextBasedRouter<TMessageQueue, TMessage, TInput>
    {
        IList<Tuple<Func<TInput, bool>, IMessageSender<TMessageQueue, TMessage>>> _destinations;
        IMessageSender<TMessageQueue, TMessage> _currentDestinationSender;

        public ContextBasedRouter(IMessageReceiver<TMessageQueue, TMessage> inputQueue) : base(inputQueue)
        {
            _destinations = new List<Tuple<Func<TInput, bool>, IMessageSender<TMessageQueue, TMessage>>>();
        }

        public IContextBasedRouter<TMessageQueue, TMessage, TInput> AddSender(Func<TInput, bool> triggerFunction, IMessageSender<TMessageQueue, TMessage> destination)
        {
            _destinations.Add(new Tuple<Func<TInput, bool>, IMessageSender<TMessageQueue, TMessage>>(triggerFunction, destination));

            return this;
        }

        public void SwitchDestination(TInput inputToVerify)
        {
            for(int index = 0; index < _destinations.Count; index++)
            {
                Tuple<Func<TInput, bool>, IMessageSender<TMessageQueue, TMessage>> destination = _destinations[index];

                if ((destination == null) || (destination.Item1 == null) || (destination.Item2 == null))
                    continue;

                if (destination.Item1(inputToVerify))
                {
                    _currentDestinationSender = destination.Item2;

                    break;
                }
            }
        }

        public override void ProcessMessage(TMessage message)
        {
            if (_currentDestinationSender != null)
                ForwardMessage(_currentDestinationSender, message);
        }

        public virtual void ForwardMessage(IMessageSender<TMessageQueue, TMessage> sender, TMessage message)
        {
            sender.Send(message);
        }
    }
}
