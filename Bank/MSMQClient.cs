using System;
using Bank.Models;
using MessageGateway;
using Messaging.Orchestration.Shared.Services;
using MsmqGateway;

namespace Bank
{
    class MSMQClient
    {
        public static IClientService GetClientService(ClientInstance instance, String[] args)
        {
            String requestQueue = string.Empty;
            String bankName = string.Empty;
            String ratePremium = string.Empty;
            String maxLoanTerm = string.Empty;

            return MQOrchestration.GetInstance().CreateClient(
                args[0],
                "MSMQ",
                ToPath(args[1]),
                ToPath(args[2])
                )
                .Register(registration =>
                {
                    //Server parameter requests
                    registration.RegisterRequiredServerParameters("requestQueue",
                        (value) => requestQueue = (string) value);
                    registration.RegisterRequiredServerParameters("bankName", (value) => bankName = (string) value);
                    registration.RegisterRequiredServerParameters("ratePremium", (value) => ratePremium = (string) value);
                    registration.RegisterRequiredServerParameters("maxLoanTerm", (value) => maxLoanTerm = (string) value);
                },
                    errorMessage =>
                    {
                        //Invalid registration
                    },
                    () =>
                    {
                        //Client parameter setup completed
                        instance.SetupBank(bankName, ratePremium, maxLoanTerm);
                        instance.SetupQueueService(
                            new MQRequestReplyService_Synchronous(
                                ToPath(requestQueue),
                                new SyncProcessMessageDelegate(instance.Bank1.ProcessRequestMessage),
                                null,
                                new GetRequestBodyTypeDelegate(() => { return typeof(BankQuoteRequest); }))
                            );

                        Console.WriteLine("Configurations ok!");
                    },
                    () =>
                    {
                        //Stand-by
                        Console.WriteLine("Stand-by Application!");
                    },
                    () =>
                    {
                        //Start
                        instance.Run();
                    },
                    () =>
                    {
                        //Stop
                        instance.Stop();
                    });
        }

        static String ToPath(String arg)
        {
            return ".\\private$\\" + arg;
        }
    }
}
