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
                this.txtControlBusData.Text = reader.ReadToEnd() + Environment.NewLine + this.txtControlBusData.Text;
            }
        }
    }
}
