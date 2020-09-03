using MangaRipper.ChromeDriver;
using MangaRipper.Core;
using MangaRipper.Core.FilenameDetectors;
using MangaRipper.Core.Logging;
using MangaRipper.Core.Outputers;
using MangaRipper.Core.Plugins;
using MangaRipper.Helpers;
using MangaRipper.Infrastructure;
using MangaRipper.Plugin.MangaFox;
using MangaRipper.Plugin.MangaHere;
using MangaRipper.Plugin.MangaReader;
using MangaRipper.Plugin.ReadOPM;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MangaRipper.Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public IConfiguration Configuration { get; private set; }

        private RemoteWebDriver chrome;

        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            chrome?.Quit();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var loading = new LoadingWindow();
            loading.Show();
            var update = new ChromeDriverUpdater(".\\");
            update.ExecuteAsync().GetAwaiter().GetResult();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
            chrome = CreateChromeDriver();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<RemoteWebDriver>(x => chrome);
            serviceCollection.AddSingleton(Configuration);
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            loading.Close();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            chrome?.Quit();
            base.OnExit(e);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(ILogger<>),typeof(NLogLogger<>));
            services.AddSingleton<RemoteWebDriver>(x =>
            {
                var options = new ChromeOptions();
                options.AddArgument("--window-size=1920,1080");
                options.AddArgument("--start-maximized");
                options.AddArgument("--headless");
                var driverService = ChromeDriverService.CreateDefaultService();
                driverService.HideCommandPromptWindow = true;
                chrome = new OpenQA.Selenium.Chrome.ChromeDriver(driverService, options);
                return chrome;
            });
            services.AddSingleton<IWorkerController, WorkerController>();
            services.AddSingleton<IPluginManager, PluginManager>();
            services.AddSingleton<IOutputFactory, OutputFactory>();
            services.AddSingleton<IRetry, Retry>();

            services.AddSingleton<IFilenameDetector, FilenameDetector>();
            services.AddSingleton<IGoogleProxyFilenameDetector, GoogleProxyFilenameDetector>();

            services.AddSingleton<IPlugin, MangaFox>();
            services.AddSingleton<IPlugin, MangaHere>();
            services.AddSingleton<IPlugin, MangaReader>();
            services.AddSingleton<IPlugin, ReadOPM>();

            services.AddSingleton<IHttpDownloader, HttpDownloader>();
            services.AddSingleton<MainWindowDataContext, MainWindowDataContext>();
            services.AddSingleton(typeof(MainWindow));
        }

        private RemoteWebDriver CreateChromeDriver()
        {
            var options = new ChromeOptions();
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--start-maximized");
            options.AddArgument("--headless");
            var driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            return new OpenQA.Selenium.Chrome.ChromeDriver(driverService, options);
        }
    }
}
