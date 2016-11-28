using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Threading;
using System.Web;
using CommonObjects;
using MessageGateway;
using Messaging.Base;

namespace Web.SignalR.LoanBroker
{
    [HubName("loanBroker")]
    public class LoanBrokerServerHub : Hub
    {
        private readonly LoanBrokerClients _clients;
        
        MessageReceiverGateway _replyQueue;
        IMessageSender<MessageQueue, Message> _requestQueue;

        public LoanBrokerServerHub() :
            this(LoanBrokerClients.Instance)
        {
        }

        public LoanBrokerServerHub(LoanBrokerClients clients)
        {
            _clients = clients;

            SetupMessagingQueue();
        }

        void SetupMessagingQueue()
        {
            _requestQueue = new MessageSenderGateway(ToPath("loanRequestQueue"));
            _replyQueue = new MessageReceiverGateway(ToPath("loanReplySignalR_Queue"), GetFormatter());
            _replyQueue.ReceiveMessageProcessor += new MessageDelegate<Message>(OnMessage);

            _replyQueue.StartReceivingMessages();
        }

        IMessageFormatter GetFormatter()
        {
            return new XmlMessageFormatter(new Type[] { typeof(LoanQuoteReply) });
        }

        public string SendRequest(int ssn, double loanAmount, int loanTerm )
        {
            LoanQuoteRequest req = new LoanQuoteRequest();

            req.SSN = ssn;
            req.LoanAmount = loanAmount;
            req.LoanTerm = loanTerm;

            Message msg = new Message(req);
            msg.AppSpecific = req.SSN;

            _replyQueue.AsReturnAddress().SetMessageReturnAddress(ref msg);
            _requestQueue.Send(msg);

            Thread.Sleep(100);

            string messageId = msg.Id;

            return messageId;
        }

        void OnMessage(Message msg)
        {
            msg.Formatter = GetFormatter();
            try
            {
                if (msg.Body is LoanQuoteReply)
                {
                    LoanQuoteReply reply = (LoanQuoteReply)msg.Body;

                    _clients.LoanReplyReceived(reply);
                }
                else
                {
                    Console.WriteLine("INVALID message received!!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.ToString());
            }
        }

        String ToPath(String arg)
        {
            return ".\\private$\\" + arg;
        }

        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}