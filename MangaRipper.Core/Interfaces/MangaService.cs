using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MangaRipper.Core.Models;
using NLog;

namespace MangaRipper.Core.Interfaces
{
    public abstract class MangaService : IMangaService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private string[] AlternativeLinks;

        public virtual void Configuration(IEnumerable<KeyValuePair<string, object>> settings)
        {
            if (settings.Any())
            {
                var settingCollection = settings.ToArray();
                if (settingCollection.Any(i => i.Key.Equals("AlternativeLinks")))
                {
                    AlternativeLinks = settingCollection.First(i => i.Key.Equals("AlternativeLinks")).Value.ToString().Split(',');
                }
            }
            else
            {
                Logger.Info("The plugin has no configuration");
            }
        }
        
        public abstract SiteInformation GetInformation();
        
        public abstract bool Of(string link);
        
        public virtual bool Of(string link, string providerUrl)
        {
            return Of(link);
        }

        public string CheckWithAlternative(Uri link, string address)
        {
            // Original Address is the right one
            if (link.Host.Equals(address))
            {
                return address;
            }
            
            // Check Alternate links
            if (AlternativeLinks != null && Array.IndexOf(AlternativeLinks, link.Host) > -1)
            {
                return AlternativeLinks[Array.IndexOf(AlternativeLinks, link.Host)];
            }

            return null;
        }

        public abstract Task<IEnumerable<Chapter>> FindChapters(string manga, IProgress<int> progress,
            CancellationToken cancellationToken);

        public abstract Task<IEnumerable<string>> FindImages(Chapter chapter, IProgress<int> progress,
            CancellationToken cancellationToken);
    }
}
