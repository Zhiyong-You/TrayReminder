using System.Windows.Threading;
using TrayReminder.Models;
using TrayReminder.Views;

namespace TrayReminder.Services;

public class ReminderNotificationService
{
    private readonly ReminderService _reminderService;
    private readonly DispatcherTimer _timer;
    private readonly HashSet<Guid> _showing = [];
    private readonly Dictionary<Guid, DateTime> _dismissedUntil = [];

    public ReminderNotificationService(ReminderService reminderService)
    {
        _reminderService = reminderService;
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
        _timer.Tick += (_, _) => CheckAndNotify();
    }

    public void Start()
    {
        _timer.Start();
        CheckAndNotify();
    }

    public void Stop() => _timer.Stop();

    private void CheckAndNotify()
    {
        var now = DateTime.Now;
        var due = _reminderService.GetAll()
            .Where(r => r.IsEnabled
                     && !r.IsCompleted
                     && r.ReminderTime <= now
                     && (r.SnoozeUntil is null || r.SnoozeUntil <= now)
                     && !_showing.Contains(r.Id)
                     && (!_dismissedUntil.TryGetValue(r.Id, out var until) || until <= now))
            .ToList();

        foreach (var item in due)
        {
            _showing.Add(item.Id);

            var window = new NotificationWindow(item);
            window.Completed += OnCompleted;
            window.Dismissed += OnDismissed;
            window.Closed += (_, _) => _showing.Remove(item.Id);
            window.Show();
        }
    }

    private void OnCompleted(ReminderItem item)
    {
        _reminderService.MarkCompleted(item.Id);
        _dismissedUntil.Remove(item.Id);
    }

    private void OnDismissed(ReminderItem item)
    {
        // 閉じてから5分間は再通知しない
        _dismissedUntil[item.Id] = DateTime.Now.AddMinutes(5);
    }
}
