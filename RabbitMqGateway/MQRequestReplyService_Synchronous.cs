using System;
using Messaging.Base;
using Messaging.Base.Constructions;
using RabbitMqGateway.Core;
using RabbitMQ.Client;

namespace RabbitMqGateway
{
    public delegate object SyncProcessMessageDelegate(object receivedMessageObject);

    public class MQRequestReplyService_Synchronous<TEntity> : RequestReply_Synchronous<IModel, Message>
    {
        private SyncProcessMessageDelegate _syncProcessMessageInvocator;

        MQRequestReplyService_Synchronous(
            SyncProcessMessageDelegate syncProcessMessageInvocator
            )
        {
            _syncProcessMessageInvocator = syncProcessMessageInvocator;
        }


        public MQRequestReplyService_Synchronous(
            ConnectionFactory factory, 
            IMessageReceiver<IModel, Message> receiver,
            SyncProcessMessageDelegate syncProcessMessageInvocator
            ) :
            this(syncProcessMessageInvocator)
        {
            QueueService = new MessageQueueService(factory, receiver);
        }

        public MQRequestReplyService_Synchronous(
            ConnectionFactory factory,
            String requestQueueName,
            SyncProcessMessageDelegate syncProcessMessageInvocator
            ) :
            this(syncProcessMessageInvocator)
        {
            QueueService = new MessageQueueService(
                factory,
                new MessageReceiverGateway<TEntity>(
                    new MessageQueueGateway(
                        factory,
                        requestQueueName
                        )
                    )
                );
        }

        public override void OnMessageReceived(Message receivedMessage)
        {
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
