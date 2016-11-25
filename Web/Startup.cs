using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Web.Startup))]
namespace Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            //Install-Package Microsoft.AspNet.SignalR
            //Install-Package Microsoft.AspNet.SignalR.Sample
            //Browse to ~/SignalR.Sample/StockTicker.html in two browsers and click the Open Market button.
            Microsoft.AspNet.SignalR.StockTicker.Startup.ConfigureSignalR(app);
            SignalR.LoanBroker.Startup.ConfigureSignalR(app);
        }
    }
}
