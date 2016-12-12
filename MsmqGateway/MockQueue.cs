/* Loan Broker Example with MSMQ
 * from Enterprise Integration Patterns (Addison-Wesley, ISBN 0321200683)
 * 
 * Copyright (c) 2003 Gregor Hohpe 
 *
 * This code is supplied as is. No warranties. 
 */

using System.Collections.Generic;
using Messaging.Base.Constructions;

namespace Gateway.Mock
{

    using System.Messaging;
    using Messaging.Base;

    public class MockQueue: IMessageSender<MessageQueue, Message> , IMessageReceiver<MessageQueue, Message>
    {
        private MessageDelegate<Message> onMsg = new MessageDelegate<Message>(DoNothing);

        public string QueueName
        {
            get { return string.Empty; }
        }
        public Message Send(Message msg)
        {
            onMsg(msg);

            return null;
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

        public void SetupSender()
        {
        }

        public void SetupReceiver()
        {
        }

        public void StopReceivingMessages()
        {

        }

        public bool Started { get { return false;} }

        public Message Send<TEntity>(TEntity message)
        {
            return null;
        }

        public Message Send<TEntity>(TEntity entity, IList<SenderProperty> propertiesToSet)
        {
            return null;
        }

        public Message Send<TEntity>(TEntity entity, IReturnAddress<Message> returnAddress,
            IList<SenderProperty> propertiesToSet)
        {
            return null;
        }

        public Message Send<TEntity>(TEntity entity, IReturnAddress<Message> returnAddress)
        {
            return null;
        }
    }
}
