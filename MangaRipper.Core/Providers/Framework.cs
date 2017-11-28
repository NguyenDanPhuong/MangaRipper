using MangaRipper.Core.Controllers;
using MangaRipper.Core.Interfaces;
using MangaRipper.Core.Services;
using NLog;
using System.Collections.Generic;
using System.Linq;
using MangaRipper.Core.CustomException;
using MangaRipper.Core.Models;

namespace MangaRipper.Core.Providers
{
    /// <summary>
    /// The framework contains all services for this app.
    /// When the app start, we have to call Init() to initialization services.
    /// </summary>
    public class Framework
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static IEnumerable<IMangaService> _services;
        private static WorkerController _worker;

        /// <summary>
        /// Initialization services.
        /// </summary>
        public static void Init(string pluginPath, string configFile)
        {
            Logger.Info("> Framework.Init()");
            var _config = new Configuration(configFile);
            var _plugins = new PluginManager(pluginPath);
            _worker = new WorkerController();
            _services = _plugins.CreateServices<IMangaService>();
            foreach (var service in _services)
            {
                InjectConfiguration(service, _config);
            }
        }

        /// <summary>
        /// Get all available services
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IMangaService> GetMangaServices()
        {
            return _services.ToList().AsReadOnly();
        }

        /// <summary>
        /// Get worker to download chapter.
        /// </summary>
        /// <returns></returns>
        public static WorkerController GetWorker()
        {
            return _worker;
        }

        /// <summary>
        /// Find service base on inputed URL.
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public static IMangaService GetService(string link)
        {
            IMangaService service = _services.FirstOrDefault(s => s.Of(link));
            if (service == null)
            {
                Logger.Error("Cannot find service for link: {0}", link);
                throw new MangaRipperException("Cannot find service to download from input site!");
            }
            return service;
        }

        private static void InjectConfiguration(IMangaService service, Configuration config)
        {
            string lookupPrefix = $@"Plugin.{service.GetInformation().Name}.";
            var configItems = config.FindConfigByPrefix(lookupPrefix);
            configItems = RemovePrefix(configItems, lookupPrefix);
            service.Configuration(configItems);
        }

        private static IEnumerable<KeyValuePair<string, object>> RemovePrefix(IEnumerable<KeyValuePair<string, object>> configItems, string prefix)
        {
            return configItems.ToArray().Select(i => new KeyValuePair<string, object>(i.Key.Remove(0, prefix.Length), i.Value));
        }
    }
}
