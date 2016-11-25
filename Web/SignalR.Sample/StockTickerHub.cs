using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR.Hubs;

namespace Microsoft.AspNet.SignalR.StockTicker
{
    //Server hub
    //Available to http client javascript implementation
    //Reference in SignalR.StockTiker.js by pointing to '$.connection.stockTicker'
    [HubName("stockTicker")]
    public class StockTickerHub : Hub
    {
        private readonly StockTicker _stockTicker;

        public StockTickerHub() :
            this(StockTicker.Instance)
        {

        }

        public StockTickerHub(StockTicker stockTicker)
        {
            _stockTicker = stockTicker;
        }

        public IEnumerable<Stock> GetAllStocks()
        {
            return _stockTicker.GetAllStocks();
        }

        public string GetMarketState()
        {
            return _stockTicker.MarketState.ToString();
        }

        public void OpenMarket()
        {
            //Broadcast status updates (STATUS = Open) to all connected clients
            _stockTicker.OpenMarket();
        }

        public void CloseMarket()
        {
            //Broadcast status updates (STATUS = Closed) to all connected clients
            _stockTicker.CloseMarket();
        }

        public void Reset()
        {
            //Broadcast 'reset' actions to all connected clients
            _stockTicker.Reset();
        }
    }
}