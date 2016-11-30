using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using Messaging.Base.Constructions;

namespace Messaging.Base.Tests
{
    public class MessageSenderGateway : SenderGateway<MessageQueue, Message>
    {
        public int Value = 0;

        public MessageSenderGateway(int value) : base(null)
        {
            Value = value;
        }

        public override void Send(Message message)
        {
        }

        public override IReturnAddress<Message> AsReturnAddress()
        {
            throw new NotImplementedException();
        }

        public override void SetupSender()
        {
            throw new NotImplementedException();
        }

        public override void Send<TEntity>(TEntity message)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class BaseRecipient
    {
        public string Name = string.Empty;

        public BaseRecipient(string name)
        {
            Name = name;
        }

        public abstract MessageSenderGateway Queue { get; }
    }

    public class TestRecipient1 : BaseRecipient
    {
        public TestRecipient1() : base("One") { }
        MessageSenderGateway queue = new MessageSenderGateway(1);

        public override MessageSenderGateway Queue
        {
            get { return queue; }
        }
    }

    public class TestRecipient2 : BaseRecipient
    {
        public TestRecipient2() : base("Two") { }
        MessageSenderGateway queue = new MessageSenderGateway(2);

        public override MessageSenderGateway Queue
        {
            get { return queue; }
        }
    }

    public class TestRecipient3 : BaseRecipient
    {
        public TestRecipient3() : base("Three") { }
        MessageSenderGateway queue = new MessageSenderGateway(3);

        public override MessageSenderGateway Queue
        {
            get { return queue; }
        }
    }

    public class TestRecipient4 : BaseRecipient
    {
        public TestRecipient4() : base("Four") { }
        MessageSenderGateway queue = new MessageSenderGateway(4);

        public override MessageSenderGateway Queue
        {
            get { return queue; }
        }
    }

    public class TestRecipient5 : BaseRecipient
    {
        public TestRecipient5() : base("Five") { }
        MessageSenderGateway queue = new MessageSenderGateway(5);

        public override MessageSenderGateway Queue
        {
            get { return queue; }
        }
    }
}
