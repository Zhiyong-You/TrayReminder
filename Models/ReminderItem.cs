namespace TrayReminder.Models;

public class ReminderItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public DateTime RemindAt { get; set; }
    public bool IsCompleted { get; set; }
}
