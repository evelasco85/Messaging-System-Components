using System.Collections.Generic;

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

        public void SendToRecipent<TMessage>(TMessage message, IList<IRawMessageSender<TMessage>> recipientList)
        {
            if ((message == null) || (recipientList == null))
                return;

            for (int index = 0; index < recipientList.Count; index++)
            {
                IRawMessageSender<TMessage> sendTo = recipientList[index];

                if(sendTo == null)
                    continue;
                
                sendTo.SendRawMessage(message);
            }
        }
    }
}
