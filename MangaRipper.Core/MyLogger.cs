using NLog;
using System;

namespace MangaRipper.Core
{
    public interface IMyLogger
    {
        void Info(string message);
        void Debug(string message);
        void Fatal(string message);
        void Fatal(Exception ex);
    }

    public class MyLogger<T> : IMyLogger
    {
        private static Logger Logger { get; set; }

        public MyLogger()
        {
            var name = nameof(T);
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
    }
}
