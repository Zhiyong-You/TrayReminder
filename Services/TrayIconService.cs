using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using TrayReminder.Views;

namespace TrayReminder.Services;

public class TrayIconService : IDisposable
{
    private readonly NotifyIcon _notifyIcon;
    private readonly MainWindow _mainWindow;
    private readonly Action _openNewReminder;
    private readonly Action _exitApp;
    private readonly Icon? _customIcon;
    private bool _disposed;

    public TrayIconService(MainWindow mainWindow, Action openNewReminder, Action exitApp)
    {
        _mainWindow = mainWindow;
        _openNewReminder = openNewReminder;
        _exitApp = exitApp;

        _customIcon = LoadAppIcon();

        _notifyIcon = new NotifyIcon
        {
            Icon = _customIcon ?? SystemIcons.Application,
            Text = "TrayReminder",
            Visible = true,
            ContextMenuStrip = BuildContextMenu()
        };

        _notifyIcon.DoubleClick += (_, _) => ShowMainWindow();
    }

    // TrayReminder.ico をアセンブリの EmbeddedResource から読み込む
    // 失敗時は null を返し、呼び出し側で SystemIcons.Application にフォールバックする
    private static Icon? LoadAppIcon()
    {
        try
        {
            var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("TrayReminder.TrayReminder.ico");
            return stream is not null ? new Icon(stream) : null;
        }
        catch
        {
            return null;
        }
    }

    private ContextMenuStrip BuildContextMenu()
    {
        var menu = new ContextMenuStrip();
        menu.Items.Add("主画面を開く",     null, (_, _) => ShowMainWindow());
        menu.Items.Add("新規リマインダー", null, (_, _) => _openNewReminder());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("終了",             null, (_, _) => _exitApp());
        return menu;
    }

    private void ShowMainWindow()
    {
        _mainWindow.Show();
        if (_mainWindow.WindowState == System.Windows.WindowState.Minimized)
            _mainWindow.WindowState = System.Windows.WindowState.Normal;
        _mainWindow.Activate();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
        _customIcon?.Dispose();
    }
}
