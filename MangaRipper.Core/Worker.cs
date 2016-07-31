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
        CancellationTokenSource source;
        SemaphoreSlim sema;

        public Worker()
        {
            source = new CancellationTokenSource();
            sema = new SemaphoreSlim(1);
        }

        public void Cancel()
        {
            source.Cancel();
        }

        public async Task DownloadChapter(Chapter chapter, string mangaLocalPath, IProgress<ChapterProgress> progress)
        {
            await Task.Run(async () =>
            {
                try
                {
                    await sema.WaitAsync();
                    chapter.IsBusy = true;
                    await DownloadChapterInternal(chapter, mangaLocalPath, progress);
                }
                 catch (Exception)
                {
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
            return await Task.Run(async () =>
            {
                try
                {
                    await sema.WaitAsync();
                    return await FindChaptersInternal(mangaPath, progress);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sema.Release();
                }
            });
        }

        private async Task DownloadChapterInternal(Chapter chapter, string mangaLocalPath, IProgress<ChapterProgress> progress)
        {
            progress.Report(new ChapterProgress(chapter, 0));
            // let service find all images of chapter
            var service = Framework.GetService(chapter.Link);
            var images = await service.FindImanges(chapter, new Progress<ChapterProgress>(), source.Token);
            // create folder to keep images
            var downloader = new Downloader();
            var folderName = chapter.Name.RemoveFileNameInvalidChar();
            var destinationPath = Path.Combine(mangaLocalPath, folderName); ;
            Directory.CreateDirectory(destinationPath);
            // download images
            foreach (var image in images)
            {
                string tempFilePath = Path.GetTempFileName();
                await downloader.DownloadFileAsync(image, tempFilePath, source.Token);
                string filePath = Path.Combine(destinationPath, Path.GetFileName(image));
                File.Move(tempFilePath, filePath);
            }
            progress.Report(new ChapterProgress(chapter, 100));
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
