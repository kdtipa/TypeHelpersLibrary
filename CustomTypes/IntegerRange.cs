using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CustomTypes;

public struct IntegerRange : IEquatable<IntegerRange>, IEnumerable<int>
{
    public IntegerRange() { }

    public IntegerRange(int minVal, int maxVal)
    {
        if (minVal > maxVal)
        {
            (minVal, maxVal) = (maxVal, minVal);
        }

        MinValue = minVal;
        MaxValue = maxVal;
    }

    private int _minValue = int.MinValue;
    /// <summary>
    /// The lowest number included in this range.  
    /// Defaults to int.MinValue
    /// </summary>
    public int MinValue
    {
        get { return _minValue; } 
        set
        {
            if (value > _maxValue)
            {
                // need to swap
                _minValue = _maxValue;
                _maxValue = value;
            }
            else
            {
                _minValue = value;
            }
        }
    }

    private int _maxValue = int.MaxValue;
    /// <summary>
    /// The highest number included in this range.
    /// Defaults to int.MaxValue
    /// </summary>
    public int MaxValue
    {
        get { return _maxValue; }
        set
        {
            if (value < _minValue)
            {
                // need to swap
                _maxValue = _minValue;
                _minValue = value;
            }
            else
            {
                _maxValue = value;
            }
        }
    }

    /// <summary>
    /// Tells you how big a range you have.  Has to be a long 
    /// to account for sizes greater than int.MaxValue.
    /// </summary>
    public long RangeSize { get { return _maxValue - _minValue + 1; } }


    public IEnumerator<int> GetEnumerator()
    {
        for (int n = _minValue; n <= _maxValue; n++)
        {
            yield return n;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Uses the highest and lowest values to adjust the range.  1 to 5 combined 
    /// with 9 to 14 would become 1 to 14.
    /// </summary>
    public void Combine(IntegerRange additionalRange)
    {
        int low = _minValue < additionalRange.MinValue ? _minValue : additionalRange.MinValue;
        int high = _maxValue > additionalRange.MaxValue ? _maxValue : additionalRange.MaxValue;
        _minValue = low;
        _maxValue = high;
    }

    /// <summary>
    /// Takes the highest and lowest values and makes a new range from those.
    /// </summary>
    public static IntegerRange Combine(IntegerRange range1, IntegerRange range2)
    {
        int low = range1.MinValue < range2.MinValue ? range1.MinValue : range2.MinValue;
        int high = range1.MaxValue > range2.MaxValue ? range1.MaxValue : range2.MaxValue;
        return new IntegerRange(low, high);
    }

    public void Shift(int amount)
    {
        _minValue += amount;
        _maxValue += amount;
    }

    public static IntegerRange Shift(IntegerRange initialRange, int shiftAmount)
    {
        return new IntegerRange(initialRange.MinValue + shiftAmount, initialRange.MaxValue + shiftAmount);
    }

    public bool InRange(int checkVal)
    {
        return _minValue <= checkVal && checkVal <= _maxValue;
    }

    public bool Overlaps(IntegerRange compareRange)
    {
        return !(_minValue > compareRange.MaxValue || compareRange.MinValue > _maxValue);
    }

    public bool Equals(IntegerRange other)
    {
        return MinValue == other.MinValue && MaxValue == other.MaxValue;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is IntegerRange irObj) { return Equals(irObj); }

        return false;
    }

    public override int GetHashCode()
    {
        return RangeSize.GetHashCode();
    }

    public override string ToString()
    {
        return $"{_minValue} to {_maxValue}";
    }

    public string ToString(string conjunction, bool? useGroupDivider = null)
    {
        if (useGroupDivider is null || useGroupDivider == false)
        {
            // need to set up the number format string first...
            string formatStr = string.Format(groupedFormat, CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator);
            var result = new StringBuilder();
            result.Append(_minValue.ToString(formatStr));
            result.Append(conjunction);
            result.Append(_maxValue.ToString(formatStr));
            return result.ToString();
        }
        else
        {
            // just put the conjunction provided in the middle
            return $"{_minValue}{conjunction}{_maxValue}";
        }
    }

    private static string groupedFormat { get; } = "#{0}##0";

    
    public bool TryLoad(string sourceString)
    {
        MatchCollection matches = IntPattern.Matches(sourceString);
        if (matches.Count != 2) { return false; }

        if (int.TryParse(matches[0].Value, out int m1) 
         && int.TryParse(matches[1].Value, out int m2))
        {
            if (m1 > m2) { (m1, m2) = (m2, m1); }
            MinValue = m1;
            MaxValue = m2;
            return true;
        }

        return false;
    }

    private static Regex IntPattern { get; } = new Regex("-?[1-9]+[0-9]*|0");

    public static bool TryParse(string sourceString, [NotNullWhen(true)] out IntegerRange? parseResult)
    {
        parseResult = null;

        IntegerRange worker = new();
        if (worker.TryLoad(sourceString))
        {
            parseResult = worker;
            return true;
        }

        return false;
    }

    

    public static bool operator ==(IntegerRange a, IntegerRange b) { return a.Equals(b); }
    public static bool operator !=(IntegerRange a, IntegerRange b) { return !a.Equals(b); }

    /// <summary>
    /// Shorthand for the Combine method
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static IntegerRange operator +(IntegerRange a, IntegerRange b) { return Combine(a, b); }


}
