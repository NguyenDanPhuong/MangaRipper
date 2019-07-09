using MangaRipper.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using MangaRipper.Core.CustomException;

namespace MangaRipper.Core.Plugins
{
    /// <summary>
    /// </summary>
    public class PluginManager : IPluginManager
    {
        IEnumerable<IMangaPlugin> _services;
        private readonly ILogger logger;

        /// <summary>
        /// Initialization services.
        /// </summary>
        public PluginManager(IEnumerable<IMangaPlugin> mangaServices, ILogger logger)
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
        public IMangaPlugin GetService(string link)
        {
            IMangaPlugin service = _services.FirstOrDefault(s => s.Of(link));
            if (service == null)
            {
                logger.Error($"Cannot find service for link: {link}");
                throw new MangaRipperException("Cannot find service to download from input site!");
            }
            return service;
        }
    }
}
