using System;
using System.Collections.Generic;

namespace MangaRipper.Core
{
    public static class TitleFactory
    {
        /// <summary>
        /// Create Title object base on uri
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static ITitle CreateTitle(String url)
        {
            var uri = new Uri(url);
            ITitle title = null;
            switch (uri.Host)
            {
                case "mangafox.me":
                    title = new TitleMangaFox(url);
                    break;
                case "read.mangashare.com":
                    title = new TitleMangaShare(url);
                    break;
                case "www.mangahere.co":
                    title = new TitleMangaHere(url);
                    break;
                case "www.mangareader.net":
                    title = new TitleMangaReader(url);
                    break;
                default:
                    string message = string.Format("This site ({0}) is not supported.", uri.Host);
                    throw new Exception(message);
            }
            return title;
        }

        /// <summary>
        /// Get list of supported manga sites
        /// </summary>
        /// <returns></returns>
        public static List<string[]> GetSupportedSites()
        {
            var lst = new List<string[]>();
            lst.Add(new string[] { "MangaFox", "http://mangafox.me/", "English" });
            lst.Add(new string[] { "MangaHere", "http://www.mangahere.co/", "English" });
            lst.Add(new string[] { "MangaReader", "http://www.mangareader.net/", "English" });
            lst.Add(new string[] { "MangaShare", "http://read.mangashare.com/", "English" });
            return lst;
        }
    }
}
