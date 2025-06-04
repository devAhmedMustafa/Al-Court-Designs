namespace OrdrMate.Utils;

public class DailyNumberGenerator
{
    private static int _currentNumber = 0;

    public static int GetNextNumber()
    {
        var currentDate = TimeService.GetCurrentDate();

        if (!TimeService.IsSameDay(currentDate))
        {
            _currentNumber = 0;
        }
        return _currentNumber++;
    }
}