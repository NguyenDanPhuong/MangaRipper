using System;
using System.IO;
using System.Windows.Forms;
using MangaRipper.Core.Providers;
using MangaRipper.Forms;
using MangaRipper.Presenters;
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
            // TODO Apply Composition Root to use DI for WinForm
            // So we can unit test.
            Logger.Info("> Main()");
            var appDomain = AppDomain.CurrentDomain;
            appDomain.UnhandledException += AppDomain_UnhandledException;
            Framework.Init(Path.Combine(Environment.CurrentDirectory, "Plugins"),
                Path.Combine(Environment.CurrentDirectory, "MangaRipper.Configuration.json"));
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var form = new FormMain();
            Application.Run(form);
            Logger.Info("< Main()");
        }

        private static void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception) e.ExceptionObject;
            Logger.Fatal(ex, "Unhandled Exception");
        }
    }
}