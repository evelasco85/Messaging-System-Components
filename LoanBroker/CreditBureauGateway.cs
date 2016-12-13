/* Loan Broker Example with MSMQ
 * from Enterprise Integration Patterns (Addison-Wesley, ISBN 0321200683)
 * 
 * Copyright (c) 2003 Gregor Hohpe 
 *
 * This code is supplied as is. No warranties. 
 */

using System;
using System.Collections.Generic;
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
        private Func<TMessage, Tuple<bool, CreditBureauReply>> _extractCreditBureauReplyFunc;

        public CreditBureauGatewayImp(
            IMessageSender<TMessage> creditRequestQueue,
            IMessageReceiver<TMessage> creditReplyQueue,
            Func<TMessage, Tuple<bool, CreditBureauReply>> extractCreditBureauReplyFunc
            )
        {
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
            OnCreditReplyEvent OnCreditResponse
            )
        {
            int appSpecific = random.Next();

            _correlationManager.AddEntity(appSpecific,
                new CreditRequestProcess
                {
                    callback = OnCreditResponse,
                });
            _creditRequestQueue.Send(quoteRequest, _creditReturnAddress,
                (assignApplicationId, assignCorrelationId, assignPriority) =>
                {
                    assignApplicationId(appSpecific.ToString());
                });
        }

        private void OnCreditResponse(TMessage msg)
        {
            Tuple<bool, CreditBureauReply> replyInfo = _extractCreditBureauReplyFunc(msg);

            int CorrelationID = Convert.ToInt32(_creditReplyQueue.GetMessageApplicationId(msg));
            bool isCreditBureauReply = replyInfo.Item1;
            CreditBureauReply replyStruct = replyInfo.Item2;

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
