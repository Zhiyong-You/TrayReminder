using System.Windows;
using TrayReminder.Services;
using TrayReminder.Views;
using WpfApplication = System.Windows.Application;

namespace TrayReminder;

public partial class App : WpfApplication
{
    private ReminderService? _reminderService;
    private ReminderNotificationService? _notificationService;
    private TrayIconService? _trayIconService;
    private QuickAddWindow? _quickAddWindow;

    public bool IsExiting { get; private set; }

    public static new App Current => (App)WpfApplication.Current;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        _reminderService = new ReminderService();
        _notificationService = new ReminderNotificationService(_reminderService);

        var mainWindow = new MainWindow(_reminderService);
        _trayIconService = new TrayIconService(mainWindow, OpenQuickAdd, ExitApp, _reminderService);
        mainWindow.Show();

        _notificationService.Start();
    }

    private void OpenQuickAdd()
    {
        // すでに開いている場合は前面に出すだけ
        if (_quickAddWindow is not null)
        {
            _quickAddWindow.Activate();
            return;
        }

        _quickAddWindow = new QuickAddWindow(_reminderService!);
        _quickAddWindow.Closed += (_, _) => _quickAddWindow = null;
        _quickAddWindow.Show();
    }

    public void ExitApp()
    {
        IsExiting = true;
        _notificationService?.Stop();
        _trayIconService?.Dispose();
        Shutdown();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _notificationService?.Stop();
        _trayIconService?.Dispose();
        base.OnExit(e);
    }
}
