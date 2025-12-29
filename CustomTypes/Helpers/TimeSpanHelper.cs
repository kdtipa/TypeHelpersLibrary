using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTypes.Helpers;

public static class TimeSpanHelper
{
    public static SimpleUnitsOfTime BestUnitOfTime(this TimeSpan ts, out double valueInGivenUnit)
    {
        if (ts.TotalDays >= 1.0)
        {
            valueInGivenUnit = ts.TotalDays;
            return SimpleUnitsOfTime.Day;
        }

        if (ts.TotalHours >= 1.0)
        {
            valueInGivenUnit = ts.TotalHours;
            return SimpleUnitsOfTime.Hour;
        }

        if (ts.TotalMinutes >= 1.0)
        {
            valueInGivenUnit = ts.TotalMinutes;
            return SimpleUnitsOfTime.Minute;
        }

        valueInGivenUnit = ts.TotalSeconds;
        return SimpleUnitsOfTime.Second;
    }

    public static IEnumerable<(int, string)> GetSimpleParts(this TimeSpan ts)
    {
        yield return (ts.Days, "Days");
        yield return (ts.Hours, "Hours");
        yield return (ts.Minutes, "Minutes");
        yield return (ts.Seconds, "Seconds");
    }



}

public enum SimpleUnitsOfTime
{
    Second = 1, 
    Minute = 60, 
    Hour = 3600, 
    Day = 86400
}
