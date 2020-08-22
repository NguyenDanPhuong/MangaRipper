using MangaRipper.Helpers;
using Xunit;

namespace MangaRipper.Test
{
    public class WinForm
    {
        [Fact]
        public void UpdateNotificationTest()
        {
            var buildNumber = UpdateNotification.GetLatestBuildNumber("1.2.333");
            Assert.Equal(333, buildNumber);
        }
    }
}
