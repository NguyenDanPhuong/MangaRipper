using MangaRipper.Core.Controllers;
using MangaRipper.Core.Interfaces;
using MangaRipper.Core.Services;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MangaRipper.Core.Providers
{
    /// <summary>
    /// The framework contains all services for this app.
    /// When the app start, we have to call Init() to initialization services.
    /// </summary>
    public class FrameworkProvider
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static IList<IMangaService> _services;
        private static WorkerController _worker;
        private static PluginService _plugins;

        /// <summary>
        /// Initialization services.
        /// </summary>
        public static void Init(string pluginPath)
        {
            Logger.Info("> Framework.Init()");
            _worker = new WorkerController();            
            _plugins = new PluginService();
            // TODO Put the path to plugins into config
            _services = _plugins.LoadWebPlugins(Environment.CurrentDirectory + pluginPath);
        }
        
        /// <summary>
        /// Get all available services
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IMangaService> GetServices()
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
                throw new Exception("Cannot find service to download from input site!");
            }
            return service;
        }
    }
}
