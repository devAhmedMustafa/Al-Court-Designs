namespace OrdrMate.Utils;

public static class TimeService
{
    private static DateOnly lastCheckTime = DateOnly.FromDateTime(DateTime.Now);

    public static DateOnly GetCurrentDate()
    {
        var currentDate = DateOnly.FromDateTime(DateTime.Now);
        if (currentDate != lastCheckTime)
        {
            lastCheckTime = currentDate;
        }
        return lastCheckTime;
    }

    public static bool IsSameDay(DateOnly date)
    {
        var currentDate = DateOnly.FromDateTime(DateTime.Now);
        return date == currentDate;
    }
}