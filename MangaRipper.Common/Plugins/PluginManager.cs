using System.Collections.Generic;
using System.Linq;
using MangaRipper.Core.CustomException;
using MangaRipper.Core.Logging;

namespace MangaRipper.Core.Plugins
{
    /// <summary>
    /// </summary>
    public class PluginManager : IPluginManager
    {
        readonly IEnumerable<IPlugin> _services;
        private readonly ILogger<PluginManager> logger;

        /// <summary>
        /// Initialization services.
        /// </summary>
        public PluginManager(IEnumerable<IPlugin> mangaServices, ILogger<PluginManager> logger)
        {
            this.logger = logger;
            this.logger.Info("> ServiceManager.Init()");
            _services = mangaServices;
        }

        /// <summary>
        /// Find service base on inputed URL.
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public IPlugin GetPlugin(string link)
        {
            IPlugin service = _services.FirstOrDefault(s => s.Of(link));
            if (service == null)
            {
                logger.Error($"Cannot find service for link: {link}");
                throw new MangaRipperException("Cannot find service to download from input site!");
            }
            return service;
        }
    }
}
