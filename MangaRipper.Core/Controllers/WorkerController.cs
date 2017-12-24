using MangaRipper.Core.DataTypes;
using MangaRipper.Core.Helpers;
using MangaRipper.Core.Models;
using MangaRipper.Core.Providers;
using MangaRipper.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MangaRipper.Core.Extensions;
using MangaRipper.Core.Interfaces;

namespace MangaRipper.Core.Controllers
{
    /// <summary>
    /// Worker support download manga chapters on back ground thread.
    /// </summary>
    public class WorkerController
    {

        public ServiceManager ServiceManager { get; }

        CancellationTokenSource _source;
        readonly SemaphoreSlim _sema;
        private readonly ILogger logger;
        private readonly PackageCbzHelper cbz;
        private readonly Downloader downloader;

        private enum ImageExtensions { Jpeg, Jpg, Png, Gif };

        public WorkerController(ServiceManager sm, ILogger logger, PackageCbzHelper cbz, Downloader downloader)
        {
            this.logger = logger;
            logger.Info("> Worker()");
            ServiceManager = sm;
            this.cbz = cbz;
            this.downloader = downloader;
            _source = new CancellationTokenSource();
            _sema = new SemaphoreSlim(2);
        }

        /// <summary>
        /// Stop download
        /// </summary>
        public void Cancel()
        {
            _source.Cancel();
        }

        /// <summary>
        /// Run task download a chapter
        /// </summary>
        /// <param name="task">Contain chapter and save to path</param>
        /// <param name="progress">Callback to report progress</param>
        /// <returns></returns>
        public async Task RunDownloadTaskAsync(DownloadChapterTask task, IProgress<int> progress)
        {
            logger.Info($"> DownloadChapter: {task.Chapter.Url} To: {task.SaveToFolder}");
            await Task.Run(async () =>
            {
                try
                {
                    _source = new CancellationTokenSource();
                    await _sema.WaitAsync();
                    task.IsBusy = true;
                    await DownloadChapter(task, progress);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"Failed to download chapter: {task.Chapter.Url}");
                    throw;
                }
                finally
                {
                    task.IsBusy = false;
                    _sema.Release();
                }
            });
        }

        /// <summary>
        /// Find all chapters of a manga
        /// </summary>
        /// <param name="mangaPath">The URL of manga</param>
        /// <param name="progress">Progress report callback</param>
        /// <returns></returns>
        public async Task<IEnumerable<Chapter>> FindChapterListAsync(string mangaPath, IProgress<int> progress)
        {
            logger.Info($"> FindChapters: {mangaPath}");
            return await Task.Run(async () =>
            {
                try
                {
                    await _sema.WaitAsync();
                    return await FindChaptersInternal(mangaPath, progress);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"Failed to find chapters: {mangaPath}");
                    throw;
                }
                finally
                {
                    _sema.Release();
                }
            });
        }

        private async Task DownloadChapter(DownloadChapterTask task, IProgress<int> progress)
        {
            var chapter = task.Chapter;
            progress.Report(0);
            var service = ServiceManager.GetService(chapter.Url);
            var images = await service.FindImages(chapter, new Progress<int>(count =>
            {
                progress.Report(count / 2);
            }), _source.Token);

            var tempFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempFolder);

            await DownloadImages(images, tempFolder, progress);

            if (task.Formats.Contains(OutputFormat.Folder))
            {
                if (!Directory.Exists(task.SaveToFolder))
                {
                    Directory.CreateDirectory(task.SaveToFolder);
                }
                ExtensionHelper.SuperMove(tempFolder, task.SaveToFolder);
            }
            if (task.Formats.Contains(OutputFormat.CBZ))
            {
                if (!Directory.Exists(task.SaveToFolder))
                {
                    Directory.CreateDirectory(task.SaveToFolder);
                }

                cbz.Create(tempFolder, task.SaveToFolder + ".cbz");
            }

            progress.Report(100);
        }

        private async Task DownloadImages(IEnumerable<string> inputImages, string destination, IProgress<int> progress)
        {
            var images = inputImages.ToArray();
            logger.Info($@"Download {images.Length} images into {destination}");

            var countImage = 0;
            foreach (var image in images)
            {
                _source.Token.ThrowIfCancellationRequested();
                await downloader.DownloadToFolder(image, destination, _source.Token);
                countImage++;
                int i = Convert.ToInt32((float)countImage / images.Count() * 100 / 2);
                progress.Report(50 + i);
            }
        }

        private async Task<IEnumerable<Chapter>> FindChaptersInternal(string mangaPath, IProgress<int> progress)
        {
            progress.Report(0);
            // let service find all chapters in manga
            var service = ServiceManager.GetService(mangaPath);
            var chapters = await service.FindChapters(mangaPath, progress, _source.Token);
            progress.Report(100);
            return chapters;
        }
    }
}
