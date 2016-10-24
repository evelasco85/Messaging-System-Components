using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommonObjects;
using MessageGateway;
using Messaging.Base;
using Messaging.Base.Constructions;
using Messaging.Base.System_Management;
using Messaging.Base.System_Management.SmartProxy;
using MsmqGateway;

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

    public class MonitorCreditBureau : TestMessage<MessageQueue, Message>, IDisposable
    {
        private Timer _sendTimer;
        private Timer _timeoutTimer;
        

        private int _ssn = 1230;
        private string _correlationId;
        Guid _monitorId = Guid.NewGuid();
        private int _millisecondsInterval;
        private int _timeoutMillisecondsInterval;
        string _lastStatus = String.Empty;
        IMessageSender<MessageQueue, Message> _routerControlQueue;

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

        public MonitorCreditBureau(
            string controlBusQueueName,
            string serviceQueueName, string monitorReplyQueueName,
            string routerControlQueueName,
            int secondsInterval, int timeoutSecondsInterval
            )
            : base(
                new MessageSenderGateway(controlBusQueueName),
                new MessageSenderGateway(serviceQueueName),
                new MessageReceiverGateway(monitorReplyQueueName)
                )
        {
            _routerControlQueue = new MessageSenderGateway(routerControlQueueName);

            _millisecondsInterval = secondsInterval*1000;
            _timeoutMillisecondsInterval = timeoutSecondsInterval*1000;

            StartMonitoring();
        }

        void StartMonitoring()
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

        void SendTestMessage()
        {
            CreditBureauRequest request = new CreditBureauRequest
            {
                SSN = _ssn
            };
            Message requestMessage = new Message(request);

            requestMessage.Priority = MessagePriority.AboveNormal;
            
            SendTestMessage(requestMessage);

            _correlationId = requestMessage.Id;
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

        public override void ReceiveTestMessageResponse(Message message)
        {
            if (_timeoutTimer != null)
                _timeoutTimer.Dispose();

            message.Formatter = new XmlMessageFormatter(new Type[] {typeof(CreditBureauReply)});

            CreditBureauReply reply;
            MonitorStatus status = new MonitorStatus
            {
                Status = MonitorStatus.STATUS_OK,
                Description = "No Error",
                MonitorID = _monitorId
            };

            try
            {
                if (message.Body is CreditBureauReply)
                {
                    reply = (CreditBureauReply) message.Body;

                    if (message.CorrelationId != _correlationId)
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

            using (StreamReader reader = new StreamReader(message.BodyStream))
            {
                status.MessageBody = reader.ReadToEnd();
            }

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
            _routerControlQueue.Send(new Message(route));
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

            SendControlBusStatus(new Message(status));
        }
    }
}
