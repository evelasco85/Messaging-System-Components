/* Loan Broker Example with MSMQ
 * from Enterprise Integration Patterns (Addison-Wesley, ISBN 0321200683)
 * 
 * Copyright (c) 2003 Gregor Hohpe 
 *
 * This code is supplied as is. No warranties. 
 */

using Messaging.Base.Constructions;

namespace Gateway.Mock
{

    using System.Messaging;
    using MessageGateway;
    using Messaging.Base;

    public class MockQueue: IMessageSender<MessageQueue, Message> , IMessageReceiver<MessageQueue, Message>
    {
        private MessageDelegate<Message> onMsg = new MessageDelegate<Message>(DoNothing);
	
        public void Send(Message msg)
        {
            onMsg(msg);
        }
	
        private static void DoNothing(Message msg)
        {
		
        }
	
        public MessageDelegate<Message> ReceiveMessageProcessor 
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

        public IReturnAddress<Message> AsReturnAddress()
        {
            throw new System.NotImplementedException();
        }
    }
}
