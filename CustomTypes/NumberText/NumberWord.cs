using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CustomTypes.NumberText;

public struct NumberWord : IEquatable<NumberWord>, IComparable<NumberWord>
{
    public NumberWord() { }

    public NumberWord(decimal initialValue, CultureInfo? language = null)
    {
        _language = language ?? CultureInfo.CurrentCulture; // setting the private property avoids the Text update
        Value = initialValue; // updates the integer and fractional parts
        Text = CalculateText();
    }

    public NumberWord(int initialValue, CultureInfo? language = null)
    {
        _language = language ?? CultureInfo.CurrentCulture;
        Value = initialValue;
        Text = CalculateText();
    }

    public NumberWord(double initialValue, CultureInfo? language = null)
    {
        _language = language ?? CultureInfo.CurrentCulture; // setting the private property avoids the Text update
        Value = Convert.ToDecimal(initialValue); // updates the integer and fractional parts
        Text = CalculateText();
    }

    

    /// <summary>
    /// The numeric value of this number
    /// </summary>
    public decimal Value
    {
        get { return _value; }
        set
        {
            if (value != _value)
            {
                _value = value;
                FractionalPart = value - (value % 1.00000m);
                IntegerPart = Convert.ToInt32(value - FractionalPart);
                Text = CalculateText();
            }
        }
    }
    private decimal _value = 0.00000m;

    /// <summary>
    /// The integer part of the base value.  3.141592539 would give you 3.
    /// </summary>
    public int IntegerPart { get; private set; } = 0;

    /// <summary>
    /// The fractional part of the base value.  3.141592539 would give you 0.141592539
    /// </summary>
    public decimal FractionalPart { get; private set; } = 0.00000m;

    /// <summary>
    /// The words that represent the number.  3.14 would give you "Three Point One Four"
    /// </summary>
    public string Text { get; private set; } = "Zero"; // magic string... I know.  But the Invariant Culture setting is English in my lookup info.

    /// <summary>
    /// Returns true if the whole value is zero or if the integer part is not zero
    /// </summary>
    public bool HasIntegerPart { get { return (IntegerPart == 0 && FractionalPart == 0.00000m) || (IntegerPart != 0); } }

    /// <summary>
    /// Returns true if the fractional part is not zero
    /// </summary>
    public bool HasFractionalPart { get { return FractionalPart != 0.00000m; } }

    /// <summary>
    /// Defaults to Invariant Culture which in the dictionary is equivalant to United Stated English.  Set to a 
    /// different language to adjust the text value
    /// </summary>
    public CultureInfo Language
    {
        get { return _language; }
        set
        {
            if (_language != value)
            {
                _language = value;
                Text = CalculateText();
            }
        }
    }
    private CultureInfo _language = CultureInfo.InvariantCulture;


    private string CalculateText()
    {
        var key = new NumberWordValueLanguageKey(0, _language);

        if (_value == 0.00000m) { return NumberWordLookUp.SpecialNumberWords[key]; }

        var result = new StringBuilder();

        bool isNegative = _value < 0.00000m;

        if (IntegerPart >= -19 && IntegerPart <= 19)
        {
            if (isNegative)
            {
                result.Append(NumberWordLookUp.NegativeWord[_language]);
                result.Append(' ');
            }

            key = new NumberWordValueLanguageKey(IntegerPart, _language);
            result.Append(NumberWordLookUp.SpecialNumberWords[key]);
        }
        else
        {
            // we know at this point we don't have a zero or a small number, 
            // so it's time to loop and build our string
            var workVal = IntegerPart;
            var bigNumberKeyVal = 1;
            while (workVal != 0)
            {
                var chunk = GetThreeDigitPart(workVal, out int reducedVal);
                var chunkKey = new NumberWordValueLanguageKey(bigNumberKeyVal, _language);

                // do we need the word "thousand" or "million"?
                if (bigNumberKeyVal > 1)
                {
                    result.Insert(0, ' ');
                    result.Insert(0, NumberWordLookUp.BigNumberWords[new NumberWordValueLanguageKey(bigNumberKeyVal, _language)]);
                }

                // chunk is a value from 0 to 999 and we need the words for it.
                result.Insert(0, ' ');
                result.Insert(0, GetThreeDigitPartWords(chunk));


                workVal = reducedVal;
                bigNumberKeyVal *= 1000;
            }
        }




        // and if we have a decimal part, we now loop through that
        if (HasFractionalPart)
        {
            result.Append(' ');
            result.Append(NumberWordLookUp.DecimalPointWord[_language]);
            result.Append(' ');

            // now loop
            decimal workDec = FractionalPart;
            while (workDec != 0.00000m)
            {
                workDec *= 10.00000m;
                int nextDigit = Convert.ToInt32(workDec - (workDec % 1.00000m));
                workDec = workDec % 1.00000m;

                result.Append(' ');
                result.Append(NumberWordLookUp.SpecialNumberWords[new NumberWordValueLanguageKey(nextDigit, _language)]);
            }
        }


        return result.ToString().Trim().Replace("  ", " ");
    }

    /// <summary>
    /// helper method to get the lowest three digits of an integer, and 
    /// get you the reduced source value, to help with looping.  For 
    /// example: 4,567,890 would return 890 and the reduced value would 
    /// be 4,567.  The return value is always a positive number so you 
    /// can rely on that while not wrecking the negativity of the source.
    /// </summary>
    private int GetThreeDigitPart(int srcVal, out int reducedSrcVal)
    {
        int result = srcVal % 1000;
        reducedSrcVal = (srcVal - result) / 1000;
        if (result < 0) { result *= -1; }
        return result;
    }

    /// <summary>
    /// won't work with a negative number because the word negative only 
    /// gets applied once at the beginning of the text.  So, by the time 
    /// you're calling this method, it should be a positive number.  It 
    /// also requires that the source value be 0 to 999.  And if it is 
    /// zero, it passes back an empty string.
    /// </summary>
    private string GetThreeDigitPartWords(int srcVal)
    {
        if (srcVal <= 0 || srcVal > 999) { return string.Empty; }

        int onesPlace = srcVal % 10;
        int tensPlace = (srcVal - onesPlace) / 10;
        int hundredsPlace = (srcVal - tensPlace - onesPlace) / 100;

        var tdpw = new StringBuilder();
        // hundreds first
        if (hundredsPlace > 0)
        {
            tdpw.Append(NumberWordLookUp.SpecialNumberWords[new NumberWordValueLanguageKey(hundredsPlace, _language)]);
            tdpw.Append(' ');
            tdpw.Append(NumberWordLookUp.BigNumberWords[new NumberWordValueLanguageKey(100, _language)]);
        }

        // tens next
        if (tensPlace > 0)
        {
            if (hundredsPlace > 0) { tdpw.Append(' '); }
            tdpw.Append(NumberWordLookUp.TensNumberWords[new NumberWordValueLanguageKey(tensPlace * 10, _language)]);
        }

        // ones place last
        if (onesPlace > 0)
        {
            if (tensPlace > 0) { tdpw.Append('-'); }
            else if (hundredsPlace > 0) { tdpw.Append(' '); }

            tdpw.Append(NumberWordLookUp.SpecialNumberWords[new NumberWordValueLanguageKey(onesPlace, _language)]);
        }


        return tdpw.ToString();
    }


    // ToDo: LoadFromText(string sourceString)



    public static bool IsAllowed(object? testValue, [NotNullWhen(true)] out Type? parsedType)
    {
        parsedType = null;
        if (testValue is null) { return false; }

        Type testType = testValue.GetType();
        if (AllowedNumericTypes.Contains(testType))
        {
            parsedType = testType;
            return true;
        }

        return false;
    }

    public static Type[] AllowedNumericTypes { get; } =
    {
        typeof(int), typeof(short), typeof(ushort),
        typeof(decimal),
        typeof(double), typeof(float),
    };

    // ToDo: TryParse(string sourceString, out NumberWord? result)


    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    public override string ToString()
    {
        return Text;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is NumberWord nwObj) { return Equals(nwObj); }

        if (obj is decimal dObj) { return _value == dObj; }

        if (obj is int iObj) { return HasIntegerPart && !HasFractionalPart && IntegerPart == iObj; }


        return false;
    }

    public bool Equals(NumberWord other)
    {
        return _value == other._value;
    }

    public int CompareTo(NumberWord other)
    {
        return _value.CompareTo(other._value);
    }


    public static NumberWord operator +(NumberWord left, NumberWord right)
    {
        return new NumberWord(left._value + right._value);
    }

    public static NumberWord operator -(NumberWord left, NumberWord right)
    {
        return new NumberWord(left._value - right._value);
    }

    public static NumberWord operator *(NumberWord left, NumberWord right)
    {
        return new NumberWord(left._value * right._value);
    }

    public static NumberWord operator /(NumberWord left, NumberWord right)
    {
        return new NumberWord(left._value / right._value);
    }

    public static NumberWord operator +(NumberWord left, decimal right)
    {
        return new NumberWord(left._value + right);
    }

    public static NumberWord operator -(NumberWord left, decimal right)
    {
        return new NumberWord(left._value - right);
    }

    public static NumberWord operator *(NumberWord left, decimal right)
    {
        return new NumberWord(left._value * right);
    }

    public static NumberWord operator /(NumberWord left, decimal right)
    {
        return new NumberWord(left._value / right);
    }

    public static decimal operator +(decimal left, NumberWord right)
    {
        return left + right._value;
    }

    public static decimal operator -(decimal left, NumberWord right)
    {
        return left - right._value;
    }

    public static decimal operator *(decimal left, NumberWord right)
    {
        return left * right._value;
    }

    public static decimal operator /(decimal left, NumberWord right)
    {
        return left / right._value;
    }



    public static bool operator ==(NumberWord left, NumberWord right) { return left.Equals(right); }
    public static bool operator !=(NumberWord left, NumberWord right) { return !left.Equals(right); }

    public static bool operator >(NumberWord left, NumberWord right) { return left._value > right._value; }
    public static bool operator <(NumberWord left, NumberWord right) { return left._value < right._value; }

    public static bool operator >=(NumberWord left, NumberWord right) { return left._value > right._value; }
    public static bool operator <=(NumberWord left, NumberWord right) { return left._value < right._value; }




}
