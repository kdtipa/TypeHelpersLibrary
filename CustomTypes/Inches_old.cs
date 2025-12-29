using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CustomTypes;

public struct Inches_old : IEquatable<Inches_old>, IComparable<Inches_old>
{
    public Inches_old() { }

    public Inches_old(decimal inchesValue)
    {
        SetValue(inchesValue);
    }

    public Inches_old(int wholeInches, int fractionalPartNumerator, int fractionalPartDenominator)
    {
        SetValue(wholeInches, fractionalPartNumerator, fractionalPartDenominator);
    }

    public Inches_old(string sourceString)
    {
        if(!TrySet(sourceString)) { throw new ArgumentException("unable to parse source string"); }
    }


    private decimal _value = 0.0000m;



    public int WholeInches
    {
        get { return Convert.ToInt32(_value - (_value % 1.0000m)); }
        set
        {
            if (value <= 0) { _value = _value % 1.0000m; }
            else
            {
                _value = (_value % 1.0000m) + value;
            }
        }
    }

    public SimpleFraction FractionalPart
    {
        get { return new SimpleFraction(_value % 1.0000m); }
        set
        {
            if (value.HasValue)
            {
                _value -= _value % 1.0000m;
                _value += value.Value!.Value;
            }
            else
            {
                throw new ArgumentException("tried to use an invalid fraction");
            }
        }
    }


    public void SetValue(decimal inchesValue)
    {
        _value = RoundToSixteenths(inchesValue);
    }

    public void SetValue(int inchesValue, int fractionalNumerator, int fractionalDenominator)
    {
        // if we got a negative number, don't bother
        if (inchesValue < 0)
        {
            _value = 0.0000m;
            return;
        }

        // build up a value to use
        decimal workValue = inchesValue;

        if (fractionalNumerator > 0 && fractionalDenominator > 0)
        {
            // this means we have a fraction
            workValue += RoundToSixteenths((decimal)fractionalNumerator / fractionalDenominator);
        }

        _value = workValue;
    }

    public bool TrySet(string sourceString)
    {
        // try decimal numbers first
        var decMatch = StringSyntaxDecimalValue.Match(sourceString);
        if (decMatch.Success 
         && decimal.TryParse(decMatch.Value, out decimal parseResult) 
         && GetClosestMeasure(parseResult, out int wi, out int fn, out int fd))
        {
            _value = ((decimal)fn / fd) + wi;
            return true;
        }

        // next try a fraction string like "4 1/8" or "3 and 5/16"
        var fracMatch = StringSyntaxWholeAndFraction.Match(sourceString);
        if (fracMatch.Success)
        {
            string strWI = fracMatch.Groups[1].Value;
            string strFN = fracMatch.Groups[2].Value;
            string strFD = fracMatch.Groups[3].Value;

            if (int.TryParse(strWI, out int intWI) 
             && int.TryParse(strFN, out int intFN) 
             && int.TryParse(strFD, out int intFD))
            {
                _value = ((decimal)intFN / intFD) + intWI;
                return true;
            }
        }

        // finally, if it's essentially an integer, we can just use that as a number of inches
        var intMatch = StringSyntaxWholeOnly.Match(sourceString);
        if (intMatch.Success && int.TryParse(intMatch.Groups[1].Value, out int parsedInt))
        {
            _value = parsedInt;
            return true;
        }

        return false;
    }


    public bool Equals(Inches_old other)
    {
        return _value == other._value;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is Inches_old inchObj) { return Equals(inchObj); }

        if (obj is decimal decObj) { return Equals(new Inches_old(decObj)); }

        if (obj is int intObj) { return Equals(new Inches_old(intObj, 0, 1)); }

        if (obj is string strObj && TryParse(strObj, out Inches_old? parseResult)) 
        {
            return Equals(parseResult.Value);
        }

        return false;
    }

    public int CompareTo(Inches_old other)
    {
        return _value.CompareTo(other._value);
    }

    public override int GetHashCode()
    {
        // for hash grouping, the whole inches value works
        return Convert.ToInt32(_value);
    }

    public override string ToString()
    {
        return $"{WholeInches} and {FractionalPart.Numerator}/{FractionalPart.Denominator}";
    }






    /// <summary>
    /// If you use the denominator of the fraction of an inch that you 
    /// want the decimal value for as the key, you'll get the decimal 
    /// as though you're doing 1/N.  So for example, if you use 8 as 
    /// the key, you'll get 0.125 (one-eigth).
    /// </summary>
    public static Dictionary<int, decimal> OneNth { get; } = new()
    {
        { 16, 0.0625m }, { 8, 0.125m }, { 4, 0.25m }, { 2, 0.5m }
    };


    public static Dictionary<int, decimal> Sixteenths { get; } = new()
    {
        { 0, 0.0000m },
        { 1, 0.0625m }, { 2, 0.1250m }, { 3, 0.1875m }, { 4, 0.2500m },
        { 5, 0.3125m }, { 6, 0.3750m }, { 7, 0.4375m }, { 8, 0.5000m },
        { 9, 0.5625m }, { 10, 0.6250m }, { 11, 0.6875m }, { 12, 0.7500m },
        { 13, 0.8125m }, { 14, 0.8750m }, { 15, 0.9375m }, { 16, 1.0000m }
    };

    public static decimal RoundToSixteenths(decimal sourceValue)
    {
        // the nearest valid measurement for a negative number is zero... so just pass that back.
        if (sourceValue < 0.0000m) { return 0.0000m; }

        // we'll need this value
        decimal oneSixteenth = OneNth[16];

        // this gets us the multiple of 1/16th of an inch below the source value
        decimal lowValue = sourceValue - (sourceValue % oneSixteenth);

        // this is the multiple of 1/16th of an inch above the source value
        decimal highValue = lowValue + oneSixteenth;

        // if the difference between the high value and the source is less than the difference between the source and the low value, return the high value
        if (highValue - sourceValue < sourceValue - lowValue) { return highValue; }
        
        // otherwise, return the low value (which means we favor rounding down)
        return lowValue;
    }

    public static (int wholeInches, int fractionalPartNumerator, int fractionalPartDenominator) GetClosestMeasure(decimal inchesValue)
    {
        if (inchesValue <= 0.0000m) { return (0, 0, 1); }

        decimal fractionalPart = inchesValue % 1.0000m;
        decimal wholePart = inchesValue - fractionalPart;
        int wi = Convert.ToInt32(wholePart);

        // if the fractional part is zero, we know what to return
        if (fractionalPart == 0.0000m) { return (wi, 0, 1); }

        decimal lowestDiff = 1.2500m;
        decimal closestMatch = 2.0000m;
        foreach (decimal fp in Sixteenths.Values)
        {
            decimal currentDiff = fp - fractionalPart;
            currentDiff *= currentDiff < 0.0000m ? -1.0m : 1.0m; // need the absolute value

            if (currentDiff < lowestDiff)
            {
                closestMatch = fp;
                lowestDiff = currentDiff;
            }
        }

        // if we somehow didn't find something closer
        if (closestMatch == 2.0000m) { return (wi, 0, 1); }

        int calculatedNumerator = Sixteenths.Where(item => item.Value == closestMatch).FirstOrDefault().Key;
        int calculatedDenominator = 16;

        // if it was the closest one, we'll just pass back the zero case
        if (calculatedNumerator == 0) { return (wi, 0, 1); }

        while (calculatedNumerator % 2 == 0)
        {
            calculatedNumerator /= 2;
            calculatedDenominator /= 2;
        }

        return (wi, calculatedNumerator, calculatedDenominator);

    }

    public static bool GetClosestMeasure(decimal inchesValue, out int wholeInches, out int fractionalNumerator, out int fractionalDenominator)
    {
        // set the defaults
        wholeInches = 0;
        fractionalNumerator = 0;
        fractionalDenominator = 0;

        // get the values
        (int wi, int fn, int fd) = GetClosestMeasure(inchesValue);

        // now we need to know if the integer part at least equals in the source and the calculated amount.  
        // if the user passed a negative number, the inches will be zero, and so this parse can fail.
        decimal fv = inchesValue % 1.0000m;
        int wv = Convert.ToInt32(inchesValue - fv);

        if (wv != wi) { return false; }

        wholeInches = wi;
        fractionalNumerator = fn;
        fractionalDenominator = fd;

        return true;
    }

    public static bool TryParse(string sourceString, [NotNullWhen(true)]out Inches_old? parseResult)
    {
        parseResult = null;

        Inches_old parseWorker = new();
        if (parseWorker.TrySet(sourceString))
        {
            parseResult = parseWorker;
            return true;
        }

        return false;
    }

    

    public static int[] ValidDenominators { get; } = { 2, 4, 8, 16 };

    public static Regex StringSyntaxWholeAndFraction { get; } = new Regex("([1-9]+[0-9]*)[^0-9\\\\\\/]+([1-9]+[0-9]*)[\\\\\\/]([1-9]+[0-9]*)", RegexOptions.Compiled);

    public static Regex StringSyntaxWholeOnly { get; } = new Regex("[^\\-]([1-9]+[0-9]*)", RegexOptions.Compiled);

    public static Regex StringSyntaxDecimalValue { get; } = new Regex("[1-9]+[0-9]*\\.[0-9]+", RegexOptions.Compiled);





    public static bool operator ==(Inches_old left, Inches_old right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Inches_old left, Inches_old right)
    {
        return !left.Equals(right);
    }

    public static bool operator <(Inches_old left, Inches_old right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(Inches_old left, Inches_old right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(Inches_old left, Inches_old right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(Inches_old left, Inches_old right)
    {
        return left.CompareTo(right) >= 0;
    }


    public static Inches_old Add(Inches_old left, Inches_old right)
    {
        return new Inches_old(left._value + right._value);
    }

    public static Inches_old operator +(Inches_old left, Inches_old right) { return Add(left, right); }

    public static Inches_old Subtract(Inches_old left, Inches_old right)
    {
        return new Inches_old(left._value - right._value);
    }

    public static Inches_old operator -(Inches_old left, Inches_old right) { return Subtract(left, right); }


}
