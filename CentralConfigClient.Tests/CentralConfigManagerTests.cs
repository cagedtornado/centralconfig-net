using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace CentralConfigClient.Tests
{
    [TestClass]
    public class CentralConfigManagerTests
    {
        [TestMethod]
        public void GetAllApplications_ReturnsApplications()
        {
            //  Arrange
            string serviceUrl = "https://chile.lan:3800";
            CentralConfigManager mgr = new CentralConfigManager(serviceUrl);
            List<string> retval = new List<string>();

            //  Act
            retval = (List<string>)mgr.GetAllApplications().Result.Data;

            //  Assert
            Assert.IsTrue(retval.Any());

        }
    }
}
