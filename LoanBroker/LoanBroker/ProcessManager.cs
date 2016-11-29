using LoanBroker.Bank;
using System;
using CommonObjects;
using Messaging.Base.Construction;
using Messaging.Base.Routing;

namespace LoanBroker.LoanBroker
{
    internal class ProcessManager<TMessage>
    {
        public ICreditBureauGateway CreditBureauInterface { get; private set; }
        public BankGateway BankInterface { get; private set; }

        IProcessManager<string, Process<TMessage>, ProcessManager<TMessage>> _manager;
        private IRequestReply_Asynchronous<TMessage> _queueService;
        private Func<TMessage, string> _extractProcessIdFunc;

        public ProcessManager(
            String creditRequestQueueName, String creditReplyQueueName,
            String bankReplyQueueName, ConnectionsManager connectionManager)
            :this(
                 (new CreditBureauGatewayImp(creditRequestQueueName, creditReplyQueueName)),
                 bankReplyQueueName,
                 connectionManager)
        {}

        public ProcessManager(
            ICreditBureauGateway creditBureau,
            String bankReplyQueueName, ConnectionsManager connectionManager)
        {

            CreditBureauInterface = creditBureau;
            CreditBureauInterface.Listen();

            BankInterface = new BankGateway(bankReplyQueueName, connectionManager);
            BankInterface.Listen();

            _manager = new ProcessManager<string, Process<TMessage>, ProcessManager<TMessage>>(
                this,
                ChildProcessNotification
                );

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
            Console.WriteLine("Current outstanding aggregate count: {0}", BankInterface.GetOutstandingAggregateCount());
        }
    }
}
