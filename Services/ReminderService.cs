using TrayReminder.Models;

namespace TrayReminder.Services;

// 将来: JSON保存・通知・スヌーズ等のロジックをここに追加する
public class ReminderService
{
    private readonly List<ReminderItem> _items = new();

    public IReadOnlyList<ReminderItem> GetAll() => _items.AsReadOnly();

    public void Add(ReminderItem item) => _items.Add(item);

    public void Remove(Guid id) => _items.RemoveAll(x => x.Id == id);
}
