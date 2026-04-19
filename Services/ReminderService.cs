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
    }

    public void Remove(Guid id)
    {
        _items.RemoveAll(x => x.Id == id);
        _storage.Save(_items);
    }

    public void Save() => _storage.Save(_items);

    public void MarkCompleted(Guid id)
    {
        var item = _items.FirstOrDefault(x => x.Id == id);
        if (item is null) return;
        item.IsCompleted = true;
        _storage.Save(_items);
        ItemUpdated?.Invoke(id);
    }

    public event Action<Guid>? ItemUpdated;
}
