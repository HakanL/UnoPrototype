using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using DMXCore.DMXCore100.Views;
using DMXCore.DMXCore100.Contracts;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Windowing;
using System.Timers;
using Microsoft.AspNetCore.Hosting;
using DMXCore.DMXCore100.ViewModels;
using Serilog;
using System.Reactive.Concurrency;
using System.Threading;
using System.Diagnostics;

namespace DMXCore.DMXCore100
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        public const string FileTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {LogContext} [{Level}] {Message}{NewLine}{Exception}";
        public const string TraceTemplate = "{Timestamp:HH:mm:ss.fff} {LogContext} [{Level}] {Message}{NewLine}{Exception}";

        private Window window;
        private ServiceCollection serviceCollection = new ServiceCollection();
        private IoManager ioManager;
        private System.Threading.CancellationTokenSource shutdownEvent = new();

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            SetupSerilog();
            InitializeLogging();

            this.InitializeComponent();



#if HAS_UNO || NETFX_CORE
            this.Suspending += OnSuspending;
#endif
        }

        public IServiceProvider Container { get; private set; }

        public ServiceCollection ServiceCollection => this.serviceCollection;

        public IIoManager IoManager => this.ioManager;

        public Frame RootFrame { get; private set; }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
#if NET6_0_OR_GREATER && WINDOWS && !HAS_UNO
            this.window = new Window();

            // Use 'this' rather than 'window' as variable if this is about the current window.
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this.window);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            var presenter = appWindow.Presenter as OverlappedPresenter;
            appWindow.Resize(new global::Windows.Graphics.SizeInt32 { Width = 800, Height = 480 });
            appWindow.Title = "DMX Core 100";
            presenter.IsResizable = false;
            presenter.IsMaximizable = false;

            this.window.Activate();
#else
            this.window = Microsoft.UI.Xaml.Window.Current;
#endif

            this.window.Closed += (s, e) =>
            {
                this.shutdownEvent.Cancel();
            };

            var splashStopwatch = new Stopwatch();

            //var loadable = new ManualLoadable { IsExecuting = true };
            var splash = new ExtendedSplashScreen
            {
                Window = this.window,
                SplashScreen = args.UWPLaunchActivatedEventArgs.SplashScreen,
//                Source = loadable
            };
            this.window.Content = splash;
            // Ensure the current window is active
            this.window.Activate();

            Log.Logger.Information("Splash displayed");
            splashStopwatch.Start();

            // This is to allow the splash/loading view to start to render, before we finishing building shell etc
            await Task.Yield();

            //loadable.IsExecuting = false;

            this.ioManager = new IoManager();

            try
            {
                var host = new WebHostBuilder()
                    .UseKestrel()
    #if WINDOWS
                    .UseUrls("http://*:5000")
    #else
                    .UseUrls("http://*:80")
    #endif
                    .UseStartup<WebStartup>()
                    .ConfigureServices(services =>
                    {
                        services.AddTransient<IMessageService, MessageService>();

                        //services.AddSingleton<HomePage>();
                        //services.AddSingleton<MainPage>();
                        //services.AddSingleton<AboutPage>();

                        services.AddSingleton<MainViewModel>();
                        services.AddSingleton<HomeViewModel>();
                        services.AddSingleton<AboutViewModel>();

                        services.AddSingleton<Contracts.IIoManager>(this.ioManager);
                    })
                    .ConfigureLogging(config =>
                    {
                        config.AddSerilog(dispose: true);
                    })
                    .Build();

                Container = host.Services;

                _ = Task.Run(() => host.RunAsync(this.shutdownEvent.Token));
            }
            catch (Exception ex)
            {
                Log.Logger.Fatal(ex, "Failed to start web server: {Message}", ex.Message);

                throw;
            }

            // Create a Frame to act as the navigation context and navigate to the first page
            RootFrame = new Frame();

            RootFrame.NavigationFailed += OnNavigationFailed;

            Log.Logger.Information("Application ready");

#if DEBUG
            // Display the splash for at least 5 seconds
            var splashDurationLeft = TimeSpan.FromSeconds(5) - splashStopwatch.Elapsed;
            if (splashDurationLeft.TotalSeconds > 0)
                await Task.Delay(splashDurationLeft);
#endif

            // Place the frame in the current Window
            this.window.Content = RootFrame;

#if !(NET6_0_OR_GREATER && WINDOWS)
            if (args.UWPLaunchActivatedEventArgs.PrelaunchActivated == false)
#endif
            {
                if (RootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    RootFrame.Navigate(typeof(HomePage), args.Arguments);
                }

                // Ensure the current window is active
                this.window.Activate();
            }

            Log.Logger.Information("Main content displayed");
        }

        private class ManualLoadable : Uno.Toolkit.ILoadable
        {
            private bool isExecuting;

            public bool IsExecuting
            {
                get => isExecuting;
                set
                {
                    isExecuting = value;
                    IsExecutingChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            public event EventHandler IsExecutingChanged;
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new InvalidOperationException($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private void SetupSerilog()
        {
            string logFolder = Path.Combine(Path.GetTempPath(), "DMXCore100Logs");
            var file = Path.Combine(logFolder, "DMXCore100-.log");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
#if DEBUG
                .WriteTo.Async(a => a.Debug(outputTemplate: TraceTemplate))
#endif
                .WriteTo.Async(a => a.Console(outputTemplate: TraceTemplate))
                .WriteTo.File(file, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 22, outputTemplate: FileTemplate)
                .CreateLogger();

            string appVersion = this.GetType().Assembly.GetName().Version.ToString();

            Log.Logger.Information("Starting up version {AppVersion}", appVersion);
        }

        /// <summary>
        /// Configures global Uno Platform logging
        /// </summary>
        private static void InitializeLogging()
        {
#if HAS_UNO_SKIA
            Uno.UI.FeatureConfiguration.ApiInformation.NotImplementedLogLevel = Uno.Foundation.Logging.LogLevel.Debug; // Raise not implemented usages as Debug messages
#endif

            // Logging is disabled by default for release builds, as it incurs a significant
            // initialization cost from Microsoft.Extensions.Logging setup. If startup performance
            // is a concern for your application, keep this disabled. If you're running on web or 
            // desktop targets, you can use url or command line parameters to enable it.
            //
            // For more performance documentation: https://platform.uno/docs/articles/Uno-UI-Performance.html

            var factory = LoggerFactory.Create(builder =>
            {
#if __WASM__
                builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__
                builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
#elif NETFX_CORE
                //builder.AddDebug();
#else
                //builder.AddConsole();
#endif

                // Exclude logs below this level
                builder.SetMinimumLevel(LogLevel.Trace);

                // Default filters for Uno Platform namespaces
                builder.AddFilter("Uno", LogLevel.Warning);
                builder.AddFilter("Windows", LogLevel.Warning);
                builder.AddFilter("Microsoft", LogLevel.Warning);

                // Generic Xaml events
                // builder.AddFilter("Windows.UI.Xaml", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.VisualStateGroup", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.StateTriggerBase", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.UIElement", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.FrameworkElement", LogLevel.Trace );

                // Layouter specific messages
                // builder.AddFilter("Windows.UI.Xaml.Controls", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.Controls.Layouter", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.Controls.Panel", LogLevel.Debug );

                // builder.AddFilter("Windows.Storage", LogLevel.Debug );

                // Binding related messages
                // builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug );

                // Binder memory references tracking
                // builder.AddFilter("Uno.UI.DataBinding.BinderReferenceHolder", LogLevel.Debug );

                // RemoteControl and HotReload related
                // builder.AddFilter("Uno.UI.RemoteControl", LogLevel.Information);

                // Debug JS interop
                // builder.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug );
            });

            factory.AddSerilog(dispose: true);

            global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;

#if HAS_UNO
            global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
#endif
        }
    }
}
