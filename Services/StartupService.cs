using Microsoft.Win32;

namespace TrayReminder.Services;

public static class StartupService
{
    private const string RegKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
    private const string AppName    = "TrayReminder";

    public static bool IsEnabled()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RegKeyPath);
        return key?.GetValue(AppName) is not null;
    }

    public static void Enable()
    {
        var exe = Environment.ProcessPath;
        if (string.IsNullOrEmpty(exe)) return;

        using var key = Registry.CurrentUser.OpenSubKey(RegKeyPath, writable: true);
        // --startup フラグ付きで登録：起動時にウィンドウを非表示にするため
        key?.SetValue(AppName, $"\"{exe}\" --startup");
    }

    public static void Disable()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RegKeyPath, writable: true);
        key?.DeleteValue(AppName, throwOnMissingValue: false);
    }
}
