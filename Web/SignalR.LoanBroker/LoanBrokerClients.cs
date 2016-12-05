using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Web;
using CommonObjects;
using MsmqGateway;

namespace Web.SignalR.LoanBroker
{
    public class LoanBrokerClients
    {
        readonly object _messageQueueReplyLock = new object();

        private readonly static Lazy<LoanBrokerClients> _instance = new Lazy<LoanBrokerClients>(
           () => new LoanBrokerClients(GlobalHost.ConnectionManager.GetHubContext<LoanBrokerServerHub>().Clients));

        private IHubConnectionContext<dynamic> Clients
        {
            get;
            set;
        }

        public static LoanBrokerClients Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private LoanBrokerClients(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;
        }

        public void LoanReplyReceived(IEnumerable<string> connectionIds, string messageId, LoanQuoteReply reply)
        {
            lock (_messageQueueReplyLock)
            {
                foreach (string connectionId in connectionIds)
                {
                    Clients.Client(connectionId).messageQueueReplyReceived(messageId, reply);
                }
            }
        }

        public void BroadcastLoanReplyReceived(string messageId, LoanQuoteReply reply)
        {
            lock (_messageQueueReplyLock)
            {
                Clients.All.messageQueueReplyReceived(messageId, reply);
            }
        }
    }
}