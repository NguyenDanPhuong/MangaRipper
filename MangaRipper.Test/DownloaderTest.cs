using Microsoft.VisualStudio.TestTools.UnitTesting;
using MangaRipper.Core.Services;
using System.Threading.Tasks;

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
        public async Task TestUrlAsync()
        {
            //string url = "http://2.bp.blogspot.com/MG09qjYxsb3sFsrMt_lTn7f9ulfgcbusQjS5wypyy0aGn0sjL7hZHQhXuS-dXZNn0tuWvdBgKICQ8WI9RFGAgNNpdYglvFdwhJZC7qiClhvEd9toNLpLky19HRRZmSFbv3zq5lw=s0?title=000_1485859774.png";
            //var downloader = new Downloader();
            //string fileFromServer = await downloader.DownloadToFolder(url, new System.Threading.CancellationToken());
            //Assert.AreEqual("000_1485859774.jpg", fileFromServer);
        }
    }
}
