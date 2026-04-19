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

        RepeatTypeBox.ItemsSource  = Enum.GetValues<RepeatType>();
        RepeatTypeBox.SelectedItem = RepeatType.None;

        ReminderTimeBox.ItemsSource = TimeSlots.All;

        var defaultDt = DateTime.Now.AddHours(1);
        ReminderDatePicker.SelectedDate = defaultDt.Date;
        ReminderTimeBox.SelectedItem    = TimeSlots.FloorToSlot(defaultDt);
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

        if (ReminderDatePicker.SelectedDate is null)
        {
            MessageBox.Show("日付を選択してください。", "TrayReminder",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (ReminderTimeBox.SelectedItem is null)
        {
            MessageBox.Show("時刻を選択してください。", "TrayReminder",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var reminderTime = CombineDateTime(
            ReminderDatePicker.SelectedDate.Value,
            (string)ReminderTimeBox.SelectedItem);

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

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();

    private static DateTime CombineDateTime(DateTime date, string timeSlot)
    {
        var parts = timeSlot.Split(':');
        return date.Date
            .AddHours(int.Parse(parts[0]))
            .AddMinutes(int.Parse(parts[1]));
    }
}
