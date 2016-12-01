using System;
using Messaging.Base.Construction;

namespace Bank
{
    class ClientInstance
    {
        IRequestReply_Synchronous _queueService = null;
        private Bank _bank = null;

        public Bank Bank1
        {
            get { return _bank; }
            set { _bank = value; }
        }

        public void SetupBank(
            String bankName,
            String ratePremium,
            String maxLoanTerm
            )
        {
            _bank = new Bank(bankName, Convert.ToDouble(ratePremium), Convert.ToInt32(maxLoanTerm));
        }

        public void SetupQueueService(IRequestReply_Synchronous queueService)
        {
            this._queueService = queueService;
        }

        public void Run()
        {
            if (_queueService != null)
            {
                _queueService.Run();
                Console.WriteLine("Starting Application!");
            }
        }

        public void Stop()
        {
            if (_queueService != null)
            {
                _queueService.StopRunning();
                Console.WriteLine("Stopping Application!");
            }
        }
    }
}
