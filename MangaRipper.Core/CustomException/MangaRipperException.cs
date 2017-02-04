using System;

namespace MangaRipper.Core.CustomException
{
    public class MangaRipperException : Exception
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
