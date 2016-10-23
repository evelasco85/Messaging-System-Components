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

            if (args.Length == 3)
            {
                _controlBus = new ControlBusConsumer(
                    ToPath(args[0]),
                    this.ProcessMessage
                    );

                _monitor = new MonitorCreditBureau(
                    ToPath(args[0]),
                    ToPath(args[1]),
                    ToPath(args[2]),
                    5,
                    10
                    );
            }
        }


        String ToPath(String arg)
        {
            return ".\\private$\\" + arg;
        }

        void ProcessMessage(Message message)
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
        }

        void PrependResponseData(XmlDocument messageDoc)
        {
            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    XmlNode data = messageDoc.SelectSingleNode("/LoanBrokerProxyInfo/Detail");

                    this.txtLoanBrokerResponseData.Text = data.InnerText + Environment.NewLine + this.txtLoanBrokerResponseData.Text;
                }));
            }
        }

        void PrependStatisticData(XmlDocument messageDoc)
        {

            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    XmlNode duration = messageDoc.SelectSingleNode("/SummaryStat/AverageReplyDuration");
                    XmlNode request = messageDoc.SelectSingleNode("/SummaryStat/AverageOutstandingRequest");

                    string data = string.Format("Ave. Reply Duration [{0:0.###}] | Ave. Outstanding Request [{1:0.###}]",
                        Convert.ToDouble(duration.InnerText),
                        Convert.ToDouble(request.InnerText)
                        );

                    this.txtLoanBrokerStatData.Text = data + Environment.NewLine + this.txtLoanBrokerStatData.Text;
                }));
            }
        }

        void PrependMonitorData(XmlDocument messageDoc)
        {
            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    XmlNode status = messageDoc.SelectSingleNode("/MonitorStatus/Status");
                    XmlNode description = messageDoc.SelectSingleNode("/MonitorStatus/Description");
                    string message = string.Format("Status: {2}{0}Description: {3}{0}Date-Time: {4}{0}{1}",
                        Environment.NewLine,
                        string.Join("", Enumerable.Repeat('-', 10)),
                        status.InnerText,
                        description.InnerText,
                        DateTime.Now.ToString("T")
                        );

                    this.txtCreditBureauStatus.Text = message + Environment.NewLine + this.txtCreditBureauStatus.Text;
                }));
            }
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
