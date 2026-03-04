using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using Microsoft.Extensions.Logging;
using Shared.Services;
using Shared.Utils;

namespace ManagementApp
{
    public partial class App : Application
    {
        private static readonly ILogger<App> Logger = LoggingService.CreateLogger<App>();

        private static string? _sharedDataDirectory;
        private static FileSystemWatcher? _languageConfigWatcher;
        private static readonly object _watcherLock = new object();

        private static int _shutdownRequested;

        internal static string? SharedDataDirectory => _sharedDataDirectory;

        protected override void OnStartup(StartupEventArgs e)
        {
            Views.SplashWindow? splash = null;

            try
            {
                ShutdownMode = ShutdownMode.OnMainWindowClose;

                SetupGeorgianCulture();

                LoggingService.ConfigureLogging();
                Logger.LogInformation("Starting Management Application");

                AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
                DispatcherUnhandledException += OnDispatcherUnhandledException;

                LoadLocalizedResources();

                base.OnStartup(e);

                splash = new Views.SplashWindow();
                splash.Show();

                Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Render);

                var mainWindow = new Views.MainWindow();
                MainWindow = mainWindow;

                mainWindow.Closed += (_, __) =>
                {
                    RequestHardShutdown(0);
                };

                mainWindow.Show();
                mainWindow.Activate();
                mainWindow.Focus();

                ApplyFlowDirection();

                StartLanguageConfigWatcher();

                Logger.LogInformation("Management Application started successfully");
            }
            catch (Exception ex)
            {
                try { splash?.Close(); } catch { }

                try { Logger.LogError(ex, "Failed to start Management Application"); } catch { }

                MessageBox.Show(
                    $"Error starting application:\n\n{ex.Message}\n\nDetails:\n{ex}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                RequestHardShutdown(1);
            }
            finally
            {
                try { splash?.Close(); } catch { }
            }
        }

        private static void SetupGeorgianCulture()
        {
            try
            {
                var culture = new CultureInfo("en-US");
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
            catch
            {
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                Logger.LogInformation("Shutting down Management Application");
            }
            catch
            {
            }

            try
            {
                Interlocked.Exchange(ref _shutdownRequested, 1);

                try { StopLanguageConfigWatcher(); } catch { }

                try { AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException; } catch { }
                try { Current.DispatcherUnhandledException -= OnDispatcherUnhandledException; } catch { }
            }
            catch
            {
            }
            finally
            {
                base.OnExit(e);

                try
                {
                    Environment.Exit(e.ApplicationExitCode);
                }
                catch
                {
                }
            }
        }

        private static void RequestHardShutdown(int exitCode)
        {
            if (Interlocked.Exchange(ref _shutdownRequested, 1) == 1)
                return;

            try { StopLanguageConfigWatcher(); } catch { }

            try { AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException; } catch { }

            try
            {
                var app = Current;
                if (app != null)
                {
                    try { app.DispatcherUnhandledException -= ((App)app).OnDispatcherUnhandledException; } catch { }

                    try { app.Shutdown(exitCode); } catch { }
                }
            }
            catch
            {
            }

            try
            {
                Environment.Exit(exitCode);
            }
            catch
            {
            }
        }

        private static void LoadLocalizedResources()
        {
            try
            {
                _sharedDataDirectory = LanguageConfigHelper.GetSharedDataDirectory();
                if (string.IsNullOrEmpty(_sharedDataDirectory))
                {
                    Logger.LogWarning("SharedData directory not found");
                    return;
                }

                var language = LanguageConfigHelper.GetCurrentLanguage(_sharedDataDirectory);
                ResourceManager.LoadResourcesForLanguageWithOverrides(_sharedDataDirectory, language);
                ResourceBridge.Instance.CurrentLanguage = language;

                Logger.LogInformation("Loaded resources for language: {Language} from: {Dir}", language, _sharedDataDirectory);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to load localized resources");
            }
        }

        internal static void ApplyFlowDirection()
        {
            if (Interlocked.CompareExchange(ref _shutdownRequested, 0, 0) == 1)
                return;

            var app = Current;
            if (app == null)
                return;

            var dispatcher = app.Dispatcher;
            if (dispatcher == null || dispatcher.HasShutdownStarted || dispatcher.HasShutdownFinished)
                return;

            var lang = ResourceBridge.Instance.CurrentLanguage;
            var flow = lang == LanguageConfigHelper.LanguageFa ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            dispatcher.BeginInvoke(new Action(() =>
            {
                var a = Current;
                if (a == null)
                    return;

                var mw = a.MainWindow;
                if (mw != null)
                    mw.FlowDirection = flow;
            }), System.Windows.Threading.DispatcherPriority.Normal);
        }

        private static void StartLanguageConfigWatcher()
        {
            if (Interlocked.CompareExchange(ref _shutdownRequested, 0, 0) == 1)
                return;

            if (string.IsNullOrEmpty(_sharedDataDirectory))
                return;

            var configPath = LanguageConfigHelper.GetLanguageConfigPath(_sharedDataDirectory);
            if (string.IsNullOrEmpty(configPath))
                return;

            var configDir = Path.GetDirectoryName(configPath);
            var configFileName = Path.GetFileName(configPath);
            if (string.IsNullOrEmpty(configDir) || string.IsNullOrEmpty(configFileName))
                return;

            try
            {
                lock (_watcherLock)
                {
                    StopLanguageConfigWatcher();

                    var watcher = new FileSystemWatcher(configDir, configFileName)
                    {
                        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.Size,
                        IncludeSubdirectories = false,
                        EnableRaisingEvents = false
                    };

                    watcher.Changed += OnLanguageConfigChanged;
                    watcher.Created += OnLanguageConfigChanged;
                    watcher.Deleted += OnLanguageConfigChanged;
                    watcher.Renamed += OnLanguageConfigRenamed;
                    watcher.Error += OnLanguageConfigWatcherError;

                    watcher.EnableRaisingEvents = true;
                    _languageConfigWatcher = watcher;
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Could not start language config watcher");
            }
        }

        private static void StopLanguageConfigWatcher()
        {
            lock (_watcherLock)
            {
                var watcher = _languageConfigWatcher;
                if (watcher == null)
                    return;

                try { watcher.EnableRaisingEvents = false; } catch { }

                try { watcher.Changed -= OnLanguageConfigChanged; } catch { }
                try { watcher.Created -= OnLanguageConfigChanged; } catch { }
                try { watcher.Deleted -= OnLanguageConfigChanged; } catch { }
                try { watcher.Renamed -= OnLanguageConfigRenamed; } catch { }
                try { watcher.Error -= OnLanguageConfigWatcherError; } catch { }

                try { watcher.Dispose(); } catch { }

                _languageConfigWatcher = null;
            }
        }

        private static void OnLanguageConfigRenamed(object sender, RenamedEventArgs e)
        {
            OnLanguageConfigChanged(sender, e);
        }

        private static void OnLanguageConfigWatcherError(object sender, ErrorEventArgs e)
        {
            try
            {
                Logger.LogWarning(e.GetException(), "Language config watcher error");
            }
            catch
            {
            }
        }

        private static void OnLanguageConfigChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (Interlocked.CompareExchange(ref _shutdownRequested, 0, 0) == 1)
                    return;

                if (string.IsNullOrEmpty(_sharedDataDirectory))
                    return;

                Thread.Sleep(50);

                var newLang = LanguageConfigHelper.GetCurrentLanguage(_sharedDataDirectory);
                if (newLang == ResourceBridge.Instance.CurrentLanguage)
                    return;

                ResourceManager.LoadResourcesForLanguage(_sharedDataDirectory, newLang);

                var app = Current;
                if (app == null)
                    return;

                var dispatcher = app.Dispatcher;
                if (dispatcher == null || dispatcher.HasShutdownStarted || dispatcher.HasShutdownFinished)
                    return;

                dispatcher.BeginInvoke(new Action(() =>
                {
                    if (Interlocked.CompareExchange(ref _shutdownRequested, 0, 0) == 1)
                        return;

                    ResourceBridge.Instance.CurrentLanguage = newLang;
                    ResourceBridge.Instance.NotifyLanguageChanged();
                    ApplyFlowDirection();
                }), System.Windows.Threading.DispatcherPriority.Normal);
            }
            catch (Exception ex)
            {
                try { Logger.LogWarning(ex, "Error applying language change from file"); } catch { }
            }
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var exception = e.ExceptionObject as Exception;
                Logger.LogError(exception, "Unhandled exception occurred");

                var errorMessage = $"Unexpected error:\n\n{exception?.Message}\n\nDetails:\n{exception}";
                MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch
            {
                MessageBox.Show("An unexpected error occurred", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                Logger.LogError(e.Exception, "Dispatcher unhandled exception occurred");

                var errorMessage = $"UI error:\n\n{e.Exception.Message}\n\nDetails:\n{e.Exception}";
                MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                e.Handled = true;
            }
            catch
            {
                MessageBox.Show("A UI error occurred", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = true;
            }
        }
    }
}
