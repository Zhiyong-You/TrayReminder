namespace TrayReminder.Models;

public class ReminderItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime ReminderTime { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsEnabled { get; set; } = true;
    public RepeatType RepeatType { get; set; } = RepeatType.None;
    public DateTime? SnoozeUntil { get; set; }
}
