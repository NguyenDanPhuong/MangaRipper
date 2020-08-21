using System;

namespace MangaRipper.Core.Logging
{
    public interface ILogger<T>
    {
        void Info(string message);
        void Debug(string message);
        void Fatal(string message);
        void Fatal(Exception ex);
        void Error(Exception ex, string v);
        void Error(string v);
    }
}
