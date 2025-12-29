using CustomTypes.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTypes;

/// <summary>
/// This struct is meant to be able to be used as a decimal, but gives 
/// you a little extra information about the number like making it easy 
/// to know how many decimal places the number has a value for and lets 
/// you limit the number of decimal places you care about.  The math and 
/// comparison operators are defined for DecimalWrapper and decimal.
/// </summary>
public struct DecimalWrapper : IEquatable<DecimalWrapper>, IComparable<DecimalWrapper>
{
    /// <summary>
    /// Gives a DecimalWrapper with a value of zero and three significant digits
    /// </summary>
    public DecimalWrapper() { }

    /// <summary>
    /// Gives a DecimalWrapper with your set value and optionally a number of significant digits
    /// </summary>
    /// <param name="significantDigits">Can be 0 or above, or null to default to 3</param>
    public DecimalWrapper(decimal value, int? significantDigits = null) 
    { 
        Value = value; 

        if (significantDigits is not null)
        {
            SignificantDigits = significantDigits.Value;
        }
    }

    /// <summary>
    /// Lets you pass in a value of other object types and it will try to parse a good list of 
    /// the other base types to find a decimal value (int, long, float, double, and string) to 
    /// try to set the initial value.
    /// </summary>
    /// <param name="significantDigits">Can be 0 or above, or null to default to 3</param>
    public DecimalWrapper(object otherTypeInitialValue, int? significantDigits = null)
    {
        var parsedVal = GetDecimalValue(otherTypeInitialValue);
        if (parsedVal is not null)
        {
            Value = parsedVal.Value;

            if (significantDigits is not null)
            {
                SignificantDigits = significantDigits.Value;
            }
        }
    }


    private decimal _value = decimal.Zero;
    private int _significantDigits = 3;
    private int _digitsWithValue = 0;

    private decimal OnlySignificantDigits(decimal val, int sigDig)
    {
        decimal mod = 1.00000m;
        for (int p = 1; p <= sigDig; p++)
        {
            mod *= 0.10000m;
        }

        return val - (val % mod);
    }

    /// <summary>
    /// This is the complete decimal value as given for setting the object
    /// </summary>
    public decimal Value
    {
        get { return _value; }
        set
        {
            _value = value;
            _digitsWithValue = DecimalHelper.DecimalPlaceDigits(value);
        }
    }

    /// <summary>
    /// This is the value but with anything past the significant 
    /// digit count removed via modulus.
    /// </summary>
    public decimal LimitedValue { get { return OnlySignificantDigits(_value, _significantDigits); } }

    /// <summary>
    /// This is the part of the value to the right of the decimal point
    /// </summary>
    public decimal FractionalPart 
    { 
        get { return _value % 1.00000m; } 
        set
        {
            decimal fpVal = value % 1.00000m;
            bool fpPositive = fpVal >= 0.00000m;
            decimal ipVal = _value - (_value % 1.00000m);
            bool ipPositive = ipVal >= 0.00000m;

            if (fpPositive != ipPositive) { fpVal *= -1.00000m; }
            
            Value = ipVal + fpVal;
        }
    }

    /// <summary>
    /// This is the part of the value to the left of the decimal point
    /// </summary>
    public decimal IntegerPart
    {
        get { return _value - (_value % 1.00000m); }
        set
        {
            decimal fpVal = _value % 1.00000m;
            bool fpPositive = fpVal >= 0.00000m;
            decimal ipVal = value - (value % 1.00000m);
            bool ipPositive = ipVal >= 0.00000m;

            if (fpPositive != ipPositive) { fpVal *= -1.00000m; }

            Value = ipVal + fpVal;
        }
    }

    /// <summary>
    /// This is a convenience property that tells you how many digits 
    /// to the right of the decimal point have value.  So, 1.04050600000 
    /// would come back as 6.
    /// </summary>
    public int DigitsWithValue { get { return _digitsWithValue; } }

    /// <summary>
    /// A number set by you telling the object how many digits to the 
    /// right of the decimal point you care about.  So, if you have 
    /// the value 3.14159 and you make SignificantDigits equal to 2, 
    /// the LimitedValue will be 3.14
    /// </summary>
    public int SignificantDigits
    {
        get { return _significantDigits; }
        set
        {
            if (value <= 0) { _significantDigits = 0; }
            else if (value > 28) { _significantDigits = 28; }
            else { _significantDigits = value; }
        }
    }

    public int CompareTo(DecimalWrapper other)
    {
        return _value.CompareTo(other._value);
    }

    public bool Equals(DecimalWrapper other)
    {
        // if the integer part isn't equal we know the whole thing isn't equal
        if (IntegerPart != other.IntegerPart) { return false; }

        // if the fractional parts are now equal after finding out the integer part is, we don't have to anything fancy
        if (FractionalPart == other.FractionalPart) { return true; }

        // now we need to make sure the fractional parts have the same number of decimal digits that are the same
        int sd = SignificantDigits >= other.SignificantDigits ? SignificantDigits : other.SignificantDigits;
        return OnlySignificantDigits(_value, sd) == OnlySignificantDigits(other._value, sd);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        var parseVal = GetDecimalValue(obj);

        if (parseVal is null) { return false; }

        return Value == parseVal.Value;
    }

    private decimal? GetDecimalValue([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return null; }

        if (obj is DecimalWrapper dwObj) { return dwObj.Value; }

        if (obj is decimal dObj) { return dObj; }

        if (obj is int iObj) { return iObj; }

        if (obj is long lObj) { return lObj; }

        if (obj is double dblObj) { return Convert.ToDecimal(dblObj); }

        if (obj is float fObj) { return Convert.ToDecimal(fObj); }

        if (obj is string strObj && decimal.TryParse(strObj, out decimal parseVal)) { return parseVal; }

        return null;
    }


    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    /// <summary>
    /// The default ToString.  In this case it works on the complete value 
    /// which is NOT limited by the significant digits.
    /// </summary>
    public override string ToString()
    {
        return Value.ToString();
    }

    /// <summary>
    /// Default ToString that allows formatting.  It works on the complete 
    /// value, so you can get parts of the value outside the significant digits.
    /// </summary>
    public string ToString(string format, IFormatProvider formatProvider)
    {
        return Value.ToString(format, formatProvider);
    }

    /// <summary>
    /// Works the same as the default ToString with no parameters, but limits 
    /// the result to the significant digits value.
    /// </summary>
    public string ToStringSignificantDigits()
    {
        return Value.ToString("0.".PadRight(SignificantDigits, '0'));
    }


    #region operators
    //=============================================================================================

    // Decimal Wrapper and Decimal Wrapper
    public static bool operator ==(DecimalWrapper left, DecimalWrapper right) { return left.Equals(right); }
    public static bool operator !=(DecimalWrapper left, DecimalWrapper right) { return !left.Equals(right); }
    public static bool operator >(DecimalWrapper left, DecimalWrapper right) { return left.Value > right.Value; }
    public static bool operator <(DecimalWrapper left, DecimalWrapper right) { return left.Value < right.Value; }
    public static bool operator >=(DecimalWrapper left, DecimalWrapper right) { return left.Value >= right.Value; }
    public static bool operator <=(DecimalWrapper left, DecimalWrapper right) { return left.Value <= right.Value; }
    public static DecimalWrapper operator +(DecimalWrapper left, DecimalWrapper right) { return new DecimalWrapper(left.Value + right.Value); }
    public static DecimalWrapper operator -(DecimalWrapper left, DecimalWrapper right) { return new DecimalWrapper(left.Value - right.Value); }
    public static DecimalWrapper operator *(DecimalWrapper left, DecimalWrapper right) { return new DecimalWrapper(left.Value * right.Value); }
    public static DecimalWrapper operator /(DecimalWrapper left, DecimalWrapper right) { return new DecimalWrapper(left.Value / right.Value); }
    public static DecimalWrapper operator %(DecimalWrapper left, DecimalWrapper right) { return new DecimalWrapper(left.Value % right.Value); }


    // Decimal Wrapper and decimal
    public static bool operator ==(DecimalWrapper left, decimal right) { return left.Value == right; }
    public static bool operator !=(DecimalWrapper left, decimal right) { return left.Value != right; }
    public static bool operator >(DecimalWrapper left, decimal right) { return left.Value > right; }
    public static bool operator <(DecimalWrapper left, decimal right) { return left.Value < right; }
    public static bool operator >=(DecimalWrapper left, decimal right) { return left.Value >= right; }
    public static bool operator <=(DecimalWrapper left, decimal right) { return left.Value <= right; }
    public static DecimalWrapper operator +(DecimalWrapper left, decimal right) { return new DecimalWrapper(left.Value + right); }
    public static DecimalWrapper operator -(DecimalWrapper left, decimal right) { return new DecimalWrapper(left.Value - right); }
    public static DecimalWrapper operator *(DecimalWrapper left, decimal right) { return new DecimalWrapper(left.Value * right); }
    public static DecimalWrapper operator /(DecimalWrapper left, decimal right) { return new DecimalWrapper(left.Value / right); }
    public static DecimalWrapper operator %(DecimalWrapper left, decimal right) { return new DecimalWrapper(left.Value % right); }


    // decimal and Decimal Wrapper
    public static bool operator ==(decimal left, DecimalWrapper right) { return left == right.Value; }
    public static bool operator !=(decimal left, DecimalWrapper right) { return left != right.Value; }
    public static bool operator >(decimal left, DecimalWrapper right) { return left > right.Value; }
    public static bool operator <(decimal left, DecimalWrapper right) { return left < right.Value; }
    public static bool operator >=(decimal left, DecimalWrapper right) { return left >= right.Value; }
    public static bool operator <=(decimal left, DecimalWrapper right) { return left <= right.Value; }
    public static decimal operator +(decimal left, DecimalWrapper right) { return left + right.Value; }
    public static decimal operator -(decimal left, DecimalWrapper right) { return left - right.Value; }
    public static decimal operator *(decimal left, DecimalWrapper right) { return left * right.Value; }
    public static decimal operator /(decimal left, DecimalWrapper right) { return left / right.Value; }
    public static decimal operator %(decimal left, DecimalWrapper right) { return left % right.Value; }




    //=============================================================================================
    #endregion


}
