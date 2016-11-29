using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.Routing
{
    public class MessageRouter : IMessageRouter
    {
        static IMessageRouter s_instance = new MessageRouter();

        private MessageRouter()
        {
        }

        public static IMessageRouter GetInstance()
        {
            return s_instance;
        }

        public void SendToRecipent<TMessage>(TMessage message, IList<IMessageSender<TMessage>> recipientList)
        {
            if ((message == null) || (recipientList == null))
                return;

            for (int index = 0; index < recipientList.Count; index++)
            {
                IMessageSender<TMessage> sendTo = recipientList[index];

                if(sendTo == null)
                    continue;
                
                sendTo.Send(message);
            }
        }
    }
}
