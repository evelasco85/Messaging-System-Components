using System;

namespace CreditBureauFailOver
{
    class ClientInstance<TMessage>
    {
        FailOverRouter<TMessage> _failOverRouter;
        FailOverControlReceiver<TMessage> _failOverControlReceiver;

        public void SetupFailOver(FailOverRouter<TMessage> failOverRouter, FailOverControlReceiver<TMessage> failOverControlReceiver)
        {
            _failOverRouter = failOverRouter;
            _failOverControlReceiver = failOverControlReceiver;

            _failOverControlReceiver.FailOverRouter = _failOverRouter;
        }

        public void Start()
        {
            if (_failOverControlReceiver != null)
            {
                _failOverControlReceiver.StartProcessing();
                Console.WriteLine("Starting Application!");
            }
        }

        public void Stop()
        {
            if (_failOverControlReceiver != null)
            {
                _failOverControlReceiver.StopProcessing();
                Console.WriteLine("Stopping Application!");
            }
        }
    }
}
