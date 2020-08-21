using MangaRipper.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MangaRipper.Core.Outputers;
using MangaRipper.Core.Plugins;
using MangaRipper.Core.Logging;
using System.Linq;

namespace MangaRipper.Core
{
    /// <summary>
    /// Worker support download manga chapters on back ground thread.
    /// </summary>
    public class WorkerController : IWorkerController
    {

        private readonly IPluginManager pluginManager;

        private readonly ILogger<WorkerController> logger;
        private readonly IOutputFactory outputFactory;
        private readonly IHttpDownloader downloader;

        public WorkerController(IPluginManager pluginManager, IOutputFactory outputFactory, IHttpDownloader downloader, ILogger<WorkerController> logger)
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
        public async Task<DownloadChapterResponse> GetChapterAsync(DownloadChapterRequest task, IProgress<string> progress, CancellationToken cancellationToken)
        {
            logger.Info($"> DownloadChapter: {task.Url} To: {task.SaveToFolder}");
            return await Task.Run(async () =>
            {
                var taskResult = new DownloadChapterResponse();
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
        public async Task<IEnumerable<Chapter>> GetChapterListAsync(string mangaPath, IProgress<string> progress, CancellationToken cancellationTokenSource)
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

        private async Task DownloadChapterImpl(DownloadChapterRequest task, IProgress<string> progress, CancellationToken cancellationToken)
        {
            progress.Report("Starting...");
            var plugin = pluginManager.GetPlugin(task.Url);
            var images = await plugin.GetImages(task.Url, new Progress<string>(count =>
            {
                progress.Report(count.ToString());
            }), cancellationToken);

            var tempFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            var index = 1;
            foreach (var image in images)
            {
                progress.Report($"Download: {index}/{images.Count()}");
                await downloader.GetFileAsync(image, tempFolder, cancellationToken);
                index++;
            }

            foreach (var format in task.Formats)
            {
                var factory = outputFactory.Create(format);
                factory.Save(tempFolder, task.SaveToFolder);
            }

            progress.Report("Done");
        }

        private async Task<IEnumerable<Chapter>> GetChapterListImpl(string mangaPath, IProgress<string> progress, CancellationToken cancellationToken)
        {
            progress.Report("....");
            var service = pluginManager.GetPlugin(mangaPath);
            var chapters = await service.GetChapters(mangaPath, progress, cancellationToken);
            progress.Report("Done");
            return chapters;
        }
    }
}
