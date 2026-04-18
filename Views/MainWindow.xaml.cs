using System.Windows;
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
        _viewModel.AddReminder();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.SaveReminders();
        MessageBox.Show("保存しました。（未実装）", "TrayReminder", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
