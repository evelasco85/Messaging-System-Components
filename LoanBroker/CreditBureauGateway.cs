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

        protected Random random	= new Random();

        public CreditBureauGatewayImp(String creditRequestQueueName, String	creditReplyQueueName)
        {
            creditRequestQueue = new MessageSenderGateway(creditRequestQueueName);

            MessageReceiverGateway receiver	= new MessageReceiverGateway(creditReplyQueueName, 
                GetFormatter(), new MessageDelegate<Message>(OnCreditResponse));
            this.creditReplyQueue =	(IMessageReceiver<MessageQueue, Message>)receiver;
        }

        public CreditBureauGatewayImp(IMessageSender<MessageQueue, Message>  creditRequestQueue, IMessageReceiver<MessageQueue, Message> creditReplyQueue)
        {
            this.creditRequestQueue	= creditRequestQueue;

            this.creditReplyQueue =	creditReplyQueue;
            this.creditReplyQueue.ReceiveMessageProcessor	+= new MessageDelegate<Message>(OnCreditResponse);
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
            requestMessage.ResponseQueue = creditReplyQueue.GetQueue();//Return Address
            requestMessage.AppSpecific = random.Next();
	
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
