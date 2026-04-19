using System.Windows;
using TrayReminder.Services;
using TrayReminder.Views;
using WpfApplication = System.Windows.Application;

namespace TrayReminder;

public partial class App : WpfApplication
{
    private ReminderNotificationService? _notificationService;
    private TrayIconService? _trayIconService;

    public bool IsExiting { get; private set; }

    // Application.Current を App 型で返す補助プロパティ
    public static new App Current => (App)WpfApplication.Current;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // ウィンドウを閉じてもアプリが終了しないようにする
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        var reminderService = new ReminderService();
        _notificationService = new ReminderNotificationService(reminderService);

        var mainWindow = new MainWindow(reminderService);
        _trayIconService = new TrayIconService(mainWindow, ExitApp);
        mainWindow.Show();

        _notificationService.Start();
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
        // ExitApp 経由でない直接 Shutdown の場合もクリーンアップ
        _notificationService?.Stop();
        _trayIconService?.Dispose();
        base.OnExit(e);
    }
}
