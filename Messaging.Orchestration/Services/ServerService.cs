using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messaging.Base;
using Messaging.Base.System_Management.SmartProxy;
using Messaging.Orchestration.Shared.Models;
using Messaging.Orchestration.Shared.Services.Interfaces;
using System.Threading;

namespace Messaging.Orchestration.Shared.Services
{
    public class ServerService<TMessageQueue, TMessage> : MessageConsumer<TMessageQueue, TMessage>, IServerService<TMessageQueue, TMessage>
    {
        private ProcessRequestDelegate _processRequest;
        private IMessageSender<TMessageQueue, TMessage> _serverReplySender;
        private SendResponseDelegate<TMessageQueue, TMessage> _sendResponse;
        private ServerRequestConverterDelegate<TMessage> _serverRequestConverter;

        public ServerService(
            IMessageReceiver<TMessageQueue, TMessage> serverRequestReceiver,
            IMessageSender<TMessageQueue, TMessage> serverReplySender,
            ServerRequestConverterDelegate<TMessage> serverRequestConverter,
            ProcessRequestDelegate processRequest,
            SendResponseDelegate<TMessageQueue, TMessage> sendResponse
            )
            : base(serverRequestReceiver)
        {
            _processRequest = processRequest;
            _serverReplySender = serverReplySender;
            _sendResponse = sendResponse;
            _serverRequestConverter = serverRequestConverter;
        }

        public override void ProcessMessage(TMessage message)
        {
            ServerMessage response = null;

            if ((message != null) && (_processRequest != null) && (_serverRequestConverter != null))
                response = _processRequest(_serverRequestConverter(message));

            SendClientMessage(response);
        }

        public void SendClientMessage(ServerMessage serverMessage)
        {
            if ((_sendResponse != null) && (serverMessage != null))
                _sendResponse(_serverReplySender, serverMessage);
        }
    }
}
