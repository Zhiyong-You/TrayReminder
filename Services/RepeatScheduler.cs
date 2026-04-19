using TrayReminder.Models;

namespace TrayReminder.Services;

public static class RepeatScheduler
{
    /// <summary>
    /// 現在の ReminderTime と RepeatType から次回通知時刻を計算する。
    /// RepeatType.None の場合は current をそのまま返す。
    /// </summary>
    public static DateTime NextOccurrence(DateTime current, RepeatType repeatType)
        => repeatType switch
        {
            RepeatType.Daily   => current.AddDays(1),
            RepeatType.Weekly  => current.AddDays(7),
            RepeatType.Workday => NextWorkday(current),
            _                  => current
        };

    private static DateTime NextWorkday(DateTime dt)
    {
        var next = dt.AddDays(1);
        while (next.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            next = next.AddDays(1);
        return next;
    }
}
