using System.ComponentModel;
using System.Windows;
using TrayReminder.Models;
using TrayReminder.Services;
using TrayReminder.ViewModels;
using MessageBox = System.Windows.MessageBox;

namespace TrayReminder.Views;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow(ReminderService reminderService)
    {
        InitializeComponent();
        _viewModel = new MainViewModel(reminderService);
        DataContext = _viewModel;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        // 「終了」メニュー以外の × ボタン操作はウィンドウを非表示にするだけ
        if (!App.Current.IsExiting)
        {
            e.Cancel = true;
            Hide();
            return;
        }
        base.OnClosing(e);
    }

    public void OpenAddReminderDialog()
    {
        var dialog = new AddEditDialog { Owner = this };
        if (dialog.ShowDialog() == true)
            _viewModel.AddReminder(dialog.Result);
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        OpenAddReminderDialog();
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (ReminderList.SelectedItem is not ReminderItem selected) return;
        var dialog = new AddEditDialog(selected) { Owner = this };
        if (dialog.ShowDialog() == true)
            _viewModel.UpdateReminder(dialog.Result);
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (ReminderList.SelectedItem is not ReminderItem selected) return;
        var answer = MessageBox.Show($"「{selected.Title}」を削除しますか？", "TrayReminder",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (answer == MessageBoxResult.Yes)
            _viewModel.RemoveReminder(selected);
    }
}
