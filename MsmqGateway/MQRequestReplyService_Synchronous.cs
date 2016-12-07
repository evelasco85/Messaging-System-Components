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
        private CanonicalDataModel<TEntity> _cdm;

        MQRequestReplyService_Synchronous(
            CanonicalDataModel<TEntity> cdm,
            SyncProcessMessageDelegate syncProcessMessageInvocator
            )
        {
            _syncProcessMessageInvocator = syncProcessMessageInvocator;
            _cdm = cdm;
        }


        public MQRequestReplyService_Synchronous(
            IMessageReceiver<MessageQueue, Message> receiver,
            SyncProcessMessageDelegate syncProcessMessageInvocator
            ) :
                this(new CanonicalDataModel<TEntity>(), syncProcessMessageInvocator)
        {
            QueueService = new MessageQueueService(receiver);
        }

        public MQRequestReplyService_Synchronous(
            String requestQueueName,
            SyncProcessMessageDelegate syncProcessMessageInvocator
            ):
            this(new CanonicalDataModel<TEntity>(), syncProcessMessageInvocator)
        {
            QueueService = new MessageQueueService(new MessageReceiverGateway(requestQueueName, _cdm.Formatter));
        }

        public override void OnMessageReceived(Message receivedMessage)
        {
            receivedMessage.Formatter = _cdm.Formatter;
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
                if (msg.Body.GetType().Equals(_cdm.GetRequestBodyType()))
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
