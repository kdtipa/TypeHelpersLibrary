using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CustomTypes.Measures;

public struct Inches : IEquatable<Inches>, IComparable<Inches>
{
    public Inches() { }

    public Inches(int inches, int? fractionNumerator = null, int? fractionDenominator = null) 
    { 
        Whole = inches; 

        if (fractionNumerator is not null && fractionDenominator is not null)
        {
            Fraction = (fractionNumerator.Value, fractionDenominator.Value);
        }
    }

    public Inches(decimal inches)
    {
        if (!LoadFromDecimal(inches))
        {
            throw new ArgumentException("Can't create a negative measurement of distance", nameof(inches));
        }
    }

    public Inches(string inchesString)
    {
        if (!LoadFromString(inchesString))
        {
            throw new ArgumentException($"Unable to parse given parameter value [{inchesString}] as inches", nameof(inchesString));
        }
    }


    private int _inches = 0;
    private int _32nds = 0;


    public int Whole
    {
        get { return _inches; }
        set
        {
            if (value <= 0) { _inches = 0; }
            else { _inches = value; }
        }
    }

    public (int numerator, int denominator) Fraction
    {
        get
        {
            if (GetFractionalPart(_32nds, out int n, out int d))
            {
                return (n, d);
            }
            else
            {
                return (0, 1);
            }
        }
        set
        {
            int n = value.numerator;
            int d = value.denominator;

            if (n < 0 || !ValidFractionDenominators.Contains(d))
            {
                throw new ArgumentException("invalid fraction for inch measurement");
            }

            while (d < 32)
            {
                n *= 2;
                d *= 2;
            }

            // handle silly fractions like 79/32
            if (n >= d)
            {
                int betterN = n % d;
                int increaseWhole = (n - betterN) / d;
                _inches += increaseWhole;
                n = betterN;
            }

            _32nds = n;
        }
    }

    public decimal DecimalValue
    {
        get
        {
            decimal worker = _inches;
            worker += (decimal)_32nds / 32.00000m;
            return worker;
        }
    }

    private static bool GetFractionalPart(int thirty_secondths, out int numerator, out int denominator)
    {
        numerator = 0;
        denominator = 32;

        int worker = thirty_secondths;
        if (worker < 0 || worker > 32) { return false; }
        if (worker == 0) { denominator = 1; return true; }

        while (worker % 2 == 0 && denominator >= 2)
        {
            worker = worker / 2;
            denominator = denominator / 2;
        }

        numerator = worker;
        return true;
    }

    public static int[] ValidFractionDenominators { get; } = { 1, 2, 4, 8, 16, 32 };

    private bool LoadFromDecimal(decimal value)
    {
        if (value < 0) { return false; }

        decimal fractionalPart = value % decimal.One;
        int wholePart = (int)(value - fractionalPart);

        decimal fractionalTS = fractionalPart * 32.00000m; // TS = thirty-secondths
        fractionalTS = fractionalTS - (fractionalTS % decimal.One);
        int tsPart = (int)fractionalTS;

        Whole = wholePart;
        _32nds = tsPart;
        return true;
    }

    private bool LoadFromString(string sourceStr)
    {
        // decimal is easiest, so let's see if the source parses to a decimal
        if (decimal.TryParse(sourceStr, out decimal parsedDecimal))
        {
            LoadFromDecimal(parsedDecimal);
            return true;
        }

        // now let's see if it's something like "4 3/8".  we're relying on the decimal parse to catch if the fraction is not present like "4".
        Match parsedWithFraction = InchesPattern.Match(sourceStr);
        if (parsedWithFraction is not null && parsedWithFraction.Success)
        {
            int w = Convert.ToInt32(parsedWithFraction.Groups[1].Value);
            int n = Convert.ToInt32(parsedWithFraction.Groups[2].Value);
            int d = Convert.ToInt32(parsedWithFraction.Groups[3].Value);

            Whole = w;
            Fraction = (n, d);
            return true;
        }

        return false;
    }

    public static Regex InchesPattern { get; } = new Regex("/([1-9]+[0-9]*) ([1-9]+[0-9]*)[\\\\|\\/](2|4|8|16|32)", RegexOptions.Compiled);

    public static bool TryParse(string sourceStr, [NotNullWhen(true)] out Inches? parsedValue)
    {
        parsedValue = null;

        Inches worker = new();

        if (worker.LoadFromString(sourceStr))
        {
            parsedValue = worker;
            return true;
        }

        return false;
    }

    public int CompareTo(Inches other)
    {
        return DecimalValue.CompareTo(other.DecimalValue);
    }

    public bool Equals(Inches other)
    {
        return _inches == other._inches && _32nds == other._32nds;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is Inches inchObj) { return Equals(inchObj); }

        if (obj is decimal decObj) { return DecimalValue == decObj; }

        if (obj is int intObj) { return _inches == intObj && _32nds == 0; }

        if (obj is string strObj && TryParse(strObj, out var parsedInch))
        {
            return Equals(parsedInch);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return DecimalValue.GetHashCode();
    }

    public override string ToString()
    {
        var str = new StringBuilder();

        str.Append(_inches.ToString("#,##0"));

        if (_32nds > 0)
        {
            str.Append($" {Fraction.numerator}/{Fraction.denominator}");
        }

        return str.ToString();
    }


    public static bool operator ==(Inches left, Inches right) { return left.Equals(right); }
    public static bool operator !=(Inches left, Inches right) { return !left.Equals(right); }

    public static bool operator >(Inches left, Inches right) { return left.DecimalValue > right.DecimalValue; }
    public static bool operator <(Inches left, Inches right) { return left.DecimalValue < right.DecimalValue; }

    public static bool operator >=(Inches left, Inches right) { return left.DecimalValue >= right.DecimalValue; }
    public static bool operator <=(Inches left, Inches right) { return left.DecimalValue <= right.DecimalValue; }


    public static bool operator ==(Inches left, decimal right) { return left.DecimalValue == right; }
    public static bool operator !=(Inches left, decimal right) { return left.DecimalValue != right; }

    public static bool operator >(Inches left, decimal right) { return left.DecimalValue > right; }
    public static bool operator <(Inches left, decimal right) { return left.DecimalValue < right; }

    public static bool operator >=(Inches left, decimal right) { return left.DecimalValue >= right; }
    public static bool operator <=(Inches left, decimal right) { return left.DecimalValue <= right; }


    public static bool operator ==(decimal left, Inches right) { return left == right.DecimalValue; }
    public static bool operator !=(decimal left, Inches right) { return left != right.DecimalValue; }

    public static bool operator >(decimal left, Inches right) { return left > right.DecimalValue; }
    public static bool operator <(decimal left, Inches right) { return left < right.DecimalValue; }

    public static bool operator >=(decimal left, Inches right) { return left >= right.DecimalValue; }
    public static bool operator <=(decimal left, Inches right) { return left <= right.DecimalValue; }

}

public enum InchDenominators
{
    Wholes = 1, 
    Halves = 2, 
    Quarters = 4, 
    Eigths = 8,
    Sixteenths = 16,
    ThirtySecondths = 32
}
