using System.Windows;
using TrayReminder.Models;
using MessageBox = System.Windows.MessageBox;

namespace TrayReminder.Views;

public partial class AddEditDialog : Window
{
    public ReminderItem Result { get; private set; }

    public AddEditDialog(ReminderItem? existing = null)
    {
        InitializeComponent();

        ReminderTimeBox.ItemsSource = TimeSlots.All;
        RepeatTypeBox.ItemsSource   = Enum.GetValues<RepeatType>();

        DialogTitleText.Text = existing is not null ? "リマインダーを編集" : "リマインダーを追加";

        if (existing is not null)
        {
            Result = existing;
            TitleBox.Text                   = existing.Title;
            DescriptionBox.Text             = existing.Description;
            ReminderDatePicker.SelectedDate = existing.ReminderTime.Date;
            ReminderTimeBox.SelectedItem    = TimeSlots.FloorToSlot(existing.ReminderTime);
            RepeatTypeBox.SelectedItem      = existing.RepeatType;
            IsEnabledCheck.IsChecked        = existing.IsEnabled;
        }
        else
        {
            Result = new ReminderItem();
            var defaultDt = DateTime.Now.AddHours(1);
            ReminderDatePicker.SelectedDate = defaultDt.Date;
            ReminderTimeBox.SelectedItem    = TimeSlots.FloorToSlot(defaultDt);
            RepeatTypeBox.SelectedItem      = RepeatType.None;
        }
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleBox.Text))
        {
            MessageBox.Show("タイトルを入力してください。", "TrayReminder",
                MessageBoxButton.OK, MessageBoxImage.Warning);
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

        Result.Title       = TitleBox.Text.Trim();
        Result.Description = DescriptionBox.Text.Trim();
        Result.ReminderTime = CombineDateTime(
            ReminderDatePicker.SelectedDate.Value,
            (string)ReminderTimeBox.SelectedItem);
        Result.RepeatType = (RepeatType)(RepeatTypeBox.SelectedItem ?? RepeatType.None);
        Result.IsEnabled  = IsEnabledCheck.IsChecked == true;

        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => DialogResult = false;

    private static DateTime CombineDateTime(DateTime date, string timeSlot)
    {
        var parts = timeSlot.Split(':');
        return date.Date
            .AddHours(int.Parse(parts[0]))
            .AddMinutes(int.Parse(parts[1]));
    }
}
