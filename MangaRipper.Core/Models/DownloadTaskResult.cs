using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaRipper.Core.Models
{
    public class DownloadTaskResult
    {
        public bool Error { get; internal set; }
        public Exception Exception { get; internal set; }
    }
}
