using System;
using System.Collections;
using System.Linq;
using System.Messaging;
using System.Threading;
using Messaging.Base;
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
        public string Content { get; set; }
        //public ArrayList PerformanceStats { get; set; }
        //public ArrayList QueueStats { get; set; }
        public double AverageReplyDuration { get; set; }
        public double AverageOutstandingRequest { get; set; }
    }

    public class LoanBrokerProxy : SmartProxyBase<MessageQueue, Message, ProxyJournal>, IDisposable
    {
        private IMessageSender<Message> _controlBus;

        static ArrayList s_performanceStats = ArrayList.Synchronized(new ArrayList());
        static ArrayList s_queueStats = ArrayList.Synchronized(new ArrayList());
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
            IMessageReceiver<Message> input,
            IMessageSender<Message> serviceRequestSender,
            IMessageReceiver<Message> serviceReplyReceiver,
            IMessageSender<Message> controlBus,
            int interval): base(
                new LoanBrokerProxySmartProxyRequestConsumer(input, serviceRequestSender, serviceReplyReceiver.AsReturnAddress(), s_queueStats),
                new LoanBrokerProxySmartProxyReplyConsumer(serviceReplyReceiver, s_queueStats, s_performanceStats, controlBus)
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

            lock (s_queueStats)
            {
                currentQueueStats = (ArrayList) s_queueStats.Clone();

                s_queueStats.Clear();
            }

            lock (s_performanceStats)
            {
                currentPerformanceStats = (ArrayList) s_performanceStats.Clone();

                s_performanceStats.Clear();
            }

            if (_controlBus != null)
            {
                double performanceCount = (currentPerformanceStats.Count > 0) ? currentPerformanceStats.Count : 1;
                double queueCount = (currentQueueStats.Count > 0) ? currentQueueStats.Count : 1;

                SummaryStat stat = new SummaryStat
                {
                    //PerformanceStats = currentPerformanceStats,
                    //QueueStats = currentQueueStats,
                    AverageReplyDuration = currentPerformanceStats
                        .ToArray()
                        .Select(statData => Convert.ToDouble(statData))
                        .Sum() / performanceCount,
                    AverageOutstandingRequest = currentQueueStats
                        .ToArray()
                        .Select(statData => Convert.ToDouble(statData))
                        .Sum() / queueCount,
                };

                _controlBus.Send(new Message(stat));
            }
        }
    }
}
