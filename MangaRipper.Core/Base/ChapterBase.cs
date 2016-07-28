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

        abstract protected List<Uri> ParsePageAddresses(string html);

        abstract protected List<Uri> ParseImageAddresses(string html);

        public string Name
        {
            get;
            set;
        }

        public Uri Address
        {
            get;
            protected set;
        }

        public List<Uri> ImageAddresses
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

        public ChapterBase(string name, Uri address)
        {
            Name = name;
            Address = address;

        }

        public Task DownloadImageAsync(string saveToDirectory, CancellationToken cancellationToken, IProgress<ChapterProgress> progress)
        {
            _cancellationToken = cancellationToken;
            _progress = progress;
            SaveTo = saveToDirectory;

            _task = Task.Factory.StartNew(() =>
            {
                _progress.Report(new ChapterProgress(this, 0));
                string html = DownloadString(Address);
                if (ImageAddresses == null)
                {
                    PopulateImageAddress(html);
                }

                string saveToFolder = SaveTo + "\\" + Name.RemoveFileNameInvalidChar();
                Directory.CreateDirectory(saveToFolder);

                int countImage = 0;

                foreach (Uri imageAddress in ImageAddresses)
                {
                    _cancellationToken.ThrowIfCancellationRequested();
                    string filename = saveToFolder + "\\" + Path.GetFileName(imageAddress.LocalPath);
                    DownloadFile(imageAddress, filename);

                    countImage++;
                    int percent = (countImage * 100 / ImageAddresses.Count / 2) + 50;
                    _progress.Report(new ChapterProgress(this, percent));
                }
            }, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);

            return _task;
        }


        public void PopulateImageAddress(string html)
        {
            List<Uri> pageAddresses = ParsePageAddresses(html);

            var sbHtml = new StringBuilder();

            int countPage = 0;

            foreach (Uri pageAddress in pageAddresses)
            {
                _cancellationToken.ThrowIfCancellationRequested();
                string content = DownloadString(pageAddress);
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

        private void DownloadFile(Uri address, string fileName)
        {
            try
            {
                if (File.Exists(fileName) == false)
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);
                    request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                    request.Proxy = Proxy;
                    request.Credentials = CredentialCache.DefaultCredentials;
                    request.Referer = Address.AbsoluteUri;
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            string tmpFileName = Path.GetTempFileName();

                            using (Stream strLocal = new FileStream(tmpFileName, FileMode.Create))
                            {
                                byte[] downBuffer = new byte[2048];
                                int bytesSize = 0;
                                while ((bytesSize = responseStream.Read(downBuffer, 0, downBuffer.Length)) > 0)
                                {
                                    _cancellationToken.ThrowIfCancellationRequested();
                                    strLocal.Write(downBuffer, 0, bytesSize);
                                }

                            }

                            File.Move(tmpFileName, fileName);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                string error = string.Format("{0} - Error while download: {2} - Reason: {3}", DateTime.Now.ToLongTimeString(), Name, address.AbsoluteUri, ex.Message);
                throw new OperationCanceledException(error, ex);
            }
        }

        private string DownloadString(Uri address)
        {
            StringBuilder result = new StringBuilder();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                request.Proxy = Proxy;
                request.Credentials = CredentialCache.DefaultCredentials;
                request.UserAgent = "Safari";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        byte[] downBuffer = new byte[2048];
                        int bytesSize = 0;
                        while ((bytesSize = responseStream.Read(downBuffer, 0, downBuffer.Length)) > 0)
                        {
                            _cancellationToken.ThrowIfCancellationRequested();
                            result.Append(Encoding.UTF8.GetString(downBuffer, 0, bytesSize));
                        }
                    }
                }
                return result.ToString();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                string error = string.Format("{0} - Error while download: {2} - Reason: {3}", DateTime.Now.ToLongTimeString(), Name, address.AbsoluteUri, ex.Message);
                throw new Exception(error, ex);
            }
        }
    }
}
