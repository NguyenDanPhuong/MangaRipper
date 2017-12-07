using Microsoft.VisualStudio.TestTools.UnitTesting;
using MangaRipper.Core.Services;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

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
            //<a class="color_0077" href="([^"]+)" >\n              ([^<]+)            </a>

            string s = @"
               <span class=""left"">
            <a class=""color_0077"" href=""//www.mangahere.co/manga/the_god_of_high_school/c325/"" >
              The God Of High School 325            </a>
          <span class=""mr6""></span></span>
";

            var r = new Regex("<a class=\"color_0077\" href=\"(?<Value>[^\"]+)\" >\r\n              (?<Name>[^<]+)            </a>").Matches(s);
            Assert.IsTrue(r.Count > 0);
        }
    }
}
