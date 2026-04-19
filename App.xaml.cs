using System.Windows;
using TrayReminder.Services;
using TrayReminder.Views;

namespace TrayReminder;

public partial class App : Application
{
    private ReminderNotificationService? _notificationService;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var reminderService = new ReminderService();
        _notificationService = new ReminderNotificationService(reminderService);

        var mainWindow = new MainWindow(reminderService);
        mainWindow.Show();

        _notificationService.Start();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _notificationService?.Stop();
        base.OnExit(e);
    }
}
