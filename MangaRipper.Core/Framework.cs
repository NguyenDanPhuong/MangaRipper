using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    public class Framework
    {
        private static IList<IManga> services;
        private static WorkManager workManger;

        public static void Init()
        {
            workManger = new WorkManager();
            services = new List<IManga>();
            services.Add(new MangaFoxImpl());
            services.Add(new MangaHereImpl());
            services.Add(new MangaReaderImpl());
            services.Add(new MangaShareImpl());
        }

        public static IEnumerable<IManga> GetServices()
        {
            return services.ToList().AsReadOnly();
        }

        public static WorkManager GetWorkManager()
        {
            return workManger;
        }

        public static IManga GetService(string link)
        {
            IManga service = services.FirstOrDefault(s => s.Of(link));
            if(service == null)
            {
                throw new Exception("Cannot find service to download from input site!");
            }
            return service;
        }
    }
}
