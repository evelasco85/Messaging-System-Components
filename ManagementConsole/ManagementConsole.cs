using ManagementConsole.Configuration;
using Messaging.Orchestration.Shared;
using Messaging.Orchestration.Shared.Models;
using Messaging.Orchestration.Shared.Services.Interfaces;
using MsmqGateway;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Message = System.Messaging.Message;

namespace ManagementConsole
{
    public partial class ManagementConsole : Form
    {
        private ControlBusConsumer _controlBus;
        private MonitorCreditBureau _monitor;
        IServerService<MessageQueue, Message> _server;
        IList<string> _clientIds = new List<string>();

        public ManagementConsole(String[] args)
        {
            InitializeComponent();

            if (args.Length == 4)
            {
                string controlBusQueue = ToPath(args[0]);
                string serviceQueue = ToPath(args[1]);
                string monitoringReplyQueue = ToPath(args[2]);
                string routerControlQueue = ToPath(args[3]);

                _controlBus = new ControlBusConsumer(
                    controlBusQueue,
                    this.ProcessMessage
                    );

                _monitor = new MonitorCreditBureau(
                    controlBusQueue,
                    serviceQueue,
                    monitoringReplyQueue,
                    routerControlQueue,
                    5,      //Verify status every n-th second(s)
                    10      //Set n-th second timeout
                    );

                _server = MQOrchestration.GetInstance().CreateServer(
                    ToPath("ServerRequestQueue"),
                    ToPath("ServerReplyQueue")
                    );

                RegisterServer(_server);
            }
        }

        void RegisterServer(IServerService<MessageQueue, Message> server)
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

            server.Process();
        }

        void DisplayServerMessage(ServerMessage response)
        {
            StringBuilder data = new StringBuilder();

            data.AppendLine(string.Format("Client ID: {0}", response.ClientId));
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

            foreach (string clientId in _clientIds)
            {
                ServerMessage serverMessage = new ServerMessage
                {
                    ClientId = clientId,
                    ClientStatus = ClientCommandStatus.Start
                };

                _server.SendClientMessage(serverMessage);
                DisplayServerMessage(serverMessage);
            }
        }

        ServerMessage ProcessRegistration(ConfigurationLoader loader, ServerRequest request)
        {
            IList<Tuple<string, string>> configurations = loader.GetConfiguration(request.ClientId);
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
                ClientId = request.ClientId,
                ClientStatus = ClientCommandStatus.SetupClientParameters,
                ClientParameters = paramList
            };

            _clientIds.Add(request.ClientId);

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

        private void ManagementConsole_Load(object sender, EventArgs e)
        {
            if (_controlBus != null)
                _controlBus.Process();

            if (_monitor != null)
                _monitor.Process();
        }

        private void btnActivateAll_Click(object sender, EventArgs e)
        {
            ActivateClients();
        }
    }
}
