/* Loan Broker Example with MSMQ
 * from Enterprise Integration Patterns (Addison-Wesley, ISBN 0321200683)
 * 
 * Copyright (c) 2003 Gregor Hohpe 
 *
 * This code is supplied as is. No warranties. 
 */

using Messaging.Base;
using Messaging.Base.Interface;
using System;
using System.Messaging;

namespace MessageGateway
{
    public class MQService : QueueService<MessageQueue, Message>
    {
        static protected readonly String InvalidMessageQueueName = ".\\private$\\invalidMessageQueue";
        IMessageSender<MessageQueue, Message>invalidQueue = new MessageSenderGateway(InvalidMessageQueueName);

        protected IMessageReceiver<MessageQueue, Message> _receiver;

        public override IMessageReceiver<MessageQueue, Message> Receiver
        {
            get { return _receiver; }
        }

        protected Type requestBodyType;
	
        public MQService(IMessageReceiver<MessageQueue, Message> receiver)
        {
            _receiver = receiver;
        }
	
        public override void Run()
        {
            _receiver.StartReceivingMessages();
        }

        public override void SendReply(Object responseObject, Message originalRequestMessage)
        {
            Message responseMessage = new Message(responseObject);
            responseMessage.CorrelationId = originalRequestMessage.Id;
            responseMessage.AppSpecific = originalRequestMessage.AppSpecific;

            if (originalRequestMessage.ResponseQueue != null) 
            {
                IMessageSender<MessageQueue, Message>  replyQueue = new MessageSenderGateway(originalRequestMessage.ResponseQueue);
                replyQueue.Send(responseMessage);
            }
            else
            {
                invalidQueue.Send(responseMessage);
            }
        }
    }
}
