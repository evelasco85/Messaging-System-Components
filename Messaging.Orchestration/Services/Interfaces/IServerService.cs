using Messaging.Base;
using Messaging.Base.System_Management.SmartProxy;
using Messaging.Orchestration.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Orchestration.Shared.Services.Interfaces
{
    public delegate ServerMessage ProcessRequestDelegate(ServerRequest request);
    public delegate void SendResponseDelegate<TMessageQueue, TMessage>(IMessageSender<TMessageQueue, TMessage> sender, ServerMessage message);
    public delegate ServerRequest ServerRequestConverterDelegate<TMessage>(TMessage message);

    public interface IServerService<TMessageQueue, TMessage> : IMessageConsumer<TMessageQueue, TMessage>
    {
        void Register(
            ServerRequestConverterDelegate<TMessage> serverRequestConverter,
            SendResponseDelegate<TMessageQueue, TMessage> sendResponse,
            ProcessRequestDelegate processRequest
            );
        void SendClientMessage(ServerMessage serverMessage);
    }
}
