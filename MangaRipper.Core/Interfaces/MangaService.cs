using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MangaRipper.Core.Models;
using NLog;

namespace MangaRipper.Core.Interfaces
{
    public abstract class MangaService : IMangaService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public virtual void Configuration(IEnumerable<KeyValuePair<string, object>> settings)
        {
            Logger.Info("The plugin has no configuration");
        }

        public abstract SiteInformation GetInformation();

        public abstract bool Of(string link);

        public abstract Task<IEnumerable<Chapter>> FindChapters(string manga, IProgress<int> progress,
            CancellationToken cancellationToken);

        public abstract Task<IEnumerable<string>> FindImages(Chapter chapter, IProgress<int> progress,
            CancellationToken cancellationToken);
    }
}
