using System;
using Bank.Models;
using MessageGateway;
using Messaging.Orchestration.Shared.Services;
using MsmqGateway;

namespace Bank
{
    public class Run
    {
        public static void Main(String[] args)
        {
            ClientInstance instance = new ClientInstance();
            IClientService service = MSMQClient.GetClientService(instance, args);

            service.StartService();

            Console.WriteLine();
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }
    }
}
