using TrayReminder.Models;

namespace TrayReminder.Services;

public static class RepeatScheduler
{
    /// <summary>
    /// 現在の ReminderTime と RepeatType から次回通知時刻を計算する。
    /// 次回時刻が現在より未来になるまでインターバルを繰り返し加算する。
    /// RepeatType.None の場合は current をそのまま返す。
    /// </summary>
    public static DateTime NextOccurrence(DateTime current, RepeatType repeatType)
    {
        if (repeatType == RepeatType.None)
            return current;

        var next = current;
        var now  = DateTime.Now;
        do
        {
            next = Advance(next, repeatType);
        }
        while (next <= now);

        return next;
    }

    // 1インターバル分だけ進める
    private static DateTime Advance(DateTime dt, RepeatType repeatType)
        => repeatType switch
        {
            RepeatType.Daily   => dt.AddDays(1),
            RepeatType.Weekly  => dt.AddDays(7),
            RepeatType.Workday => NextWorkday(dt),
            _                  => dt.AddDays(1)
        };

    private static DateTime NextWorkday(DateTime dt)
    {
        var next = dt.AddDays(1);
        while (next.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            next = next.AddDays(1);
        return next;
    }
}
