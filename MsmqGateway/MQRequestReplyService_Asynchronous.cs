using MessageGateway;
using Messaging.Base;
using Messaging.Base.Constructions;
using System;
using System.Collections.Generic;
using System.Messaging;
using System.Text;

namespace MessageGateway
{
    public delegate void ProcessMessageDelegate2(Object receivedMessageObject, Message msg);

    public class MQRequestReplyService_Asynchronous : RequestReply_Asynchronous<MessageQueue, Message>
    {
        private ProcessMessageDelegate2 _processMessageInvocator;
        private GetFormatterDelegate _getFormatterInvocator;
        private GetRequestBodyTypeDelegate _getRequestBodyTypeInvocator;

        MQRequestReplyService_Asynchronous(
            ProcessMessageDelegate2 processMessageInvocator,
            GetFormatterDelegate getFormatterInvocator,
            GetRequestBodyTypeDelegate getRequestBodyTypeInvocator
            )
        {
            _processMessageInvocator = processMessageInvocator;

            if (getFormatterInvocator == null)
                _getFormatterInvocator = new GetFormatterDelegate(DefaultGetFormatter);
            else
                _getFormatterInvocator = getFormatterInvocator;

            if (getRequestBodyTypeInvocator == null)
                _getRequestBodyTypeInvocator = new GetRequestBodyTypeDelegate(DefaultGetRequestBodyType);
            else
                _getRequestBodyTypeInvocator = getRequestBodyTypeInvocator;
        }

        public MQRequestReplyService_Asynchronous(
            IMessageReceiver<MessageQueue, Message> receiver,
            ProcessMessageDelegate2 processMessageInvocator,
            GetFormatterDelegate getFormatterInvocator,
            GetRequestBodyTypeDelegate getRequestBodyTypeInvocator
            )
            : this(processMessageInvocator, getFormatterInvocator, getRequestBodyTypeInvocator)
        {
            QueueService = new MQService(receiver);
        }

        public MQRequestReplyService_Asynchronous(
            String requestQueueName,
            ProcessMessageDelegate2 processMessageInvocator,
            GetFormatterDelegate getFormatterInvocator,
            GetRequestBodyTypeDelegate getRequestBodyTypeInvocator
            )
            : this(processMessageInvocator, getFormatterInvocator, getRequestBodyTypeInvocator)
        {
            QueueService = new MQService(new MessageReceiverGateway(requestQueueName, _getFormatterInvocator()));
        }

        public override void OnMessageReceived(Message receivedMessage)
        {
            receivedMessage.Formatter = _getFormatterInvocator();
            Object inBody = GetTypedMessageBody(receivedMessage);

            if (inBody != null)
            {
                ProcessRequestMessage(inBody, receivedMessage);
            }
        }

        public override void ProcessRequestMessage(Object receivedMessageObject, Message msg)
        {
            if (_processMessageInvocator != null)
                _processMessageInvocator(receivedMessageObject, msg);
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
