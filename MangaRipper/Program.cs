using System;
using System.IO;
using System.Windows.Forms;
using MangaRipper.Core.Providers;
using MangaRipper.Forms;
using NLog;
using SimpleInjector;
using MangaRipper.Core.Interfaces;
using System.Linq;
using System.Reflection;
using MangaRipper.Core.Models;
using MangaRipper.Core.Controllers;

namespace MangaRipper
{
    internal static class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static Container container;

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

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Bootstrap();
            Application.Run(container.GetInstance<FormMain>());
            Logger.Info("< Main()");
        }

        private static void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            Logger.Fatal(ex, "Unhandled Exception");
        }

        private static void Bootstrap()
        {
            container = new Container();
            var pluginPath = Path.Combine(Environment.CurrentDirectory, "Plugins");
            var configPath = Path.Combine(Environment.CurrentDirectory, "MangaRipper.Configuration.json");
            var pluginAssemblies = new DirectoryInfo(pluginPath).GetFiles()
                .Where(file => file.Extension.ToLower() == ".dll" && file.Name.StartsWith("MangaRipper.Plugin."))
                .Select(file => Assembly.Load(AssemblyName.GetAssemblyName(file.FullName)));

            container.Register(() => new Configuration(configPath));
            container.RegisterCollection<IMangaService>(pluginAssemblies);
            container.Register<ServiceManager>(Lifestyle.Singleton);
            container.Register<WorkerController>(Lifestyle.Singleton);
            container.Register<FormMain>(Lifestyle.Singleton);
            container.Verify();
        }
    }
}