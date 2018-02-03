using System;
using System.IO;
using System.Net.Http.Headers;

namespace MangaRipper.Core.FilenameDetectors
{
    public class FilenameDetector
    {
        private readonly GoogleProxyFilenameDetector googleProxyFilenameParser;

        public FilenameDetector(GoogleProxyFilenameDetector googleProxyFilenameParser)
        {
            this.googleProxyFilenameParser = googleProxyFilenameParser;
        }

        public string GetFilename(string url, HttpContentHeaders headers)
        {
            var googleProxyName = googleProxyFilenameParser.ParseFilename(url);
            if (!string.IsNullOrEmpty(googleProxyName))
            {
                return googleProxyName;
            }

            var fileNameFromServer = headers.ContentDisposition?.FileName?.Trim().Trim(new char[] { '"' });
            if (!string.IsNullOrEmpty(fileNameFromServer))
            {
                return fileNameFromServer;
            }

            var uri = new Uri(url);
            return Path.GetFileName(uri.LocalPath);
        }
    }
}