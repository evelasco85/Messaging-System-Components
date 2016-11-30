/* Loan Broker Example with MSMQ
 * from Enterprise Integration Patterns (Addison-Wesley, ISBN 0321200683)
 * 
 * Copyright (c) 2003 Gregor Hohpe 
 *
 * This code is supplied as is. No warranties. 
 */

using System;
using CommonObjects;
using Messaging.Base;
using LoanBroker.CreditBureau;
using Messaging.Base.Constructions;

namespace LoanBroker
{
    public interface ICreditBureauGateway
    {
        void GetCreditScore(CreditBureauRequest	quoteRequest, OnCreditReplyEvent OnCreditResponse);
        void Listen();
    }

    internal class CreditBureauGatewayImp<TMessage> :	ICreditBureauGateway
    {
        protected IMessageSender<TMessage> _creditRequestQueue;
        protected IMessageReceiver<TMessage> _creditReplyQueue;
        
        protected Random random = new Random();

        IReturnAddress<TMessage> _creditReturnAddress;
        ICorrelationManager<int, CreditRequestProcess> _correlationManager = new CorrelationManager<int, CreditRequestProcess>();
        private Func<int, CreditBureauRequest, TMessage> _constructCreditBureauRequestMessageFunc;
        private Func<TMessage, Tuple<int, bool, CreditBureauReply>> _extractCreditBureauReplyFunc;

        public CreditBureauGatewayImp(
            IMessageSender<TMessage> creditRequestQueue,
            IMessageReceiver<TMessage> creditReplyQueue,
            Func<int, CreditBureauRequest, TMessage> constructCreditBureauRequestMessageFunc,
            Func<TMessage, Tuple<int, bool, CreditBureauReply>> extractCreditBureauReplyFunc
            )
        {
            _constructCreditBureauRequestMessageFunc = constructCreditBureauRequestMessageFunc;
            _extractCreditBureauReplyFunc = extractCreditBureauReplyFunc;

            Initialize(creditRequestQueue, creditReplyQueue, new MessageDelegate<TMessage>(OnCreditResponse));
        }

        void Initialize(IMessageSender<TMessage> creditRequestQueue,
            IMessageReceiver<TMessage> creditReplyQueue, MessageDelegate<TMessage> responseDelegate)
        {
            this._creditRequestQueue = creditRequestQueue;
            this._creditReplyQueue = creditReplyQueue;
            this._creditReplyQueue.ReceiveMessageProcessor += responseDelegate;
            _creditReturnAddress = creditReplyQueue.AsReturnAddress();
        }

        public void	Listen()
        {
            _creditReplyQueue.StartReceivingMessages();
        }

        public void	GetCreditScore(CreditBureauRequest quoteRequest, 
            OnCreditReplyEvent OnCreditResponse)
        {
            int appSpecific = random.Next();
            TMessage requestMessage = _constructCreditBureauRequestMessageFunc(appSpecific, quoteRequest);

            _creditReturnAddress.SetMessageReturnAddress(ref requestMessage);
            _correlationManager.AddEntity(appSpecific,
                new CreditRequestProcess
                {
                    callback = OnCreditResponse,
                });

            _creditRequestQueue.Send(requestMessage);
        }

        private void OnCreditResponse(TMessage msg)
        {
            Tuple<int, bool, CreditBureauReply> replyInfo = _extractCreditBureauReplyFunc(msg);

            int CorrelationID = replyInfo.Item1;
            bool isCreditBureauReply = replyInfo.Item2;
            CreditBureauReply replyStruct = replyInfo.Item3;

            try	
            {
                if (isCreditBureauReply) 
                {
                    if (_correlationManager.EntityExists(CorrelationID))
                    {
                        _correlationManager
                            .GetEntity(CorrelationID)
                            .callback(replyStruct);

                        _correlationManager.RemoveEntity(CorrelationID);
                    }
                    else { Console.WriteLine("Incoming credit response does	not	match any request"); }
                }
                else
                { Console.WriteLine("Illegal reply."); }
            }
            catch (Exception e)	
            {
                Console.WriteLine("Exception: {0}",	e.ToString());	  
            }
        }
    }

}
