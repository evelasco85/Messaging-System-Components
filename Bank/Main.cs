using System;
using Messaging.Orchestration.Shared.Services;

namespace Bank
{
    public class Run
    {
        public static void Main(String[] args)
        {
            ClientInstance instance = new ClientInstance();
            IClientService service = MQClient.GetClientService(instance, args);

            service.StartService();

            Console.WriteLine();
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }
    }
}
