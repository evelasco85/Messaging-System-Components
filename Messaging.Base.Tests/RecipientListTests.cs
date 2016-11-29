using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Messaging;
using Messaging.Base.Routing;
using System.Collections.Generic;

namespace Messaging.Base.Tests
{
    [TestClass]
    public class RecipientListTests
    {
        IRecipientList<BaseRecipient, MessageQueue, Message> _recipientList = new RecipientList<BaseRecipient, MessageQueue, Message>(recipient => recipient.Queue);

        [TestInitialize]
        public void Init()
        {
            TestRecipient1 r1 = new TestRecipient1();
            TestRecipient2 r2 = new TestRecipient2();
            TestRecipient3 r3 = new TestRecipient3();
            TestRecipient4 r4 = new TestRecipient4();
            TestRecipient5 r5 = new TestRecipient5();

            _recipientList.AddRecipient(r1);
            _recipientList.AddRecipient(r2);
            _recipientList.AddRecipient(r3);
            _recipientList.AddRecipient(r4);
            _recipientList.AddRecipient(r5);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_FailedCondition()
        {
            IRecipientList<BaseRecipient, MessageQueue, Message> recipientList = new RecipientList<BaseRecipient, MessageQueue, Message>(null);
        }

        [TestMethod]
        public void TestCondition_1()
        {
            IList<IMessageSender<Message>> recipients = _recipientList.GetRecipients(recipient => recipient.Queue.Value > 3);
            Func<IMessageSender<Message>, MessageSenderGateway> cast = (sender => (MessageSenderGateway)sender);

            Assert.AreEqual(recipients.Count, 2);
            Assert.AreEqual(cast(recipients[0]).Value, 4);
            Assert.AreEqual(cast(recipients[1]).Value, 5);
        }

        [TestMethod]
        public void TestCondition_2()
        {
            IList<IMessageSender<Message>> recipients = _recipientList.GetRecipients(recipient => recipient.Queue.Value < 3);
            Func<IMessageSender<Message>, MessageSenderGateway> cast = (sender => (MessageSenderGateway)sender);

            Assert.AreEqual(recipients.Count, 2);
            Assert.AreEqual(cast(recipients[0]).Value, 1);
            Assert.AreEqual(cast(recipients[1]).Value, 2);
        }

        [TestMethod]
        public void TestCondition_3()
        {
            IList<IMessageSender<Message>> recipients = _recipientList.GetRecipients(recipient => recipient.Queue.Value == 3);
            Func<IMessageSender<Message>, MessageSenderGateway> cast = (sender => (MessageSenderGateway)sender);

            Assert.AreEqual(recipients.Count, 1);
            Assert.AreEqual(cast(recipients[0]).Value, 3);
        }

        [TestMethod]
        public void TestCondition_SelectAll()
        {
            IList<IMessageSender<Message>> recipients = _recipientList.GetRecipients();

            Assert.AreEqual(recipients.Count, 5);
        }

        [TestMethod]
        public void TestCondition_SelectAll2()
        {
            IList<IMessageSender<Message>> recipients = _recipientList.GetRecipients(null);

            Assert.AreEqual(recipients.Count, 5);
        }
    }
}