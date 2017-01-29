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
        private const int MaxRetries = 3;
        private int retries;

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
            request.UserAgent = new ProductInfoHeaderValue("Client", "1.0").ToString();

            // Search for cookies. Both cookies should be presented.
            //request.CookieContainer = ((Cookies != null) || (CookiesCollection != null)) ? new CookieContainer() : null;
            if (Cookies != null)
            {
                request.CookieContainer = new CookieContainer();
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
            retries = 0;
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
            retries = 0;
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

                // if we can't access server, and in Headers we found "cloudflare-nginx" - it's our client
                if (response.Headers["Server"] == CloudFlareServerName)
                {
                    while (MaxRetries < 0 || retries <= MaxRetries)
                    {
                        await SolverCloudFlare(response, html);
                        retries++;
                        var res = await WorkWithStreams(url, fileName, cancellationToken);
                    }
                }

                return html;
            }
        }

        private async Task SolverCloudFlare(HttpWebResponse response, string html)
        {
            // Solve the CloudFlare challenge
            var scheme = response.ResponseUri.Scheme;
            var host = response.ResponseUri.Host;
            var port = response.ResponseUri.Port;
            var solution = ChallengeSolver.Solve(html, host);
            var clearanceUri = $"{scheme}://{host}:{port}{solution.ClearanceQuery}";

            // Save the cookies from response
            SetTheCookie(response.Headers["Set-Cookie"]);
            //CookiesCollection = CookiesCollection ?? new CookieContainer();

            //if (response.Headers["Set-Cookie"] != null)
            //{
            //    var cookies = response.Headers["Set-Cookie"].Split(';');
            //    //[0]: "__cfduid=d797194a284bbc5fb1e45f2403156f0641485695919"
            //    //[1]: " expires=Mon, 29-Jan-18 13:18:39 GMT"
            //    //[2]: " path=/"
            //    //[3]: " domain=.kissmanga.com"
            //    //[4]: " HttpOnly"
            //    string name = cookies[0].Split('=')[0];
            //    if (name.Trim() == IdCookieName)
            //    {
            //        string value = cookies[0].Substring(name.Length + 1);
            //        string path = cookies[2].Split('=')[1];
            //        string domain = cookies[3].Split('=')[1];
            //        CookiesCollection.Add(new Cookie(name.Trim(), value.Trim(), path, domain));
            //    }
            //}

            await Task.Delay(5000);

            // Make the request to specific URL which should be solved
            var request = CreateRequest(clearanceUri);
            //var request = WebRequest.CreateHttp(clearanceUri);
            //request.Method = WebRequestMethods.Http.Get;

            request.CookieContainer = null;
            try
            {
                // This one should return the ClearanceCookieName. Use them for new request
                using (HttpWebResponse clearanceResponse = (HttpWebResponse)request.GetResponse())
                {
                    //var newcookies = responsedouble.Cookies;
                    SetTheCookie(clearanceResponse.Headers["Set-Cookie"]);
                }
            }
            catch (WebException ex) // Maybe we do not need them
            {
                if (ex.Response == null)
                    throw new HttpRequestException(ex.Message, ex);

                response = (HttpWebResponse)ex.Response;
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                    html = streamReader.ReadToEnd();
            }
            //var clearanceRequest = new HttpRequestMessage(HttpMethod.Get, clearanceUri);            
        }

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
                    string name = cookie[0].Split('=')[0];
                    if (name.Trim() == IdCookieName || name.Trim() == ClearanceCookieName)
                    {
                        string value = cookie[0].Substring(name.Length + 1);
                        string path = cookie[2].Split('=')[1];
                        string domain = cookie[3].Split('=')[1];
                        CookiesCollection.Add(new Cookie(name.Trim(), value.Trim(), path, domain));
                    }

                    position += "HttpOnly,".Length;
                    if ((cookies.Length - position) < 0)
                        break;
                    cookies = cookies.Substring(position, cookies.Length - position);
                }
            }
        }


    }
}
