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
            User user  =  repo.GetUser("100-000028");
            Assert.IsNotNull(user);
        }
    }
}
