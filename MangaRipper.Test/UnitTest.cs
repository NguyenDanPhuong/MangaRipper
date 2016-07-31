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
        CancellationTokenSource source;
        [TestInitialize]
        public void Initialize()
        {
            Framework.Init();
            source = new CancellationTokenSource();
        }

        [TestMethod]
        public async Task TestMangaReader_Test()
        {
            string url = "http://www.mangareader.net/naruto";
            var service = Framework.GetService(url);
            var chapters = await service.FindChapters(url, new Progress<int>(), source.Token);
            Assert.IsTrue(chapters.Count > 0, "Cannot find chapters.");
            var chapter = chapters[0];
            var images = await service.FindImanges(chapter, new Progress<ChapterProgress>(), source.Token);
            Assert.IsTrue(images.Count > 0, "Cannot find images.");
        }

        [TestMethod]
        public async Task MangaFox_Test()
        {
            string url = "http://mangafox.me/manga/poputepipikku";
            var service = Framework.GetService(url);
            var chapters = await service.FindChapters(url, new Progress<int>(), source.Token);
            Assert.IsTrue(chapters.Count > 0, "Cannot find chapters.");
            var chapter = chapters[0];
            var images = await service.FindImanges(chapter, new Progress<ChapterProgress>(), source.Token);
            Assert.IsTrue(images.Count > 0, "Cannot find images.");
        }

        [TestMethod]
        public async Task TestMangaHere_Test()
        {
            string url = "http://www.mangahere.co/manga/the_god_of_high_school/";
            var service = Framework.GetService(url);
            var chapters = await service.FindChapters(url, new Progress<int>(), source.Token);
            Assert.IsTrue(chapters.Count > 0, "Cannot find chapters.");
            var chapter = chapters[0];
            var images = await service.FindImanges(chapter, new Progress<ChapterProgress>(), source.Token);
            Assert.IsTrue(images.Count > 0, "Cannot find images.");
        }

        [TestMethod]
        public async Task TestMangaShare_Test()
        {
            string url = "http://read.mangashare.com/Beelzebub";
            var service = Framework.GetService(url);
            var chapters = await service.FindChapters(url, new Progress<int>(), source.Token);
            Assert.IsTrue(chapters.Count > 0, "Cannot find chapters.");
            var chapter = chapters[0];
            var images = await service.FindImanges(chapter, new Progress<ChapterProgress>(), source.Token);
            Assert.IsTrue(images.Count > 0, "Cannot find images.");
        }
    }
}
