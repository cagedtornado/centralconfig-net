using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CentralConfigClient.Tests
{
    /// <summary>
    /// Summary description for FeatureFlagTests
    /// </summary>
    [TestClass]
    public class FeatureFlagTests
    {
        private string _serviceUrl = "";

        [TestInitialize]
        public void Setup_Tests()
        {
            //  Set this to the base url for your centralconfig service
            _serviceUrl = Environment.GetEnvironmentVariable("centralconfig_test_endpoint");
        }

        [TestMethod]
        public void FeatureEnabled_ValidFeatureFlag_ReturnsTrue()
        {
            //  Arrange
            CentralConfigManager config = new CentralConfigManager(_serviceUrl, "UnitTest", "testuser", "testgroup", false, false);
            bool expectedFeatureState = true;

            //  Act
            bool featureEnabled = config.FeatureEnabled("Unit test feature");

            //  Assert
            Assert.AreEqual(expectedFeatureState, featureEnabled);
        }
    }
}
