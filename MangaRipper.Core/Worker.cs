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
    public class Worker
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        CancellationTokenSource source;
        SemaphoreSlim sema;

        public Worker()
        {
            logger.Info("> Worker()");
            source = new CancellationTokenSource();
            sema = new SemaphoreSlim(1);
        }

        public void Cancel()
        {
            source.Cancel();
        }

        public async Task DownloadChapter(Chapter chapter, string mangaLocalPath, IProgress<int> progress)
        {
            logger.Info("> DownloadChapter: {0} To: {1}", chapter.Link, mangaLocalPath);
            await Task.Run(async () =>
            {
                try
                {
                    source = new CancellationTokenSource();
                    await sema.WaitAsync();
                    chapter.IsBusy = true;
                    await DownloadChapterInternal(chapter, mangaLocalPath, progress);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Failed to download chapter: {0}", chapter.Link);
                    throw;
                }
                finally
                {
                    chapter.IsBusy = false;
                    sema.Release();
                }
            });
        }

        public async Task<IList<Chapter>> FindChapters(string mangaPath, IProgress<int> progress)
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
            var folderName = chapter.Name.RemoveFileNameInvalidChar();
            var destinationPath = Path.Combine(mangaLocalPath, folderName); ;
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
                int i = Convert.ToInt32((float)countImage / images.Count * 100 / 2);
                progress.Report(50 + i);
            }
            progress.Report(100);
        }


        private async Task<IList<Chapter>> FindChaptersInternal(string mangaPath, IProgress<int> progress)
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
