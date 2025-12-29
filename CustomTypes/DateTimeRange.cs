using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomTypes.Helpers;

namespace CustomTypes;

public struct DateTimeRange : IEquatable<DateTimeRange>
{
    public DateTimeRange() { }

    public DateTimeRange(DateTime begin, DateTime end)
    {
        SetRange(begin, end);
    }

    public void SetRange(DateTime begin, DateTime end)
    {
        if (begin > end)
        {
            Begin = end;
            End = Begin;
        }
        else
        {
            Begin = begin;
            End = end;
        }

        Span = End - Begin;
    }

    public DateTime Begin { get; private set; } = DateTime.MinValue;

    public DateTime End { get; private set; } = DateTime.MaxValue;

    public TimeSpan Span { get; private set; } = DateTime.MaxValue - DateTime.MinValue;

    public bool Overlaps(DateTimeRange other)
    {
        if (End < other.Begin || other.End < Begin) { return false; }
        return true;
    }

    public static bool Overlaps(DateTimeRange dtr1, DateTimeRange dtr2)
    {
        return dtr1.Overlaps(dtr2);
    }

    public TimeSpan TimeUntil()
    {
        if (DateTime.Now >= Begin) { return TimeSpan.Zero; }
        return Begin - DateTime.Now;
    }

    public bool Equals(DateTimeRange other)
    {
        return Begin == other.Begin && End == other.End;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is DateTimeRange dtr) { return Equals(dtr); }


        return false;
    }

    public override int GetHashCode()
    {
        return Begin.GetHashCode();
    }

    public override string ToString()
    {
        var result = new StringBuilder();

        result.Append(Begin.ToString("yyyy-MM-dd HH:mm:ss"));
        result.Append(" to ");
        result.Append(End.ToString("yyyy-MM-dd HH:mm:ss"));
        
        return result.ToString();
    }

    public static bool TryParse(string sourceStr, out DateTimeRange? result)
    {
        result = null;

        string workString = cleanRangeSeparator(sourceStr);
        List<string> parts = StringHelper.Split(workString, " to ", true, true, true).ToList();

        if (parts.Count != 2) { return false; }

        if (DateTime.TryParse(parts[0], out DateTime beginResult) 
         && DateTime.TryParse(parts[1], out DateTime endResult))
        {
            result = new DateTimeRange(beginResult, endResult);
            return true;
        }

        return false;
    }

    private static string[] _altRangeSeparators = { " - ", " > ", " >> ", " >>> " };
    private static string cleanRangeSeparator(string sourceStr)
    {
        string result = sourceStr;
        foreach (var alt in _altRangeSeparators)
        {
            result = result.Replace(alt, " to ");
        }
        return result;
    }


}
