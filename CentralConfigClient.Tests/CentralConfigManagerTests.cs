using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using CentralConfigClient.Models;

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
            _serviceUrl = "https://chile.lan:3800";
        }

        [TestMethod]
        public void GetAllApplications_ReturnsApplications()
        {
            //  Arrange
            CentralConfigManager mgr = new CentralConfigManager(_serviceUrl);
            List<string> retval = new List<string>();

            //  Act
            retval = mgr.GetAllApplications().Result.Data;

            //  Assert
            Assert.IsTrue(retval.Any());

        }

        [TestMethod]
        public void GetAllConfigItems_ReturnsConfigItems()
        {
            //  Arrange
            CentralConfigManager mgr = new CentralConfigManager(_serviceUrl);
            List<ConfigItem> retval = new List<ConfigItem>();

            //  Act
            retval = mgr.GetAllConfigItems().Result.Data;

            //  Assert
            Assert.IsTrue(retval.Any());
            Assert.AreNotEqual<DateTime>(DateTime.MinValue, retval[0].LastUpdated);
        }
    }
}
