using System;
using System.Collections.Generic;
using System.Threading;
using CommonObjects;
using Messaging.Base;
using Messaging.Base.System_Management;

namespace ManagementConsole
{
    public class MonitorStatus
    {
        public const string STATUS_ANNOUNCE = "Announce";
        public const string STATUS_TIMEOUT = "Timeout (Backup Credit Bureau is Active, Primary is Down)";
        public const string STATUS_FAILED_CORRELATION = "Failed Correlation";
        public const string STATUS_INVALID_DATA = "Invalid Data";
        public const string STATUS_INVALID_FORMAT = "Invalid Format";
        public const string STATUS_OK = "A-OK (Primary Credit Bureau is Active)";        

        public string Status { get; set; }
        public string Description { get; set; }
        public Guid MonitorID { get; set; }
        public string MessageBody { get; set; }
    }

    public class MonitorCreditBureau<TMessage> : TestMessage<TMessage>, IDisposable
    {
        private Timer _sendTimer;
        private Timer _timeoutTimer;
        

        private int _ssn = 1230;
        private string _correlationId;
        Guid _monitorId = Guid.NewGuid();
        private int _millisecondsInterval;
        private int _timeoutMillisecondsInterval;
        string _lastStatus = String.Empty;
        IMessageSender<TMessage> _routerControlQueue;

        public void Dispose()
        {
            if (_sendTimer != null)
            {
                _sendTimer.Dispose();
                _sendTimer = null;
            }
            if (_timeoutTimer != null)
            {
                _timeoutTimer.Dispose();
                _timeoutTimer = null;
            }
        }

        private Func<TMessage, Tuple<string, bool, string, CreditBureauReply>> _extractCreditBureauReplyFunc;
        private object _priority;
        private Func<TMessage, string> _extractMessageCorrelationIdFunc;

        public MonitorCreditBureau(
            IMessageSender<TMessage> controlBusQueue,
            IMessageSender<TMessage> serviceQueue,
            IMessageReceiver<TMessage> monitorReplyQueue,
            IMessageSender<TMessage> routerControlQueue,
            int secondsInterval, int timeoutSecondsInterval,
            object priority,
            Func<TMessage, string> extractMessageCorrelationIdFunc,
            Func<TMessage, Tuple<string, bool, string, CreditBureauReply>> extractCreditBureauReplyFunc
            )
            : base(
                controlBusQueue,
                serviceQueue,
                monitorReplyQueue
                )
        {
            _routerControlQueue = routerControlQueue;
            _millisecondsInterval = secondsInterval*1000;
            _timeoutMillisecondsInterval = timeoutSecondsInterval*1000;
            _priority = priority;
            _extractMessageCorrelationIdFunc = extractMessageCorrelationIdFunc;
            _extractCreditBureauReplyFunc = extractCreditBureauReplyFunc;
        }

        public override bool StartProcessing()
        {
            if (!ProcessStarted)
            {
                _sendTimer = new Timer(new TimerCallback(this.OnSendTimerEvent), null, _millisecondsInterval, Timeout.Infinite);

                MonitorStatus status = new MonitorStatus
                {
                    Status = MonitorStatus.STATUS_ANNOUNCE,
                    Description = "Monitor On-line",
                    MonitorID = _monitorId
                };

                SwitchRoute(FailOverRouteEnum.Standby);
                SendStatus(status);
                SendTestMessage();

                _lastStatus = status.Status;
            }

            return base.StartProcessing();
        }

        void SendTestMessage()
        {
            CreditBureauRequest request = new CreditBureauRequest
            {
                SSN = _ssn
            };

            TMessage requestMessage  = SendTestMessage(request,
                new List<SenderProperty>()
                {
                    new SenderProperty(){Name = "Priority", Value = _priority}
                });

            _correlationId = _extractMessageCorrelationIdFunc(requestMessage);
        }

        void OnSendTimerEvent(object state)
        {
            SendTestMessage();
            
            _timeoutTimer = new Timer(new TimerCallback(this.OnTimeoutEvent), null, _timeoutMillisecondsInterval, Timeout.Infinite);
        }

        void OnTimeoutEvent(object state)
        {
            MonitorStatus status = new MonitorStatus
            {
                Status = MonitorStatus.STATUS_TIMEOUT,
                Description = "Timeout",
                MonitorID = _monitorId
            };

            SendStatus(status);
            
            _lastStatus = status.Status;

            _timeoutTimer.Dispose();

            _sendTimer = new Timer(new TimerCallback(this.OnSendTimerEvent), null, _millisecondsInterval, Timeout.Infinite);
        }

        public override void ReceiveTestMessageResponse(TMessage message)
        {
            if (_timeoutTimer != null)
                _timeoutTimer.Dispose();

            Tuple<string, bool, string, CreditBureauReply> replyInfo = _extractCreditBureauReplyFunc(message);

            string correlationID = replyInfo.Item1;
            bool isCreditBureauReply = replyInfo.Item2;
            string messageBody = replyInfo.Item3;
            CreditBureauReply reply = replyInfo.Item4;

            MonitorStatus status = new MonitorStatus
            {
                Status = MonitorStatus.STATUS_OK,
                Description = "No Error",
                MonitorID = _monitorId
            };

            try
            {
                if (isCreditBureauReply)
                {
                    if (correlationID != _correlationId)
                    {
                        status.Status = MonitorStatus.STATUS_FAILED_CORRELATION;
                        status.Description = "Incoming message correlation ID does not match outgoing message ID";
                    }
                    else
                    {
                        if (
                            (reply.CreditScore < 300) || (reply.CreditScore > 900) ||
                            (reply.HistoryLength < 1) || (reply.HistoryLength > 24)
                            )
                        {
                            status.Status = MonitorStatus.STATUS_INVALID_DATA;
                            status.Description = "Credit score values out-of-range";
                        }
                    }
                }
                else
                {
                    status.Status = MonitorStatus.STATUS_INVALID_FORMAT;
                    status.Description = "Invalid message format";
                }
            }
            catch (Exception ex)
            {
                status.Status = MonitorStatus.STATUS_INVALID_FORMAT;
                status.Description = "Could not deserialize message body";
            }

            status.MessageBody = messageBody;

            bool statusHasChanges = (
                (status.Status != MonitorStatus.STATUS_OK) ||
                ((status.Status == MonitorStatus.STATUS_OK) && (_lastStatus != MonitorStatus.STATUS_OK))
                );

            if (statusHasChanges)
                SendStatus(status);

            _lastStatus = status.Status;

            if (_sendTimer != null)
                _sendTimer.Dispose();

            _sendTimer = new Timer(new TimerCallback(this.OnSendTimerEvent), null, _millisecondsInterval, Timeout.Infinite);
        }

        void SwitchRoute(FailOverRouteEnum route)
        {
            _routerControlQueue.Send(route);
        }

        void SendStatus(MonitorStatus status)
        {
            switch (status.Status)
            {
                case MonitorStatus.STATUS_OK:
                    SwitchRoute(FailOverRouteEnum.Primary);
                    break;
                case MonitorStatus.STATUS_TIMEOUT:
                    SwitchRoute(FailOverRouteEnum.Backup);
                    break;
            }

            SendControlBusStatus(status);
        }
    }
}
