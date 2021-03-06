﻿using System;
using Messaging.Base;
using Messaging.Base.Constructions;
using RabbitMqGateway.Core;
using RabbitMQ.Client;

namespace RabbitMqGateway
{
    public delegate void AsyncProcessMessageDelegate(Object receivedMessageObject, Message msg);

    public class MQRequestReplyService_Asynchronous<TEntity> : RequestReply_Asynchronous<IModel, Message>
    {
        private AsyncProcessMessageDelegate _asyncProcessMessageInvocator;

        MQRequestReplyService_Asynchronous(
            AsyncProcessMessageDelegate asyncProcessMessageInvocator
            )
        {
            _asyncProcessMessageInvocator = asyncProcessMessageInvocator;
        }

        public MQRequestReplyService_Asynchronous(
            ConnectionFactory factory, 
            IMessageReceiver<IModel, Message> receiver,
            AsyncProcessMessageDelegate asyncProcessMessageInvocator
            )
            : this(asyncProcessMessageInvocator)
        {
            QueueService = new MessageQueueService(factory, receiver);
        }

        public MQRequestReplyService_Asynchronous(
            ConnectionFactory factory,
            String requestQueueName,
            AsyncProcessMessageDelegate asyncProcessMessageInvocator
            )
            : this(asyncProcessMessageInvocator)
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
                ProcessRequestMessage(inBody, receivedMessage);
            }
        }

        public override void ProcessRequestMessage(Object receivedMessageObject, Message msg)
        {
            if (_asyncProcessMessageInvocator != null)
                _asyncProcessMessageInvocator(receivedMessageObject, msg);
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
