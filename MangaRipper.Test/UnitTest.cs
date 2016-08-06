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
            Assert.IsTrue(chapters.Count > 0, "Cannot find chapters.");
            // Test chapters are in correct order.
            var chapter = chapters.Last();
            Assert.AreEqual("Naruto 1", chapter.Name);
            Assert.AreEqual("http://www.mangareader.net/naruto/1", chapter.Link);
            // Test there're no duplicated chapters.
            var anyDuplicated = chapters.GroupBy(x => x.Link).Any(g => g.Count() > 1);
            Assert.IsFalse(anyDuplicated, "There're duplicated chapters.");
            // Test service can find images.
            var images = await service.FindImanges(chapter, new Progress<int>(), source.Token);
            Assert.AreEqual(53, images.Count);
            Assert.AreEqual("http://i10.mangareader.net/naruto/1/naruto-1564773.jpg", images[0]);
            Assert.AreEqual("http://i4.mangareader.net/naruto/1/naruto-1564774.jpg", images[1]);
            Assert.AreEqual("http://i1.mangareader.net/naruto/1/naruto-1564825.jpg", images[52]);
        }

        [TestMethod]
        public async Task MangaFox_Test()
        {
            // Test with unlicensed manga. Appveyor CI is US based and cannot access licensed manga in the US. 
            // If we test with a licensed manga, this test will failed.
            string url = "http://mangafox.me/manga/tian_jiang_xian_shu_nan/";
            var service = Framework.GetService(url);
            var chapters = await service.FindChapters(url, new Progress<int>(), source.Token);
            Assert.IsTrue(chapters.Count > 0, "Cannot find chapters.");
            var chapter = chapters.Last();
            Assert.AreEqual("Tian Jiang Xian Shu Nan 1", chapter.Name);
            Assert.AreEqual("http://mangafox.me/manga/tian_jiang_xian_shu_nan/c001/1.html", chapter.Link);
            var images = await service.FindImanges(chapter, new Progress<int>(), source.Token);
            Assert.AreEqual(15, images.Count);
            Assert.AreEqual("http://h.mfcdn.net/store/manga/19803/001.0/compressed/q001.jpg", images[0]);
            Assert.AreEqual("http://h.mfcdn.net/store/manga/19803/001.0/compressed/q002.jpg", images[1]);
            Assert.AreEqual("http://h.mfcdn.net/store/manga/19803/001.0/compressed/q015.jpg", images[14]);
        }

        [TestMethod]
        public async Task MangaHere_Test()
        {
            string url = "http://www.mangahere.co/manga/the_god_of_high_school/";
            var service = Framework.GetService(url);
            var chapters = await service.FindChapters(url, new Progress<int>(), source.Token);
            Assert.IsTrue(chapters.Count > 0, "Cannot find chapters.");
            var chapter = chapters.Last();
            Assert.AreEqual("The God Of High School 1", chapter.Name);
            Assert.AreEqual("http://www.mangahere.co/manga/the_god_of_high_school/c001/", chapter.Link);
            var images = await service.FindImanges(chapter, new Progress<int>(), source.Token);
            Assert.AreEqual(55, images.Count);
            Assert.AreEqual("http://h.mhcdn.net/store/manga/9275/001.0/compressed/m001.01.jpg", images[0]);
            Assert.AreEqual("http://h.mhcdn.net/store/manga/9275/001.0/compressed/m001.02.jpg", images[1]);
            Assert.AreEqual("http://h.mhcdn.net/store/manga/9275/001.0/compressed/m001.55.jpg", images[54]);
        }

        [TestMethod]
        public async Task MangaShare_Test()
        {
            string url = "http://read.mangashare.com/Beelzebub";
            var service = Framework.GetService(url);
            var chapters = await service.FindChapters(url, new Progress<int>(), source.Token);
            Assert.IsTrue(chapters.Count > 0, "Cannot find chapters.");
            var chapter = chapters.Last();
            Assert.AreEqual("Beelzebub 1", chapter.Name);
            Assert.AreEqual("http://read.mangashare.com/Beelzebub/chapter-001/page001.html", chapter.Link);
            var images = await service.FindImanges(chapter, new Progress<int>(), source.Token);
            Assert.AreEqual(58, images.Count);
            Assert.AreEqual("http://dl01.mangashare.com/manga/Beelzebub/001/001.jpg", images[0]);
            Assert.AreEqual("http://dl01.mangashare.com/manga/Beelzebub/001/002.jpg", images[1]);
            Assert.AreEqual("http://dl01.mangashare.com/manga/Beelzebub/001/058.jpg", images[57]);
        }
    }
}
