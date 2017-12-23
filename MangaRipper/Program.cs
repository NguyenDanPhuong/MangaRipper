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
using MangaRipper.Core;
using MangaRipper.Infrastructure;
using SimpleInjector.Lifestyles;

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
            container.RegisterConditional(typeof(Core.ILogger),
               c => typeof(NLogLogger<>).MakeGenericType(c.Consumer.ImplementationType),
               Lifestyle.Transient,
               c => true
               );

            var configPath = Path.Combine(Environment.CurrentDirectory, "MangaRipper.Configuration.json");
            container.Register(() => new Configuration(configPath));
            container.Register<IScriptEngine, JurassicScriptEngine>();

            var pluginPath = Path.Combine(Environment.CurrentDirectory, "Plugins");
            var pluginAssemblies = new DirectoryInfo(pluginPath).GetFiles()
                .Where(file => file.Extension.ToLower() == ".dll" && file.Name.StartsWith("MangaRipper.Plugin."))
                .Select(file => Assembly.Load(AssemblyName.GetAssemblyName(file.FullName)));

            container.RegisterCollection<IMangaService>(pluginAssemblies);
            container.Register<FormMain>();
            //container.Verify();
        }
    }
}