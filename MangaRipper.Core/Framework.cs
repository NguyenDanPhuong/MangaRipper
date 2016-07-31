using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    public class Framework
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static IList<IMangaService> services;
        private static Worker worker;

        public static void Init()
        {
            logger.Info("> Framework.Init()");
            worker = new Worker();
            services = new List<IMangaService>();
            services.Add(new MangaFoxService());
            services.Add(new MangaHereService());
            services.Add(new MangaReaderService());
            services.Add(new MangaShareService());
        }

        public static IEnumerable<IMangaService> GetServices()
        {
            return services.ToList().AsReadOnly();
        }

        public static Worker GetWorker()
        {
            return worker;
        }

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
