using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using MangaRipper.Properties;
using System.Security;

namespace MangaRipper
{
    static class Option
    {
        /// <summary>
        /// Get proxy setting
        /// </summary>
        /// <returns></returns>
        public static IWebProxy GetProxy()
        {
            IWebProxy wp = null;

            if (Settings.Default.ProxyEnable == true)
            {
                try
                {
                    string host = Settings.Default.ProxyHost;
                    int port = Convert.ToInt32(Settings.Default.ProxyPort);
                    string userName = Settings.Default.ProxyUserName;
                    string password = Settings.Default.ProxyPassword;

                    wp = new WebProxy(host, port);
                    wp.Credentials = new NetworkCredential(userName, password);
                }
                catch
                {
                    wp = null;
                }
            }

            return wp;
        }
    }
}
