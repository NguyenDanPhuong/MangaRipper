using MangaRipper.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MangaRipper.Test
{
    [TestClass]
    public class WinForm
    {

        [TestInitialize]
        public void Initialize()
        {
            
        }

        [TestCleanup]
        public void Cleanup()
        {

        }

        [TestMethod]
        public void UpdateNotificationTest()
        {
            var buildNumber = UpdateNotification.GetLatestBuildNumber("1.2.333");
            Assert.AreEqual("333", buildNumber);
        }
    }
}
