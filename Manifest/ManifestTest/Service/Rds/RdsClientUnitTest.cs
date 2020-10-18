using System;
using System.Collections.Generic;
using Manifest.Config;
using Manifest.Models;
using Manifest.Services;
using Manifest.Services.Rds;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManifestTest.Service.Rds
{
    [TestClass]
    public class RdsClientUnitTest
    {

        IDataClient rdsClient = DataFactory.Instance.GetDataClient();
        [TestMethod]
        public void TestUserNotNull()
        {
            var userTask = rdsClient.GetUser("100-000028");
            userTask.Wait();
            User user = userTask.Result;
            Assert.IsTrue(user != null);
            Assert.IsTrue(user.FirstName != null);
            Assert.IsTrue(user.ImportantPeople!=null);
            Assert.IsTrue(user.TimeSettings != null);
        }

        [TestMethod]
        public void TestOccuranceNotNull()
        {
            var occuranceTask = rdsClient.GetOccurances("100-000028");
            occuranceTask.Wait();
            List<Occurance> occurances = occuranceTask.Result;
            Assert.IsTrue(occurances != null);
            Assert.IsTrue(occurances.Count>0);
        }

        [TestMethod]
        public void TestSubOccuranceNotNull()
        {
            var subOccuranceTask = rdsClient.GetSubOccurances("300-000049");
            subOccuranceTask.Wait();
            List<SubOccurance> subOccurances = subOccuranceTask.Result;
            Assert.IsTrue(subOccurances != null);
            Assert.IsTrue(subOccurances.Count > 0);
        }

        [TestMethod]
        public void TestUpdateOccurance()
        {
            Occurance occur = new Occurance()
            {
                Id = "300-000001",
                DateTimeCompleted = DateTime.UtcNow,
                DateTimeStarted = DateTime.UtcNow,
                IsInProgress = false,
                IsComplete = false
            };

            rdsClient.UpdateOccurance(occur);
            Assert.IsTrue(true);
        }
    }
}
