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
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Load plugins for WebSites
        /// </summary>
        /// <param name="path">path to folder in which dll's are located</param>
        /// <returns>List with all founded Services</returns>
        public IList<IMangaService> LoadWebPlugins(string path)
        {
            List<IMangaService> result = new List<IMangaService>();

            //Go through all the files in the plugin directory
            foreach (string fileOn in Directory.GetFiles(path))
            {
                FileInfo file = new FileInfo(fileOn);

                //Preliminary check, must be .dll
                if (file.Extension.Equals(".dll"))
                {
                    //Add the 'plugin'
                    Assembly.LoadFrom(fileOn);
                }
            }

            //Loop through all opened assemblies
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in a.GetTypes())
                {
                    // We need only "IMangaService"
                    if (t.GetInterface("IMangaService") != null)
                    {
                        try
                        {
                            IMangaService pluginclass = Activator.CreateInstance(t) as IMangaService;
                            if (pluginclass != null)
                                result.Add(pluginclass);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex);
                        }
                    }
                }
            }

            return result;
        }
    }
}
