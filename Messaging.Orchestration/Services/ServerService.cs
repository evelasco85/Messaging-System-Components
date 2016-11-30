using Messaging.Base;
using Messaging.Base.System_Management.SmartProxy;
using Messaging.Orchestration.Shared.Models;
using Messaging.Orchestration.Shared.Services.Interfaces;

namespace Messaging.Orchestration.Shared.Services
{
    public class ServerService<TMessage> : MessageConsumer<TMessage>, IServerService<TMessage>
    {
        private ProcessRequestDelegate _processRequest;
        private IMessageSender<TMessage> _serverReplySender;
        private SendResponseDelegate<TMessage> _sendResponse;
        private ServerRequestConverterDelegate<TMessage> _serverRequestConverter;

        public ServerService(
            IMessageReceiver<TMessage> serverRequestReceiver,
            IMessageSender<TMessage> serverReplySender
            )
            : base(serverRequestReceiver)
        {
            _serverReplySender = serverReplySender;
        }

        public void Register(
            ServerRequestConverterDelegate<TMessage> serverRequestConverter,
            SendResponseDelegate<TMessage> sendResponse,
            ProcessRequestDelegate processRequest
            )
        {
            _processRequest = processRequest;
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
