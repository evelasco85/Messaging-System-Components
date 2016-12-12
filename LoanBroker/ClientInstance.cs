using System;
using Bank;
using CommonObjects;
using LoanBroker.Bank;
using LoanBroker.LoanBroker;
using Messaging.Base;
using Messaging.Base.Construction;
using Messaging.Base.System_Management.SmartProxy;

namespace LoanBroker
{
    class ClientInstance<TMessageQueue, TMessage> : IDisposable
    {
        LoanBrokerProxy<TMessageQueue, TMessage> _loanBrokerProxy = null;
        private BankGateway<TMessage> _bankInterface = null;
        private ProcessManager<TMessage> _processManager = null;
        private ICreditBureauGateway _creditBureauInterface = null;
        IRequestReply_Asynchronous<TMessage> _queueService = null;



        public ProcessManager<TMessage> ProcessManager
        {
            get { return _processManager; }
            set { _processManager = value; }
        }

        public void Dispose()
        {
            if (_loanBrokerProxy != null)
            {
                _loanBrokerProxy.Dispose();
                _loanBrokerProxy = null;
            }
        }

        public void SetupLoanBrokerProxy(
            IMessageSender<TMessage> controlBus,
            ISmartProxyRequestConsumer<TMessageQueue, TMessage, ProxyJournal> requestConsumer,
            ISmartProxyReplyConsumer<TMessageQueue, TMessage, ProxyJournal> replyConsumer
            )
        {
            _loanBrokerProxy = new LoanBrokerProxy<TMessageQueue, TMessage>(
                controlBus,
                requestConsumer,
                replyConsumer,
                3);
        }

        public void SetupBankGateway(
            IMessageReceiver<TMessage>  bankReplyReceiver,
            ConnectionsManager<TMessage> connectionManager,
            Func<int, BankQuoteRequest, TMessage> constructBankQuoteRequestMessageFunc,
            Func<TMessage, Tuple<int, bool, BankQuoteReply>> extractBankQuoteReplyFunc
            )
        {
            _bankInterface = new BankGateway<TMessage>(
                bankReplyReceiver,
                connectionManager,
                constructBankQuoteRequestMessageFunc,
                extractBankQuoteReplyFunc
                );
        }

        public void SetupCreditBureauInterface(
            IMessageSender<TMessage> creditBureauSender,
            IMessageReceiver<TMessage> creditBureauReceiver,
            Func<TMessage, Tuple<int, bool, CreditBureauReply>> extractCreditBureauReplyFunc
            )
        {
            _creditBureauInterface = new CreditBureauGatewayImp<TMessage>
                (
                creditBureauSender,
                creditBureauReceiver,
                extractCreditBureauReplyFunc
                );
        }

        public void SetupProcessManager()
        {
            _processManager = new ProcessManager<TMessage>(
                _bankInterface,
                _creditBureauInterface
                );
        }

        public void SetupQueueService(IRequestReply_Asynchronous<TMessage> queueService)
        {
            _queueService = queueService;
            _processManager.HookQueueService(queueService);
        }

        public void HookMessageIdExtractor(Func<TMessage, string> extractProcessIdFunc)
        {
            _processManager.HookProcessIdExtractor(extractProcessIdFunc);
        }

        public void StartProcessingManagerListening()
        {
            _processManager.CreditBureauInterface.Listen();
            _processManager.BankInterface.Listen();
        }

        public void Start()
        {
            if ((_queueService != null) && (_loanBrokerProxy != null))
            {
                _loanBrokerProxy.Process();
                _queueService.Run();
            }
        }

        public void Stop()
        {
            if ((_queueService != null) && (_loanBrokerProxy != null))
            {
                _loanBrokerProxy.StopProcessing();
                _queueService.StopRunning();
            }
        }
    }
}
