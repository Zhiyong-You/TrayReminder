using System.Windows;
using System.Windows.Threading;
using TrayReminder.Models;

namespace TrayReminder.Views;

public partial class NotificationWindow : Window
{
    public event Action<ReminderItem>? Completed;
    public event Action<ReminderItem>? Dismissed;
    public event Action<ReminderItem, TimeSpan>? Snoozed;

    private readonly ReminderItem _item;
    private readonly DispatcherTimer _autoCloseTimer;
    private bool _resultHandled;

    private const int AutoCloseSeconds = 30;

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

        _autoCloseTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(AutoCloseSeconds) };
        _autoCloseTimer.Tick += (_, _) => Dismiss();
        _autoCloseTimer.Start();
    }

    private void CompleteButton_Click(object sender, RoutedEventArgs e)
    {
        _resultHandled = true;
        Completed?.Invoke(_item);
        Close();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Dismiss();

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

    // 「閉じる」ボタン・自動クローズ・×ボタン共通の dismiss 処理
    private void Dismiss()
    {
        _resultHandled = true;
        Dismissed?.Invoke(_item);
        Close();
    }

    protected override void OnClosed(EventArgs e)
    {
        _autoCloseTimer.Stop();
        // ×ボタンで閉じた場合も dismiss として扱う
        if (!_resultHandled)
            Dismissed?.Invoke(_item);
        base.OnClosed(e);
    }
}
