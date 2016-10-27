using System;
using Messaging.Orchestration.Shared.Models;
using Messaging.Orchestration.Shared.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Messaging.Orchestration.Tests
{
    public class TestObject 
    {
        public const int VERSION = 29;
        public const QueueTypeEnum QUEUE_TYPE = QueueTypeEnum.MSMQ;
        public static readonly Guid ID = Guid.NewGuid();

        #region Public read-write properties
        public bool TestBoolProperty { get; set; }
        public int TestIntProperty { get; set; }
        public string TestStringProperty { get; set; }
#endregion
        #region Read-only fields
        public string ConstructParam {
            get { return _constructParam; }
        }

        public string ConstructParam2
        {
            get { return _constructParam2; }
        }

        public int ConstructParam3
        {
            get { return _constructParam3; }
        }
#endregion
        #region Private variables
        private string _constructParam;
        private string _constructParam2;
        private int _constructParam3;
#endregion

        public TestObject(string constructParam, string constructParam2, int constructParam3)
        {
            _constructParam = constructParam;
            _constructParam2 = constructParam2;
            _constructParam3 = constructParam3;
        }
    }

    [TestClass]
    public class DataTransferServiceTests
    {
        [TestMethod]
        public void TestGetObjectInformation()
        {
            IDataTransferService service = DataTransferService.GetInstance();
            IObjectInformation<int> transferObjectInformation = service.GetObjectInformation<int, TestObject>(
                TestObject.QUEUE_TYPE,
                TestObject.ID,
                TestObject.VERSION
                );


            Assert.AreEqual(29, transferObjectInformation.VersionInfo.Version);
            Assert.AreEqual(QueueTypeEnum.MSMQ, transferObjectInformation.VersionInfo.QueueType);
            Assert.IsNotNull(transferObjectInformation.VersionInfo.ID);
            Assert.IsTrue(
                (transferObjectInformation.Properties.ContainsKey("TestIntProperty")) &&
                (transferObjectInformation.Properties["TestIntProperty"] == "System.Int32")
                );
            Assert.AreEqual(2, transferObjectInformation.ConstructorParameters.Count);
            Assert.IsTrue(transferObjectInformation
                .ConstructorParameters[1]
                .ContainsKey("constructParam2"));
        }
    }
}
