namespace Messaging.Base.System_Management.SmartProxy
{
    public interface IMessageConsumer
    {
        bool ProcessStarted { get; }
        bool StartProcessing();
        void StopProcessing();
    }

    public interface IMessageConsumer<TMessage> : IMessageConsumer
    {
        void ProcessMessage(TMessage message);
    }
}
