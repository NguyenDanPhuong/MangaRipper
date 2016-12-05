using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MangaRipper.Core;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Linq;

namespace MangaRipper.Test
{
    [TestClass]
    public class UnitTest
    {
        CancellationTokenSource source;
        [TestInitialize]
        public void Initialize()
        {
            Framework.Init();
            source = new CancellationTokenSource();
        }

        [TestMethod]
        public async Task MangaReader_Test()
        {
            string url = "http://www.mangareader.net/naruto";
            var service = Framework.GetService(url);
            // Test service can find chapters
            var chapters = await service.FindChapters(url, new Progress<int>(), source.Token);
            Assert.IsTrue(chapters.Count() > 0, "Cannot find chapters.");
            // Test chapters are in correct order.
            var chapter = chapters.Last();
            Assert.AreEqual("Naruto 1", chapter.Name);
            Assert.AreEqual("http://www.mangareader.net/naruto/1", chapter.Url);
            // Test there're no duplicated chapters.
            var anyDuplicated = chapters.GroupBy(x => x.Url).Any(g => g.Count() > 1);
            Assert.IsFalse(anyDuplicated, "There're duplicated chapters.");
            // Test service can find images.
            var images = await service.FindImanges(chapter, new Progress<int>(), source.Token);
            Assert.AreEqual(53, images.Count());
            Assert.AreEqual("http://i10.mangareader.net/naruto/1/naruto-1564773.jpg", images.ToArray()[0]);
            Assert.AreEqual("http://i4.mangareader.net/naruto/1/naruto-1564774.jpg", images.ToArray()[1]);
            Assert.AreEqual("http://i1.mangareader.net/naruto/1/naruto-1564825.jpg", images.ToArray()[52]);

            var downloader = new Downloader();
            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0]);
            Assert.IsNotNull(imageString, "Cannot download image!");
        }

        [TestMethod]
        public async Task MangaFox_Test()
        {
            // Test with unlicensed manga. Appveyor CI is US based and cannot access licensed manga in the US. 
            // If we test with a licensed manga, this test will failed.
            string url = "http://mangafox.me/manga/tian_jiang_xian_shu_nan/";
            var service = Framework.GetService(url);
            var chapters = await service.FindChapters(url, new Progress<int>(), source.Token);
            Assert.IsTrue(chapters.Count() > 0, "Cannot find chapters.");
            var chapter = chapters.Last();
            Assert.AreEqual("Tian Jiang Xian Shu Nan 1", chapter.Name);
            Assert.AreEqual("http://mangafox.me/manga/tian_jiang_xian_shu_nan/c001/1.html", chapter.Url);
            var images = await service.FindImanges(chapter, new Progress<int>(), source.Token);
            Assert.AreEqual(15, images.Count());
            Assert.AreEqual("http://h.mfcdn.net/store/manga/19803/001.0/compressed/q001.jpg", images.ToArray()[0]);
            Assert.AreEqual("http://h.mfcdn.net/store/manga/19803/001.0/compressed/q002.jpg", images.ToArray()[1]);
            Assert.AreEqual("http://h.mfcdn.net/store/manga/19803/001.0/compressed/q015.jpg", images.ToArray()[14]);

            var downloader = new Downloader();
            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0]);
            Assert.IsNotNull(imageString, "Cannot download image!");
        }

        [TestMethod]
        public async Task MangaHere_Test()
        {
            string url = "http://www.mangahere.co/manga/the_god_of_high_school/";
            var service = Framework.GetService(url);
            var chapters = await service.FindChapters(url, new Progress<int>(), source.Token);
            Assert.IsTrue(chapters.Count() > 0, "Cannot find chapters.");
            var chapter = chapters.Last();
            Assert.AreEqual("The God Of High School 1", chapter.Name);
            Assert.AreEqual("http://www.mangahere.co/manga/the_god_of_high_school/c001/", chapter.Url);
            var images = await service.FindImanges(chapter, new Progress<int>(), source.Token);
            Assert.AreEqual(55, images.Count());
            Assert.AreEqual("http://h.mhcdn.net/store/manga/9275/001.0/compressed/m001.01.jpg", images.ToArray()[0]);
            Assert.AreEqual("http://h.mhcdn.net/store/manga/9275/001.0/compressed/m001.02.jpg", images.ToArray()[1]);
            Assert.AreEqual("http://h.mhcdn.net/store/manga/9275/001.0/compressed/m001.55.jpg", images.ToArray()[54]);

            var downloader = new Downloader();
            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0]);
            Assert.IsNotNull(imageString, "Cannot download image!");
        }

        [TestMethod]
        public async Task MangaShare_Test()
        {
            string url = "http://read.mangashare.com/Gantz";
            var service = Framework.GetService(url);
            var chapters = await service.FindChapters(url, new Progress<int>(), source.Token);
            Assert.IsTrue(chapters.Count() > 0, "Cannot find chapters.");
            var chapter = chapters.Last();
            Assert.AreEqual("Gantz 1", chapter.Name);
            Assert.AreEqual("http://read.mangashare.com/Gantz/chapter-001/page001.html", chapter.Url);
            var images = await service.FindImanges(chapter, new Progress<int>(), source.Token);
            Assert.AreEqual(43, images.Count());
            Assert.AreEqual("http://dl01.mangashare.com/manga/Gantz/001/001.jpg", images.ToArray()[0]);
            Assert.AreEqual("http://dl01.mangashare.com/manga/Gantz/001/002.jpg", images.ToArray()[1]);
            Assert.AreEqual("http://dl01.mangashare.com/manga/Gantz/001/043.jpg", images.ToArray()[42]);

            var downloader = new Downloader();
            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0]);
            Assert.IsNotNull(imageString, "Cannot download image!");
        }

        //[TestMethod]
        public async Task KissManga_Test()
        {
            string url = "http://kissmanga.com/Manga/Beelzebub";
            var service = Framework.GetService(url);
            var chapters = await service.FindChapters(url, new Progress<int>(), source.Token);
            Assert.IsTrue(chapters.Count() > 0, "Cannot find chapters.");
            var chapter = chapters.Last();
            Assert.AreEqual("Beelzebub Babu 000", chapter.Name);
            Assert.AreEqual("http://kissmanga.com/Manga/Beelzebub/Babu-000?id=285306", chapter.Url);
            var images = await service.FindImanges(chapter, new Progress<int>(), source.Token);
            Assert.AreEqual(49, images.Count());
            Assert.AreEqual("http://2.bp.blogspot.com/-E8XYLQErJFc/Vjg04wA34iI/AAAAAAABEsY/oDSYgrsnCJM/s16000/0000-001.jpg", images.ToArray()[0]);
            Assert.AreEqual("http://2.bp.blogspot.com/-mSKwfmFrAqU/Vjgwuhxru8I/AAAAAAABDrA/N7pxjd0d_UA/s16000/0000-002.jpg", images.ToArray()[1]);
            Assert.AreEqual("http://2.bp.blogspot.com/-92zJArhtoVs/VjgyxNnDjtI/AAAAAAABEKg/367AcQpasUU/s16000/0000-049.jpg", images.ToArray()[48]);

            var downloader = new Downloader();
            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0]);
            Assert.IsNotNull(imageString, "Cannot download image!");
        }
    }
}
