using System;
using System.IO;
using System.Web;

namespace MangaRipper.Core.FilenameDetectors
{
    public class GoogleProxyFilenameDetector : IGoogleProxyFilenameDetector
    {
        public string ParseFilename(string url)
        {
            var uri = new Uri(url);
            if (uri.Host.Equals("images1-focus-opensocial.googleusercontent.com"))
            {
                var p = HttpUtility.ParseQueryString(uri.Query);
                var originalUrl = p["url"];
                return Path.GetFileName(originalUrl);
            }
            return null;
        }
    }
}
