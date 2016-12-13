using Messaging.Base;
using RabbitMQ.Client;
using System;

namespace RabbitMqGateway
{
    public class MessageQueueGateway : QueueGateway<IModel>, IDisposable
    {
        private string _queueName;
        private IModel _channel;

        public override string QueueName
        {
            get { return _queueName; }
        }

        public void Dispose()
        {
            if (_channel != null)
                _channel.Dispose();
        }

        public MessageQueueGateway(ConnectionFactory factory, string queueName)
        {
            _queueName = queueName;

            IConnection connection = factory.CreateConnection();
            IModel channel = connection.CreateModel();

            channel.QueueDeclare(_queueName, false, false, false, null);

            SetQueue(channel);
        }

        public override void SetQueue(IModel queue)
        {
            _channel = queue;
        }

        public override IModel GetQueue()
        {
            return _channel;
        }
    }
}
