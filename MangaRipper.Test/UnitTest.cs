using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MangaRipper.Core;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Threading;

namespace MangaRipper.Test
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public async Task TestMangaReader_GetChapters()
        {
            string naruto = "http://www.mangareader.net/naruto";
            ITitle title = TitleFactory.CreateTitle(naruto);
            Assert.IsNotNull(title);
            var chapters = await title.PopulateChapterAsync(new Progress<int>(percent =>
            {
                Console.WriteLine(percent);
            }), new CancellationTokenSource().Token);

            Assert.IsTrue(chapters.Count > 0);
        }

        [TestMethod]
        public async Task TestMangaReader_ParseImages()
        {
            string naruto_700 = "http://www.mangareader.net/naruto/700";
            var chap = new ChapterMangaReader("Naruto 700", naruto_700);
            var webclient = new WebClient();
            string html = await webclient.DownloadStringTaskAsync(naruto_700);
            Assert.IsTrue(chap.ImageAddresses == null);
            await chap.PopulateImageAddress(html);
            Assert.IsTrue(chap.ImageAddresses.Count > 0);
        }

        //[TestMethod]
        public async Task TestMangaFox_GetChapters()
        {
            string naruto = "http://mangafox.me/manga/poputepipikku/";
            ITitle title = TitleFactory.CreateTitle(naruto);
            Assert.IsNotNull(title);
            var chapters = await title.PopulateChapterAsync(new Progress<int>(percent =>
            {
                Console.WriteLine(percent);
            }), new CancellationTokenSource().Token);

            Assert.IsTrue(chapters.Count > 0);
        }

        //[TestMethod]
        public async Task TestMangaFox_ParseImages()
        {
            string naruto_700 = "http://mangafox.me/manga/poputepipikku/v02/c021/1.html";
            var chap = new ChapterMangaFox("Naruto 700", naruto_700);

            var webclient = WebRequest.CreateHttp(naruto_700);
            webclient.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            var response = await webclient.GetResponseAsync();
            using (Stream stream = response.GetResponseStream())
            {
                StreamReader sr = new StreamReader(stream, System.Text.Encoding.UTF8);
                string html = await sr.ReadToEndAsync();
                await chap.PopulateImageAddress(html);
            }

            Assert.IsTrue(chap.ImageAddresses.Count > 0);
        }
    }
}
