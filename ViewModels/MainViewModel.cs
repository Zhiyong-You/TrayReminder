using System.Collections.ObjectModel;
using TrayReminder.Models;
using TrayReminder.Services;

namespace TrayReminder.ViewModels;

public class MainViewModel
{
    private readonly ReminderService _service;

    public ObservableCollection<ReminderItem> Reminders { get; } = new();

    public MainViewModel(ReminderService service)
    {
        _service = service;
        _service.ItemAdded  += item => Reminders.Add(item);
        _service.ItemUpdated += OnItemUpdated;

        foreach (var item in _service.GetAll())
            Reminders.Add(item);
    }

    public void AddReminder(ReminderItem item)
    {
        // ItemAdded イベント経由で Reminders へ追加されるため直接 Add しない
        _service.Add(item);
    }

    public void UpdateReminder(ReminderItem item)
    {
        _service.Save();
        var index = Reminders.IndexOf(item);
        if (index >= 0)
        {
            Reminders.RemoveAt(index);
            Reminders.Insert(index, item);
        }
    }

    public void RemoveReminder(ReminderItem item)
    {
        _service.Remove(item.Id);
        Reminders.Remove(item);
    }

    public void SnoozeReminder(ReminderItem item, TimeSpan duration)
    {
        _service.Snooze(item.Id, duration);
    }

    private void OnItemUpdated(Guid id)
    {
        var item = Reminders.FirstOrDefault(x => x.Id == id);
        if (item is null) return;
        var index = Reminders.IndexOf(item);
        Reminders.RemoveAt(index);
        Reminders.Insert(index, item);
    }
}
