using Messaging.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Messaging;
using System.Text;

namespace LoanBroker.Bank
{
    internal class MessageRouter
    {
        public static void SendToRecipientList(Message msg, IMessageSender<MessageQueue, Message>[] recipientList)
        {
            IEnumerator e = recipientList.GetEnumerator();
            while (e.MoveNext())
            {
                ((IMessageSender<MessageQueue, Message>)e.Current).Send(msg);
            }
        }
    }
}
