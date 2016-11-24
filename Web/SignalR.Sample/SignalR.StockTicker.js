/// <reference path="../Scripts/jquery-1.10.2.js" />
/// <reference path="../Scripts/jquery.signalR-2.1.1.js" />

/*!
    ASP.NET SignalR Stock Ticker Sample
*/

// Crockford's supplant method (poor man's templating)
if (!String.prototype.supplant) {
    String.prototype.supplant = function (o) {
        return this.replace(/{([^{}]*)}/g,
            function (a, b) {
                var r = o[b];
                return typeof r === 'string' || typeof r === 'number' ? r : a;
            }
        );
    };
}

// A simple background color flash effect that uses jQuery Color plugin
jQuery.fn.flash = function (color, duration) {
    var current = this.css('backgroundColor');
    this.animate({ backgroundColor: 'rgb(' + color + ')' }, duration / 2)
        .animate({ backgroundColor: current }, duration / 2);
};

$(function () {

    //-->ticker A.K.A 'StockTickerHub' contains both client and server objects of SignalR
    //-->'ticker.client' for client specific implementation
    //-->'ticker.server' for hub specific(non-extendible) implementation
    //-->'$.connection.stockTicker' where 'stockTicker' is the hubname in server implementation
    var ticker = $.connection.stockTicker, // the generated client-side hub proxy
        up = '▲',
        down = '▼',
        $stockTable = $('#stockTable'),
        $stockTableBody = $stockTable.find('tbody'),
        rowTemplate = '<tr data-symbol="{Symbol}"><td>{Symbol}</td><td>{Price}</td><td>{DayOpen}</td><td>{DayHigh}</td><td>{DayLow}</td><td><span class="dir {DirectionClass}">{Direction}</span> {Change}</td><td>{PercentChange}</td></tr>',
        $stockTicker = $('#stockTicker'),
        $stockTickerUl = $stockTicker.find('ul'),
        liTemplate = '<li data-symbol="{Symbol}"><span class="symbol">{Symbol}</span> <span class="price">{Price}</span> <span class="change"><span class="dir {DirectionClass}">{Direction}</span> {Change} ({PercentChange})</span></li>';

    function formatStock(stock) {
        return $.extend(stock, {
            Price: stock.Price.toFixed(2),
            PercentChange: (stock.PercentChange * 100).toFixed(2) + '%',
            Direction: stock.Change === 0 ? '' : stock.Change >= 0 ? up : down,
            DirectionClass: stock.Change === 0 ? 'even' : stock.Change >= 0 ? 'up' : 'down'
        });
    }

    function scrollTicker() {
        var w = $stockTickerUl.width();
        $stockTickerUl.css({ marginLeft: w });
        $stockTickerUl.animate({ marginLeft: -w }, 15000, 'linear', scrollTicker);
    }

    function stopTicker() {
        $stockTickerUl.stop();
    }

    function init() {
        //-->Calls StockTickerHub.GetAllStocks()
        //-->Retrieve enumerated 'stocks'
        return ticker.server.getAllStocks().done(function (stocks) {
            $stockTableBody.empty();
            $stockTickerUl.empty();
            //-->Perform iteration against enumerated stocks
            $.each(stocks, function () {
                var stock = formatStock(this);
                $stockTableBody.append(rowTemplate.supplant(stock));
                $stockTickerUl.append(liTemplate.supplant(stock));
            });
        });
    }

    // Add client-side hub methods that the server will call
    //-->Attach dynamic/new function/method to client that the server can call
    //-->Register exposable client-side implementations to server
    //-->Extending 'this' SignalR client
    $.extend(ticker.client, {
        //-->Available in server as 'Clients.All.updateStockPrice(stock)'
        updateStockPrice: function (stock) {
            var displayStock = formatStock(stock),
                $row = $(rowTemplate.supplant(displayStock)),
                $li = $(liTemplate.supplant(displayStock)),
                bg = stock.LastChange < 0
                        ? '255,148,148' // red
                        : '154,240,117'; // green

            $stockTableBody.find('tr[data-symbol=' + stock.Symbol + ']')
                .replaceWith($row);
            $stockTickerUl.find('li[data-symbol=' + stock.Symbol + ']')
                .replaceWith($li);

            $row.flash(bg, 1000);
            $li.flash(bg, 1000);
        },

        //-->Available in server as 'Clients.All.marketOpened()'
        marketOpened: function () {
            $("#open").prop("disabled", true);
            $("#close").prop("disabled", false);
            $("#reset").prop("disabled", true);
            scrollTicker();
        },

        //-->Available in server as 'Clients.All.marketClosed()'
        marketClosed: function () {
            $("#open").prop("disabled", false);
            $("#close").prop("disabled", true);
            $("#reset").prop("disabled", false);
            stopTicker();
        },

        //-->Available in server as 'Clients.All.marketReset()'
        marketReset: function () {
            return init();
        }
    });

    // Start the connection
    $.connection.hub.start()
        .then(init)
        .then(function () {
            //-->Invokes server call('StockTickerHub.GetMarketState()')
            return ticker.server.getMarketState();
        })
        .done(function (state) {
            if (state === 'Open') {
                //-->Invokes attached/created 'marketOpened()' method, specific to 'this' client
                ticker.client.marketOpened();
            } else {
                //-->Invokes attached/created 'marketClosed()' method, specific to 'this' client
                ticker.client.marketClosed();
            }

            // Wire up the buttons
            $("#open").click(function () {
                //-->Invokes server call('StockTickerHub.openMarket()')
                ticker.server.openMarket();
            });

            $("#close").click(function () {
                //-->Invokes server call('StockTickerHub.closeMarket()')
                ticker.server.closeMarket();
            });

            $("#reset").click(function () {
                //-->Invokes server call('StockTickerHub.reset()')
                ticker.server.reset();
            });
        });
});