using Messaging.Base;
using Messaging.Base.Constructions;
using System;
using System.Messaging;

namespace MessageGateway
{
    public delegate object SyncProcessMessageDelegate(object receivedMessageObject);
    public delegate IMessageFormatter GetFormatterDelegate();
    public delegate Type GetRequestBodyTypeDelegate();

    public class MQRequestReplyService_Synchronous : RequestReply_Synchronous<MessageQueue, Message>
    {
        private SyncProcessMessageDelegate _syncProcessMessageInvocator;
        private GetFormatterDelegate _getFormatterInvocator;
        private GetRequestBodyTypeDelegate _getRequestBodyTypeInvocator;

        MQRequestReplyService_Synchronous(
            SyncProcessMessageDelegate syncProcessMessageInvocator,
            GetFormatterDelegate getFormatterInvocator,
            GetRequestBodyTypeDelegate getRequestBodyTypeInvocator
            )
        {
            _syncProcessMessageInvocator = syncProcessMessageInvocator;

            if (getFormatterInvocator == null)
                _getFormatterInvocator = new GetFormatterDelegate(DefaultGetFormatter);
            else
                _getFormatterInvocator = getFormatterInvocator;

            if (getRequestBodyTypeInvocator == null)
                _getRequestBodyTypeInvocator = new GetRequestBodyTypeDelegate(DefaultGetRequestBodyType);
            else
                _getRequestBodyTypeInvocator = getRequestBodyTypeInvocator;
        }

        public MQRequestReplyService_Synchronous(
            IMessageReceiver<MessageQueue, Message> receiver,
            SyncProcessMessageDelegate syncProcessMessageInvocator,
            GetFormatterDelegate getFormatterInvocator,
            GetRequestBodyTypeDelegate getRequestBodyTypeInvocator
            ) :
            this(syncProcessMessageInvocator, getFormatterInvocator, getRequestBodyTypeInvocator)
        {
            QueueService = new MQService(receiver);
        }

        public MQRequestReplyService_Synchronous(
            String requestQueueName,
            SyncProcessMessageDelegate syncProcessMessageInvocator,
            GetFormatterDelegate getFormatterInvocator, 
            GetRequestBodyTypeDelegate getRequestBodyTypeInvocator
            ):
            this(syncProcessMessageInvocator, getFormatterInvocator, getRequestBodyTypeInvocator)
        {
            QueueService = new MQService(new MessageReceiverGateway(requestQueueName, _getFormatterInvocator()));
        }

        public override void OnMessageReceived(Message receivedMessage)
        {
            receivedMessage.Formatter = _getFormatterInvocator();
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

        IMessageFormatter DefaultGetFormatter()
        {
            return new XmlMessageFormatter(new Type[] { _getRequestBodyTypeInvocator() });
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
