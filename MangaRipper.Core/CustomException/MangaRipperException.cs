using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaRipper.Core.CustomException
{
    class MangaRipperException : Exception
    {
        public MangaRipperException(string message)
        : base(message)
        {
        }

        public MangaRipperException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
