using MangaRipper.Core.Models;
using MangaRipper.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MangaRipper.Core.Outputers;
using MangaRipper.Core.Plugins;

namespace MangaRipper.Core
{
    /// <summary>
    /// Worker support download manga chapters on back ground thread.
    /// </summary>
    public class WorkerController : IWorkerController
    {

        private IPluginManager pluginManager { get; }

        private readonly ILogger logger;
        private readonly IOutputFactory outputFactory;
        private readonly IHttpDownloader downloader;

        public WorkerController(IPluginManager pluginManager, IOutputFactory outputFactory, IHttpDownloader downloader, ILogger logger)
        {
            this.logger = logger;
            logger.Info("> Worker()");
            this.pluginManager = pluginManager;
            this.outputFactory = outputFactory;
            this.downloader = downloader;
        }

        /// <summary>
        /// Run task download a chapter
        /// </summary>
        /// <param name="task">Contain chapter and save to path</param>
        /// <param name="progress">Callback to report progress</param>
        /// <returns></returns>
        public async Task<DownloadTaskResult> DownloadChapterAsync(DownloadChapterTask task, IProgress<int> progress, CancellationToken cancellationToken)
        {
            logger.Info($"> DownloadChapter: {task.Url} To: {task.SaveToFolder}");
            return await Task.Run(async () =>
            {
                var taskResult = new DownloadTaskResult();
                try
                {
                    await DownloadChapterImpl(task, progress, cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"Failed to download chapter: {task.Url}");
                    taskResult.Error = true;
                    taskResult.Exception = ex;
                }
                return taskResult;
            });
        }

        /// <summary>
        /// Find all chapters of a manga
        /// </summary>
        /// <param name="mangaPath">The URL of manga</param>
        /// <param name="progress">Progress report callback</param>
        /// <returns></returns>
        public async Task<IEnumerable<Chapter>> GetChapterListAsync(string mangaPath, IProgress<int> progress, CancellationToken cancellationTokenSource)
        {
            logger.Info($"> FindChapters: {mangaPath}");
            return await Task.Run(async () =>
            {
                try
                {
                    return await GetChapterListImpl(mangaPath, progress, cancellationTokenSource);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"Failed to find chapters: {mangaPath}");
                    throw;
                }
            });
        }

        private async Task DownloadChapterImpl(DownloadChapterTask task, IProgress<int> progress, CancellationToken cancellationToken)
        {
            progress.Report(0);
            var plugin = pluginManager.GetService(task.Url);
            var images = await plugin.GetImages(task.Url, new Progress<int>(count =>
            {
                progress.Report(count / 2);
            }), cancellationToken);

            var tempFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            foreach (var image in images)
            {
                await downloader.DownloadToFolder(image, tempFolder, cancellationToken);
            }

            foreach (var format in task.Formats)
            {
                var factory = outputFactory.Create(format);
                factory.Save(tempFolder, task.SaveToFolder);
            }

            progress.Report(100);
        }

        private async Task<IEnumerable<Chapter>> GetChapterListImpl(string mangaPath, IProgress<int> progress, CancellationToken cancellationToken)
        {
            progress.Report(0);
            var service = pluginManager.GetService(mangaPath);
            var chapters = await service.GetChapters(mangaPath, progress, cancellationToken);
            progress.Report(100);
            return chapters;
        }
    }
}
