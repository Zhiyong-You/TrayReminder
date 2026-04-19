using System.Globalization;
using System.Windows;
using TrayReminder.Models;

namespace TrayReminder.Views;

public partial class AddEditDialog : Window
{
    public ReminderItem Result { get; private set; }

    public AddEditDialog(ReminderItem? existing = null)
    {
        InitializeComponent();

        RepeatTypeBox.ItemsSource = Enum.GetValues<RepeatType>();

        if (existing is not null)
        {
            Result = existing;
            TitleBox.Text = existing.Title;
            DescriptionBox.Text = existing.Description;
            ReminderTimeBox.Text = existing.ReminderTime.ToString("yyyy/MM/dd HH:mm");
            RepeatTypeBox.SelectedItem = existing.RepeatType;
            IsEnabledCheck.IsChecked = existing.IsEnabled;
        }
        else
        {
            Result = new ReminderItem();
            ReminderTimeBox.Text = DateTime.Now.AddHours(1).ToString("yyyy/MM/dd HH:mm");
            RepeatTypeBox.SelectedItem = RepeatType.None;
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

        if (!DateTime.TryParseExact(ReminderTimeBox.Text, "yyyy/MM/dd HH:mm",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out var time))
        {
            MessageBox.Show("通知日時の形式が正しくありません。\n例: 2026/04/20 09:00",
                "TrayReminder", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Result.Title = TitleBox.Text.Trim();
        Result.Description = DescriptionBox.Text.Trim();
        Result.ReminderTime = time;
        Result.RepeatType = (RepeatType)(RepeatTypeBox.SelectedItem ?? RepeatType.None);
        Result.IsEnabled = IsEnabledCheck.IsChecked == true;

        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
