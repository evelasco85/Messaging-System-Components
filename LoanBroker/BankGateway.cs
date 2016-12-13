using System;
using System.Linq;
using Bank;
using Messaging.Base;
using LoanBroker.Bank;
using Messaging.Base.Routing;
using Messaging.Base.Constructions;

namespace LoanBroker
{
    internal class BankGateway<TMessage>
    {
        protected IMessageReceiver<TMessage> _bankReplyQueue;
        protected ConnectionsManager<TMessage> _connectionManager;
        IAggregator<int, BankQuoteReply, BankQuoteReply, BankQuoteAggregate> _aggregator = new Aggregator<int, BankQuoteReply, BankQuoteReply, BankQuoteAggregate>();
        protected int _aggregationCorrelationID;
        IReturnAddress<TMessage> _bankReturnAddress;
        private Func<int, BankQuoteRequest, TMessage> _constructBankQuoteRequestMessageFunc;
        private Func<TMessage, Tuple<bool, BankQuoteReply>> _extractBankQuoteReplyFunc;

        public BankGateway(
            IMessageReceiver<TMessage> receiver,
            ConnectionsManager<TMessage> connectionManager,
            Func<int, BankQuoteRequest, TMessage> constructBankQuoteRequestMessageFunc,
            Func<TMessage, Tuple<bool, BankQuoteReply>> extractBankQuoteReplyFunc
            )
        {
            receiver.ReceiveMessageProcessor += new MessageDelegate<TMessage>(OnBankMessage);

            _constructBankQuoteRequestMessageFunc = constructBankQuoteRequestMessageFunc;
            _extractBankQuoteReplyFunc = extractBankQuoteReplyFunc;
            _bankReplyQueue = receiver;
            _connectionManager = connectionManager; 
            _aggregationCorrelationID = 0;

            _bankReturnAddress = _bankReplyQueue.AsReturnAddress();
        }

        public void Listen()
        {
            _bankReplyQueue.StartReceivingMessages();
        }

        public void GetBestQuote(BankQuoteRequest quoteRequest, OnNotifyAggregationCompletion<BankQuoteReply> onBestQuoteEvent)
        {
            TMessage requestMessage = _constructBankQuoteRequestMessageFunc(_aggregationCorrelationID, quoteRequest);

            _bankReturnAddress.SetMessageReturnAddress(ref requestMessage);


            IMessageSender<TMessage>[] eligibleBanks = 
                _connectionManager.GetEligibleBankQueues(quoteRequest.CreditScore, quoteRequest.HistoryLength, 
                quoteRequest.LoanAmount);

            _aggregator.AddAggregate(_aggregationCorrelationID, new BankQuoteAggregate(_aggregationCorrelationID, eligibleBanks.Length, onBestQuoteEvent));
            _aggregationCorrelationID++;

            MessageRouter
                .GetInstance()
                .SendToRecipent(requestMessage, eligibleBanks.ToList());
        }


        private void OnBankMessage(TMessage msg)
        {
            Tuple<bool, BankQuoteReply> replyInfo = _extractBankQuoteReplyFunc(msg);

            int aggregationCorrelationId = Convert.ToInt32(_bankReplyQueue.GetMessageApplicationId(msg));

            bool isBankQuoteReply = replyInfo.Item1;
            BankQuoteReply replyStruct = replyInfo.Item2;

            try 
            {
                if (isBankQuoteReply) 
                {
                    Console.WriteLine("Quote {0:0.00}% {1} {2}", replyStruct.InterestRate, replyStruct.QuoteID, replyStruct.ErrorCode);

                    if (_aggregator.Contains(aggregationCorrelationId))
                    {
                        BankQuoteAggregate aggregate = _aggregator.GetAggregate(aggregationCorrelationId);

                        aggregate.AggregateValue(replyStruct);

                        if (aggregate.IsComplete()) 
                        {
                            aggregate.NotifyAggregationCompletion();
                            _aggregator.RemoveAggregate(aggregate);
                        }
                    }
                    else 
                    { Console.WriteLine("Incoming bank response does not match any aggregate"); }
                }
                else
                { Console.WriteLine("Illegal request."); }
            }
            catch (Exception e) 
            {
                Console.WriteLine("Exception: {0}", e.ToString());    
            }
        }

        public int GetOutstandingAggregateCount()
        {
            return _aggregator.GetAggregateCount();
        }
    }
}
