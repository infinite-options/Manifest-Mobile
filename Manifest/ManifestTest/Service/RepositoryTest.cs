using System;
using Manifest.Models;
using Manifest.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManifestTest.Service
{
    [TestClass]
    public class RepositoryTest
    {
        Repository repo = Repository.Instance;

        [TestMethod]
        public void GetUserTest()
        {
            var task = repo.GetUser("100-000028");
            task.Wait();
            User user = task.Result;  
            Assert.IsNotNull(user);
        }
    }
}
