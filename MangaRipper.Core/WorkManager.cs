using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    public class WorkManager
    {
        CancellationTokenSource source;
        SemaphoreSlim sema;

        public WorkManager()
        {
            source = new CancellationTokenSource();
            sema = new SemaphoreSlim(3);
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
            // let service find all images of chapter
            var service = Framework.GetService(chapter.Link);
            var images = await service.FindImanges(chapter, progress, source.Token);
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
        }


        private async Task<IList<Chapter>> FindChaptersInternal(string mangaPath, IProgress<int> progress)
        {
            // let service find all chapters in manga
            var service = Framework.GetService(mangaPath);
            return await service.FindChapters(mangaPath, progress, source.Token);
        }
    }
}
