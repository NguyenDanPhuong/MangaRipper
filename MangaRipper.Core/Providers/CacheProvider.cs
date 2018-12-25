using System;
using System.Collections.Generic;
using System.Net;

namespace MangaRipper.Core.Providers
{
    /// <summary>
    /// Our Singleton to keep the cookies (For now it's only CloudFlare cookies)
    /// </summary>
    public sealed class CacheProvider
    {
        private static readonly Lazy<CacheProvider> lazy =
        new Lazy<CacheProvider>(() => new CacheProvider());

        public static CacheProvider Instance { get { return lazy.Value; } }

        private Dictionary<string, Cookie> Cache;

        private CacheProvider()
        {
            Cache = new Dictionary<string, Cookie>();
        }

        public bool CacheExists(string key)
        {
            return Cache.ContainsKey(key);
        }

        public Cookie GetCacheValue(string key)
        {
            if (CacheExists(key))
            {
                return Cache[key];
            }
            else
            {
                return null;
            }
        }

        public void SetCacheValue(string key, Cookie value)
        {
            Cache[key] = value;
        }

    }
}
