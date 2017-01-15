using CloudFlareUtilities;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MangaRipper.Core.Services
{
    /// <summary>
    /// Support download web page to string and image file to folder.
    /// </summary>
    public class DownloadService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private const string IdCookieName = "__cfduid";
        
        public CookieCollection Cookies { get; set; }

        private CookieContainer CookiesCollection { get; set; }

        public string Referer { get; set; }

        private HttpWebRequest CreateRequest(string url)
        {
            var uri = new Uri(url);
            var request = WebRequest.CreateHttp(uri);
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Referer = Referer;
            request.AllowAutoRedirect = false;
            request.Headers.Add("User-Agent",new ProductInfoHeaderValue("Client", "1.0").ToString());
            //request.CookieContainer = ((Cookies != null) || (CookiesCollection != null)) ? new CookieContainer() : null;


            if (Cookies != null)
            {
                request.CookieContainer.Add(Cookies);
            }
            if (CookiesCollection != null)
            {
                request.CookieContainer = CookiesCollection;
            }


            return request;
        }

        /// <summary>
        /// Download single web page to string.
        /// </summary>
        /// <param name="url">The URL to download</param>
        /// <returns></returns>
        public async Task<string> DownloadStringAsync(string url)
        {
            Logger.Info("> DownloadStringAsync: {0}", url);
            var request = CreateRequest(url);
            string html;

            try
            {
                HttpWebResponse responseWait = (HttpWebResponse)await
                    request.GetResponseAsync();
            }
            catch (WebException ex)
            {
                using (var sr = new StreamReader(ex.Response.GetResponseStream()))
                    html = sr.ReadToEnd();

                var scheme = ex.Response.ResponseUri.Scheme;
                var host = ex.Response.ResponseUri.Host;
                var port = ex.Response.ResponseUri.Port;
                var solution = ChallengeSolver.Solve(html, "kissmanga.com");
                var clearanceUri = $"{scheme}://{host}:{port}{solution.ClearanceQuery}";

                var headers = ex.Response.Headers;
                //.Where(pair => pair.Key == HttpHeader.SetCookie)
                //.SelectMany(pair => pair.Value)
                //.Where(cookie => cookie.StartsWith($"__cfduid="));
                //var cookies = headers.GetValues("Set-Cookie");

                CookiesCollection = new CookieContainer();
                //CookiesCollection = headers.GetValues("Set-Cookie");
                string _cookie = "";
                foreach (var cookie in headers.GetValues("Set-Cookie"))
                {
                    _cookie += cookie;
                }
                CookiesCollection.SetCookies(ex.Response.ResponseUri, _cookie);

                await Task.Delay(5000);

                request = CreateRequest(clearanceUri);
                //var clearanceRequest = new HttpRequestMessage(HttpMethod.Get, clearanceUri);
            }

            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    var streamReader = new StreamReader(responseStream, Encoding.UTF8);
                    return await streamReader.ReadToEndAsync();
                }
            }
        }

        /// <summary>
        /// Download a list of web page.
        /// </summary>
        /// <param name="urls">List of URL</param>
        /// <param name="progress">Progress report callback</param>
        /// <param name="cancellationToken">Cancellation control</param>
        /// <returns></returns>
        public async Task<string> DownloadStringAsync(IEnumerable<string> urls, IProgress<int> progress, CancellationToken cancellationToken)
        {
            Logger.Info("> DownloadStringAsync - Total: {0}", urls.Count());
            var sb = new StringBuilder();
            var count = 0;
            progress.Report(count);
            foreach (var url in urls)
            {
                string input = await DownloadStringAsync(url);
                sb.Append(input);
                cancellationToken.ThrowIfCancellationRequested();
                progress.Report(count++);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Download file and save to folder
        /// </summary>
        /// <param name="url">The URL to download</param>
        /// <param name="fileName">Save to filename</param>
        /// <param name="cancellationToken">Cancellation control</param>
        /// <returns></returns>
        public async Task DownloadFileAsync(string url, string fileName, CancellationToken cancellationToken)
        {
            Logger.Info("> DownloadFileAsync: {0} - {1}", url, fileName);
            var request = CreateRequest(url);
            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            {
                using (var responseStream = response.GetResponseStream())
                using (var streamReader = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    await responseStream.CopyToAsync(streamReader, 81920, cancellationToken);
                }
            }
        }
    }
}
