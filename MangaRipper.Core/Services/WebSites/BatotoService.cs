using MangaRipper.Core.Helpers;
using MangaRipper.Core.Interfaces;
using MangaRipper.Core.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Core.Services.WebSites
{
    /// <summary>
    /// Support find chapters, images from Batoto
    /// </summary>
    class BatotoService : IMangaService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private const string User = "k61150";
        private const string Password = "123";

        public SiteInformation GetInformation()
        {
            return new SiteInformation("Batoto", "http://bato.to", "Multiple Languages");
        }

        public bool Of(string link)
        {
            var uri = new Uri(link);
            return uri.Host.Equals("bato.to");
        }

        public async Task<IEnumerable<Chapter>> FindChapters(string manga, IProgress<int> progress, CancellationToken cancellationToken)
        {
            progress.Report(0);
            var downloader = new DownloadService
            {
                Cookies = LoginBatoto(User, Password),
                Referer = "http://bato.to/reader"
            };
            var parser = new ParserHelper();

            // find all chapters in a manga
            string input = await downloader.DownloadStringAsync(manga);
            var chaps = parser.ParseGroup("<a href=\"(?<Value>http://bato.to/reader#[^\"]+)\" title=\"(?<Name>[^|]+)", input, "Name", "Value");
            progress.Report(100);
            return chaps;
        }

        public async Task<IEnumerable<string>> FindImanges(Chapter chapter, IProgress<int> progress, CancellationToken cancellationToken)
        {
            progress.Report(0);
            var downloader = new DownloadService
            {
                Cookies = LoginBatoto(User, Password),
                Referer = "http://bato.to/reader"
            };
            var parser = new ParserHelper();

            // find all pages in a chapter
            var chapterUrl = TransformChapterUrl(chapter.Url);
            var input = await downloader.DownloadStringAsync(chapterUrl);
            var pages = parser.Parse(@"<option value=""(?<Value>http://bato.to/reader#[^""]+)""[^>]+>page", input, "Value");

            // transform pages link
            pages = pages.Select(TransformChapterUrl).ToList();

            // find all images in pages
            var pageData = await downloader.DownloadStringAsync(
                pages,
                new Progress<int>((count) =>
                {
                    var f = (float)count / pages.Count();
                    var i = Convert.ToInt32(f * 100);
                    progress.Report(i);
                }),
                cancellationToken);

            var images = parser.Parse("img src=\"(?<Value>[^\"]+)\" style=\"z-index: 1003", pageData, "Value");

            progress.Report(100);
            return images;
        }

        private CookieCollection LoginBatoto(string user, string password)
        {
            var request =
               WebRequest.CreateHttp("https://bato.to/forums/index.php?app=core&module=global&section=login&do=process");
            request.Method = WebRequestMethods.Http.Post;
            var postData =
                $@"auth_key=880ea6a14ea49e853634fbdc5015a024&referer=https%3A%2F%2Fbato.to%2F&ips_username={user}&ips_password={password}&rememberMe=1";

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            request.CookieContainer = new CookieContainer();
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Logger.Debug("Login response from Batoto");
                return response.Cookies;
            }
        }

        private string TransformChapterUrl(string url)
        {
            var id = url.Substring(url.LastIndexOf('#') + 1);
            var page = "1";
            if (id.LastIndexOf('_') != -1)
            {
                page = id.Substring(id.LastIndexOf('_') + 1);
                id = id.Remove(id.LastIndexOf('_'));
            };
            return $@"http://bato.to/areader?id={id}&p={page}";
        }
    }
}
