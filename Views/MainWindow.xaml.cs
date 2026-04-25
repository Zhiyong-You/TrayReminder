using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
    private ICollectionView _view = null!;

    public MainWindow(ReminderService reminderService)
    {
        InitializeComponent();
        _viewModel = new MainViewModel(reminderService);
        DataContext = _viewModel;

        // ソート：未完了→有効→ReminderTime昇順→タイトル昇順
        _view = CollectionViewSource.GetDefaultView(_viewModel.Reminders);
        _view.SortDescriptions.Add(new SortDescription(nameof(ReminderItem.IsCompleted),  ListSortDirection.Ascending));
        _view.SortDescriptions.Add(new SortDescription(nameof(ReminderItem.IsEnabled),    ListSortDirection.Descending));
        _view.SortDescriptions.Add(new SortDescription(nameof(ReminderItem.ReminderTime), ListSortDirection.Ascending));
        _view.SortDescriptions.Add(new SortDescription(nameof(ReminderItem.Title),        ListSortDirection.Ascending));

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

    private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_view is null) return;   // InitializeComponent 中の早期発火を無視
        _view.Filter = FilterComboBox.SelectedIndex switch
        {
            1 => o => o is ReminderItem r && r.IsEnabled,
            2 => o => o is ReminderItem r && !r.IsCompleted,
            3 => o => o is ReminderItem r && !r.IsCompleted && r.IsEnabled && r.ReminderTime < DateTime.Now,
            _ => null   // 0: すべて
        };
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

        SetStatusBadge(item);

        var desc = item.Description?.Trim();
        DetailDescription.Text = string.IsNullOrEmpty(desc) ? "（メモなし）" : desc;
    }

    private void SetStatusBadge(ReminderItem item)
    {
        if (item.IsCompleted)
        {
            DetailEnabledBadge.Visibility = Visibility.Visible;
            DetailEnabledBadge.Background = new SolidColorBrush(Color.FromRgb(0xEB, 0xE6, 0xF8));
            DetailEnabled.Text       = "✓ 完了";
            DetailEnabled.Foreground = new SolidColorBrush(Color.FromRgb(0x7C, 0x65, 0xD8));
        }
        else if (!item.IsEnabled)
        {
            DetailEnabledBadge.Visibility = Visibility.Visible;
            DetailEnabledBadge.Background = new SolidColorBrush(Color.FromRgb(0xE0, 0xDD, 0xF0));
            DetailEnabled.Text       = "○ 無効";
            DetailEnabled.Foreground = new SolidColorBrush(Color.FromRgb(0x88, 0x88, 0x99));
        }
        else if (item.ReminderTime < DateTime.Now)
        {
            DetailEnabledBadge.Visibility = Visibility.Visible;
            DetailEnabledBadge.Background = new SolidColorBrush(Color.FromRgb(0xFF, 0xED, 0xCC));
            DetailEnabled.Text       = "⚠ 期限超過";
            DetailEnabled.Foreground = new SolidColorBrush(Color.FromRgb(0x95, 0x4E, 0x00));
        }
        else
        {
            // 通常状態（有効・未来日時・未完了）: バッジ非表示
            DetailEnabledBadge.Visibility = Visibility.Collapsed;
        }
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
