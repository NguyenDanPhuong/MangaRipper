using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.Net;
using System.Text;
using MangaRipper.Core.Services;
using MangaRipper.Core.Providers;

namespace MangaRipper.Test
{
    [TestClass]
    public class UnitTest
    {
        CancellationTokenSource source;

        [TestInitialize]
        public void Initialize()
        {
            FrameworkProvider.Init();
            source = new CancellationTokenSource();
        }

        [TestCleanup]
        public void Cleanup()
        {
        }

        [TestMethod]
        public async Task MangaReader_Test()
        {
            string url = "http://www.mangareader.net/naruto";
            var service = FrameworkProvider.GetService(url);
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

            var downloader = new DownloadService();
            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0]);
            Assert.IsNotNull(imageString, "Cannot download image!");
        }

        [TestMethod]
        public async Task MangaFox_Test()
        {
            // Test with unlicensed manga. Appveyor CI is US based and cannot access licensed manga in the US. 
            // If we test with a licensed manga, this test will failed.
            string url = "http://mangafox.me/manga/tian_jiang_xian_shu_nan/";
            var service = FrameworkProvider.GetService(url);
            var chapters = await service.FindChapters(url, new Progress<int>(), source.Token);
            Assert.IsTrue(chapters.Count() > 0, "Cannot find chapters.");
            var chapter = chapters.Last();
            Assert.AreEqual("Tian Jiang Xian Shu Nan 1", chapter.Name);
            Assert.AreEqual("http://mangafox.me/manga/tian_jiang_xian_shu_nan/c001/1.html", chapter.Url);
            var images = await service.FindImanges(chapter, new Progress<int>(), source.Token);
            Assert.AreEqual(15, images.Count());
            Assert.IsTrue(images.ToArray()[0].StartsWith("http://h.mfcdn.net/store/manga/19803/001.0/compressed/q001.jpg"));
            Assert.IsTrue(images.ToArray()[1].StartsWith("http://h.mfcdn.net/store/manga/19803/001.0/compressed/q002.jpg"));
            Assert.IsTrue(images.ToArray()[2].StartsWith("http://h.mfcdn.net/store/manga/19803/001.0/compressed/q003.jpg"));

            var downloader = new DownloadService();
            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0]);
            Assert.IsNotNull(imageString, "Cannot download image!");
        }

        [TestMethod]
        public async Task MangaHere_Test()
        {
            string url = "http://www.mangahere.co/manga/the_god_of_high_school/";
            var service = FrameworkProvider.GetService(url);
            var chapters = await service.FindChapters(url, new Progress<int>(), source.Token);
            Assert.IsTrue(chapters.Count() > 0, "Cannot find chapters.");
            var chapter = chapters.Last();
            Assert.AreEqual("The God Of High School 1", chapter.Name);
            Assert.AreEqual("http://www.mangahere.co/manga/the_god_of_high_school/c001/", chapter.Url);
            var images = await service.FindImanges(chapter, new Progress<int>(), source.Token);
            Assert.AreEqual(55, images.Count());
            Assert.IsTrue(images.ToArray()[0].StartsWith("http://h.mhcdn.net/store/manga/9275/001.0/compressed/m001.01.jpg"));
            Assert.IsTrue(images.ToArray()[1].StartsWith("http://h.mhcdn.net/store/manga/9275/001.0/compressed/m001.02.jpg"));
            Assert.IsTrue(images.ToArray()[54].StartsWith("http://h.mhcdn.net/store/manga/9275/001.0/compressed/m001.55.jpg"));

            var downloader = new DownloadService();
            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0]);
            Assert.IsNotNull(imageString, "Cannot download image!");
        }

        [TestMethod]
        public async Task MangaShare_Test()
        {
            string url = "http://read.mangashare.com/Gantz";
            var service = FrameworkProvider.GetService(url);
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

            var downloader = new DownloadService();
            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0]);
            Assert.IsNotNull(imageString, "Cannot download image!");
        }

        [TestMethod]
        public async Task Batoto_Test()
        {
            string url = "http://bato.to/comic/_/comics/21st-century-boys-r1591";
            var service = FrameworkProvider.GetService(url);
            var chapters = await service.FindChapters(url, new Progress<int>(), source.Token);
            Assert.IsTrue(chapters.Any(), "Cannot find chapters.");
            var chapter = chapters.Last();
            Assert.AreEqual("Vol.01 Ch.01 Read Online", chapter.Name);
            Assert.AreEqual("http://bato.to/reader#900d11d96d1466f2", chapter.Url);
            var images = await service.FindImanges(chapter, new Progress<int>(), source.Token);
            Assert.AreEqual(31, images.Count());
            Assert.IsTrue(images.ToArray()[0].StartsWith("http://img.bato.to/comics/2014/10/08/2/read54357eb5e1ca9/img000001.jpg"));
            Assert.IsTrue(images.ToArray()[1].StartsWith("http://img.bato.to/comics/2014/10/08/2/read54357eb5e1ca9/img000002.jpg"));
            Assert.IsTrue(images.ToArray()[2].StartsWith("http://img.bato.to/comics/2014/10/08/2/read54357eb5e1ca9/img000003.jpg"));

            var downloader = new DownloadService();
            string imageString = await downloader.DownloadStringAsync(images.ToArray()[0]);
            Assert.IsNotNull(imageString, "Cannot download image!");
        }
    }
}
