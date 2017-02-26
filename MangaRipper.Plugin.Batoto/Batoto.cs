using MangaRipper.Core.Helpers;
using MangaRipper.Core.Interfaces;
using MangaRipper.Core.Models;
using MangaRipper.Core.Services;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Plugin.Batoto
{
    /// <summary>
    /// Support find chapters, images from Batoto
    /// </summary>
    public class Batoto : MangaService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private string _username = "gufrohepra";
        private string _password = "123";

        public override void Configuration(IEnumerable<KeyValuePair<string, object>> settings)
        {
            var settingCollection = settings.ToArray();
            if (settingCollection.Any(i => i.Key.Equals("Username")))
            {
                var user = settingCollection.First(i => i.Key.Equals("Username")).Value;
                Logger.Info($@"Current Username: {_username}. New Username: {user}");
                _username = user as string;
            }

            if (settingCollection.Any(i => i.Key.Equals("Password")))
            {
                var pass = settingCollection.First(i => i.Key.Equals("Password")).Value;
                Logger.Info($@"Current Password: {_password}. New Password: {pass}");
                _password = pass as string;
            }
        }

        public override SiteInformation GetInformation()
        {
            return new SiteInformation(nameof(Batoto), "http://bato.to", "Multiple Languages");
        }

        public override bool Of(string link)
        {
            var uri = new Uri(link);
            return uri.Host.Equals("bato.to");
        }

        public override async Task<IEnumerable<Chapter>> FindChapters(string manga, IProgress<int> progress, CancellationToken cancellationToken)
        {
            progress.Report(0);
            var downloader = new DownloadService
            {
                Cookies = LoginBatoto(_username, _password),
                Referrer = "http://bato.to/reader"
            };
            var parser = new ParserHelper();

            // find all chapters in a manga
            string input = await downloader.DownloadStringAsync(manga);
            var chaps = parser.ParseGroup("<a href=\"(?<Value>http://bato.to/reader#[^\"]+)\" title=\"(?<Name>[^|]+)", input, "Name", "Value");
            progress.Report(100);
            return chaps;
        }

        public override async Task<IEnumerable<string>> FindImanges(Chapter chapter, IProgress<int> progress, CancellationToken cancellationToken)
        {
            progress.Report(0);
            var downloader = new DownloadService
            {
                Cookies = LoginBatoto(_username, _password),
                Referrer = "http://bato.to/reader"
            };
            var parser = new ParserHelper();

            // find all pages in a chapter
            var chapterUrl = TransformChapterUrl(chapter.Url);
            var input = await downloader.DownloadStringAsync(chapterUrl);
            var pages = parser.Parse(@"<option value=""(?<Value>http://bato.to/reader#[^""]+)""[^>]+>page", input, "Value");

            // transform pages link
            var transformedPages = pages.Select(TransformChapterUrl).ToList();

            // find all images in pages
            var pageData = await downloader.DownloadStringAsync(
                transformedPages,
                new Progress<int>((count) =>
                {
                    var f = (float)count / transformedPages.Count();
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
