using TrayReminder.Models;

namespace TrayReminder.Services;

public class ReminderService
{
    private readonly List<ReminderItem> _items;
    private readonly ReminderStorageService _storage = new();

    public ReminderService()
    {
        _items = _storage.Load();
    }

    public IReadOnlyList<ReminderItem> GetAll() => _items.AsReadOnly();

    public void Add(ReminderItem item)
    {
        _items.Add(item);
        _storage.Save(_items);
        ItemAdded?.Invoke(item);
    }

    public event Action<ReminderItem>? ItemAdded;

    public void Remove(Guid id)
    {
        _items.RemoveAll(x => x.Id == id);
        _storage.Save(_items);
    }

    public void Save() => _storage.Save(_items);

    public void Snooze(Guid id, TimeSpan duration)
    {
        var item = _items.FirstOrDefault(x => x.Id == id);
        if (item is null) return;
        item.SnoozeUntil = DateTime.Now + duration;
        _storage.Save(_items);
        ItemUpdated?.Invoke(id);
    }

    public void MarkCompleted(Guid id)
    {
        var item = _items.FirstOrDefault(x => x.Id == id);
        if (item is null) return;

        if (item.RepeatType == RepeatType.None)
        {
            item.IsCompleted = true;
        }
        else
        {
            // 繰り返しリマインダーは次回時刻へ進め、再び通知対象にする
            item.ReminderTime = RepeatScheduler.NextOccurrence(item.ReminderTime, item.RepeatType);
            item.IsCompleted = false;
        }

        item.SnoozeUntil = null;
        _storage.Save(_items);
        ItemUpdated?.Invoke(id);
    }

    public event Action<Guid>? ItemUpdated;
}
