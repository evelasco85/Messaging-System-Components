namespace RabbitMqGateway.Core
{
    public class RQMessage
    {
        public string MessageId { get; set; }
        public object Body { get; set; }
        public string AppSpecific { get; set; }
        public string CorrelationId { get; set; }
        public byte Priority { get; set; }
        public string ReplyTo { get; set; }
    }
}
