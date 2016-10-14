using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.System_Management.SmartProxy
{
    public interface ISmartProxyRequestConsumer<TMessageQueue, TMessage> : IMessageConsumer<TMessageQueue, TMessage>
    {
        void AnalyzeMessage(TMessage message);
    }

    public abstract class SmartProxyRequestConsumer<TMessageQueue, TMessage> : MessageConsumer<TMessageQueue, TMessage>, ISmartProxyRequestConsumer<TMessageQueue, TMessage>
    {
        private IMessageSender<TMessageQueue, TMessage> _serviceRequest;
        //private IMessageSender<TMessageQueue, TMessage> _serviceReply;
        

        public SmartProxyRequestConsumer(
            IMessageReceiver<TMessageQueue, TMessage> request,
            IMessageSender<TMessageQueue, TMessage> serviceRequest
            //,IMessageSender<TMessageQueue, TMessage> serviceReply
            ) : base(request)
        {
            _serviceRequest = serviceRequest;
            //_serviceReply = serviceReply;
        }

        public override void ProcessMessage(TMessage message)
        {
            MessageReferenceData<TMessageQueue, TMessage> reference = GetReferenceData(message);

            _serviceRequest.Send(message);
            ReferenceData.Add(reference);
            AnalyzeMessage(message);
        }

        public abstract void AnalyzeMessage(TMessage message);
        
    }
}
