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

        public async Task DownloadChapter(Chapter chapter, string mangaLocalPath)
        {
            await Task.Run(async () =>
            {
                try
                {
                    await sema.WaitAsync();
                    await DownloadChapterInternal(chapter, mangaLocalPath);
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

        public async Task<IList<Chapter>> FindChapters(string mangaPath)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    await sema.WaitAsync();
                    return await FindChaptersInternal(mangaPath);
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

        private async Task DownloadChapterInternal(Chapter chapter, string mangaLocalPath)
        {
            // let service find all images of chapter
            var service = Framework.GetService(chapter.Link);
            var images = await service.FindImanges(chapter, source.Token);
            // create folder to keep images
            var downloader = new Downloader();
            var folderName = chapter.Name.RemoveFileNameInvalidChar();
            var destinationPath = new Uri(new Uri(mangaLocalPath), folderName).AbsolutePath;
            Directory.CreateDirectory(destinationPath);
            // download images
            foreach (var image in images)
            {
                string tempFilePath = Path.GetTempFileName();
                await downloader.DownloadFileAsync(image, tempFilePath, source.Token);
                File.Move(tempFilePath, destinationPath);
            }
        }


        private async Task<IList<Chapter>> FindChaptersInternal(string mangaPath)
        {
            // let service find all chapters in manga
            var service = Framework.GetService(mangaPath);
            return await service.FindChapters(mangaPath, source.Token);
        }
    }
}
