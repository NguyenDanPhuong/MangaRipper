using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MangaRipper.Core.Services
{
    class PluginManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private IEnumerable<Assembly> LoadedAssemblies;

        private string PluginsPath { get; set; }
        public PluginManager(string pluginsPath)
        {
            PluginsPath = pluginsPath;
            LoadedAssemblies = LoadPluginAssemblies(PluginsPath);
        }

        /// <summary>
        /// Load plugins for WebSites
        /// </summary>
        /// <returns>List with all founded Services</returns>
        public IEnumerable<T> CreateServices<T>() where T : class
        {
            var result = new List<T>();
            foreach (var a in LoadedAssemblies)
            {
                var pluginTypes = a.GetTypes().Where(t => !t.IsAbstract && typeof(T).IsAssignableFrom(t));
                try
                {
                    foreach (var pluginType in pluginTypes)
                    {
                        var pluginclass = Activator.CreateInstance(pluginType) as T;
                        if (pluginclass != null)
                            result.Add(pluginclass);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
            return result;
        }

        private IEnumerable<Assembly> LoadPluginAssemblies(string path)
        {
            if (!Directory.Exists(path))
            {
                var error = $"The plugins path: `{path}` is not exist!";
                Logger.Error(error);
                throw new DirectoryNotFoundException(error);
            }

            foreach (var fileOn in Directory.GetFiles(path))
            {
                FileInfo file = new FileInfo(fileOn);
                if (file.Extension.Equals(".dll", StringComparison.OrdinalIgnoreCase))
                {
                    Logger.Info($@"Load plugin from file: {fileOn}");
                    yield return Assembly.LoadFrom(fileOn);
                }
            }
        }
    }
}
