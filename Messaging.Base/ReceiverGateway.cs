using Messaging.Base.Constructions;

namespace Messaging.Base
{
    /// <summary>
    /// Abstract implementation for receiving messages
    /// </summary>
    /// <typeparam name="TMessageQueue">Type of message queue to listen from</typeparam>
    /// <typeparam name="TMessage">Type of message to receive</typeparam>
    public abstract class ReceiverGateway<TMessageQueue, TMessage> : IMessageReceiver<TMessageQueue, TMessage>
    {
        IQueueGateway<TMessageQueue> _queueGateway;
        private bool _started = false;

        public string QueueName
        {
            get { return _queueGateway.QueueName; }
        }

        public bool Started
        {
            get { return _started;}
            protected set { _started = value; }
        }

        public abstract MessageDelegate<TMessage> ReceiveMessageProcessor
        {
            get;
            set;
        }

        public ReceiverGateway(IQueueGateway<TMessageQueue> queueGateway)
        {
            _queueGateway = queueGateway;

            SetupReceiver();
        }

        public TMessageQueue GetQueue()
        {
            return _queueGateway.GetQueue();
        }

        public abstract IReturnAddress<TMessage> AsReturnAddress();

        public abstract void StartReceivingMessages();
        public abstract void StopReceivingMessages();
        public abstract void SetupReceiver();

        public abstract string GetMessageId(TMessage message);
        public abstract string GetMessageCorrelationId(TMessage message);
        public abstract string GetMessageApplicationId(TMessage message);
    }
}
