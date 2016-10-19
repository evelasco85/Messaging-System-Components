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
        private string _controlBusQueueName;
        private ControlBusConsumer _controlBus;
        public ManagementConsole(String[] args)
        {
            if (args.Length == 1)
            {
                _controlBusQueueName = ToPath(args[0]);
            }

            InitializeComponent();
        }

        String ToPath(String arg)
        {
            return ".\\private$\\" + arg;
        }

        private void ManagementConsole_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_controlBusQueueName))
            {
                _controlBus = new ControlBusConsumer(_controlBusQueueName, this.ProcessMessage);

                _controlBus.Process();
            }
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
                }
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
    }
}
