using Messaging.Base;
using Messaging.Base.Constructions;
using System;
using System.Messaging;
using MsmqGateway.Core;

namespace MsmqGateway
{
    public delegate object SyncProcessMessageDelegate(object receivedMessageObject);
    public delegate IMessageFormatter GetFormatterDelegate();
    public delegate Type GetRequestBodyTypeDelegate();

    public class MQRequestReplyService_Synchronous<TEntity> : RequestReply_Synchronous<MessageQueue, Message>
    {
        private SyncProcessMessageDelegate _syncProcessMessageInvocator;
        private GetRequestBodyTypeDelegate _getRequestBodyTypeInvocator;
        private CanonicalDataModel<TEntity> _cdm;

        MQRequestReplyService_Synchronous(
            CanonicalDataModel<TEntity> cdm,
            SyncProcessMessageDelegate syncProcessMessageInvocator,
            GetRequestBodyTypeDelegate getRequestBodyTypeInvocator
            )
        {
            _syncProcessMessageInvocator = syncProcessMessageInvocator;

            if (getRequestBodyTypeInvocator == null)
                _getRequestBodyTypeInvocator = new GetRequestBodyTypeDelegate(DefaultGetRequestBodyType);
            else
                _getRequestBodyTypeInvocator = getRequestBodyTypeInvocator;

            _cdm = cdm;
        }


        public MQRequestReplyService_Synchronous(
            IMessageReceiver<MessageQueue, Message> receiver,
            CanonicalDataModel<TEntity> cdm,
            SyncProcessMessageDelegate syncProcessMessageInvocator,
            GetRequestBodyTypeDelegate getRequestBodyTypeInvocator
            ) :
                this(cdm, syncProcessMessageInvocator, getRequestBodyTypeInvocator)
        {
            QueueService = new MessageQueueService(receiver);
        }

        public MQRequestReplyService_Synchronous(
            IMessageReceiver<MessageQueue, Message> receiver,
            SyncProcessMessageDelegate syncProcessMessageInvocator,
            GetRequestBodyTypeDelegate getRequestBodyTypeInvocator
            ) :
            this(new CanonicalDataModel<TEntity>(), syncProcessMessageInvocator, getRequestBodyTypeInvocator)
        {
            QueueService = new MessageQueueService(receiver);
        }

        public MQRequestReplyService_Synchronous(
            String requestQueueName,
            CanonicalDataModel<TEntity> cdm,
            SyncProcessMessageDelegate syncProcessMessageInvocator,
            GetRequestBodyTypeDelegate getRequestBodyTypeInvocator
            ):
            this(cdm, syncProcessMessageInvocator, getRequestBodyTypeInvocator)
        {
            QueueService = new MessageQueueService(new MessageReceiverGateway(requestQueueName, _cdm.Formatter));
        }

        public MQRequestReplyService_Synchronous(
            String requestQueueName,
            SyncProcessMessageDelegate syncProcessMessageInvocator,
            GetRequestBodyTypeDelegate getRequestBodyTypeInvocator
            ) :
            this(new CanonicalDataModel<TEntity>(), syncProcessMessageInvocator, getRequestBodyTypeInvocator)
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

        Type DefaultGetRequestBodyType()
        {
            return typeof(System.String);
        }

        Object GetTypedMessageBody(Message msg)
        {
            try
            {
                if (msg.Body.GetType().Equals(_getRequestBodyTypeInvocator()))
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
