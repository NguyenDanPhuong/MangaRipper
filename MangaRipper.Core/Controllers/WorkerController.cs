using MangaRipper.Core.DataTypes;
using MangaRipper.Core.Helpers;
using MangaRipper.Core.Models;
using MangaRipper.Core.Providers;
using MangaRipper.Core.Services;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MangaRipper.Core.Extensions;

namespace MangaRipper.Core.Controllers
{
    /// <summary>
    /// Worker support download manga chapters on back ground thread.
    /// </summary>
    public class WorkerController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        CancellationTokenSource _source;
        readonly SemaphoreSlim _sema;

        private enum ImageExtensions { Jpeg, Jpg, Png, Gif };

        public WorkerController()
        {
            Logger.Info("> Worker()");
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
            Logger.Info("> DownloadChapter: {0} To: {1}", task.Chapter.Url, task.SaveToFolder);
            await Task.Run(async () =>
            {
                try
                {
                    _source = new CancellationTokenSource();
                    await _sema.WaitAsync();
                    task.IsBusy = true;
                    await DownloadChapter(task, task.SaveToFolder, progress);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Failed to download chapter: {0}", task.Chapter.Url);
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
            Logger.Info("> FindChapters: {0}", mangaPath);
            return await Task.Run(async () =>
            {
                try
                {
                    await _sema.WaitAsync();
                    return await FindChaptersInternal(mangaPath, progress);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Failed to find chapters: {0}", mangaPath);
                    throw;
                }
                finally
                {
                    _sema.Release();
                }
            });
        }

        private async Task DownloadChapter(DownloadChapterTask task, string mangaLocalPath, IProgress<int> progress)
        {
            var chapter = task.Chapter;
            progress.Report(0);
            var service = FrameworkProvider.GetService(chapter.Url);
            var images = await service.FindImages(chapter, new Progress<int>(count =>
            {
                progress.Report(count / 2);
            }), _source.Token);

            var tempFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempFolder);

            await DownloadImages(images, tempFolder, progress);

            var folderName = chapter.Name;
            var finalFolder = Path.Combine(mangaLocalPath, folderName);

            if (task.Formats.Contains(OutputFormat.Folder))
            {
                if (!Directory.Exists(finalFolder))
                {
                    Directory.CreateDirectory(finalFolder);
                }
                ExtensionHelper.SuperMove(tempFolder, finalFolder);
            }
            if (task.Formats.Contains(OutputFormat.CBZ))
            {
                PackageCbzHelper.Create(tempFolder, Path.Combine(task.SaveToFolder, task.Chapter.Name + ".cbz"));
            }

            progress.Report(100);
        }

        private async Task DownloadImages(IEnumerable<string> inputImages, string destination, IProgress<int> progress)
        {
            var images = inputImages.ToArray();
            Logger.Info($@"Download {images.Length} images into {destination}");

            var countImage = 0;
            foreach (var image in images)
            {
                _source.Token.ThrowIfCancellationRequested();
                await DownloadImage(image, destination, countImage);
                countImage++;
                int i = Convert.ToInt32((float)countImage / images.Count() * 100 / 2);
                progress.Report(50 + i);
            }
        }

        private async Task DownloadImage(string image, string destination, int imageNum)
        {
            var downloader = new DownloadService();
            string tempFilePath = Path.GetTempFileName();
            string filePath = Path.Combine(destination, GetFilenameFromUrl(image, imageNum));
            if (!File.Exists(filePath))
            {
                await downloader.DownloadFileAsync(image, tempFilePath, _source.Token);
                File.Move(tempFilePath, filePath);
            }
        }

        /// <summary>
        /// Use the name from URL, or use the numbers if name is unappropriated
        /// </summary>
        /// <param name="url">Image URL</param>
        /// <param name="imageNum">image's order</param>
        /// <returns></returns>
        private string GetFilenameFromUrl(string url, int imageNum)
        {
            var uri = new Uri(url);
            var path = Path.GetFileName(uri.LocalPath);
            var nameInParam = false;

            // if everything in parameters and path is incorrect
            // e.g. https://images1-focus-opensocial.googleusercontent.com/gadgets/proxy?container=focus&gadget=a&no_expand=1&resize_h=0&rewriteMime=image%2F*&url=http%3a%2f%2f2.p.mpcdn.net%2f50%2f531513%2f1.jpg&imgmax=30000
            string extension = path.Split('.').FirstOrDefault(x => Enum.GetNames(typeof(ImageExtensions)).Contains(x, StringComparer.OrdinalIgnoreCase));
            if (extension == null)
            {
                nameInParam = true;
                extension = uri.PathAndQuery.Split('.', '&').FirstOrDefault(x => Enum.GetNames(typeof(ImageExtensions)).Contains(x, StringComparer.OrdinalIgnoreCase));
            }

            // Some names - just a gibberish text which is TOO LONG
            // e.g. MG09qjYxsb3sFsrMt_lTn7f9ulfgcbusQjS5wypyy0aGn0sjL7hZHQhXuS-dXZNn0tuWvdBgKICQ8WI9RFGAgNNpdYglvFdwhJZC7qiClhvEd9toNLpLky19HRRZmSFbv3zq5lw=s0?title=000_1485859774.png
            if (uri.LocalPath.Length > 50 || nameInParam)
            {
                imageNum++;
                path = imageNum.ToString("0000") + "." + extension;
            }
            return path;
        }


        private async Task<IEnumerable<Chapter>> FindChaptersInternal(string mangaPath, IProgress<int> progress)
        {
            progress.Report(0);
            // let service find all chapters in manga
            var service = FrameworkProvider.GetService(mangaPath);
            var chapters = await service.FindChapters(mangaPath, progress, _source.Token);
            progress.Report(100);
            return chapters;
        }
    }
}
