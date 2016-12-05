using System;
using System.Messaging;
using Messaging.Orchestration.Shared.Services;

namespace LoanBroker {

	internal class Run{

	    public static void Main(String[] args)
	    {
            ClientInstance<MessageQueue, Message> instance = new ClientInstance<MessageQueue, Message>();
            IClientService service = MQClient.GetClientService(instance, args);
            
            service.StartService();

	        Console.WriteLine();
	        Console.WriteLine("Press Enter to exit...");
	        Console.ReadLine();
	    }

        private static String ToPath(String arg){
			return ".\\private$\\" + arg;
		}
		
		public static void PrintMessage(Message m)
		{
			m.Formatter =  new System.Messaging.XmlMessageFormatter(new String[] {"System.String,mscorlib"});	
			string body = (string)m.Body;
			Console.WriteLine("Received Message: " + body);
			return; 
		}	
	}
}
