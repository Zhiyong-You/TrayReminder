using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
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

        _viewModel.Reminders.CollectionChanged += (_, _) => UpdateCountText();
        UpdateCountText();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
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
        {
            _viewModel.UpdateReminder(dialog.Result);
            ShowDetail(dialog.Result);
        }
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (ReminderList.SelectedItem is not ReminderItem selected) return;
        var dialog = new ConfirmDeleteDialog(selected.Title) { Owner = this };
        if (dialog.ShowDialog() == true)
        {
            _viewModel.RemoveReminder(selected);
            ClearDetail();
        }
    }

    private void ClearCompletedButton_Click(object sender, RoutedEventArgs e)
    {
        var count = _viewModel.Reminders.Count(x => x.IsCompleted);
        if (count == 0)
        {
            MessageBox.Show("完了済みのリマインダーはありません。", "TrayReminder",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var dialog = new ConfirmDeleteDialog(count) { Owner = this };
        if (dialog.ShowDialog() == true)
        {
            var selectedWasCompleted = ReminderList.SelectedItem is ReminderItem sel && sel.IsCompleted;
            _viewModel.RemoveCompletedReminders();
            if (selectedWasCompleted)
                ClearDetail();
        }
    }

    private void SnoozeButton_Click(object sender, RoutedEventArgs e)
    {
        if (ReminderList.SelectedItem is not ReminderItem selected) return;
        _viewModel.SnoozeReminder(selected, TimeSpan.FromMinutes(10));
        ShowDetail(selected);
    }

    private void ReminderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ReminderList.SelectedItem is ReminderItem selected)
            ShowDetail(selected);
        else
            ClearDetail();
    }

    private void ShowDetail(ReminderItem item)
    {
        NoSelectionPanel.Visibility = Visibility.Collapsed;
        DetailPanel.Visibility = Visibility.Visible;

        DetailTitle.Text = item.Title;
        DetailTime.Text = item.ReminderTime.ToString("yyyy/MM/dd HH:mm");
        DetailRepeat.Text = item.RepeatType switch
        {
            RepeatType.Daily   => "毎日",
            RepeatType.Weekly  => "毎週",
            RepeatType.Workday => "平日",
            _                  => "なし"
        };

        if (item.IsEnabled)
        {
            DetailEnabled.Text = "● 有効";
            DetailEnabled.Foreground = Brushes.White;
            DetailEnabledBadge.Background = new SolidColorBrush(Color.FromRgb(0x38, 0x8A, 0x61));
        }
        else
        {
            DetailEnabled.Text = "○ 無効";
            DetailEnabled.Foreground = new SolidColorBrush(Color.FromRgb(0x88, 0x88, 0x99));
            DetailEnabledBadge.Background = new SolidColorBrush(Color.FromRgb(0xE0, 0xDD, 0xF0));
        }

        var desc = item.Description?.Trim();
        DetailDescription.Text = string.IsNullOrEmpty(desc) ? "（メモなし）" : desc;
    }

    private void ClearDetail()
    {
        DetailPanel.Visibility = Visibility.Collapsed;
        NoSelectionPanel.Visibility = Visibility.Visible;
    }

    private void UpdateCountText()
    {
        ReminderCountText.Text = $"{_viewModel.Reminders.Count} 件";
    }
}
