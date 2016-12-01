using System;
using Messaging.Base.Construction;

namespace CreditBureau
{
    class ClientInstance
    {
        IRequestReply_Synchronous queueService = null;
        CreditBureau proc = new CreditBureau();

        public CreditBureau Proc
        {
            get { return proc; }
            set { proc = value; }
        }

        public void SetupQueueService(IRequestReply_Synchronous queueService)
        {
            this.queueService = queueService;
        }

        public void Run()
        {
            if (queueService != null)
            {
                queueService.Run();
                Console.WriteLine("Starting Application!");
            }
        }

        public void Stop()
        {
            if (queueService != null)
            {
                queueService.StopRunning();
                Console.WriteLine("Stopping Application!");
            }
        }
    }
}
