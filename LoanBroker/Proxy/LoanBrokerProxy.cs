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

namespace LoanBroker
{
    public class ProxyJournal
    {
        public string CorrelationId { get; set; }
        public int AppSpecific { get; set; }
    }

    public class SummaryStat
    {
        public ArrayList PerformanceStats { get; set; }
        public ArrayList QueueStats { get; set; }
    }

    public class LoanBrokerProxy : SmartProxyBase<MessageQueue, Message, ProxyJournal>, IDisposable
    {
        private IMessageSender<MessageQueue, Message> _controlBus;

        static ArrayList _performanceStats = ArrayList.Synchronized(new ArrayList());
        static ArrayList _queueStats = ArrayList.Synchronized(new ArrayList());
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
            IMessageReceiver<MessageQueue, Message> input,
            IMessageSender<MessageQueue, Message> serviceRequestSender,
            IReturnAddress<Message> returnAddress,
            IMessageReceiver<MessageQueue, Message> serviceReplyReceiver,
            IMessageSender<MessageQueue, Message> controlBus,
            int interval): base(
                new LoanBrokerProxyRequestConsumer(input, serviceRequestSender, returnAddress, _queueStats),
                new LoanBrokerProxyReplyConsumer(serviceReplyReceiver, _queueStats, _performanceStats)
            )
        {
            _controlBus = controlBus;
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
                SummaryStat stat = new SummaryStat
                {
                    PerformanceStats = currentPerformanceStats,
                    QueueStats = currentQueueStats
                };

                //_controlBus.GetQueue().Send(stat);
            }
        }
    }
}
