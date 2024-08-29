using Bluegrams.Application;
using CefSharp;
using CefSharp.Wpf;
using FlyffUAutoFSPro;
using FlyffUAutoFSPro._Script;
using FlyffUAutoFSPro._Script.Bot;
using FlyffUAutoFSPro.AppSettings;
using Sentry;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FlyffUniverseFarmBot
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        DispatcherTimer timer = new DispatcherTimer();
        private readonly IDisposable _sentry;
        private readonly BotController _botController = new BotController();

        protected override void OnStartup(StartupEventArgs e)
        {
            LogService.Initalize();
            LogService.Log("=============  Started Logging  =============");

            //Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            SentrySdk.Init(o =>
            {
                // Tells which project in Sentry to send events to:
                o.Dsn = "https://e7578385338a457fa35c860f2575860c@o1329582.ingest.sentry.io/4504113838096384";

                // Set traces_sample_rate to 1.0 to capture 100% of transactions for performance monitoring.
                // We recommend adjusting this value in production.
                o.TracesSampleRate = 1.0;

                // Enable Global Mode since this is a client app
                o.IsGlobalModeEnabled = true;

                o.Release = GlobalValues.VersionName;
            });

            SetupExceptionHandling();

            PortableSettingsProvider.SettingsFileName = "settings.config";
            PortableSettingsProvider.SettingsDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\FlyffUAutoFSPro";
            PortableSettingsProvider.ApplyProvider(Settings.Default);

            CefSettings _browserSettings = new CefSettings();
            _browserSettings.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.110 Safari/537.36 /CefSharp Browser" + Cef.CefSharpVersion;
            _browserSettings.CachePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\FlyffUAutoFSPro\\Cache";
            _browserSettings.LogSeverity = LogSeverity.Disable;
            _browserSettings.CefCommandLineArgs.Add("enable-gpu");
            _browserSettings.CefCommandLineArgs.Add("off-screen-rendering-enabled");
            _browserSettings.CefCommandLineArgs.Add("disable-gpu-compositing");

            Cef.Initialize(_browserSettings);
            Cef.EnableHighDPISupport();
          
            base.OnStartup(e);
            _botController.Startup();
        }

        
        private void SetupExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                SentrySdk.CaptureException((Exception)e.ExceptionObject);
            };
           
            DispatcherUnhandledException += (s, e) =>
            {
                SentrySdk.CaptureException(e.Exception);
                e.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                SentrySdk.CaptureException(e.Exception);
                e.SetObserved();
            };
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _botController.Dispose();
        }
    }
}
