using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using TrayReminder.Models;

namespace TrayReminder.Views;

public partial class NotificationWindow : Window
{
    // 表示中の全通知ウィンドウを追跡（積み上げ位置計算用）
    private static readonly List<NotificationWindow> _openWindows = [];

    public event Action<ReminderItem>? Completed;
    public event Action<ReminderItem>? Dismissed;
    public event Action<ReminderItem, TimeSpan>? Snoozed;

    private readonly ReminderItem _item;
    private readonly DispatcherTimer _autoCloseTimer;
    private bool _resultHandled;

    private const int AutoCloseSeconds = 30;
    private const int EdgeMargin = 14;
    private const int WindowGap = 10;

    public NotificationWindow(ReminderItem item)
    {
        InitializeComponent();
        _item = item;
        TitleText.Text = item.Title;
        TimeText.Text = item.ReminderTime.ToString("yyyy/MM/dd HH:mm");
        if (string.IsNullOrWhiteSpace(item.Description))
        {
            DescriptionText.Text = "メモなし";
            DescriptionText.Foreground = (System.Windows.Media.Brush)FindResource("DialogLabelFg");
        }
        else
        {
            DescriptionText.Text = item.Description;
        }

        // 現在の表示中リストをもとに右下位置を計算（Show() 前なので自分はまだリスト外）
        PositionAtBottomRight();

        // Show された後にリストへ追加し、Closed 時に除去する
        Loaded += (_, _) => _openWindows.Add(this);
        Closed += (_, _) => _openWindows.Remove(this);

        _autoCloseTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(AutoCloseSeconds) };
        _autoCloseTimer.Tick += (_, _) => Dismiss();
        _autoCloseTimer.Start();
    }

    // 表示中ウィンドウの実際の高さを積み上げて Top を決定する
    private void PositionAtBottomRight()
    {
        var workArea = SystemParameters.WorkArea;

        // 下端基準点（余白を引いた位置）から上方向に積み上げる
        double bottom = workArea.Bottom - EdgeMargin;
        foreach (var win in _openWindows)
        {
            var h = win.ActualHeight > 0 ? win.ActualHeight : win.Height;
            bottom -= h + WindowGap;
        }

        Left = workArea.Right - Width - EdgeMargin;
        Top  = bottom - Height;
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // ビジュアルツリーを上方向に辿り、ButtonBase 祖先があればドラッグしない
        var src = e.OriginalSource as DependencyObject;
        while (src is not null && src != this)
        {
            if (src is System.Windows.Controls.Primitives.ButtonBase) return;
            src = VisualTreeHelper.GetParent(src);
        }
        DragMove();
    }

    private void CompleteButton_Click(object sender, RoutedEventArgs e)
    {
        _resultHandled = true;
        Completed?.Invoke(_item);
        Close();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Dismiss();

    private void Snooze5Button_Click(object sender, RoutedEventArgs e)
        => DoSnooze(TimeSpan.FromMinutes(5));

    private void Snooze10Button_Click(object sender, RoutedEventArgs e)
        => DoSnooze(TimeSpan.FromMinutes(10));

    private void DoSnooze(TimeSpan duration)
    {
        _resultHandled = true;
        Snoozed?.Invoke(_item, duration);
        Close();
    }

    // 「閉じる」ボタン・自動クローズ・×ボタン共通の dismiss 処理
    private void Dismiss()
    {
        _resultHandled = true;
        Dismissed?.Invoke(_item);
        Close();
    }

    protected override void OnClosed(EventArgs e)
    {
        _autoCloseTimer.Stop();
        // ×ボタンで閉じた場合も dismiss として扱う
        if (!_resultHandled)
            Dismissed?.Invoke(_item);
        base.OnClosed(e);
    }
}
