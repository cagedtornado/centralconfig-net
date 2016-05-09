using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using CentralConfigClient.Models;
using System.Net;

namespace CentralConfigClient.Tests
{
    [TestClass]
    public class CentralConfigManagerTests
    {
        private string _serviceUrl = "";

        [TestInitialize]
        public void Setup_Tests()
        {
            //  Set this to the base url for your centralconfig service
            _serviceUrl = Environment.GetEnvironmentVariable("centralconfig_test_endpoint");
        }

        [TestMethod]
        public void GetAllApplications_ReturnsApplications()
        {
            //  Arrange
            CentralConfigManager config = new CentralConfigManager(_serviceUrl);
            List<string> retval = new List<string>();

            //  Act
            var response = config.GetAllApplications().Result;
            retval = response.Data;

            //  Assert
            Assert.IsTrue(retval.Any());

        }

        [TestMethod]
        public void GetAll_ReturnsConfigItems()
        {
            //  Arrange
            CentralConfigManager config = new CentralConfigManager(_serviceUrl);
            List<ConfigItem> retval = new List<ConfigItem>();

            //  Act
            var response = config.GetAll().Result;
            retval = response.Data;

            //  Assert
            Assert.IsTrue(retval.Any());
            Assert.AreNotEqual<DateTime>(DateTime.MinValue, retval[0].LastUpdated);
        }

        [TestMethod]
        public void Get_ValidParameters_ReturnsConfigItem()
        {
            //  Arrange
            CentralConfigManager config = new CentralConfigManager(_serviceUrl);
            ConfigItem retval = new ConfigItem();
            string application = "Formbuilder";
            string configName = "AnotherItem";

            //  Act
            var response = config.Get(application, configName).Result;
            retval = response.Data;

            //  Assert
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.AreEqual<string>(configName, retval.Name);
            Assert.AreEqual<string>(application, retval.Application);
            Assert.AreEqual<string>("Test", retval.Value);
        }

        [TestMethod]
        public void GetAll_ValidApplication_ReturnsConfigItems()
        {
            //  Arrange
            CentralConfigManager config = new CentralConfigManager(_serviceUrl);
            List<ConfigItem> retval = new List<ConfigItem>();
            string application = "Formbuilder";

            //  Act
            var response = config.GetAll(application).Result;
            retval = response.Data;

            //  Assert
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.IsTrue(retval.Any());
            Assert.AreNotEqual<DateTime>(DateTime.MinValue, retval[0].LastUpdated);
        }


        [TestMethod]
        public void Set_ValidParameters_Successful()
        {
            //  Arrange
            CentralConfigManager config = new CentralConfigManager(_serviceUrl);
            ConfigItem retval = new ConfigItem();
            ConfigItem newItem = new ConfigItem()
            {
                Application = "AwesomeApp",
                Name = "Squeaky",
                Value = "Clean"
            };

            //  Act
            var response = config.Set(newItem).Result;
            retval = response.Data;

            //  Assert
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.AreEqual<string>(newItem.Name, retval.Name);
            Assert.AreEqual<string>(newItem.Application, retval.Application);
            Assert.AreEqual<string>(newItem.Value, retval.Value);
            Assert.AreNotEqual(newItem.Id, retval.Value);
        }

        [TestMethod]
        public void Remove_ValidParameters_Successful()
        {
            //  Arrange
            CentralConfigManager config = new CentralConfigManager(_serviceUrl);
            ConfigItem removedItem = new ConfigItem()
            {
                Application = "AwesomeApp",
                Name = "Squeaky"
            };

            //  Act
            var response = config.Remove(removedItem).Result;

            //  Assert
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            
        }
    }
}
