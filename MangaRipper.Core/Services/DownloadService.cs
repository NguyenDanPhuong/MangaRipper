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

        // variables for passing CloudFlare challenge
        private const string CloudFlareServerName = "cloudflare-nginx";
        private const string ClearanceCookieName = "cf_clearance";
        private const string IdCookieName = "__cfduid";
        private const int MaxRetries = 3;
        private int _retries;

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

            // Add the Headers for "User Agent"
            request.UserAgent = new ProductInfoHeaderValue("Client", "1.0").ToString();
            request.AllowAutoRedirect = false;
                        
            if (Cookies != null || CookiesCollection != null)
            {
                request.CookieContainer = CookiesCollection ?? new CookieContainer();
                if (Cookies != null)
                    request.CookieContainer.Add(Cookies);
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
            _retries = 0;
            return await WorkWithStreams(url);
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
            var inputUrls = urls.ToArray();
            Logger.Info("> DownloadStringAsync(IEnumerable) - Total: {0}", inputUrls.Count());
            var sb = new StringBuilder();
            var count = 0;
            progress.Report(count);
            foreach (var url in inputUrls)
            {
                var input = await DownloadStringAsync(url);
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
            _retries = 0;
            var result = await WorkWithStreams(url, fileName, cancellationToken);
            Logger.Info("> DownloadFileAsync result: {0} - {1}", url, result);
        }

        private async Task<string> WorkWithStreams(string url, string fileName = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = CreateRequest(url);

            try
            {
                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream == null)
                            throw new HttpRequestException();

                        if (string.IsNullOrWhiteSpace(fileName))
                        {
                            // Get the html from site
                            using (var streamReader = new StreamReader(responseStream, Encoding.UTF8))
                            {
                                return await streamReader.ReadToEndAsync();
                            }
                        }
                        else
                        {
                            //Download the file
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
                if (ex.Response == null)
                    throw new HttpRequestException(ex.Message, ex);
                var response = (HttpWebResponse)ex.Response;

                var html = "";
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                    html = streamReader.ReadToEnd();

                // if we can't access server, and in Headers we found "cloudflare-nginx" - Solve the challenge
                if (response.Headers["Server"] == CloudFlareServerName)
                {
                    while ((_retries <= MaxRetries) && CookiesCollection == null)
                    {
                        await SolverCloudFlare(response, html);
                        _retries++;
                        html = await WorkWithStreams(url, fileName, cancellationToken);
                    }
                }

                return html;
            }
        }

        #region CloudFlare

        /// <summary>
        /// Solve the CloudFlare challenge
        /// </summary>
        /// <param name="response">Response from the site</param>
        /// <param name="html">HTML text of response</param>
        /// <returns></returns>
        private async Task SolverCloudFlare(HttpWebResponse response, string html)
        {
            var scheme = response.ResponseUri.Scheme;
            var host = response.ResponseUri.Host;
            var port = response.ResponseUri.Port;
            var solution = ChallengeSolver.Solve(html, host);
            var clearanceUri = $"{scheme}://{host}:{port}{solution.ClearanceQuery}";

            // Save the cookies from response
            SetTheCookie(response.Headers["Set-Cookie"]);

            await Task.Delay(5000);

            // Make the request to specific URL which should be solved
            var request = CreateRequest(clearanceUri);

            request.CookieContainer = null;

            // This one should return the ClearanceCookieName. Use them for new request
            using (HttpWebResponse clearanceResponse = (HttpWebResponse)request.GetResponse())
            {
                SetTheCookie(clearanceResponse.Headers["Set-Cookie"]);
            }
        }

        /// <summary>
        /// Get the cookies from the string by "split", "substring" operations
        /// </summary>
        /// <param name="cookies">cookies from response</param>
        private void SetTheCookie(string cookies)
        {
            if (cookies != null)
            {
                // Save the cookies from response
                CookiesCollection = new CookieContainer();

                while (cookies.Length > 0)
                {
                    var position = cookies.IndexOf("HttpOnly");

                    var cookie = cookies.Substring(0, position).Split(';');

                    //[0]: "__cfduid=d797194a284bbc5fb1e45f2403156f0641485695919"
                    //[0]: "cf_clearance=3ffcd03fb190bb0bd80b8f8c2a1e0dd3b49330a4-1485712339-86400"
                    //[1]: " expires=Mon, 29-Jan-18 13:18:39 GMT"
                    //[2]: " path=/"
                    //[3]: " domain=.kissmanga.com"
                    //[4]: " HttpOnly"
                    var name = cookie[0].Split('=')[0];
                    if (name.Trim() == IdCookieName || name.Trim() == ClearanceCookieName)
                    {
                        var value = cookie[0].Substring(name.Length + 1);
                        var path = cookie[2].Split('=')[1];
                        var domain = cookie[3].Split('=')[1];
                        CookiesCollection.Add(new Cookie(name.Trim(), value.Trim(), path, domain));
                    }

                    position += "HttpOnly,".Length;
                    if ((cookies.Length - position) < 0)
                        break;
                    cookies = cookies.Substring(position, cookies.Length - position);
                }

                // wait for "__cfduid" and "cf_clearance" to be complete
                if (CookiesCollection.Count < 2)
                    CookiesCollection = null;
            }
        }
        #endregion
    }
}
