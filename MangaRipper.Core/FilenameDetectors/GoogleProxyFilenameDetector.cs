using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MangaRipper.Core.FilenameDetectors
{
    public class GoogleProxyFilenameDetector
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
