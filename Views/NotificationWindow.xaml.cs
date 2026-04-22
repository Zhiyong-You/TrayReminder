using System.Windows;
using TrayReminder.Models;

namespace TrayReminder.Views;

public partial class NotificationWindow : Window
{
    public event Action<ReminderItem>? Completed;
    public event Action<ReminderItem>? Dismissed;
    public event Action<ReminderItem, TimeSpan>? Snoozed;

    private readonly ReminderItem _item;
    private bool _resultHandled;

    public NotificationWindow(ReminderItem item)
    {
        InitializeComponent();
        _item = item;
        TitleText.Text = item.Title;
        TimeText.Text = item.ReminderTime.ToString("yyyy/MM/dd HH:mm");
        if (string.IsNullOrWhiteSpace(item.Description))
        {
            DescriptionText.Text = "メモなし";
            DescriptionText.Foreground = (System.Windows.Media.Brush)FindResource("DialogLabelFg");
        }
        else
        {
            DescriptionText.Text = item.Description;
        }
    }

    private void CompleteButton_Click(object sender, RoutedEventArgs e)
    {
        _resultHandled = true;
        Completed?.Invoke(_item);
        Close();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        _resultHandled = true;
        Dismissed?.Invoke(_item);
        Close();
    }

    private void Snooze5Button_Click(object sender, RoutedEventArgs e)
        => DoSnooze(TimeSpan.FromMinutes(5));

    private void Snooze10Button_Click(object sender, RoutedEventArgs e)
        => DoSnooze(TimeSpan.FromMinutes(10));

    private void DoSnooze(TimeSpan duration)
    {
        _resultHandled = true;
        Snoozed?.Invoke(_item, duration);
        Close();
    }

    protected override void OnClosed(EventArgs e)
    {
        // ×ボタンで閉じた場合も dismiss として扱う
        if (!_resultHandled)
            Dismissed?.Invoke(_item);
        base.OnClosed(e);
    }
}
