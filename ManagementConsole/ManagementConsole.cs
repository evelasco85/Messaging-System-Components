using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
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
            }
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
    }
}
