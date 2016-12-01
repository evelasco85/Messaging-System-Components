
namespace Messaging.Base.System_Management.SmartProxy
{
    public abstract class MessageConsumer<TMessage> : IMessageConsumer<TMessage>
    {
        private IMessageReceiver<TMessage> _receiver;
        bool _processStarted = false;

        public bool ProcessStarted
        {
            get { return _processStarted; }
        }

        public MessageConsumer(IMessageReceiver<TMessage> receiver)
        {
            _receiver = receiver;

            _receiver.ReceiveMessageProcessor += new MessageDelegate<TMessage>(ProcessMessage);
        }

        public bool StartProcessing()
        {
            if ((_receiver != null) && (!_processStarted))
            {
                _receiver.StartReceivingMessages();

                _processStarted = true;
            }

            return _processStarted;
        }

        public abstract void ProcessMessage(TMessage message);

        public virtual void StopProcessing()
        {
            if ((_receiver != null) && (_processStarted))
            {
                _receiver.StopReceivingMessages();

                _processStarted = false;
            }
        }
    }
}
