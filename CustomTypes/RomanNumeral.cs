using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CustomTypes;

/// <summary>
/// This struct is meant to provide a Roman Numeral value type.  As with roman numerals 
/// in general, they can represent numbers from 1 to 4999 (if you accept that "MMMM" can 
/// represent 4000).  You can instantiate the variable of this type with the constructor 
/// accepting either integers or roman numeral strings.  Or you can set the integer Item 
/// or the string NumeralString.  You can use this type in math with integers.  And the 
/// == and != are defined well.  There's also a method to enumerate through RomanNumerals 
/// called Iterate.
/// </summary>
public struct RomanNumeral : IEquatable<RomanNumeral>, IComparable<RomanNumeral>
{
    public RomanNumeral() { }

    public RomanNumeral(int numericValue)
    {
        Value = numericValue;
    }

    public RomanNumeral(string romanNumeralString)
    {
        NumeralString = romanNumeralString;
    }

    private int _value = 1;
    private string _rnString = "I";
    private static int _minValue = 1;
    private static int _maxValue = 4999;

    /// <summary>
    /// The integer value of this Roman Numeral
    /// </summary>
    public int Value 
    { 
        get { return _value; }
        set
        {
            if (value < _minValue || value > _maxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"Given value [{value}] is outside allowed range {_minValue} to {_maxValue}.");
            }
            _value = value;
            _rnString = RomanNumeralHelper.GetRomanNumeral(value, false, false);
        }
    }

    /// <summary>
    /// The string representation of this Roman Numeral
    /// </summary>
    public string NumeralString
    {
        get { return _rnString; }
        set
        {
            if (RomanNumeralHelper.TryParse(value, out int parseResult))
            {
                _value = parseResult;
                _rnString = NormalizeCase(value);
            }
            else
            {
                throw new ArgumentException($"[{value}] is not a valid roman numeral");
            }
        }
    }

    private string NormalizeCase(string sourceString)
    {
        if (string.IsNullOrWhiteSpace(sourceString)) { return string.Empty; }

        bool ShouldBeUpperCase = sourceString[0] >= 65 && sourceString[0] <= 90;
        bool ShouldBeLowerCase = sourceString[0] >= 97 && sourceString[0] <= 122;

        if (!ShouldBeLowerCase && !ShouldBeUpperCase)
        {
            // we got a string that doesn't start with a letter, so just return it
            return sourceString;
        }

        var retVal = new StringBuilder();

        if (ShouldBeUpperCase)
        {
            foreach (char c in sourceString)
            {
                if (c >= 65 && c <= 90) { retVal.Append((char)(c + 32)); }
                else { retVal.Append(c); }
            }
        }
        else if (ShouldBeLowerCase)
        {
            foreach (char c in sourceString)
            {
                if (c >= 97 && c <= 122) { retVal.Append((char)(c - 32)); }
                else { retVal.Append(c); }
            }
        }
        
        return retVal.ToString();
    }

    /// <summary>
    /// The minimum value a Roman Numeral can represent
    /// </summary>
    public static int MinValue { get { return _minValue; } }

    /// <summary>
    /// The maximum value a Roman Numeral can represent
    /// </summary>
    public static int MaxValue { get { return _maxValue; } }

    /// <summary>
    /// Compares the value to see if they are the same.
    /// </summary>
    public bool Equals(RomanNumeral other)
    {
        return _value == other._value;
    }

    /// <summary>
    /// Returns -1 if this instance should come before the 
    /// compared instance; 1 if this instance should come 
    /// after the compared instance; and 0 if their sorting 
    /// priority is the same.
    /// </summary>
    public int CompareTo(RomanNumeral other)
    {
        if (_value < other._value) { return -1; }
        else if (_value > other._value) { return 1; }
        return 0;
    }

    /// <summary>
    /// Allows equality comparison with other object types 
    /// including strings and integers.
    /// </summary>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is RomanNumeral compareObj) { return _value == compareObj._value; }

        if (obj is string strObj) { return _value == new RomanNumeral(strObj)._value; }

        if (obj is int intObj) { return _value == intObj; }

        return false;
    }

    /// <summary>
    /// Given the limited number of values, just returns the value
    /// </summary>
    public override int GetHashCode()
    {
        return _value;
    }

    /// <summary>
    /// Returns the string representation of the roman numeral, using 
    /// whichever case it was stored as.
    /// </summary>
    public override string ToString()
    {
        return _rnString;
    }

    /// <summary>
    /// Returns the string representation of the roman numeral.  Allows 
    /// you to choose additive or subtractive notation.  Allows you to 
    /// pick which letter case you want.
    /// </summary>
    /// <param name="useSubtractiveNotation">
    /// defaults to using additive notation so that 4 looks like "IIII", but 
    /// if set to true, uses subtractive notation so that 4 looks like "IV".
    /// </param>
    /// <param name="useLowerCase">
    /// defaults to using the upper case letters, but if set to true, returns 
    /// the string in lower case.
    /// </param>
    /// <returns>a string representation of the roman numeral like "XVI"</returns>
    public string ToString(bool useSubtractiveNotation, bool? useLowerCase = null)
    {
        var retVal = RomanNumeralHelper.GetRomanNumeral(_value, useSubtractiveNotation, useLowerCase);

        if (retVal is null) { return string.Empty; }

        return retVal;
    }


    public static bool TryParse(string sourceString, [NotNullWhen(true)] out RomanNumeral? result)
    {
        result = null;

        try
        {
            // just separating the code that might throw an exception from the assignment to be sure
            var testResult = new RomanNumeral(sourceString);
            result = testResult;
        }
        catch
        {
            return false;
        }

        return true;
    }


    public static IEnumerable<RomanNumeral> Iterate(RomanNumeral? startingValue = null, RomanNumeral? endingValue = null)
    {
        int start = startingValue is null ? MinValue : startingValue.Value.Value; // nullable variable, variable value, roman numeral value
        int end = endingValue is null ? MaxValue : endingValue.Value.Value; // nullable variable, variable value, roman numeral value

        if (start > end)
        {
            (end, start) = (start, end);
        }

        for (int n = start; n < end; n++)
        {
            yield return new RomanNumeral(n);
        }
    }


    #region operators
    // ===========================================================================================================

    public static bool operator ==(RomanNumeral leftVal, RomanNumeral rightVal) { return leftVal.Value == rightVal.Value; }

    public static bool operator !=(RomanNumeral leftVal, RomanNumeral rightVal) { return leftVal.Value != rightVal.Value; }

    public static RomanNumeral operator +(RomanNumeral leftVal, RomanNumeral rightVal) { return new RomanNumeral(leftVal.Value + rightVal.Value); }

    public static RomanNumeral operator -(RomanNumeral leftVal, RomanNumeral rightVal) { return new RomanNumeral(leftVal.Value - rightVal.Value); }

    public static RomanNumeral operator *(RomanNumeral leftVal, RomanNumeral rightVal) { return new RomanNumeral(leftVal.Value * rightVal.Value); }

    public static RomanNumeral operator /(RomanNumeral leftVal, RomanNumeral rightVal) { return new RomanNumeral(leftVal.Value / rightVal.Value); }

    public static RomanNumeral operator %(RomanNumeral leftVal, RomanNumeral rightVal) { return new RomanNumeral(leftVal.Value + rightVal.Value); }

    // ===========================================================================================================

    public static bool operator ==(RomanNumeral leftVal, int rightVal) { return leftVal.Value == rightVal; }

    public static bool operator !=(RomanNumeral leftVal, int rightVal) { return leftVal.Value != rightVal; }

    public static RomanNumeral operator +(RomanNumeral leftVal, int rightVal) { return new RomanNumeral(leftVal.Value + rightVal); }

    public static RomanNumeral operator -(RomanNumeral leftVal, int rightVal) { return new RomanNumeral(leftVal.Value - rightVal); }

    public static RomanNumeral operator *(RomanNumeral leftVal, int rightVal) { return new RomanNumeral(leftVal.Value * rightVal); }

    public static RomanNumeral operator /(RomanNumeral leftVal, int rightVal) { return new RomanNumeral(leftVal.Value / rightVal); }

    public static RomanNumeral operator %(RomanNumeral leftVal, int rightVal) { return new RomanNumeral(leftVal.Value + rightVal); }

    // ===========================================================================================================

    public static bool operator ==(int leftVal, RomanNumeral rightVal) { return leftVal == rightVal.Value; }

    public static bool operator !=(int leftVal, RomanNumeral rightVal) { return leftVal != rightVal.Value; }

    public static int operator +(int leftVal, RomanNumeral rightVal) { return leftVal + rightVal.Value; }

    public static int operator -(int leftVal, RomanNumeral rightVal) { return leftVal + rightVal.Value; }

    public static int operator *(int leftVal, RomanNumeral rightVal) { return leftVal * rightVal.Value; }

    public static int operator /(int leftVal, RomanNumeral rightVal) { return leftVal / rightVal.Value; }

    public static int operator %(int leftVal, RomanNumeral rightVal) { return leftVal + rightVal.Value; }

    // ===========================================================================================================

    public static RomanNumeral operator ++(RomanNumeral val)
    {
        return new RomanNumeral(val.Value + 1);
    }

    public static RomanNumeral operator --(RomanNumeral val)
    {
        return new RomanNumeral(val.Value - 1);
    }

    // ===========================================================================================================

    public static implicit operator string(RomanNumeral val) { return val._rnString; }

    public static implicit operator RomanNumeral(string sourceString) { return new RomanNumeral(sourceString); }

    // ===========================================================================================================
    #endregion




}

public static class RomanNumeralHelper 
{
    public static int MinValue { get; } = 1;

    public static int MaxValue { get; } = 4999;

    public static Dictionary<string, int> FourDigitParts { get; } = new()
    {
        { "IIII", 4 }, { "XXXX", 40 }, { "CCCC", 400 }, { "MMMM", 4000 }
    };

    public static Dictionary<string, int> ThreeDigitParts { get; } = new()
    {
        { "III", 3 }, { "XXX", 30 }, { "CCC", 300 }, { "MMM", 3000 }
    };

    public static Dictionary<string, int> TwoDigitParts { get; } = new()
    {
        { "II", 2 }, { "IV", 4 }, { "IX", 9 }, 
        { "XX", 20 }, { "XL", 40 }, { "XC", 90 },
        { "CC", 200 }, { "CD", 400 }, { "CM", 900 },
        { "MM", 2000 }
    };

    public static Dictionary<char, int> OneDigitParts { get; } = new()
    {
        { 'I', 1 }, { 'V', 5 }, { 'X', 10 }, { 'L', 50 }, { 'C', 100 }, { 'D', 500 }, { 'M', 1000 }
    };

    public static Dictionary<int, char> RomanNumeralIntToAdditiveChar = new()
    {
        { 1, 'I' }, { 5, 'V' }, { 10, 'X' }, { 50, 'L' }, { 100, 'C' }, { 500, 'D' }, { 1000, 'M' }
    };

    public static Dictionary<int, string> RomanNumeralIntToSubtractiveString = new()
    {
        { 1, "I" }, { 4, "IV" }, { 5, "V" }, { 9, "IX" }, { 10, "X" }, { 40, "XL" }, { 50, "L" }, { 90, "XC" },
        { 100, "C" }, { 400, "CD" }, { 500, "D" }, { 900, "CM" }, { 1000, "M" }
    };

    public static Dictionary<char, int> RomanNumeralCharToInt = new()
    {
        { 'i', 1 }, { 'I', 1 },
        { 'v', 5 }, { 'V', 5 },
        { 'x', 10 }, { 'X', 10 },
        { 'l', 50 }, { 'L', 50 },
        { 'c', 100 }, { 'C', 100 },
        { 'd', 500 }, { 'D', 500 },
        { 'm', 1000 }, { 'M', 1000 }
    };

    public static char[] FourDigitMax { get; } = { 'I', 'i', 'X', 'x', 'C', 'c', 'M', 'm' };

    public static char[] OneDigitMax { get; } = { 'V', 'v', 'L', 'l', 'D', 'd' };

    public static Regex ValidRomanNumeralCharacters { get; } = new Regex("^[IVXLCDMivxlcdm]+$", RegexOptions.Compiled);

    public static Regex InterimParseStagePattern { get; } = new Regex("<([0-9]+)>", RegexOptions.Compiled);

    public static string InterimParseStageString { get; } = "<{0}>";

    public static Regex InterimParseStageValidCharacters { get; } = new Regex("^[0-9><]+$");

    /// <summary>
    /// accepts a representation of one of the base values for roman numerals, like "IIII"; 
    /// "XXX"; "IV"; or "L" and translates it to the integer r4 of that part.  Mostly 
    /// useful for parsing methods.  This method is a way to get the r4 regardless of 
    /// letter case from all four of the r4 dictionaries.  Will return null if the string 
    /// given doesn't match any of the keys from the four dictionaries.
    /// </summary>
    public static int? GetPartValue(string romanNumeralPart)
    {
        if (string.IsNullOrWhiteSpace(romanNumeralPart) || romanNumeralPart.Length > 4) { return null; }

        if (!ValidRomanNumeralCharacters.IsMatch(romanNumeralPart)) { return null; }

        var upperCaseRN = romanNumeralPart.ToUpper();
        var RNLen = upperCaseRN.Length;

        if (RNLen == 4 && FourDigitParts.TryGetValue(upperCaseRN, out int r4))
        {
            return r4;
        }

        if (RNLen == 3 && ThreeDigitParts.TryGetValue(upperCaseRN, out int r3))
        {
            return r3;
        }

        if (RNLen == 2 && TwoDigitParts.TryGetValue(upperCaseRN, out int r2))
        {
            return r2;
        }

        if (RNLen == 1 && OneDigitParts.TryGetValue(upperCaseRN[0], out int r1))
        {
            return r1;
        }
        
        return null;
    }

    public static bool HasTooManyConsecutiveDigits(string sourceString)
    {
        char prevChar = '\0';
        int currentCount = 0;

        foreach (char c in sourceString)
        {
            if (c == prevChar)
            {
                currentCount++;
                if (currentCount > MaxConsecutiveForDigit(c)) { return true; }
            }
            else
            {
                prevChar = c;
                currentCount = 0;
            }
        }

        return false;
    }

    public static int MaxConsecutiveForDigit(char digit)
    {
        if (FourDigitMax.Contains(digit)) { return 4; }
        if (OneDigitMax.Contains(digit)) { return 1; }

        return 0;
    }


    public static bool TryParse(string sourceString, out int romanNumeralValue)
    {
        // set the default return value which is not a valid roman numeral value
        romanNumeralValue = 0;

        // if the string is empty return false
        if (string.IsNullOrWhiteSpace(sourceString)) { return false; }

        // check to see if the source string is a valid set of characters
        if (!ValidRomanNumeralCharacters.IsMatch(sourceString)) { return false; }

        // make sure we don't have a string like "IIIIIIIIIII" or "VV"
        if (HasTooManyConsecutiveDigits(sourceString)) { return false; }

        // now comes the more complicated part.  We're going to replace parts of the string with stuff 
        // from the four dictionaries to turn the roman numeral into a delimited string of integer 
        // values that are easier to work with.
        var workString = sourceString.ToUpper();

        foreach (var part4 in FourDigitParts)
        {
            workString = workString.Replace(part4.Key, string.Format(InterimParseStageString, part4.Value));
        }

        foreach (var part3 in ThreeDigitParts)
        {
            workString = workString.Replace(part3.Key, string.Format(InterimParseStageString, part3.Value));
        }

        foreach (var part2 in TwoDigitParts)
        {
            workString = workString.Replace(part2.Key, string.Format(InterimParseStageString, part2.Value));
        }

        foreach (var part1 in OneDigitParts)
        {
            workString = workString.Replace(part1.Key.ToString(), string.Format(InterimParseStageString, part1.Value));
        }

        // now instead of "VIII" we have "<5><3>".  With roman numerals, order is important which is why I kept the string.
        // if there are any characters other than the angle brackets and numbers, the parse failed.
        if (!InterimParseStageValidCharacters.IsMatch(workString)) { return false; }

        // let's now get a list of the actual integer values in their source order
        MatchCollection partMatches = InterimParseStagePattern.Matches(workString);
        
        List<int> values = new();

        foreach (Match partMatch in partMatches.Cast<Match>())
        {
            if (int.TryParse(partMatch.Groups[1].Value, out int parseResult))
            {
                values.Add(parseResult);
            }
            else { return false; } // should never happen, but if we somehow got a non-integer, we should fail
        }

        // roman numerals can't repeat any parts.  For example "VIVI" would become "<6><6>" in our parse, and that needs to fail
        if(values.GroupBy(i => i).Any(g => g.Count() != 1)) { return false; }

        // now we just need to be sure the values are in descending order.  "IC" is not a valid roman numeral but "CI" is.
        if (values.Zip(values.Skip(1), (a, b) => a > b).Any(x => !x)) { return false; }


        romanNumeralValue = values.Sum();

        return true;
    }


    public static string GetRomanNumeral(int fromInt, bool? useSubtractive = null, bool? lowerCase = null)
    {
        if (fromInt < MinValue || fromInt > MaxValue) 
        { 
            throw new ArgumentOutOfRangeException(
                nameof(fromInt), 
                $"{fromInt} is not a value that can be represented by this implementation of Roman Numerals"
                ); 
        }

        // we need a result and a control variable for our loop
        var worker = new StringBuilder();
        int workInt = fromInt;

        // this is what we'll use in the loop for figuring out the next part to add to the roman numeral string
        Dictionary<int, string> translationLegend;

        if (useSubtractive is null || useSubtractive.Value == false)
        {
            // we're using additive, so we need to translate the chars to strings
            translationLegend = new Dictionary<int, string>();
            foreach (var item in RomanNumeralIntToAdditiveChar)
            {
                translationLegend.Add(item.Key, item.Value.ToString());
            }
        }
        else
        {
            // we're using subtractive, so we can just copy the existing dictionary
            translationLegend = RomanNumeralIntToSubtractiveString;
        }

        // here's our main loop which will look for the highest key value that still fits in the worker number and tack on the related string
        while (workInt > 0)
        {
            var highestKey = translationLegend.Where(item => item.Key <= workInt).Max(item => item.Key);
            workInt -= highestKey;
            worker.Append(translationLegend[highestKey]);
        }

        if (lowerCase is not null && lowerCase.Value == true)
        {
            return worker.ToString().ToLower();
        }

        return worker.ToString();
    }

}



