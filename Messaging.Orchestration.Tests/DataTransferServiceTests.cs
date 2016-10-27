using System;
using Messaging.Orchestration.Shared.Models;
using Messaging.Orchestration.Shared.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Messaging.Orchestration.Tests
{
    public class TestObject : IVersionInfo<int>
    {
        public int Version {
            get { return 29; }
        }

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

        public TestObject()
        {
        }

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
            TestObject obj = new TestObject();
            IObjectInformation<int> transferObjectInformation = service.GetObjectInformation<int, TestObject>(obj);

            Assert.AreEqual(29, transferObjectInformation.VersionInfo.Version);
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
