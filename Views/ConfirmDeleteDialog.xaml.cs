using System.Windows;

namespace TrayReminder.Views;

public partial class ConfirmDeleteDialog : Window
{
    public ConfirmDeleteDialog(string itemTitle)
    {
        InitializeComponent();
        MessageText.Text = $"「{itemTitle}」を削除しますか？\nこの操作は取り消せません。";
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e) => DialogResult = true;
    private void CancelButton_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}
