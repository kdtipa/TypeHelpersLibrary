using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTypes;

public struct DisplayNumber : IEquatable<DisplayNumber>, IComparable<DisplayNumber>
{
    public DisplayNumber() { }
    public DisplayNumber(decimal sourceValue)
    {
        setValues(sourceValue);
    }

    public DisplayNumber(string sourceValue)
    {
        if (decimal.TryParse(sourceValue, out var parseVal))
        {
            setValues(parseVal);
        }
        else
        {
            throw new ArgumentException($"Unable to parse a valid decimal value from the given string [{sourceValue}]");
        }
    }

    private decimal _preciseValue = 0.00000m;
    /// <summary>
    /// We keep the precise value.  Whatever you set to this object will be stored here exactly.  
    /// The DisplayValue property gets set to the value but only to 3 decimal digits.
    /// </summary>
    public decimal PreciseValue
    {
        get { return _preciseValue * (_isNegative ? -1.0000m : 1.0000m); }
        set
        {
            setValues(value);
        }
    }

    private decimal _displayValue = 0.0m;
    public decimal DisplayValue
    {
        get { return _displayValue * (_isNegative ? -1.0000m : 1.0000m); }
        set
        {
            setValues(value);
        }
    }

    private bool _isNegative = false;
    private long _integerPart = 0;
    private int _tenthsPlace = 0;
    private int _hundredthsPlace = 0;
    private int _thousandthsPlace = 0;

    private int _maxDecimalPlaces = 1; // default to showing one decimal place
    public int MaxDecimalPlaces
    {
        get { return _maxDecimalPlaces; }
        set
        {
            if (value < 0 || value > 3) { _maxDecimalPlaces = 1; }
            else { _maxDecimalPlaces = value; }
        }
    }

    public bool HasFractionalValue { get { return _tenthsPlace > 0 || _hundredthsPlace > 0 || _thousandthsPlace > 0; } }

    private void setValues(decimal sourceValue)
    {
        _isNegative = sourceValue < 0;
        decimal absMultiplier = sourceValue >= 0.0000m ? 1.0000m : -1.0000m;
        _preciseValue = sourceValue * absMultiplier;
        decimal sourceWorker = sourceValue * absMultiplier;

        decimal fractionalPart = sourceWorker % 1.00000m;
        _integerPart = Convert.ToInt64((sourceWorker - fractionalPart) * absMultiplier);

        decimal worker = fractionalPart * 10.00000m; // separate tenths place from smaller parts
        fractionalPart = worker % 1.00000m; // get the smaller parts
        _tenthsPlace = Convert.ToInt32(worker - fractionalPart);

        worker = fractionalPart * 10.00000m; // get the hundredths place
        fractionalPart = worker % 1.00000m; // get the smaller parts
        _hundredthsPlace = Convert.ToInt32(worker - fractionalPart);

        worker = fractionalPart * 10.00000m; // get the thousandths place
        fractionalPart = worker % 1.00000m; // get the remainder because we might still need to round that last place
        _thousandthsPlace = Convert.ToInt32(worker - fractionalPart);

        if (fractionalPart >= 0.50000m)
        {
            _thousandthsPlace += 1;
            if (_thousandthsPlace > 9)
            {
                _hundredthsPlace += 1;
                _thousandthsPlace = 0;

                if (_hundredthsPlace > 9)
                {
                    _tenthsPlace += 1;
                    _hundredthsPlace = 0;

                    if (_tenthsPlace > 9)
                    {
                        _integerPart += 1;
                        _tenthsPlace = 0;
                    }
                }
            }
        }

        //now that those things are set, lets set our display value from the cultivated values...
        _displayValue = Convert.ToDecimal(_integerPart) * absMultiplier;
        _displayValue += 0.10000m * _tenthsPlace;
        _displayValue += 0.01000m * _hundredthsPlace;
        _displayValue += 0.00100m * _thousandthsPlace;
        _displayValue *= absMultiplier; // put it back to what it's supposed to be
    }

    public bool Equals(DisplayNumber other)
    {
        return _displayValue == other._displayValue;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is DisplayNumber dnObj) { return Equals(dnObj); }

        if (obj is decimal dObj) { return Equals(new DisplayNumber(dObj)); }

        if (obj is double dblObj) { return Equals(new DisplayNumber(Convert.ToDecimal(dblObj))); }

        if (obj is int iObj) { return Equals(new DisplayNumber(iObj)); }

        if (obj is string sObj && decimal.TryParse(sObj, out decimal parsedDObj)) { return Equals(new DisplayNumber(parsedDObj)); }

        return false;
    }

    public override int GetHashCode()
    {
        return _displayValue.GetHashCode();
    }

    /// <summary>
    /// Gives you a relatively nice display version of the value.  Something like 
    /// 2,420,175 could come back as "2.4 Million".  Or 3.14159 could be returned 
    /// as 3.14 (if set to max 2 digits).
    /// </summary>
    public override string ToString()
    {
        // start with BIG numbers...
        List<long> mults = LargeNumberLabels.Keys.ToList();
        mults.Sort(); // just make sure
        int multCount = mults.Count;

        long div = 1;
        string label = string.Empty;

        for (int mult = multCount - 1; mult >= 0; mult--)
        {
            if (_integerPart >= mults[mult])
            {
                div = mults[mult];
                label = LargeNumberLabels[div];
                break;
            }
        }

        // now we know if we are thousands or above or not, so next comes building the output
        var output = new StringBuilder();

        if (_isNegative) { output.Append('-'); }

        if (div > 1)
        {
            long intPart = (_integerPart - (_integerPart % div)) / div;
            output.Append(intPart);
            output.Append($" {label}");
            return output.ToString();
        }

        //if we got here, it means it's a small enough number to not need a label like thousands or more
        output.Append(_integerPart);

        if (HasFractionalValue && MaxDecimalPlaces > 0)
        {
            output.Append('.');
            output.Append(_tenthsPlace);

            if (MaxDecimalPlaces > 1 && (_hundredthsPlace > 0 || _thousandthsPlace > 0))
            {
                output.Append(_hundredthsPlace);

                if (MaxDecimalPlaces > 2 && _thousandthsPlace > 0)
                {
                    output.Append(_thousandthsPlace);
                }
            }
        }

        return output.ToString();
    }

    public int CompareTo(DisplayNumber other)
    {
        return _preciseValue.CompareTo(other._preciseValue);
    }

    public static Dictionary<long, string> LargeNumberLabels = new()
    {
        { 1000, "Thousand" },
        { 1000000, "Million" },
        { 1000000000, "Billion" },
        { 1000000000000, "Trillion" },
        { 1000000000000000, "Quadrillion" }
    };


    public static bool operator ==(DisplayNumber left, DisplayNumber right) { return left.Equals(right); }
    public static bool operator !=(DisplayNumber left, DisplayNumber right) { return !left.Equals(right); }
    public static bool operator >(DisplayNumber left, DisplayNumber right) { return left.PreciseValue > right.PreciseValue; }
    public static bool operator <(DisplayNumber left, DisplayNumber right) { return left.PreciseValue < right.PreciseValue; }
    public static bool operator >=(DisplayNumber left, DisplayNumber right) { return left.PreciseValue >= right.PreciseValue; }
    public static bool operator <=(DisplayNumber left, DisplayNumber right) { return left.PreciseValue <= right.PreciseValue; }


    public static bool operator ==(DisplayNumber left, decimal right) { return left.Equals(right); }
    public static bool operator !=(DisplayNumber left, decimal right) { return !left.Equals(right); }
    public static bool operator >(DisplayNumber left, decimal right) { return left.PreciseValue > right; }
    public static bool operator <(DisplayNumber left, decimal right) { return left.PreciseValue < right; }
    public static bool operator >=(DisplayNumber left, decimal right) { return left.PreciseValue >= right; }
    public static bool operator <=(DisplayNumber left, decimal right) { return left.PreciseValue <= right; }


    public static bool operator ==(decimal left, DisplayNumber right) { return right.Equals(left); }
    public static bool operator !=(decimal left, DisplayNumber right) { return !right.Equals(left); }
    public static bool operator >(decimal left, DisplayNumber right) { return left > right.PreciseValue; }
    public static bool operator <(decimal left, DisplayNumber right) { return left < right.PreciseValue; }
    public static bool operator >=(decimal left, DisplayNumber right) { return left >= right.PreciseValue; }
    public static bool operator <=(decimal left, DisplayNumber right) { return left <= right.PreciseValue; }



    public static DisplayNumber operator +(DisplayNumber left, DisplayNumber right) { return new DisplayNumber(left.PreciseValue + right.PreciseValue); }
    public static DisplayNumber operator -(DisplayNumber left, DisplayNumber right) { return new DisplayNumber(left.PreciseValue - right.PreciseValue); }
    public static DisplayNumber operator *(DisplayNumber left, DisplayNumber right) { return new DisplayNumber(left.PreciseValue * right.PreciseValue); }
    public static DisplayNumber operator /(DisplayNumber left, DisplayNumber right) { return new DisplayNumber(left.PreciseValue / right.PreciseValue); }
    public static DisplayNumber operator %(DisplayNumber left, DisplayNumber right) { return new DisplayNumber(left.PreciseValue % right.PreciseValue); }


    public static DisplayNumber operator +(DisplayNumber left, decimal right) { return new DisplayNumber(left.PreciseValue + right); }
    public static DisplayNumber operator -(DisplayNumber left, decimal right) { return new DisplayNumber(left.PreciseValue - right); }
    public static DisplayNumber operator *(DisplayNumber left, decimal right) { return new DisplayNumber(left.PreciseValue * right); }
    public static DisplayNumber operator /(DisplayNumber left, decimal right) { return new DisplayNumber(left.PreciseValue / right); }
    public static DisplayNumber operator %(DisplayNumber left, decimal right) { return new DisplayNumber(left.PreciseValue % right); }


    public static decimal operator +(decimal left, DisplayNumber right) { return left + right.PreciseValue; }
    public static decimal operator -(decimal left, DisplayNumber right) { return left - right.PreciseValue; }
    public static decimal operator *(decimal left, DisplayNumber right) { return left * right.PreciseValue; }
    public static decimal operator /(decimal left, DisplayNumber right) { return left / right.PreciseValue; }
    public static decimal operator %(decimal left, DisplayNumber right) { return left % right.PreciseValue; }



}
