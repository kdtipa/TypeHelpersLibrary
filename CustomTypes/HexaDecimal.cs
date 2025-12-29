using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using CustomTypes.Helpers;

namespace CustomTypes;

public struct HexaDecimal : IEquatable<HexaDecimal>, IComparable<HexaDecimal>
{
    public HexaDecimal() { }

    public HexaDecimal(long decimalValue)
    {
        DecimalValue = decimalValue;
    }

    public HexaDecimal(string hexadecimalString)
    {
        Value = hexadecimalString;
    }

    private long _decimalValue = 0;
    private string _hexadecimalString = "0";

    private string translateToHexaDecimal(long decValue, bool? useLowerCase = null)
    {
        if (decValue == 0) { return "0"; }

        var isNegative = decValue < 0;
        var resultStr = new StringBuilder();

        var workValue = decValue;
        long currentMod = 16;

        while (workValue != 0)
        {
            var modResult = workValue % currentMod; // keep negative if it's there
            workValue -= modResult; // remove the bit we're working with right now.  remember subtracting a negative from a negative is moving closer to zero.

            var digitChar = GetDigitFromDecimalValue(AbsVal(modResult), useLowerCase);
            resultStr.Insert(0, digitChar);
        }

        if (isNegative) { resultStr.Insert(0, '-'); }

        return resultStr.ToString();
    }

    /// <summary>
    /// must be used on number greater than min val, but 
    /// that should be the case since I'll always be using 
    /// it on results of a mod operation.
    /// </summary>
    private long AbsVal(long sourceVal)
    {
        if (sourceVal < 0) { return sourceVal * -1; }
        return sourceVal;
    }

    /// <summary>
    /// The one problem with this method at the moment 
    /// is that it can't handle min value for long
    /// </summary>
    private long? translateToLong(string hexVal)
    {
        if (!IsValidHexadecimalString(hexVal)) { return null; }

        if (hexVal == "0") { return 0; }

        long result = 0;
        var cleanString = hexVal.Replace(" ", "");
        var cleanLen = cleanString.Length;
        var isNegative = cleanLen > 1 && cleanString[0] == '-';
        if (isNegative)
        {
            cleanString = cleanString.Substring(1);
            cleanLen = cleanString.Length;
        }

        // now that we have a string with only hexadecimal digits,
        // we loop through them and add them to the return value.
        // of course, we have to start at the ones digit...
        long multiplier = 1;
        for (var i = cleanLen - 1; i >= 0; i--)
        {
            var digit = cleanString[i];
            if (HexadecimalDigitValues.ContainsKey(digit))
            {
                result += HexadecimalDigitValues[digit] * multiplier;
                multiplier *= 16;
            }
            else
            {
                // if the validity check failed, we might get a character we don't have a value for
                return null;
            }
        }


        if (isNegative)
        {
            result *= -1;
        }

        return result;
    }

    public long DecimalValue
    {
        get { return _decimalValue; }
        set
        {
            _decimalValue = value;
            _hexadecimalString = translateToHexaDecimal(_decimalValue);
        }
    }

    public string Value
    {
        get { return _hexadecimalString; }
        set
        {
            var testVal = translateToLong(value);
            if (testVal is null)
            {
                throw new ArgumentException($"Given value [{value.LimitString(12, true)}] does not translate to a hexadecimal value");
            }

            _decimalValue = testVal.Value;
            _hexadecimalString = translateToHexaDecimal(_decimalValue);
        }
    }




    public int CompareTo(HexaDecimal other)
    {
        return _decimalValue.CompareTo(other._decimalValue);
    }

    public bool Equals(HexaDecimal other)
    {
        return _decimalValue == other._decimalValue;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is HexaDecimal other) { return Equals(other); }

        if (obj is long longObj) { return _decimalValue == longObj; }

        if (obj is int intObj) { return _decimalValue == intObj; }

        if (obj is string strObj)
        {
            // parse string
            // return whether the equates to a hexadecimal value that is equal to this
            return false;
        }

        return false;
    }

    public override int GetHashCode() { return _decimalValue.GetHashCode(); }

    public override string ToString()
    {
        return _hexadecimalString;
    }


    public static char[] ValidHexadecimalDigits = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D', 'E', 'F'];

    public static Dictionary<char, int> HexadecimalDigitValues = new()
    {
        { '0', 0 }, { '1', 1 }, { '2', 2 }, { '3', 3 }, { '4', 4 },
        { '5', 5 }, { '6', 6 }, { '7', 7 }, { '8', 8 }, { '9', 9 },
        { 'a', 10 }, { 'b', 11 }, { 'c', 12 }, { 'd', 13 }, { 'e', 14 }, { 'f', 15 },
        { 'A', 10 }, { 'B', 11 }, { 'C', 12 }, { 'D', 13 }, { 'E', 14 }, { 'F', 15 }
    };

    /// <summary>
    /// A potentially useful method (definitely useful under the hood) that gets you the hexadecimal 
    /// digit that matches a decimal value from 0 to 15.  If you pass a value other than that, you'll 
    /// get null as a return value.  You can specify whether you want upper or lower case for the 
    /// digits from A to F.  Defaults to upper case.
    /// </summary>
    /// <param name="decimalValue">The value from 0 to 15 that you want the hexadecimal digit for.</param>
    /// <param name="useLowerCase">Whether you want the A to F digits to be lower case or not.</param>
    /// <returns>null if a value outside 0 to 15, and then 0 to 9 + A to F</returns>
    public static char? GetDigitFromDecimalValue(long decimalValue, bool? useLowerCase = null)
    {
        if (decimalValue < 0 || decimalValue > 15) { return null; }

        var ulc = false;
        if (useLowerCase is not null && useLowerCase.Value == true)
        {
            ulc = true;
        }

        var possibleVals = HexadecimalDigitValues.Where(entry => entry.Value == decimalValue).ToList();

        // shouldn't be possible but, to make the compiler happy...
        if (possibleVals is null || possibleVals.Count == 0) { return null; }

        // if we only have one value, it's one of the normal numeric digits, so just return it
        if (possibleVals.Count == 1) { return possibleVals[0].Key; }

        // at this point we must have 2 _values.  We return the one that matches the case specified by the parameter
        if (ulc) { return possibleVals[0].Key; }
        else { return possibleVals[1].Key; }
    }

    public static bool IsValidHexadecimalString(string testString)
    {
        if (string.IsNullOrWhiteSpace(testString)) { return false; }

        var cleanTest = testString.Trim();
        var testLen = cleanTest.Length;
        var isNegative = cleanTest[0] == '-' && testLen > 1;
        if (isNegative)
        {
            cleanTest = cleanTest.Substring(1).Trim(); // trim again in case they put a space after the dash
            testLen = cleanTest.Length;
        }

        for (var i = 0; i < testLen; i++)
        {
            if (!ValidHexadecimalDigits.Contains(cleanTest[i]))
            {
                return false;
            }
        }

        return true;
    }

    public static bool TryParse(string hexadecimalString, out HexaDecimal parseResult)
    {
        parseResult = new HexaDecimal();

        // this private translate method returns null if the parse fails
        var longVal = parseResult.translateToLong(hexadecimalString);
        if (longVal is null) { return false; }

        parseResult.DecimalValue = longVal.Value;
        return true;
    }

    /// <summary>
    /// Gets you a HexaDecimal, but will return a zero if the source string 
    /// isn't a hexadecimal string.  If it's important whether the string is 
    /// valid hexadecimal, use TryParse instead.
    /// </summary>
    public static HexaDecimal Parse(string hexadecimalString)
    {
        if (TryParse(hexadecimalString, out HexaDecimal parseResult))
        {
            return parseResult;
        }
        else
        {
            return new HexaDecimal();
        }
    }

    /// <summary>
    /// Uses the date, time, and a pseudo random number to create a key that is 
    /// very unlikely to accidentally match another such key generated this way.
    /// </summary>
    public static string GenerateHexaDecimalKey()
    {
        var theDate = DateTime.Now;
        var rng = new Random();

        long dtVal = long.Parse($"{rng.Next(1,9)}{theDate:ddMMyyyyfffHHssmm}");
        HexaDecimal result = new(dtVal);

        return result.Value;
    }

    //==============================

    public static bool operator ==(HexaDecimal left, HexaDecimal right)
    {
        return left._decimalValue == right._decimalValue;
    }

    public static bool operator !=(HexaDecimal left, HexaDecimal right)
    {
        return left._decimalValue != right._decimalValue;
    }

    public static bool operator >(HexaDecimal left, HexaDecimal right)
    {
        return left._decimalValue > right._decimalValue;
    }

    public static bool operator <(HexaDecimal left, HexaDecimal right)
    {
        return left._decimalValue < right._decimalValue;
    }

    public static bool operator >=(HexaDecimal left, HexaDecimal right)
    {
        return left._decimalValue >= right._decimalValue;
    }

    public static bool operator <=(HexaDecimal left, HexaDecimal right)
    {
        return left._decimalValue <= right._decimalValue;
    }

    public static HexaDecimal operator +(HexaDecimal left, HexaDecimal right)
    {
        return new HexaDecimal(left._decimalValue + right._decimalValue);
    }

    public static HexaDecimal operator -(HexaDecimal left, HexaDecimal right)
    {
        return new HexaDecimal(left._decimalValue - right._decimalValue);
    }

    public static HexaDecimal operator *(HexaDecimal left, HexaDecimal right)
    {
        return new HexaDecimal(left._decimalValue * right._decimalValue);
    }

    public static HexaDecimal operator /(HexaDecimal left, HexaDecimal right)
    {
        return new HexaDecimal(left._decimalValue / right._decimalValue);
    }

    public static HexaDecimal operator ^(HexaDecimal left, HexaDecimal right)
    {
        return new HexaDecimal(left._decimalValue ^ right._decimalValue);
    }

    public static HexaDecimal operator %(HexaDecimal left, HexaDecimal right)
    {
        return new HexaDecimal(left._decimalValue % right._decimalValue);
    }


    //==============================

    public static bool operator ==(HexaDecimal left, long right)
    {
        return left._decimalValue == right;
    }

    public static bool operator !=(HexaDecimal left, long right)
    {
        return left._decimalValue != right;
    }

    public static bool operator >(HexaDecimal left, long right)
    {
        return left._decimalValue > right;
    }

    public static bool operator <(HexaDecimal left, long right)
    {
        return left._decimalValue < right;
    }

    public static bool operator >=(HexaDecimal left, long right)
    {
        return left._decimalValue >= right;
    }

    public static bool operator <=(HexaDecimal left, long right)
    {
        return left._decimalValue <= right;
    }

    public static HexaDecimal operator +(HexaDecimal left, long right)
    {
        return new HexaDecimal(left._decimalValue + right);
    }

    public static HexaDecimal operator -(HexaDecimal left, long right)
    {
        return new HexaDecimal(left._decimalValue - right);
    }

    public static HexaDecimal operator *(HexaDecimal left, long right)
    {
        return new HexaDecimal(left._decimalValue * right);
    }

    public static HexaDecimal operator /(HexaDecimal left, long right)
    {
        return new HexaDecimal(left._decimalValue / right);
    }

    public static HexaDecimal operator ^(HexaDecimal left, long right)
    {
        return new HexaDecimal(left._decimalValue ^ right);
    }

    public static HexaDecimal operator %(HexaDecimal left, long right)
    {
        return new HexaDecimal(left._decimalValue % right);
    }


}
