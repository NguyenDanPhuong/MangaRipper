using MangaRipper.Core;
using MangaRipper.Core.Helpers;
using MangaRipper.Core.Interfaces;
using MangaRipper.Core.Models;
using MangaRipper.Core.Services;
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
    public class Batoto : IMangaService
    {
        private ILogger Logger;
        private readonly Downloader downloader;
        private readonly ParserHelper parser;
        private string _username = "gufrohepra";
        private string _password = "123";
        private string _languagesRegEx;

        public Batoto(Configuration config, ILogger myLogger, Downloader downloader, ParserHelper parser)
        {
            Logger = myLogger;
            this.downloader = downloader;
            this.parser = parser;
            if (config == null)
            {
                return;
            }
            Configuration(config.FindConfigByPrefix("Plugin.Batoto"));
        }

        public void Configuration(IEnumerable<KeyValuePair<string, object>> settings)
        {
            // TODO FIX THIS
            var settingCollection = settings.ToArray();
            if (settingCollection.Any(i => i.Key.Equals("Plugin.Batoto.Username")))
            {
                var user = settingCollection.First(i => i.Key.Equals("Plugin.Batoto.Username")).Value;
                Logger.Info($@"Current Username: {_username}. New Username: {user}");
                _username = user as string;
            }

            if (settingCollection.Any(i => i.Key.Equals("Plugin.Batoto.Password")))
            {
                var pass = settingCollection.First(i => i.Key.Equals("Plugin.Batoto.Password")).Value;
                Logger.Info($@"Current Password: {_password}. New Password: {pass}");
                _password = pass as string;
            }

            if (settingCollection.Any(i => i.Key.Equals("Plugin.Batoto.Languages")))
            {
                var languages = settingCollection.First(i => i.Key.Equals("Plugin.Batoto.Languages")).Value as string;
                Logger.Info($@"Only the follow languages will be selected: {languages}");
                // For test purpose
                if (!string.IsNullOrEmpty(languages))
                {
                    var languagesRegEx = languages.Replace(" ", String.Empty).Replace(",", "|");
                    _languagesRegEx = "<tr class=\"\\w+ lang_(" + languagesRegEx + ") \\w+\"( style=\"display:none;\")?>\\s*<td style=\"[^\"]+\">\\s*";
                }
                else
                {
                    _languagesRegEx = null;
                }
            }
        }

        public SiteInformation GetInformation()
        {
            return new SiteInformation(nameof(Batoto), "http://bato.to", "Multiple Languages");
        }

        public bool Of(string link)
        {
            var uri = new Uri(link);
            return uri.Host.Equals("bato.to");
        }

        public async Task<IEnumerable<Chapter>> FindChapters(string manga, IProgress<int> progress, CancellationToken cancellationToken)
        {
            progress.Report(0);
            downloader.Cookies = LoginBatoto(_username, _password);
            downloader.Referrer = "http://bato.to/reader";

            // find all chapters in a manga
            string input = await downloader.DownloadStringAsync(manga, cancellationToken);

            var allLanguagesRegEx = "<a href=\"(?<Value>https://bato.to/reader#[^\"]+)\" title=\"(?<Name>[^|]+)";
            // Choose only specific languages if it set so in config
            if (!string.IsNullOrEmpty(_languagesRegEx))
            {
                allLanguagesRegEx = _languagesRegEx + allLanguagesRegEx;
            }
            var chaps = parser.ParseGroup(allLanguagesRegEx, input, "Name", "Value");
            progress.Report(100);
            return chaps;
        }

        public async Task<IEnumerable<string>> FindImages(Chapter chapter, IProgress<int> progress, CancellationToken cancellationToken)
        {
            progress.Report(0);
            downloader.Cookies = LoginBatoto(_username, _password);
            downloader.Referrer = "http://bato.to/reader";

            // find all pages in a chapter
            var chapterUrl = TransformChapterUrl(chapter.Url);
            var input = await downloader.DownloadStringAsync(chapterUrl, cancellationToken);
            var pages = parser.Parse(@"<option value=""(?<Value>http://bato.to/reader#[^""]+)""[^>]+>page", input, "Value");

            // transform pages link
            var transformedPages = pages.Select(TransformChapterUrl).ToList();

            // find all images in pages
            var pageData = await downloader.DownloadStringAsync(
                transformedPages,
                new Progress<int>((count) =>
                {
                    var f = (float)count / transformedPages.Count;
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
            return $@"https://bato.to/areader?id={id}&p={page}";
        }
    }
}
