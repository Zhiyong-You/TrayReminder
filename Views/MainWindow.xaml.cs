using System.Windows;
using TrayReminder.Models;
using TrayReminder.ViewModels;

namespace TrayReminder.Views;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _viewModel;
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new AddEditDialog { Owner = this };
        if (dialog.ShowDialog() == true)
            _viewModel.AddReminder(dialog.Result);
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
