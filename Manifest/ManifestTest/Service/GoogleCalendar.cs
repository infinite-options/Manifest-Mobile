using Manifest.Services.Google;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManifestTest.Service
{
    [TestClass]
    public class GoogleCalendar
    {
        [TestMethod]
        public void CheckingResponse()
        {
            Calendar calendar = new Calendar();
            //string accessToken = "ya29.a0AfH6SMDgwKuLiyJBvUCzyaCI3JHVPAq_ricVMI2LarulbY7WHNi0pBEkdnMQwuPK11OMhvvE8ygpL77O-YKcbYJeYqtiPcmIiy-cFHUjEUEALfb9BL4NVOpqqCTVINU3eYbrc9AfTrjBorrOCOQH56xIAMSx0dirDbg";
            string accessToken = "ya29.a0AfH6SMDgwKuLiyJBvUCzyaCI3JHVPAq_ricVMI2LarulbY7WHNi0pBEkdnMQwuPK11OMhvvE8ygpL77O-YKcbYJeYqtiPcmIiy-cFHUjEUEALfb9BL4NVOpqqCTVINU3eYbrc9AfTrjBorrOCOQH56xIAMSx0dirDbg";
            _ = calendar.GetEventsList(accessToken, DateTime.Now, DateTimeOffset.Now);
            Assert.IsTrue(true);
        }
    }
}
