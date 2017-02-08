using System;

namespace MangaRipper.Core.CustomException
{
    [Serializable]
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
