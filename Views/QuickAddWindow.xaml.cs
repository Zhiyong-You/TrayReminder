using System.Globalization;
using System.Windows;
using TrayReminder.Models;
using TrayReminder.Services;
using MessageBox = System.Windows.MessageBox;

namespace TrayReminder.Views;

public partial class QuickAddWindow : Window
{
    private readonly ReminderService _service;

    public QuickAddWindow(ReminderService service)
    {
        InitializeComponent();
        _service = service;

        RepeatTypeBox.ItemsSource = Enum.GetValues<RepeatType>();
        RepeatTypeBox.SelectedItem = RepeatType.None;

        DateBox.Text = DateTime.Now.ToString("yyyy/MM/dd");
        TimeBox.Text = DateTime.Now.AddHours(1).ToString("HH:mm");
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleBox.Text))
        {
            MessageBox.Show("タイトルを入力してください。", "TrayReminder",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            TitleBox.Focus();
            return;
        }

        var dateTimeText = $"{DateBox.Text.Trim()} {TimeBox.Text.Trim()}";
        if (!DateTime.TryParseExact(dateTimeText, "yyyy/MM/dd HH:mm",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out var reminderTime))
        {
            MessageBox.Show("日付・時刻の形式が正しくありません。\n日付: 2026/04/20\n時刻: 09:00",
                "TrayReminder", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var item = new ReminderItem
        {
            Title        = TitleBox.Text.Trim(),
            Description  = DescriptionBox.Text.Trim(),
            ReminderTime = reminderTime,
            RepeatType   = (RepeatType)(RepeatTypeBox.SelectedItem ?? RepeatType.None),
            IsEnabled    = IsEnabledCheck.IsChecked == true
        };

        _service.Add(item);
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
