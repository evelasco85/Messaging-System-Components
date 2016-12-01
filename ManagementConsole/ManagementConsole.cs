using ManagementConsole.Configuration;
using Messaging.Orchestration.Shared;
using Messaging.Orchestration.Shared.Models;
using Messaging.Orchestration.Shared.Services.Interfaces;
using MsmqGateway;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using CommonObjects;
using MessageGateway;
using Message = System.Messaging.Message;

namespace ManagementConsole
{
    public partial class ManagementConsole : Form
    {
        private ControlBusConsumer<Message> _controlBus;
        private MonitorCreditBureau<Message> _monitor;
        IServerService<Message> _server;
        IList<Tuple<string, string, string>> _clients = new List<Tuple<string, string, string>>();

        public ManagementConsole(String[] args)
        {
            InitializeComponent();

            string serverRequestQueue = ToPath(args[0]);
            string serverReplyQueue = ToPath(args[1]);
            string controlBusQueue = ToPath(args[2]);
            string serviceQueue = ToPath(args[3]);
            string monitoringReplyQueue = ToPath(args[4]);
            string routerControlQueue = ToPath(args[5]);
            int secondsInterval = Convert.ToInt32(args[6]);
            int timeoutSecondsInterval = Convert.ToInt32(args[7]);

            RunControlBus(
                controlBusQueue, serviceQueue,
                monitoringReplyQueue, routerControlQueue,
                secondsInterval, timeoutSecondsInterval
                );

            _server = MQOrchestration.GetInstance().CreateServer(
               serverRequestQueue,
               serverReplyQueue
               );
            RegisterServer(ref _server);
            _server.Process();
        }

        void RunControlBus(
            string controlBusQueue, string serviceQueue,
            string monitoringReplyQueue, string routerControlQueue,
            int secondsInterval, int timeoutSecondsInterval
            )
        {
            _controlBus = new ControlBusConsumer<Message>(
                new MessageReceiverGateway(controlBusQueue),
                this.ProcessMessage
                );

            _monitor = new MonitorCreditBureau<Message>(
                new MessageSenderGateway(controlBusQueue),
                new MessageSenderGateway(serviceQueue),
                new MessageReceiverGateway(monitoringReplyQueue, new XmlMessageFormatter(new Type[] { typeof(CreditBureauReply) })),
                new MessageSenderGateway(routerControlQueue),
                secondsInterval, //Verify status every n-th second(s)
                timeoutSecondsInterval, //Set n-th second timeout,
                ((request) =>
                {
                    Message requestMessage = new Message(request)
                    {
                        Priority = MessagePriority.AboveNormal
                    };

                    return requestMessage;
                }),
                (message =>
                {
                    return message.Id;
                }),
                (message =>
                {
                    message.Formatter = new XmlMessageFormatter(new Type[] {typeof(CreditBureauReply)});
                    string messageBody = string.Empty;
                    string correlationId = message.CorrelationId;
                    bool isCreditBureauReply = message.Body is CreditBureauReply;
                    CreditBureauReply reply = (CreditBureauReply) message.Body;

                    using (StreamReader reader = new StreamReader(message.BodyStream))
                    {
                        messageBody = reader.ReadToEnd();
                    }

                    return new Tuple<string, bool, string, CreditBureauReply>(
                        correlationId,
                        isCreditBureauReply,
                        messageBody,
                        reply
                        );
                })
                );

            _controlBus.Process();
            _monitor.Process();
        }

        void RegisterServer(ref IServerService<Message> server)
        {
            ConfigurationLoader loader = new ConfigurationLoader();

            loader.LoadXml(Resources.ClientConfigurations);
            
            server.Register(
                MQOrchestration.GetInstance().CreateServerRequestConverter(),
                MQOrchestration.GetInstance().CreateServerSendResponse(),
                request =>
                {
                    ServerMessage response = null;

                    if (request == null)
                        return response;

                    if (InvokeRequired)
                    {
                        this.Invoke(new MethodInvoker(() =>
                        {
                            switch (request.RequestType)
                            {
                                case ServerRequestType.Register:
                                    response = ProcessRegistration(loader, request);
                                    break;
                            }

                            DisplayServerMessage(response);
                        }));
                    }

                    return response;
                });
        }

        void DisplayServerMessage(ServerMessage response)
        {
            StringBuilder data = new StringBuilder();

            data.AppendLine(string.Format("Client ID: {0}", response.ClientId));
            data.AppendLine(string.Format("Client Name: {0}", response.ClientName));
            data.AppendLine(string.Format("Group Id: {0}", response.GroupId));
            data.AppendLine(string.Format("Client Status: {0}", response.ClientStatus.ToString()));

            for(int index = 0; (response.ClientParameters != null) && (index < response.ClientParameters.Count); index++)
            {
                ParameterEntry entry = response.ClientParameters[index];

                data.AppendLine(string.Format("**{0}={1}", entry.Name, entry.Value));
            }

            data.AppendLine(string.Join("", Enumerable.Repeat("-", 10)));
            this.txtServerResponse.Text = data.ToString() + Environment.NewLine + this.txtServerResponse.Text;
        }

        void ActivateClients()
        {

            foreach (Tuple<string, string, string> client in _clients)
            {
                ServerMessage serverMessage = new ServerMessage
                {
                    GroupId = client.Item1,
                    ClientId = client.Item2,
                    ClientName = client.Item3,
                    ClientStatus = ClientCommandStatus.Start
                };

                _server.SendClientMessage(serverMessage);
                DisplayServerMessage(serverMessage);
            }
        }

        void StopClients()
        {

            foreach (Tuple<string, string, string> client in _clients)
            {
                ServerMessage serverMessage = new ServerMessage
                {
                    GroupId = client.Item1,
                    ClientId = client.Item2,
                    ClientStatus = ClientCommandStatus.Stop
                };

                _server.SendClientMessage(serverMessage);
                DisplayServerMessage(serverMessage);
            }
        }

        ServerMessage ProcessRegistration(IConfigurationLoader loader, ServerRequest request)
        {
            IList<Tuple<string, string>> configurations = loader.GetConfiguration(request.ClientId, request.GroupId);
            List<ParameterEntry> paramList = configurations
                .Where(config => request.ParameterList.Any(param => param == config.Item1))
                .Select(param => new ParameterEntry
                {
                    Name = param.Item1,
                    Value = param.Item2
                })
                .ToList();

            ServerMessage response = new ServerMessage
            {
                GroupId = request.GroupId,
                ClientId = request.ClientId,
                ClientStatus = ClientCommandStatus.SetupClientParameters,
                ClientParameters = paramList
            };

            loader.SetClientInfo(request.ClientId, request.GroupId, ref response);
            _clients.Add(new Tuple<string, string, string>(response.GroupId, response.ClientId, response.ClientName));

            return response;
        }

        String ToPath(String arg)
        {
            return ".\\private$\\" + arg;
        }

        void ProcessMessage(Message message)
        {
            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    using (StreamReader reader = new StreamReader(message.BodyStream))
                    {
                        XmlDocument messageDoc = new XmlDocument();

                        messageDoc.LoadXml(reader.ReadToEnd());

                        XmlNode root = messageDoc.SelectSingleNode("/*");

                        switch (root.Name)
                        {
                            case "SummaryStat":
                                PrependStatisticData(messageDoc);
                                break;
                            case "LoanBrokerProxyInfo":
                                PrependResponseData(messageDoc);
                                break;
                            case "MonitorStatus":
                                PrependMonitorData(messageDoc);
                                break;
                            default:
                                break;
                        }
                    }
                }));
            }
        }

        void PrependResponseData(XmlDocument messageDoc)
        {
                XmlNode data = messageDoc.SelectSingleNode("/LoanBrokerProxyInfo/Detail");

                this.txtLoanBrokerResponseData.Text = data.InnerText + Environment.NewLine + this.txtLoanBrokerResponseData.Text;
        }

        void PrependStatisticData(XmlDocument messageDoc)
        {
            XmlNode duration = messageDoc.SelectSingleNode("/SummaryStat/AverageReplyDuration");
            XmlNode request = messageDoc.SelectSingleNode("/SummaryStat/AverageOutstandingRequest");

            string data = string.Format("Ave. Reply Duration [{0:0.###}] | Ave. Outstanding Request [{1:0.###}]",
                Convert.ToDouble(duration.InnerText),
                Convert.ToDouble(request.InnerText)
                );

            this.txtLoanBrokerStatData.Text = data + Environment.NewLine + this.txtLoanBrokerStatData.Text;
        }

        void PrependMonitorData(XmlDocument messageDoc)
        {
            XmlNode statusNode = messageDoc.SelectSingleNode("/MonitorStatus/Status");
            XmlNode descriptionNode = messageDoc.SelectSingleNode("/MonitorStatus/Description");
            string message = string.Format("Status: {2}{0}Description: {3}{0}Date-Time: {4}{0}{1}",
                Environment.NewLine,
                string.Join("", Enumerable.Repeat('-', 10)),
                statusNode.InnerText,
                descriptionNode.InnerText,
                DateTime.Now.ToString("T")
                );

            this.txtCreditBureauStatus.Text = message + Environment.NewLine + this.txtCreditBureauStatus.Text;
        }

        private void btnActivateAll_Click(object sender, EventArgs e)
        {
            ActivateClients();
        }

        private void btnStopAllClients_Click(object sender, EventArgs e)
        {
            StopClients();
        }
    }
}
