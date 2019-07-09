using NLog;
using System;

namespace MangaRipper.Infrastructure
{
    public class NLogLogger<T> : Core.Logging.ILogger
    {
        private static Logger Logger { get; set; }

        public NLogLogger()
        {
            var name = typeof(T).Name;
            Logger = LogManager.GetLogger(name);
        }

        public void Info(string message)
        {
            Logger.Info(message);
        }

        public void Debug(string message)
        {
            Logger.Debug(message);
        }

        public void Fatal(string message)
        {
            Logger.Fatal(message);
        }

        public void Fatal(Exception ex)
        {
            Logger.Fatal(ex);
        }

        public void Error(Exception ex, string message)
        {
            Logger.Error(ex, message);
        }

        public void Error(string message)
        {
            Logger.Error(message);
        }
    }
}
