using System.Collections.ObjectModel;
using TrayReminder.Models;
using TrayReminder.Services;

namespace TrayReminder.ViewModels;

public class MainViewModel
{
    private readonly ReminderService _service = new();

    public ObservableCollection<ReminderItem> Reminders { get; } = new();

    public MainViewModel()
    {
        foreach (var item in _service.GetAll())
            Reminders.Add(item);
    }

    public void AddReminder(ReminderItem item)
    {
        _service.Add(item);
        Reminders.Add(item);
    }

    public void UpdateReminder(ReminderItem item)
    {
        _service.Save();
        // ObservableCollection は同一オブジェクトの内部変更を検知しないため再挿入で更新
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
}
