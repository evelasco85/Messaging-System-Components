using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using MessageGateway;
using Messaging.Base;
using Messaging.Orchestration.Models;

namespace ManagementConsole
{
    public class OrchestrationServer
    {
        private IMessageReceiver<MessageQueue, Message> _requestReceiver;
        private IMessageSender<MessageQueue, Message> _replySender;
        public OrchestrationServer(string serverRequestQueue, string serverResponseQueue)
        {
            _requestReceiver = new MessageReceiverGateway(serverRequestQueue,
                GetObjectInformationFormatter());

            _requestReceiver.ReceiveMessageProcessor += this.OnReceiveMessage;

            _replySender = new MessageSenderGateway(serverResponseQueue);
        }

        IMessageFormatter GetObjectInformationFormatter()
        {
            return new XmlMessageFormatter(new Type[] { typeof(ObjectInformation<int>) });
        }

        private void OnReceiveMessage(Message msg)
        {
            if (msg.Body is ObjectInformation<int>)
            {
                ObjectInformation<int> info = (ObjectInformation<int>) msg.Body;
            }
        }
    }
}
