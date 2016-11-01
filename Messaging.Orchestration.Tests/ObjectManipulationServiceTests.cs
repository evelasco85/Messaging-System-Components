using System;
using System.Collections.Generic;
using Messaging.Orchestration.Models;
using Messaging.Orchestration.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Messaging.Orchestration.Shared.Services.Interfaces;

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

        public TestObject(string constructParam, string constructParam2)
        {
            _constructParam = constructParam;
            _constructParam2 = constructParam2;
        }

        public TestObject(string constructParam, string constructParam2, int constructParam3)
        {
            _constructParam = constructParam;
            _constructParam2 = constructParam2;
            _constructParam3 = constructParam3;
        }
    }

    [TestClass]
    public class ObjectManipulationServiceTests
    {
        [TestMethod]
        public void TestGetObjectInformation()
        {
            IObjectManipulationService service = ObjectManipulationService.GetInstance();
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

        [TestMethod]
        public void TestCreateDictionaryWithEmptyValues()
        {
            IObjectManipulationService service = ObjectManipulationService.GetInstance();
            IObjectInformation<int> transferObjectInformation = service.GetObjectInformation<int, TestObject>(
                TestObject.QUEUE_TYPE,
                TestObject.ID,
                TestObject.VERSION
                );

            IDictionary<string, object> emptyValueDictionary = service
                .CreateDictionaryWithEmptyValues(transferObjectInformation.ConstructorParameters[0]);

            foreach (KeyValuePair<string, object> kvp in emptyValueDictionary)
            {
                Assert.IsNull(kvp.Value);
            }
        }

        [TestMethod]
        public void TestInstantiateObject()
        {
            IObjectManipulationService service = ObjectManipulationService.GetInstance();
            IObjectInformation<int> transferObjectInformation = service.GetObjectInformation<int, TestObject>(
                TestObject.QUEUE_TYPE,
                TestObject.ID,
                TestObject.VERSION
                );

            IDictionary<string, object> constructorParams = service.CreateDictionaryWithEmptyValues(transferObjectInformation.ConstructorParameters[0]);
            IDictionary<string, object> properties = service.CreateDictionaryWithEmptyValues(transferObjectInformation.Properties);


            constructorParams["constructParam"] = "string 1";
            constructorParams["constructParam2"] = "string 2";
            constructorParams["constructParam3"] = 101;

            properties["TestBoolProperty"] = true;
            properties["TestIntProperty"] = 102;
            properties["TestStringProperty"] = "Test Property";

            TestObject obj = service.InstantiateObject<TestObject>(constructorParams, properties);

            Assert.AreEqual("string 1", obj.ConstructParam);
            Assert.AreEqual("string 2", obj.ConstructParam2);
            Assert.AreEqual(101, obj.ConstructParam3);
            Assert.AreEqual(true, obj.TestBoolProperty);
            Assert.AreEqual(102, obj.TestIntProperty);
            Assert.AreEqual("Test Property", obj.TestStringProperty);
        }
    }
}

