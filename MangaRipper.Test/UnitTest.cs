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
        [TestInitialize]
        public void Initialize()
        {
            Framework.Register(new MangaFoxImpl());
        }

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

        [TestMethod]
        public async Task MangaFox_Test()
        {
            string url = "http://mangafox.me/manga/poputepipikku";
            var service = Framework.GetService(url);
            var chapters = await service.FindChapters(url);
            Assert.IsTrue(chapters.Count > 0, "Cannot find chapters.");
            var chapter = chapters[0];
            var images = await service.FindImanges(chapter);
            Assert.IsTrue(images.Count > 0, "Cannot find images.");
        }
    }
}
