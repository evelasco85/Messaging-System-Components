/* Loan Broker Example with MSMQ
 * from Enterprise Integration Patterns (Addison-Wesley, ISBN 0321200683)
 * 
 * Copyright (c) 2003 Gregor Hohpe 
 *
 * This code is supplied as is. No warranties. 
 */

namespace Gateway.Mock
{

    using System.Messaging;
    using MessageGateway;
    using Messaging.Base.Interface;

    public class MockQueue: IMessageSender<MessageQueue, Message> , IMessageReceiver<MessageQueue, Message>
    {
        private OnMsgEvent<Message> onMsg = new OnMsgEvent<Message>(DoNothing);
	
        public void Send(Message msg)
        {
            onMsg(msg);
        }
	
        private static void DoNothing(Message msg)
        {
		
        }
	
        public OnMsgEvent<Message> OnMessage 
        { 
            get { return onMsg; } 
            set { onMsg = value; }
        }
	
        public void StartReceivingMessages()
        {
		
        }

        public MessageQueue GetQueue()
        {
            return null;
        }
	
    }
}
