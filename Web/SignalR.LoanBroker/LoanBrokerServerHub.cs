using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using CommonObjects;
using MessageGateway;
using Messaging.Base;

namespace Web.SignalR.LoanBroker
{
    [HubName("loanBroker")]
    public class LoanBrokerServerHub : Hub
    {
        readonly LoanBrokerClients _clients;
        MessageReceiverGateway _replyQueue;
        IMessageSender<Message> _requestQueue;
        readonly static ConnectionMapper _connectionMap = new ConnectionMapper();


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

        public string SendRequest(int ssn, double loanAmount, int loanTerm)
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
            string connectionId = GetConnectionId();

            _connectionMap.Add(connectionId, messageId);

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
                    string correlationId = msg.CorrelationId;
                    IEnumerable<string> connections = _connectionMap.GetConnections(correlationId);

                    if (connections.Any())
                    {
                        _clients.LoanReplyReceived(connections, correlationId, reply);
                    }
                    else
                    {
                        //Try broadcasting if not correlation found
                        _clients.BroadcastLoanReplyReceived(correlationId, reply);
                    }

                    _connectionMap.RemoveByMessageId(correlationId);
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

        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            _connectionMap.RemoveByConnectionId(GetConnectionId());

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }
    }
}