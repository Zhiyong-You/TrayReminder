using System.Collections.ObjectModel;
using TrayReminder.Models;
using TrayReminder.Services;

namespace TrayReminder.ViewModels;

public class MainViewModel
{
    private readonly ReminderService _service = new();

    public ObservableCollection<ReminderItem> Reminders { get; } = new();

    // 将来: INotifyPropertyChanged, RelayCommand等を導入してバインディングを強化する

    public void AddReminder()
    {
        // 将来: ダイアログ表示・入力受け付けをここで行う
        var item = new ReminderItem
        {
            Title = "（新しいリマインダー）",
            RemindAt = DateTime.Now.AddHours(1),
        };
        _service.Add(item);
        Reminders.Add(item);
    }

    public void SaveReminders()
    {
        // 将来: JSON等への永続化をここで行う
    }
}
