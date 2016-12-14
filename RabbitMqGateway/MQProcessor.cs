using System;
using Messaging.Base;
using RabbitMqGateway.Core;
using RabbitMQ.Client;

namespace RabbitMqGateway
{
    public class Processor
    {
        protected IMessageReceiver<IModel, Message> inputQueue;
        protected IMessageSender<IModel, Message> outputQueue;
        private CanonicalDataModel<string> _cdm = new CanonicalDataModel<string>();

        public Processor(IMessageReceiver<IModel, Message> receiver, IMessageSender<IModel, Message> sender)
        {
            inputQueue = receiver;
            outputQueue = sender;
            Register(inputQueue);
        }

        public void Register(IMessageReceiver<IModel, Message> rec)
        {
            MessageDelegate<Message> ev = new MessageDelegate<Message>(OnMessage);
            rec.ReceiveMessageProcessor += ev;
        }

        public void Run()
        {
            inputQueue.StartReceivingMessages();

            Console.WriteLine();
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        private void OnMessage(Message inMsg)
        {
            Message outMsg = ProcessMessage(inMsg);
            if (outMsg != null)
            {
                outMsg.CorrelationId = inMsg.Id;
                outMsg.AppSpecific = inMsg.AppSpecific;
                outputQueue.SendRawMessage(outMsg);
            }
        }

        protected virtual Message ProcessMessage(Message m)
        {
            string body = (string)m.Body;
            Console.WriteLine("Received Message: " + body);
            return m;
        }

    }
}
