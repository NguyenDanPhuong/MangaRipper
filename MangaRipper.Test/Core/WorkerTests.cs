using MangaRipper.Core;
using MangaRipper.Core.Logging;
using MangaRipper.Core.Models;
using MangaRipper.Core.Outputers;
using MangaRipper.Core.Plugins;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MangaRipper.Test.Core
{
    public class WorkerTests
    {
        private readonly Mock<IPluginManager> pluginManager;
        private readonly Mock<IOutputFactory> outputFactory;
        private readonly Mock<IHttpDownloader> httpDownloader;
        private readonly Mock<ILogger> logger;
        private readonly Mock<IPlugin> plugin;
        private readonly Mock<IOutputer> outputFolder;
        private readonly Mock<IOutputer> outputCbz;
        private WorkerController worker;

        public WorkerTests()
        {
            pluginManager = new Mock<IPluginManager>();
            outputFactory = new Mock<IOutputFactory>();
            httpDownloader = new Mock<IHttpDownloader>();
            logger = new Mock<ILogger>();

            plugin = new Mock<IPlugin>();
            pluginManager.Setup(pm => pm.GetPlugin(It.IsAny<string>())).Returns(plugin.Object);

            plugin.Setup(p => p.GetChapters(It.IsAny<string>(), It.IsAny<IProgress<int>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new Chapter[]
                {
                    new Chapter("NAME1", "URL1"),
                    new Chapter("NAME2", "URL2"),
                    new Chapter("NAME3", "URL3"),
                    new Chapter("NAME4", "URL4"),
                    new Chapter("NAME5", "URL5"),
                }.AsEnumerable()));

            outputFolder = new Mock<IOutputer>();
            outputCbz = new Mock<IOutputer>();
            outputFactory.Setup(of => of.Create(OutputFormat.Folder)).Returns(outputFolder.Object);
            outputFactory.Setup(of => of.Create(OutputFormat.CBZ)).Returns(outputCbz.Object);

            worker = new WorkerController(pluginManager.Object, outputFactory.Object, httpDownloader.Object, logger.Object);

        }
        [Fact]
        public async void GetChapterListAsync()
        {
            var mangaUrl = "MANGA1";

            var cs = await worker.GetChapterListAsync(mangaUrl, new Progress<int>(), new CancellationTokenSource().Token);
            var chapters = cs.ToList();
            pluginManager.Verify(pm => pm.GetPlugin(mangaUrl));

            plugin.Verify(p => p.GetChapters(mangaUrl, It.IsAny<IProgress<int>>(), It.IsAny<CancellationToken>()));

            Assert.Equal(5, chapters.Count());

            for (int i = 1; i <= 5; i++)
            {
                Assert.Equal("NAME" + i, chapters[i - 1].Name);
                Assert.Equal("URL" + i, chapters[i - 1].Url);
            }
        }



        [Fact]
        public async void DownloadChapterAsync()
        {
            plugin.Setup(p => p.GetImages("URL1", It.IsAny<Progress<int>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new string[] 
                {
                    "IMG1",
                    "IMG2",
                    "IMG3",
                    "IMG4",
                    "IMG5",
                }.AsEnumerable()));

            var task = new DownloadChapterTask("CHAP1", "URL1", "C:\\TEST", new OutputFormat[] { OutputFormat.Folder, OutputFormat.CBZ });
            var result = await worker.GetChapterAsync(task, new Progress<int>(), new CancellationToken());

            Assert.False(result.Error);

            pluginManager.Verify(pm => pm.GetPlugin("URL1"));

            plugin.Verify(p => p.GetImages("URL1", It.IsAny<Progress<int>>(), It.IsAny<CancellationToken>()));

            for (int i = 0; i < 5; i++)
            {
                httpDownloader.Verify(hd => hd.GetFileAsync("IMG" + (i + 1), It.IsAny<string>(), It.IsAny<CancellationToken>()));
            }

            outputFactory.Verify(of => of.Create(OutputFormat.Folder), Times.Once);
            outputFactory.Verify(of => of.Create(OutputFormat.CBZ), Times.Once);

            outputFolder.Verify(o => o.Save(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            outputCbz.Verify(o => o.Save(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
