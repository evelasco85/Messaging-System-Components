using Messaging.Base;
using Messaging.Base.Constructions;
using System;
using System.Messaging;
using MsmqGateway.Core;

namespace MsmqGateway
{
    public delegate object SyncProcessMessageDelegate(object receivedMessageObject);

    public class MQRequestReplyService_Synchronous<TEntity> : RequestReply_Synchronous<MessageQueue, Message>
    {
        private SyncProcessMessageDelegate _syncProcessMessageInvocator;
        private IMessageFormatter _formatter = new XmlMessageFormatter(new Type[] { typeof(TEntity) });

        MQRequestReplyService_Synchronous(
            SyncProcessMessageDelegate syncProcessMessageInvocator
            )
        {
            _syncProcessMessageInvocator = syncProcessMessageInvocator;
        }


        public MQRequestReplyService_Synchronous(
            IMessageReceiver<MessageQueue, Message> receiver,
            SyncProcessMessageDelegate syncProcessMessageInvocator
            ) :
                this(syncProcessMessageInvocator)
        {
            QueueService = new MessageQueueService(receiver);
        }

        public MQRequestReplyService_Synchronous(
            String requestQueueName,
            SyncProcessMessageDelegate syncProcessMessageInvocator
            ):
            this(syncProcessMessageInvocator)
        {
            QueueService = new MessageQueueService(new MessageReceiverGateway<TEntity>(requestQueueName));
        }

        public override void OnMessageReceived(Message receivedMessage)
        {
            receivedMessage.Formatter = _formatter;
            Object inBody = GetTypedMessageBody(receivedMessage);

            if (inBody != null)
            {
                Object replyObject = ProcessRequestMessage(inBody);

                if (replyObject != null)
                {
                    QueueService.SendReply(replyObject, receivedMessage);
                }
            }
        }
        

        public override object ProcessRequestMessage(object receivedMessageObject)
        {
            object result = null;

            if (_syncProcessMessageInvocator != null)
                result = _syncProcessMessageInvocator(receivedMessageObject);

            return result;
        }

        Object GetTypedMessageBody(Message msg)
        {
            try
            {
                if (msg.Body.GetType() == typeof(TEntity))
                {
                    return msg.Body;
                }
                else
                {
                    Console.WriteLine("Illegal message format.");
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Illegal message format" + e.Message);
                return null;
            }
        }
    }
}
