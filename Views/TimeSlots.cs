namespace TrayReminder.Views;

internal static class TimeSlots
{
    internal static IReadOnlyList<string> All { get; } = Generate();

    private static IReadOnlyList<string> Generate()
    {
        var list = new List<string>(24 * 12);
        for (var h = 0; h < 24; h++)
            for (var m = 0; m < 60; m += 5)
                list.Add($"{h:D2}:{m:D2}");
        return list;
    }

    /// <summary>
    /// DateTime の時刻を 5分単位に切り下げた "HH:mm" 文字列を返す。
    /// </summary>
    internal static string FloorToSlot(DateTime dt)
        => $"{dt.Hour:D2}:{dt.Minute / 5 * 5:D2}";
}
