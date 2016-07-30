using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaRipper.Core
{
    public class Framework
    {
        private static IList<IManga> services = new List<IManga>();

        public static void Init()
        {
            Register(new MangaFoxImpl());
        }

        public static void Register(IManga manga)
        {
            services.Add(manga);
        }

        public static IEnumerable<IManga> GetServices()
        {
            return services.ToList().AsReadOnly();
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
