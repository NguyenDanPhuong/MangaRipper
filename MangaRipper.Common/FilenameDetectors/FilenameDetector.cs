using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace MangaRipper.Core.FilenameDetectors
{
    public class FilenameDetector : IFilenameDetector
    {
        private readonly IGoogleProxyFilenameDetector googleProxyFilenameParser;

        public FilenameDetector(IGoogleProxyFilenameDetector googleProxyFilenameParser)
        {
            this.googleProxyFilenameParser = googleProxyFilenameParser;
        }

        public string GetFilename(HttpResponseMessage response)
        {
            var url = response.RequestMessage.RequestUri.AbsoluteUri;
            var googleProxyName = googleProxyFilenameParser.ParseFilename(url);
            if (!string.IsNullOrEmpty(googleProxyName))
            {
                return googleProxyName;
            }

            var headers = response.Content.Headers;
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