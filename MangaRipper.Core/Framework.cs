using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    /// <summary>
    /// The framework contains all services for this app.
    /// When the app start, we have to call Init() to initilazation services.
    /// </summary>
    public class Framework
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static IList<IMangaService> services;
        private static Worker worker;

        /// <summary>
        /// Initilaztion services.
        /// </summary>
        public static void Init()
        {
            logger.Info("> Framework.Init()");
            worker = new Worker();
            services = new List<IMangaService>();
            services.Add(new MangaFoxService());
            services.Add(new MangaHereService());
            services.Add(new MangaReaderService());
            services.Add(new MangaShareService());
            //services.Add(new KissMangaService());
        }

        /// <summary>
        /// Get all avaible services
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IMangaService> GetServices()
        {
            return services.ToList().AsReadOnly();
        }

        /// <summary>
        /// Get worker to download chapter.
        /// </summary>
        /// <returns></returns>
        public static Worker GetWorker()
        {
            return worker;
        }

        /// <summary>
        /// Find service base on inputed url.
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public static IMangaService GetService(string link)
        {
            IMangaService service = services.FirstOrDefault(s => s.Of(link));
            if(service == null)
            {
                logger.Error("Cannot find service for link: {0}", link);
                throw new Exception("Cannot find service to download from input site!");
            }
            return service;
        }
    }
}
