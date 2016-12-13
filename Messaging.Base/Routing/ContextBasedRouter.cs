using Messaging.Base.System_Management.SmartProxy;
using System;
using System.Collections.Generic;

namespace Messaging.Base.Routing
{
    public class ContextBasedRouter<TMessage, TInput> : MessageConsumer<TMessage>, IContextBasedRouter<TMessage, TInput>
    {
        IList<Tuple<Func<TInput, bool>, IRawMessageSender<TMessage>>> _destinations;
        IRawMessageSender<TMessage> _currentDestinationSender;

        public bool DestinationIsSet
        {
            get { return _currentDestinationSender != null; }
        }

        public ContextBasedRouter(IMessageReceiver<TMessage> inputQueue) : base(inputQueue)
        {
            _destinations = new List<Tuple<Func<TInput, bool>, IRawMessageSender<TMessage>>>();
        }

        public IContextBasedRouter<TMessage, TInput> AddSender(Func<TInput, bool> invocationConditionFunction, IRawMessageSender<TMessage> destination)
        {
            _destinations.Add(new Tuple<Func<TInput, bool>, IRawMessageSender<TMessage>>(invocationConditionFunction, destination));

            return this;
        }

        public void SwitchDestination(TInput inputToVerify)
        {
            for(int index = 0; index < _destinations.Count; index++)
            {
                Tuple<Func<TInput, bool>, IRawMessageSender<TMessage>> destination = _destinations[index];

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

        public virtual void ForwardMessage(IRawMessageSender<TMessage> sender, TMessage message)
        {
            sender.SendRawMessage(message);
        }
    }
}
