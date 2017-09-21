using Messaging.Base;
using System;
using System.Messaging;
using MsmqGateway.Core;

namespace MsmqGateway
{
	public class Processor
	{
		protected IMessageReceiver<MessageQueue, Message> inputQueue;
		protected IMessageSender<MessageQueue, Message>  outputQueue;
        private IMessageFormatter _formatter = new XmlMessageFormatter(new Type[] { typeof(System.String) });
		
		public Processor(IMessageReceiver<MessageQueue, Message> receiver, IMessageSender<MessageQueue, Message>  sender)
		{
			inputQueue = receiver;
			outputQueue = sender;
			Register(inputQueue);
		}
		
		public Processor(String inputQueueName, String outputQueueName){
            MessageReceiverGateway<string> q = new MessageReceiverGateway<string>(inputQueueName);
            Register(q);
            this.inputQueue = q;

            outputQueue = new MessageSenderGateway(outputQueueName);
            Console.WriteLine("Processing messages from " + inputQueueName + " to " + outputQueueName);
        }
				
		public void Register(IMessageReceiver<MessageQueue, Message> rec){
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
            inMsg.Formatter = _formatter;
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
