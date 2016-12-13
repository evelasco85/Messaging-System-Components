using System;
using System.Messaging;
using Messaging.Orchestration.Shared.Services;

namespace Test
{
    class Run
    {
        [STAThread]
        static void Main(string[] args)
        {
            ClientInstance<Message> instance = new ClientInstance<Message>();
            IClientService service = MQClient.GetClientService(instance, args);

            service.StartService();

            Console.ReadLine();
        }
    }

}
