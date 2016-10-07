/* Loan Broker Example with MSMQ
 * from Enterprise Integration Patterns (Addison-Wesley, ISBN 0321200683)
 * 
 * Copyright (c) 2003 Gregor Hohpe 
 *
 * This code is supplied as is. No warranties. 
 */

using System;
using System.Messaging;
using System.Collections;
using MessageGateway;
using CreditBureau;
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

    internal class CreditBureauGatewayImp :	ICreditBureauGateway
    {
        protected IMessageSender<MessageQueue, Message>  creditRequestQueue;
        protected IMessageReceiver<MessageQueue, Message> creditReplyQueue;
        protected IDictionary activeProcesses =	(IDictionary)(new Hashtable());

        IReturnAddress<Message> _creditReturnAddress;

        protected Random random	= new Random();

        public CreditBureauGatewayImp(String creditRequestQueueName, String	creditReplyQueueName)
        {
            MessageDelegate<Message> responseDelegate = new MessageDelegate<Message>(OnCreditResponse);

            Initialize(
                new MessageSenderGateway(creditRequestQueueName),
                new MessageReceiverGateway(creditReplyQueueName, GetFormatter(), responseDelegate),
                responseDelegate);
        }

        public CreditBureauGatewayImp(IMessageSender<MessageQueue, Message>  creditRequestQueue, IMessageReceiver<MessageQueue, Message> creditReplyQueue)
        {
            Initialize(creditRequestQueue, creditReplyQueue, new MessageDelegate<Message>(OnCreditResponse));
        }

        void Initialize(IMessageSender<MessageQueue, Message> creditRequestQueue,
            IMessageReceiver<MessageQueue, Message> creditReplyQueue, MessageDelegate<Message> responseDelegate)
        {
            this.creditRequestQueue = creditRequestQueue;
            this.creditReplyQueue = creditReplyQueue;
            this.creditReplyQueue.ReceiveMessageProcessor += responseDelegate;

            _creditReturnAddress = new ReturnAddress<MessageQueue, Message>(creditReplyQueue,
                (MessageQueue queue, ref Message message) =>
                {
                    message.ResponseQueue = queue;
                });
        }

        protected IMessageFormatter	GetFormatter()
        {
            return new XmlMessageFormatter(new Type[] {typeof(CreditBureauReply)});
        }

        public void	Listen()
        {
            creditReplyQueue.StartReceivingMessages();
        }

        public void	GetCreditScore(CreditBureauRequest quoteRequest, 
            OnCreditReplyEvent OnCreditResponse)
        {
            Message	requestMessage = new Message(quoteRequest);

            _creditReturnAddress.SetMessageReturnAddress(ref requestMessage);
            //requestMessage.ResponseQueue = creditReplyQueue.GetQueue();//Return Address

            requestMessage.AppSpecific = random.Next();//Correlation Identifier
	
            CreditRequestProcess processInstance = new CreditRequestProcess();
            processInstance.callback = OnCreditResponse;
            processInstance.CorrelationID =	requestMessage.AppSpecific;

            creditRequestQueue.Send(requestMessage);

            activeProcesses.Add(processInstance.CorrelationID, processInstance);
        }

        private	void OnCreditResponse(Message msg)
        {
            msg.Formatter =	 GetFormatter();

            CreditBureauReply replyStruct;
            try	
            {
                if (msg.Body is	CreditBureauReply) 
                {
                    replyStruct	= (CreditBureauReply)msg.Body;
                    int	CorrelationID =	msg.AppSpecific;

                    if (activeProcesses.Contains(CorrelationID))
                    {
                        CreditRequestProcess processInstance = 
                            (CreditRequestProcess)(activeProcesses[CorrelationID]);
                        processInstance.callback(replyStruct);
                        activeProcesses.Remove(CorrelationID);
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
