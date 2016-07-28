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
        public static ITitle CreateTitle(Uri uri)
        {
            ITitle title = null;
            switch (uri.Host)
            {
                case "mangafox.me":
                    title = new TitleMangaFox(uri);
                    break;
                case "read.mangashare.com":
                    title = new TitleMangaShare(uri);
                    break;
                case "www.mangatoshokan.com":
                    title = new TitleMangaToshokan(uri);
                    break;
                case "www.mangahere.com":
                    title = new TitleMangaHere(uri);
                    break;
                case "www.otakuworks.com":
                    title = new TitleOtakuworks(uri);
                    break;
                case "www.mangareader.net":
                    title = new TitleMangaReader(uri);
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
            //lst.Add(new string[] { "MangaFox", "http://mangafox.me/", "English" });
            //lst.Add(new string[] { "MangaHere", "http://www.mangahere.com/", "English" });
            lst.Add(new string[] { "MangaReader", "http://www.mangareader.net/", "English" }); 
            //lst.Add(new string[] { "MangaShare", "http://read.mangashare.com/", "English" });
            //lst.Add(new string[] { "MangaToshokan", "http://www.mangatoshokan.com/", "English" });
            //lst.Add(new string[] { "Otakuworks", "http://www.otakuworks.com/", "English" });
            return lst;
        }
    }
}
