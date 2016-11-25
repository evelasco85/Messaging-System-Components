using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.SignalR.LoanBroker
{
    public class LoanBrokerClients
    {
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
    }
}