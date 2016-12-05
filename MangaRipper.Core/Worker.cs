using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    /// <summary>
    /// Worker support download manga chapters on back ground thread.
    /// </summary>
    public class Worker
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        CancellationTokenSource source;
        SemaphoreSlim sema;

        public Worker()
        {
            logger.Info("> Worker()");
            source = new CancellationTokenSource();
            sema = new SemaphoreSlim(2);
        }

        /// <summary>
        /// Stop download
        /// </summary>
        public void Cancel()
        {
            source.Cancel();
        }

        /// <summary>
        /// Run task download a chapter
        /// </summary>
        /// <param name="task">Contain chapter and save to path</param>
        /// <param name="progress">Callback to report progress</param>
        /// <returns></returns>
        public async Task Run(DownloadChapterTask task, IProgress<int> progress)
        {
            logger.Info("> DownloadChapter: {0} To: {1}", task.Chapter.Link, task.SaveToFolder);
            await Task.Run(async () =>
            {
                try
                {
                    source = new CancellationTokenSource();
                    await sema.WaitAsync();
                    task.IsBusy = true;
                    await DownloadChapterInternal(task.Chapter, task.SaveToFolder, progress);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Failed to download chapter: {0}", task.Chapter.Link);
                    throw;
                }
                finally
                {
                    task.IsBusy = false;
                    if (task.Formats.Contains(OutputFormat.CBZ))
                    {
                        PackageCbz.Create(Path.Combine(task.SaveToFolder, task.Chapter.NomalizeName), Path.Combine(task.SaveToFolder, task.Chapter.NomalizeName + ".cbz"));
                    }
                    sema.Release();
                }
            });
        }



        /// <summary>
        /// Find all chapters of a manga
        /// </summary>
        /// <param name="mangaPath">The url of manga</param>
        /// <param name="progress">Progress report callback</param>
        /// <returns></returns>
        public async Task<IEnumerable<Chapter>> FindChapters(string mangaPath, IProgress<int> progress)
        {
            logger.Info("> FindChapters: {0}", mangaPath);
            return await Task.Run(async () =>
            {
                try
                {
                    await sema.WaitAsync();
                    return await FindChaptersInternal(mangaPath, progress);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Failed to find chapters: {0}", mangaPath);
                    throw;
                }
                finally
                {
                    sema.Release();
                }
            });
        }

        private async Task DownloadChapterInternal(Chapter chapter, string mangaLocalPath, IProgress<int> progress)
        {
            progress.Report(0);
            // let service find all images of chapter
            var service = Framework.GetService(chapter.Link);
            var images = await service.FindImanges(chapter, new Progress<int>((count) =>
            {
                progress.Report(count / 2);
            }), source.Token);
            // create folder to keep images
            var downloader = new Downloader();
            var folderName = chapter.NomalizeName;
            var destinationPath = Path.Combine(mangaLocalPath, folderName);
            Directory.CreateDirectory(destinationPath);
            // download images
            int countImage = 0;
            foreach (var image in images)
            {
                string tempFilePath = Path.GetTempFileName();
                string filePath = Path.Combine(destinationPath, Path.GetFileName(image));
                if (!File.Exists(filePath))
                {
                    await downloader.DownloadFileAsync(image, tempFilePath, source.Token);
                    File.Move(tempFilePath, filePath);
                }
                countImage++;
                int i = Convert.ToInt32((float)countImage / images.Count() * 100 / 2);
                progress.Report(50 + i);
            }
            progress.Report(100);
        }


        private async Task<IEnumerable<Chapter>> FindChaptersInternal(string mangaPath, IProgress<int> progress)
        {
            progress.Report(0);
            // let service find all chapters in manga
            var service = Framework.GetService(mangaPath);
            var chapters = await service.FindChapters(mangaPath, progress, source.Token);
            progress.Report(100);
            return chapters;
        }
    }
}
