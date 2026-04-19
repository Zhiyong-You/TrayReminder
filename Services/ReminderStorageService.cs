using System.IO;
using System.Text.Json;
using TrayReminder.Models;

namespace TrayReminder.Services;

public class ReminderStorageService
{
    private readonly string _filePath;
    private static readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    public ReminderStorageService()
    {
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "TrayReminder");
        Directory.CreateDirectory(dir);
        _filePath = Path.Combine(dir, "reminders.json");
    }

    public List<ReminderItem> Load()
    {
        try
        {
            if (!File.Exists(_filePath)) return [];
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<ReminderItem>>(json, _options) ?? [];
        }
        catch
        {
            return [];
        }
    }

    public void Save(IEnumerable<ReminderItem> items)
    {
        try
        {
            var json = JsonSerializer.Serialize(items, _options);
            File.WriteAllText(_filePath, json);
        }
        catch { }
    }
}
