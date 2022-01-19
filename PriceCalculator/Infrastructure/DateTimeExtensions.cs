using System;

namespace PriceCalculator.Infrastructure;

public static class DateTimeExtensions
{
    /// <summary>
    /// Return the week number
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static byte WeekNumberOfYear(this DateTimeOffset d) => (byte)(d.DayOfYear / 7);
}
