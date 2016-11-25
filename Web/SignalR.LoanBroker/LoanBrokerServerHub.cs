using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.SignalR.LoanBroker
{
    [HubName("loanBroker")]
    public class LoanBrokerServerHub : Hub
    {
        private readonly LoanBrokerClients _clients;

        public LoanBrokerServerHub() :
            this(LoanBrokerClients.Instance)
        {

        }

        public LoanBrokerServerHub(LoanBrokerClients clients)
        {
            _clients = clients;
        }

        public void SetupMessageQueue()
        {

        }
    }
}