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

        // variables for passing JS challenge
        private const string CloudFlareServerName = "cloudflare-nginx";
        private const string ClearanceCookieName = "cf_clearance";
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
            // Add the Headers for "User Agent"
            request.Headers.Add("User-Agent",new ProductInfoHeaderValue("Client", "1.0").ToString());
            // Search for cookies. Both cookies should be presented.
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
            return await WorkWithStreams(url);

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

                // Do we need to use JS challenge
                //var isCloudFlareServer = ex.Response.Headers
                    //.Server.Any(i => i.Product != null && i.Product.Name == CloudFlareServerName);

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
            Logger.Info("> DownloadFileAsync begin: {0} - {1}", url, fileName);
            var result = await WorkWithStreams(url, fileName, cancellationToken);
            Logger.Info("> DownloadFileAsync result: {0} - {1}", url, result);
        }

        private async Task<string> WorkWithStreams(string url, string fileName = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = CreateRequest(url);

            try
            {
                using (var response = (HttpWebResponse) await request.GetResponseAsync())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream == null)
                            throw new HttpRequestException();

                        if (string.IsNullOrWhiteSpace(fileName))
                        {
                            using (var streamReader = new StreamReader(responseStream, Encoding.UTF8))
                            {
                                return await streamReader.ReadToEndAsync();
                            }
                        }
                        else
                        {
                            using (var streamReader = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                            {
                                await responseStream.CopyToAsync(streamReader, 81920, cancellationToken);
                                return response.StatusCode.ToString();
                            }
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Response== null)
                    throw new HttpRequestException();

                var html = "";
                using (var streamReader = new StreamReader(ex.Response.GetResponseStream()))
                    html = streamReader.ReadToEnd();

                return html;


                // if we can't access server, and in Headers we found "cloudflare-nginx" - it's our client
                //var isCloudFlareServer = ex.Response.Headers
                //.Server.Any(i => i.Product != null && i.Product.Name == CloudFlareServerName);

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

                // Save the cookies from response
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
                // Make the request to specific URL which should be solved
                //var clearanceRequest = new HttpRequestMessage(HttpMethod.Get, clearanceUri);

                // This one should return the ClearanceCookieName. Use them for new request

            }
        }

    }
}
