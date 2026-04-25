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
    // 表示中ウィンドウの参照（削除時のクローズに使用）
    private readonly Dictionary<Guid, NotificationWindow> _openWindowMap = [];

    public ReminderNotificationService(ReminderService reminderService)
    {
        _reminderService = reminderService;
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
        _timer.Tick += (_, _) => CheckAndNotify();
        _reminderService.ItemRemoved += OnItemRemoved;
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
            _openWindowMap[item.Id] = window;
            window.Completed += OnCompleted;
            window.Dismissed += OnDismissed;
            window.Snoozed  += OnSnoozed;
            window.Closed += (_, _) =>
            {
                _showing.Remove(item.Id);
                _openWindowMap.Remove(item.Id);
            };
            window.Show();
        }
    }

    // Reminder 削除時：実行時状態をクリーンアップし、表示中なら安全にウィンドウを閉じる
    private void OnItemRemoved(Guid id)
    {
        _showing.Remove(id);
        _dismissedUntil.Remove(id);

        if (_openWindowMap.TryGetValue(id, out var window))
        {
            // Closed イベント内の Remove が再実行されても Dictionary/HashSet は安全なので問題なし
            _openWindowMap.Remove(id);
            window.Close();
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

    private void OnSnoozed(ReminderItem item, TimeSpan duration)
    {
        _reminderService.Snooze(item.Id, duration);
        // スヌーズは SnoozeUntil で管理するため _dismissedUntil は不要
        _dismissedUntil.Remove(item.Id);
    }
}
