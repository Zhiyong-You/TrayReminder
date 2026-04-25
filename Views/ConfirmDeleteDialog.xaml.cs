using System.Windows;

namespace TrayReminder.Views;

public partial class ConfirmDeleteDialog : Window
{
    public ConfirmDeleteDialog(string itemTitle)
    {
        InitializeComponent();
        MessageText.Text = $"「{itemTitle}」を削除しますか？";
    }

    // 完了済み一括削除用
    public ConfirmDeleteDialog(int completedCount)
    {
        InitializeComponent();
        MessageText.Text = $"完了済みのリマインダー {completedCount} 件をすべて削除しますか？";
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e) => DialogResult = true;
    private void CancelButton_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}
