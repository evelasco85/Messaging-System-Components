using LoanBroker.Bank;
using System;
using CommonObjects;
using Messaging.Base.Construction;
using Messaging.Base.Routing;

namespace LoanBroker.LoanBroker
{
    internal class ProcessManager<TMessage>
    {
        IProcessManager<string, Process<TMessage>, ProcessManager<TMessage>> _manager;
        private IRequestReply_Asynchronous<TMessage> _queueService;
        private Func<TMessage, string> _extractProcessIdFunc;
        private BankGateway _bankInterface;
        private ICreditBureauGateway _creditBureauInterface;

        public ProcessManager(
            BankGateway bankInterface,
            ICreditBureauGateway creditBureauInterface
            )
        {
            _bankInterface = bankInterface;
            _creditBureauInterface = creditBureauInterface;
            _manager = new ProcessManager<string, Process<TMessage>, ProcessManager<TMessage>>(
                this,
                ChildProcessNotification
                );
        }

        public BankGateway BankInterface
        {
            get { return _bankInterface; }
        }

        public ICreditBureauGateway CreditBureauInterface
        {
            get { return _creditBureauInterface; }
        }

        public Type GetRequestBodyType()
        {
            return typeof(LoanQuoteRequest);
        }

        public void ProcessRequestMessage(Object o, TMessage incomingMessage)
        {
            LoanQuoteRequest quoteRequest = (LoanQuoteRequest)o;

            String processID = _extractProcessIdFunc(incomingMessage);
            Process<TMessage> newProcess = new Process<TMessage>(processID, quoteRequest, incomingMessage);

            _manager.AddProcess(newProcess);

            newProcess.StartProcess();
        }

        public void AddSetup(
            IRequestReply_Asynchronous<TMessage> queueService,
            Func<TMessage, string> extractProcessIdFunc
            )
        {
            _queueService = queueService;
            _extractProcessIdFunc = extractProcessIdFunc;
        }

        public void SendReply(Object responseObject, TMessage originalRequestMessage)
        {
            if(_queueService != null)
                _queueService.SendReply(responseObject, originalRequestMessage);
        }

        void ChildProcessNotification(IProcess<string, Process<TMessage>, ProcessManager<TMessage>> process)
        {
            _manager.RemoveProcess(process);
            Console.WriteLine("Current outstanding aggregate count: {0}", _bankInterface.GetOutstandingAggregateCount());
        }
    }
}
