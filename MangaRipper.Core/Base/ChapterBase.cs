using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    [Serializable]
    public abstract class ChapterBase : IChapter
    {
        [NonSerialized]
        private CancellationToken _cancellationToken;

        [NonSerialized]
        private Task _task;

        [NonSerialized]
        private IProgress<ChapterProgress> _progress;

        abstract protected List<string> ParsePageAddresses(string html);

        abstract protected List<string> ParseImageAddresses(string html);

        public string Name
        {
            get;
            set;
        }

        public string Address
        {
            get;
            protected set;
        }

        public List<string> ImageAddresses
        {
            get;
            private set;
        }

        public string SaveTo
        {
            get;
            protected set;
        }

        public bool IsBusy
        {
            get
            {
                bool result = false;
                if (_task != null)
                {
                    switch (_task.Status)
                    {
                        case TaskStatus.Created:
                        case TaskStatus.Running:
                        case TaskStatus.WaitingForActivation:
                        case TaskStatus.WaitingForChildrenToComplete:
                        case TaskStatus.WaitingToRun:
                            result = true;
                            break;
                    }
                }
                return result;
            }
        }

        public IWebProxy Proxy { get; set; }

        public ChapterBase(string name, string address)
        {
            Name = name;
            Address = address;

        }

        public async Task DownloadImageAsync(string saveToDirectory, CancellationToken cancellationToken, IProgress<ChapterProgress> progress)
        {
            _cancellationToken = cancellationToken;
            _progress = progress;
            SaveTo = saveToDirectory;

            _task = Task.Run(async () =>
            {
                _progress.Report(new ChapterProgress(this, 0));
                string html = await Downloader.DownloadStringAsync(Address);
                if (ImageAddresses == null)
                {
                    await PopulateImageAddress(html);
                }

                string saveToFolder = SaveTo + "\\" + Name.RemoveFileNameInvalidChar();
                Directory.CreateDirectory(saveToFolder);

                int countImage = 0;

                foreach (string imageAddress in ImageAddresses)
                {
                    _cancellationToken.ThrowIfCancellationRequested();
                    string filename = saveToFolder + "\\" + Path.GetFileName(new Uri(imageAddress).LocalPath);
                    await DownloadImagePage(imageAddress, filename);

                    countImage++;
                    int percent = (countImage * 100 / ImageAddresses.Count / 2) + 50;
                    _progress.Report(new ChapterProgress(this, percent));
                }
            });

            await _task;
        }


        public async Task PopulateImageAddress(string html)
        {
            var pageAddresses = ParsePageAddresses(html);

            var sbHtml = new StringBuilder();

            int countPage = 0;

            foreach (string pageAddress in pageAddresses)
            {
                _cancellationToken.ThrowIfCancellationRequested();
                string content = await Downloader.DownloadStringAsync(pageAddress);
                sbHtml.AppendLine(content);

                countPage++;
                int percent = countPage * 100 / (pageAddresses.Count * 2);
                if (_progress != null)
                {
                    _progress.Report(new ChapterProgress(this, percent));
                }
            }

            ImageAddresses = ParseImageAddresses(sbHtml.ToString());
        }

        private async Task DownloadImagePage(string address, string fileName)
        {
            try
            {
                if (File.Exists(fileName) == false)
                {
                    string tmpFileName = Path.GetTempFileName();
                    await Downloader.DownloadFileAsync(address, tmpFileName, _cancellationToken);
                    File.Move(tmpFileName, fileName);
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                string error = string.Format("{0} - Error while download: {2} - Reason: {3}", DateTime.Now.ToLongTimeString(), Name, address, ex.Message);
                throw new OperationCanceledException(error, ex);
            }
        }
    }
}
