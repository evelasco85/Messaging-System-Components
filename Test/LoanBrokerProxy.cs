using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessageGateway;
using Messaging.Base;
using Messaging.Base.Constructions;
using Messaging.Base.System_Management.SmartProxy;

namespace Test
{
    public class ProxyJournal
    {
        public string CorrelationId { get; set; }
        public int AppSpecific { get; set; }
    }

    public class LoanBrokerProxy : SmartProxyBase<MessageQueue, Message, ProxyJournal>, IDisposable
    {
        private IMessageReceiver<MessageQueue, Message> _input;
        private IMessageSender<MessageQueue, Message> _controlBus;

        ArrayList _performanceStats;
        ArrayList _queueStats;
        private Timer _timer;
        private int _interval;

        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }

        public LoanBrokerProxy(
            MessageReceiverGateway input,
            ISmartProxyRequestConsumer<MessageQueue, Message, ProxyJournal> requestConsumer,
            ISmartProxyReplyConsumer<MessageQueue, Message, ProxyJournal> replyConsumer,
            MessageSenderGateway controlBus,
            int interval): base(requestConsumer, replyConsumer)
        {
            _performanceStats = ArrayList.Synchronized(new ArrayList());
            _queueStats = ArrayList.Synchronized(new ArrayList());

            _controlBus = controlBus;
            _input = input;
            _interval = interval;
        }

        public override void Process()
        {
            base.Process();

            TimerCallback callback = new TimerCallback(this.OnTimerEvent);

            _timer = new Timer(callback, null, _interval * 1000, _interval * 1000);
        }

        void OnTimerEvent(object state)
        {
            ArrayList currentQueueStats = new ArrayList();
            ArrayList currentPerformanceStats = new ArrayList();

            lock (_queueStats)
            {
                currentQueueStats = (ArrayList) _queueStats.Clone();

                _queueStats.Clear();
            }

            lock (_performanceStats)
            {
                currentPerformanceStats = (ArrayList) _performanceStats.Clone();

                _performanceStats.Clear();
            }

            if (_controlBus != null)
            {
                Message message = new Message(new Tuple<ArrayList, ArrayList>(
                    currentQueueStats, currentPerformanceStats
                    ));

                //_controlBus.Send(message);
            }
        }
    }
}
