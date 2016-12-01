using System;
using System.Messaging;
using Messaging.Orchestration.Shared.Services;

namespace CreditBureauFailOver
{
    class Program
    {
        static void Main(string[] args)
        {
            ClientInstance<Message> instance = new ClientInstance<Message>();
            IClientService service = MSMQClient.GetClientService(instance, args);

            service.StartService();

            Console.WriteLine();
            Console.WriteLine("Press Enter to exit...");
            Console.WriteLine();
            Console.ReadLine();
        }

        private static String ToPath(String arg)
        {
            return ".\\private$\\" + arg;
        }
    }
}
