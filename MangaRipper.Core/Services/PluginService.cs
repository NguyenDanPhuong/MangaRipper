using MangaRipper.Core.Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MangaRipper.Core.Services
{
    public class PluginService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Load plugins for WebSites
        /// </summary>
        /// <param name="path">path to folder in which dll's are located</param>
        /// <returns>List with all founded Services</returns>
        public IEnumerable<IMangaService> LoadWebPlugins(string path)
        {
            LoadPluginAssemblies(path);
            return CreateServices();
        }

        private void LoadPluginAssemblies(string path)
        {
            foreach (var fileOn in Directory.GetFiles(path))
            {
                FileInfo file = new FileInfo(fileOn);
                if (file.Extension.Equals(".dll", StringComparison.OrdinalIgnoreCase))
                {
                    Assembly.LoadFrom(fileOn);
                }
            }
        }

        private IEnumerable<IMangaService> CreateServices()
        {
            IList<IMangaService> result = new List<IMangaService>();
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var t in a.GetTypes())
                {
                    if (t.GetInterface(nameof(IMangaService)) != null)
                    {
                        try
                        {
                            var pluginclass = Activator.CreateInstance(t) as IMangaService;
                            if (pluginclass != null)
                                result.Add(pluginclass);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }
                    }
                }
            }
            return result;
        }
    }
}
