using MangaRipper.Core.Helpers;
using MangaRipper.Core.Interfaces;
using MangaRipper.Core.Models;
using NLog;
using MangaRipper.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace MangaRipper.Plugin.ReadComics
{
    public class ReadComics : MangaService
    {
        /* 
         * Author: Jed Burke
         * Acknowledging that this is mostly a copy-paste job of MangaFox.cs
         * 
         * Series tested:
         *  http://readcomics.tv/comic/teen-titans-go-2013
         *  
         */

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly Uri HostUri = new Uri("http://readcomics.tv");

        public override async Task<IEnumerable<Chapter>> FindChapters(string manga, IProgress<int> progress, CancellationToken cancellationToken)
        {
            Logger.Info($@"> FindChapters(): {manga}");

            progress.Report(0);

            var downloader = new Downloader();
            var parser = new ParserHelper();

            try
            {
                string html = await downloader.DownloadStringAsync(manga, cancellationToken);
                
                progress.Report(50);

                string pattern = "<a\\s+class=\"ch-name\"\\s+href=\"(?<Value>.[^\"]*)\">(?<Name>.*)(?=\\<)";
                
                var chapters = parser.ParseGroup(pattern, html, "Name", "Value");
                
                progress.Report(90);

                ((List<Chapter>)chapters).Reverse();

                progress.Report(100);

                return chapters;
            }
            finally
            {
                downloader = null;
                parser = null;
            }

        }

        public override async Task<IEnumerable<string>> FindImages(Chapter chapter, IProgress<int> progress, CancellationToken cancellationToken)
        {
            progress.Report(0);
            var downloader = new Downloader();
            var parser = new ParserHelper();
            string
                html,
                chapterUrl;

            try
            {
                Logger.Info("> FindImages()");

                chapterUrl = string.Concat(chapter.Url, "/full");
                html = await downloader.DownloadStringAsync(chapterUrl, cancellationToken);

                if (!string.IsNullOrWhiteSpace(html))
                {
                    string pattern = "<img\\s+class=\"chapter_img\".*src=\"(?<Value>.[^\"]*)";
                    
                    var pages = parser.Parse(pattern, html, "Value");

                    return pages;
                }

            }
            finally
            {
                downloader = null;
                parser = null;
                html = null;
                chapterUrl = null;
            }

            throw new Core.CustomException.MangaRipperException("Can't find pages.");
        }

        public override SiteInformation GetInformation()
        {
            return new SiteInformation(nameof(ReadComics), HostUri.ToString(), "English");
        }

        public override bool Of(string link)
        {
            Uri uri;

            try
            {
                if (Uri.TryCreate(link, UriKind.Absolute, out uri))
                {
                    return (string.Compare(uri.Host, HostUri.Host, true, System.Globalization.CultureInfo.InvariantCulture) == 0);
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                uri = null;
            }

        }
    }
}
