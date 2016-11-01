using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messaging.Base;
using Messaging.Base.System_Management.SmartProxy;
using Messaging.Orchestration.Shared.Models;

namespace Messaging.Orchestration.Shared.Services
{
    public interface IServerService
    {
    }

    public class ServerService<TMessageQueue, TMessage> : MessageConsumer<TMessageQueue, TMessage>, IServerService
    {
        private Func<ServerRequest, ServerResponse> _requestProcessor;
        private IMessageSender<TMessageQueue, TMessage> _serverReplySender;
        private Action<IMessageSender<TMessageQueue, TMessage>, ServerResponse> _sendResponse;
        private Func<TMessage, ServerRequest> _serverRequestConverter;

        public ServerService(
            IMessageReceiver<TMessageQueue, TMessage> serverRequestReceiver,
            IMessageSender<TMessageQueue, TMessage> serverReplySender,
            Func<TMessage, ServerRequest> serverRequestConverter,
            Func<ServerRequest, ServerResponse> requestProcessor,
            Action<IMessageSender<TMessageQueue, TMessage>, ServerResponse> sendResponse
            
            )
            : base(serverRequestReceiver)
        {
            _requestProcessor = requestProcessor;
            _serverReplySender = serverReplySender;
            _sendResponse = sendResponse;
            _serverRequestConverter = serverRequestConverter;
        }

        public override void ProcessMessage(TMessage message)
        {
            ServerResponse response = null;

            if ((message != null) && (_requestProcessor != null) && (_serverRequestConverter != null))
                response = _requestProcessor(_serverRequestConverter(message));

            if (_sendResponse != null)
                _sendResponse(_serverReplySender, response);
        }
    }
}
