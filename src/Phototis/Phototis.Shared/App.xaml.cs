﻿using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;

namespace Phototis
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        #region Fields

        private static Window _window;
        private SystemNavigationManager _systemNavigationManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeLogging();

            InitializeComponent();

            //Uno.UI.ApplicationHelper.RequestedCustomTheme = "Dark";

#if HAS_UNO || NETFX_CORE
            Suspending += OnSuspending;
#endif
            UnhandledException += App_UnhandledException;

            Uno.UI.FeatureConfiguration.Page.IsPoolingEnabled = true;

            _systemNavigationManager = SystemNavigationManager.GetForCurrentView();
        }

        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
#if DEBUG
            Console.WriteLine(e.Message);
#endif
            e.Handled = true;
        }

        #endregion

        #region Properties

        public static Account Account { get; set; }

        public static bool IsFullScreenToggled { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

#if NET6_0_OR_GREATER && WINDOWS && !HAS_UNO
            _window = new Window();
            _window.Activate();
#else
            _window = Microsoft.UI.Xaml.Window.Current;
#endif

            var rootFrame = _window.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;
                rootFrame.IsNavigationStackEnabled = true;

                if (args.UWPLaunchActivatedEventArgs.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                _window.Content = rootFrame;
            }

#if !(NET6_0_OR_GREATER && WINDOWS)
            if (args.UWPLaunchActivatedEventArgs.PrelaunchActivated == false)
#endif
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(StartPage), args.Arguments);
                }
                // Ensure the current window is active
                _window.Activate();
            }

            _systemNavigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            _systemNavigationManager.BackRequested += OnBackRequested;
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            var rootFrame = _window.Content as Frame;

            if (rootFrame.CanGoBack)
                rootFrame.GoBack();
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

        #endregion

        #region Methods

        /// <summary>
        /// Configures global Uno Platform logging
        /// </summary>
        private static void InitializeLogging()
        {
#if DEBUG
            // Logging is disabled by default for release builds, as it incurs a significant
            // initialization cost from Microsoft.Extensions.Logging setup. If startup performance
            // is a concern for your application, keep this disabled. If you're running on web or 
            // desktop targets, you can use url or command line parameters to enable it.
            //
            // For more performance documentation: https://platform.uno/docs/articles/Uno-UI-Performance.html

            var factory = LoggerFactory.Create(builder =>
            {
#if __WASM__
                builder.AddProvider(new Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__
                builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
#elif NETFX_CORE
                builder.AddDebug();
#else
                builder.AddConsole();
#endif
                // Exclude logs below this level
                builder.SetMinimumLevel(LogLevel.Information);

                // Default filters for Uno Platform namespaces
                builder.AddFilter("Uno", LogLevel.Warning);
                builder.AddFilter("Windows", LogLevel.Warning);
                builder.AddFilter("Microsoft", LogLevel.Warning);


                // Generic Xaml events
                //builder.AddFilter("Microsoft.UI.Xaml", LogLevel.Debug);
                //builder.AddFilter("Microsoft.UI.Xaml.VisualStateGroup", LogLevel.Debug);
                //builder.AddFilter("Microsoft.UI.Xaml.StateTriggerBase", LogLevel.Debug);
                //builder.AddFilter("Microsoft.UI.Xaml.UIElement", LogLevel.Debug);
                //builder.AddFilter("Microsoft.UI.Xaml.FrameworkElement", LogLevel.Trace);

                // Layouter specific messages
                //builder.AddFilter("Microsoft.UI.Xaml.Controls", LogLevel.Debug);
                //builder.AddFilter("Microsoft.UI.Xaml.Controls.Layouter", LogLevel.Debug);
                //builder.AddFilter("Microsoft.UI.Xaml.Controls.Panel", LogLevel.Debug);

                builder.AddFilter("Windows.Storage", LogLevel.Debug);

                // Binding related messages
                //builder.AddFilter("Microsoft.UI.Xaml.Data", LogLevel.Debug);
                //builder.AddFilter("Microsoft.UI.Xaml.Data", LogLevel.Debug);

                // Binder memory references tracking
                builder.AddFilter("Uno.UI.DataBinding.BinderReferenceHolder", LogLevel.Debug);

                // RemoteControl and HotReload related
                builder.AddFilter("Uno.UI.RemoteControl", LogLevel.Information);

                // Debug JS interop
                builder.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug);
            });

            Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;

#if HAS_UNO
            Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
#endif

#endif
        }

        public static void SetIsBusy(bool isBusy)
        {
            //_mainPage.SetIsBusy(isBusy, message);

            var rootFrame = _window.Content as Frame;
            rootFrame.IsEnabled = !isBusy;
            rootFrame.Opacity = isBusy ? 0.6 : 1;
        }

        public static void EnterFullScreen(bool value)
        {
            var view = ApplicationView.GetForCurrentView();

            if (view is not null)
            {
                if (value)
                {
                    view.TryEnterFullScreenMode();
                    IsFullScreenToggled = true;
                }
                else
                {
                    view.ExitFullScreenMode();
                    IsFullScreenToggled = false;
                }
            }
        }

        public static void SetAccount()
        {
            //_mainPage.SetAccount();
        }

        public static void NavigateToPage(Type page, object parameter = null)
        {
            var rootFrame = _window.Content as Frame;
            rootFrame.Navigate(page, parameter);
        }

        #endregion
    }
}
