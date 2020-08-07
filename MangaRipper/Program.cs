using System;
using System.IO;
using System.Windows.Forms;
using MangaRipper.Forms;
using NLog;
using SimpleInjector;
using System.Linq;
using System.Reflection;
using MangaRipper.Infrastructure;
using MangaRipper.Core.Outputers;
using MangaRipper.Core.FilenameDetectors;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using MangaRipper.Core.Plugins;
using MangaRipper.Core;
using MangaRipper.Tools.ChromeDriver;
using MangaRipper.Helpers;

namespace MangaRipper
{
    internal static class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static Container container;
        static ChromeDriver driver;
        static Timer splashTimer;
        static SplashScreen splash;
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
            splashTimer = new Timer
            {
                Interval = 500
            };
            splashTimer.Tick += SplashTimer_TickAsync;
            splashTimer.Start();
            splash = new SplashScreen();
            splash.ShowDialog();
            Application.Run(container.GetInstance<FormMain>());

            driver?.Close();
            driver?.Quit();
            Logger.Info("< Main()");
        }

        private static async void SplashTimer_TickAsync(object sender, EventArgs e)
        {
            if(driver == null && splash.Visible)
            {
                splashTimer.Stop();
                var update = new ChromeDriverUpdater(".\\");
                await update.ExecuteAsync();
                Bootstrap();
                splash.Close();
            }
        }

        private static void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            Logger.Fatal(ex, "Unhandled Exception");
            driver?.Close();
            driver?.Quit();
        }

        private static void Bootstrap()
        {
            InitChromeDriver();

            container = new Container();

            container.RegisterConditional(typeof(Core.Logging.ILogger),
               c => typeof(NLogLogger<>).MakeGenericType(c.Consumer.ImplementationType),
               Lifestyle.Singleton,
               c => true
               );

            container.Register<RemoteWebDriver>(() => driver, Lifestyle.Singleton);

            container.Register<IWorkerController, WorkerController>(Lifestyle.Singleton);
            container.Register<IPluginManager, PluginManager>(Lifestyle.Singleton);
            container.Register<IOutputFactory, OutputFactory>(Lifestyle.Singleton);

            var configPath = Path.Combine(Environment.CurrentDirectory, "MangaRipper.Configuration.json");
            container.Register<IHttpDownloader, HttpDownloader>(Lifestyle.Singleton);
            container.Register<IRetry, Retry>(Lifestyle.Singleton);

            container.Register<IFilenameDetector, FilenameDetector>(Lifestyle.Singleton);
            container.Register<IGoogleProxyFilenameDetector, GoogleProxyFilenameDetector>(Lifestyle.Singleton);

            container.Register<ApplicationConfiguration>(Lifestyle.Singleton);

            var pluginPath = Path.Combine(Environment.CurrentDirectory, "Plugins");
            var pluginAssemblies = new DirectoryInfo(pluginPath).GetFiles()
                .Where(file => file.Extension.ToLower() == ".dll" && file.Name.StartsWith("MangaRipper.Plugin."))
                .Select(file => Assembly.Load(AssemblyName.GetAssemblyName(file.FullName)));

            container.Collection.Register<IPlugin>(pluginAssemblies);

            container.RegisterDecorator<IHttpDownloader, DownloadLogging>(Lifestyle.Singleton);
            container.Register<FormMain>(Lifestyle.Singleton);
            //container.Verify();
        }

        private static void InitChromeDriver()
        {
            var options = new ChromeOptions();
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--start-maximized");
            options.AddArgument("--headless");
            var driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            driver = new ChromeDriver(driverService, options);
        }
    }
}