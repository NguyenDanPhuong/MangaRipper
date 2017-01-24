using MangaRipper.Core.Controllers;
using MangaRipper.Core.Interfaces;
using MangaRipper.Core.Services;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MangaRipper.Core.CustomException;
using MangaRipper.Core.Models;

namespace MangaRipper.Core.Providers
{
    /// <summary>
    /// The framework contains all services for this app.
    /// When the app start, we have to call Init() to initialization services.
    /// </summary>
    public class FrameworkProvider
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static IEnumerable<IMangaService> _services;
        private static WorkerController _worker;
        private static PluginService _plugins;
        private static Configuration _config;

        /// <summary>
        /// Initialization services.
        /// </summary>
        public static void Init(string pluginPath, string configFile)
        {
            Logger.Info("> Framework.Init()");
            _worker = new WorkerController();
            _config = new Configuration(configFile);
            _plugins = new PluginService(pluginPath, _config);
            _services = _plugins.LoadWebPlugins();
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
    }
}
