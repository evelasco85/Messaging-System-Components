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
        public const string STATUS_TIMEOUT = "Timeout";
        public const string STATUS_FAILED_CORRELATION = "Failed Correlation";
        public const string STATUS_INVALID_DATA = "Invalid Data";
        public const string STATUS_INVALID_FORMAT = "Invalid Format";
        public const string STATUS_OK = "A-OK";        

        public string Status { get; set; }
        public string Description { get; set; }
        public Guid MonitorID { get; set; }
        public string MessageBody { get; set; }
    }

    public class MonitorCreditBureau : TestMessage<MessageQueue, Message>, IDisposable
    {
        private Timer _sendTimer;
        private Timer _timeoutTimer;
        

        private int _ssn = 123;
        const int TEST_MESSAGE_ID = 1230;       //Used as a correlation id for test, 'Message.CorrelationId' will fail on message forwarding mechanism
        Guid _monitorId = Guid.NewGuid();
        private int _millisecondsInterval;
        private int _timeoutMillisecondsInterval;
        string _lastStatus = String.Empty;

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
            string serviceQueueName, string monitorQueueName,
            int secondsInterval, int timeoutSecondsInterval
            )
            : base(
                new MessageSenderGateway(controlBusQueueName),
                new MessageSenderGateway(serviceQueueName),
                new MessageReceiverGateway(monitorQueueName)
                )
        {
            _millisecondsInterval = secondsInterval*1000;
            _timeoutMillisecondsInterval = timeoutSecondsInterval*1000;

            ActivateTimer();
        }

        void ActivateTimer()
        {
            _sendTimer = new Timer(new TimerCallback(this.OnSendTimerEvent), null, _millisecondsInterval, Timeout.Infinite);
            MonitorStatus status = new MonitorStatus
            {
                Status = MonitorStatus.STATUS_ANNOUNCE,
                Description = "Monitor On-Line",
                MonitorID = _monitorId
            };

            SendControlBusStatus(new Message(status));

            _lastStatus = status.Status;
        }

        void OnSendTimerEvent(object state)
        {
            CreditBureauRequest request = new CreditBureauRequest
            {
                SSN = _ssn
            };
            Message requestMessage = new Message(request);

            requestMessage.Priority = MessagePriority.AboveNormal;
            requestMessage.AppSpecific = TEST_MESSAGE_ID;       //Utilitize 'AppSpecific' field as correlation since 'Message.CorrelationId' will fail on message forwarding mechanism

            SendTestMessage(requestMessage);

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

            SendControlBusStatus(new Message(status));

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

                    if (message.AppSpecific != TEST_MESSAGE_ID)
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
                SendControlBusStatus(new Message(status));

            _lastStatus = status.Status;

            _sendTimer.Dispose();

            _sendTimer = new Timer(new TimerCallback(this.OnSendTimerEvent), null, _millisecondsInterval, Timeout.Infinite);
        }
    }
}
