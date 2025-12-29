using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeHelpers;

public static class DateTimeHelper
{
    public static bool IsLeapYear(this DateTime dateTime)
    {
        return IsLeapYear(dateTime.Year);
    }

    public static bool IsLeapYear(int theYear)
    {
        if (theYear < 1) { return false; }

        if ((theYear % 4 == 0 && theYear % 100 != 0) || theYear % 400 == 0)
        {
            return true;
        }
        return false;
    }

    public static string MonthName(this DateTime dateTime)
    {
        return dateTime.ToString("MMMM");
    }

    public static string? MonthName(int theMonth)
    {
        if (theMonth < 1 || theMonth > 12) { return null; }

        var dt = new DateTime(2000, theMonth, 1);
        return dt.ToString("MMMM");
    }


    public static string DayOfWeekName(this DateTime dateTime)
    {
        return dateTime.ToString("dddd");
    }

    /// <summary>
    /// Gives you the collection of days of the week in order, usually starting with Sunday, 
    /// unless you're neurotic like me and want the weekend to be at the end of the week, 
    /// which means the start of the week is Monday.  It makes so much more sense.  But I 
    /// also realize everything is set up to treat Sunday as the beginning of the week, so 
    /// Sunday being first is the default.
    /// </summary>
    /// <param name="useAbbreviatedDayNames">
    /// Defaults to using the full name, but setting this to true will get you the short version. 
    /// </param>
    /// <param name="useMondayFirst">
    /// Defaults to using Sunday first which is how everything is set up in DateTime, but it 
    /// makes more sense for the weekend to be at the end of the week, so Monday really SHOULD 
    /// be the first day of the week.  If you want to, this method allows the good way.
    /// </param>
    /// <returns>The collection of days of the week in order.</returns>
    public static IEnumerable<string> DaysOfTheWeek(bool? useAbbreviatedDayNames = null, bool? useMondayFirst = null)
    {
        string dayNamePattern = useAbbreviatedDayNames is not null && useAbbreviatedDayNames == true ? "ddd" : "dddd";

        bool useMon1 = useMondayFirst is not null && useMondayFirst == true;
        DayOfWeek first = useMon1 ? DayOfWeek.Monday : DayOfWeek.Sunday;

        var worker = new DateTime();
        while (worker.DayOfWeek != first) { worker.AddDays(1.0); }

        for (int counter = 1; counter <= 7; counter++)
        {
            yield return worker.ToString(dayNamePattern);
            worker.AddDays(1.0);
        }
    }


    public static int? GetClosestFourDigitYear(int TwoDigitYear)
    {
        if (TwoDigitYear < 1) { return null; }

        var cleanTwoDigits = TwoDigitYear % 100;
        var currentYear = DateTime.Now.Year;
        var currentCentury = currentYear - (currentYear % 100);
        var previousCentury = currentCentury - 100;
        var nextCentury = currentCentury + 100;

        // get a difference for the current century
        var currentDiff = currentYear - (currentCentury + cleanTwoDigits);
        if (currentDiff < 0) { currentDiff *= -1; }

        // get a difference for the previous century
        var previousDiff = currentYear - (previousCentury + cleanTwoDigits);
        if (previousDiff < 0) { previousDiff *= -1; }

        // get a difference for the next century
        var nextDiff = currentYear - (nextCentury + cleanTwoDigits);
        if (nextDiff < 0) { nextDiff *= -1; }

        // now figure out which one is the lowest difference
        if (currentDiff < previousDiff && currentDiff < nextDiff) { return currentCentury + cleanTwoDigits; }
        if (previousDiff < currentDiff && previousDiff < nextDiff) { return previousCentury + cleanTwoDigits; }
        if (nextDiff < currentDiff && nextDiff < previousDiff) { return nextCentury + cleanTwoDigits; }

        return null; // we shouldn't be able to get here, but whatever.
    }


    private static int[] _daysInMonth { get; } = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

    /// <summary>
    /// Gets you the number of days in the given month
    /// </summary>
    /// <param name="month">a number from 1 to 12 representing the month</param>
    /// <param name="year">
    /// providing a year means the method can tell if it's a leap year or not, 
    /// and potentially give you the 29 for a leap year February.
    /// </param>
    /// <returns>The number of days in the month you provide</returns>
    public static int? DaysInMonth(int month, int? year)
    {
        if (month < 1 || month > 12) { return null; }

        bool isLeapYear = IsLeapYear(year ?? 7); // 7 default is definitely not a leap year

        if (isLeapYear && month == 2) { return 29; }

        return _daysInMonth[month - 1];
    }

    /// <summary>
    /// Gets you the number of days in the month and year of this instance of DateTime
    /// </summary>
    public static int? DaysInMonth(this DateTime dateTime)
    {
        return DaysInMonth(dateTime.Month, dateTime.Year);
    }


    public static string SQLDateTimeFormat(
        this DateTime dateTime, 
        bool? IncludeDate = null, 
        bool? IncludeTime = null, 
        bool? IncludeMilliseconds = null)
    {
        // set up our bools so we know what we're including
        bool inclD = IncludeDate ?? true;
        bool inclT = IncludeTime ?? true;
        bool inclM = IncludeMilliseconds ?? false;

        // this method isn't meant for just retrieving the milliseconds, so 
        // if the user wants the milliseconds, we need the time too.
        if (inclM && !inclT) { inclT = true; }

        // get our return variable...
        var retVal = new StringBuilder();

        // handle date
        if (inclD) { retVal.Append(dateTime.ToString("yyyy-MM-dd")); }

        // handle time
        if (inclT)
        {
            // need a space if we have a date already
            if (inclD) { retVal.Append(' '); }

            retVal.Append(dateTime.ToString("HH:mm:ss"));
        }

        // handle millisecond
        if (inclM) { retVal.Append(dateTime.ToString(".fff")); }

        // return our nifty well formatted value...
        return retVal.ToString();
    }

    /// <summary>
    /// This method gives you a string that gives date and time information 
    /// in a concise and clear way.  The date is in the format 4 January 2025, 
    /// so that there can't be confusion about which number in 4/1/25 is the 
    /// month and which is the day.  The time is always a 24 hour time, so we 
    /// don't need to worry about which half of the day we're in, like 17:42
    /// </summary>
    /// <param name="includeDate">
    /// Defaults to true, but can be excluded if all you want is the time. 
    /// If you pass false for both, includeDate will be defaulted to true.
    /// </param>
    /// <param name="includeTime">
    /// Defaults to false.  Set to true if you want the time to be included 
    /// in the return string.
    /// </param>
    /// <param name="fullMonthName">
    /// Defaults to true in which case it will show something like "January".  
    /// If set to false, it would show something like "Jan" instead.
    /// </param>
    /// <returns>A string representing a date and/or time in a clear way.</returns>
    public static string ClearDateTimeFormat(
        this DateTime dateTime, 
        bool? includeDate = null, 
        bool? includeTime = null, 
        bool? fullMonthName = null)
    {
        var retVal = new StringBuilder();

        bool inclD = includeDate ?? true;
        bool inclT = includeTime ?? false;

        if (!inclD && !inclT) { inclD = true; }

        if (inclD) { retVal.Append(dateTime.ToString("d MMMM yyyy")); }
        if (inclD && inclT) { retVal.Append(", "); }
        if (inclT) { retVal.Append(dateTime.ToString("H:mm")); }

        return retVal.ToString();
    }



    // end class
}
