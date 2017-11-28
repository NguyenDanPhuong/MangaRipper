using Microsoft.VisualStudio.TestTools.UnitTesting;
using MangaRipper.Core.Services;

namespace MangaRipper.Test
{
    [TestClass]
    public class DownloaderTest
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
        public void TestUrl()
        {
            string url = "http://2.bp.blogspot.com/MG09qjYxsb3sFsrMt_lTn7f9ulfgcbusQjS5wypyy0aGn0sjL7hZHQhXuS-dXZNn0tuWvdBgKICQ8WI9RFGAgNNpdYglvFdwhJZC7qiClhvEd9toNLpLky19HRRZmSFbv3zq5lw=s0?title=000_1485859774.png";
            var downloader = new Downloader();
            downloader.DownloadFileAsync(url, new System.Threading.CancellationToken());
        }
    }
}
