using System;
using System.IO;
using System.Windows.Forms;
using MangaRipper.Core.Providers;
using NLog;

namespace MangaRipper
{
    internal static class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Logger.Info("> Main()");
            var appDomain = AppDomain.CurrentDomain;
            appDomain.UnhandledException += AppDomain_UnhandledException;
            FrameworkProvider.Init(Path.Combine(Environment.CurrentDirectory, "Plugins"),
                Path.Combine(Environment.CurrentDirectory, "MangaRipper.Configuration.json"));
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
            Logger.Info("< Main()");
        }

        private static void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception) e.ExceptionObject;
            Logger.Fatal(ex, "Unhandled Exception");
        }
    }
}