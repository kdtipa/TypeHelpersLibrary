using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTypes;

public struct IntegerDigit : IEquatable<IntegerDigit>, IComparable<IntegerDigit>
{
    public IntegerDigit() { }

    /// <summary>
    /// create the IntegerDigit with either the integer value 0 - 9 or 
    /// the character '0' - '9' which implicitly translate to integer 
    /// values 48 to 57 (ASCII).
    /// </summary>
    /// <param name="value"></param>
    public IntegerDigit(int value) 
    {
        if (!TrySet(value))
        {
            throw new ArgumentOutOfRangeException($"{value} is not 0 to 9 or the ASCII value 48 to 57.");
        }
    }


    private int _value = 0;
    private char _char = '0';

    public int Value
    {
        get { return _value; }
        set
        {
            if (ValCharDictionary.TryGetValue(value, out char parsedVal))
            {
                _value = value;
                _char = parsedVal;
            }
            else
            {
                throw new InvalidOperationException($"{value} is not among these values: {string.Join(", ", ValueCharacters)}");
            }
        }
    }

    public char Char
    {
        get { return _char; }
        set
        {
            if (CharValDictionary.TryGetValue(value, out int parsedVal))
            {
                _value = parsedVal;
                _char = value;
            }
            else
            {
                throw new InvalidOperationException($"{value} is not among these values: {string.Join(", ", ValueCharacters)}");
            }
        }
    }


    public bool TrySet(int value)
    {
        if (value >= 0 && value <= 9) { Value = value; return true; }

        if (value >= 48 && value <= 57) { Value = value - 48; return true; }

        return false;
    }

    public int CompareTo(IntegerDigit other)
    {
        return _value.CompareTo(other._value);
    }

    public bool Equals(IntegerDigit other)
    {
        return _value == other._value;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is IntegerDigit idObj) { return Equals(idObj); }

        if (obj is char cObj) { return Equals(new IntegerDigit(cObj)); }

        if (obj is int iObj) { return Equals(new IntegerDigit(iObj)); }

        if (obj is string sObj && sObj.Length == 1) { return Equals(new IntegerDigit(sObj[0])); }

        return false;
    }

    public override int GetHashCode()
    {
        return Value;
    }

    public override string ToString()
    {
        return _char.ToString();
    }

    public static bool operator ==(IntegerDigit left, IntegerDigit right) { return left.Equals(right); }
    public static bool operator !=(IntegerDigit left, IntegerDigit right) { return !left.Equals(right); }
    public static bool operator >(IntegerDigit left, IntegerDigit right) { return left.Value > right.Value; }
    public static bool operator <(IntegerDigit left, IntegerDigit right) { return left.Value < right.Value; }
    public static bool operator >=(IntegerDigit left, IntegerDigit right) { return left.Value >= right.Value; }
    public static bool operator <=(IntegerDigit left, IntegerDigit right) { return left.Value <= right.Value; }

    public static bool operator ==(IntegerDigit left, int right) { return left.Value == right; }
    public static bool operator !=(IntegerDigit left, int right) { return left.Value != right; }
    public static bool operator >(IntegerDigit left, int right) { return left.Value > right; }
    public static bool operator <(IntegerDigit left, int right) { return left.Value < right; }
    public static bool operator >=(IntegerDigit left, int right) { return left.Value >= right; }
    public static bool operator <=(IntegerDigit left, int right) { return left.Value <= right; }

    public static bool operator ==(int left, IntegerDigit right) { return left == right.Value; }
    public static bool operator !=(int left, IntegerDigit right) { return left != right.Value; }
    public static bool operator >(int left, IntegerDigit right) { return left > right.Value; }
    public static bool operator <(int left, IntegerDigit right) { return left < right.Value; }
    public static bool operator >=(int left, IntegerDigit right) { return left >= right.Value; }
    public static bool operator <=(int left, IntegerDigit right) { return left <= right.Value; }


    public static IntegerDigit operator +(IntegerDigit left, IntegerDigit right) { return new IntegerDigit(left.Value + right.Value); }
    public static IntegerDigit operator -(IntegerDigit left, IntegerDigit right) { return new IntegerDigit(left.Value - right.Value); }

    public static IntegerDigit operator +(IntegerDigit left, int right) { return new IntegerDigit(left.Value + right); }
    public static IntegerDigit operator -(IntegerDigit left, int right) { return new IntegerDigit(left.Value - right); }

    public static int operator +(int left, IntegerDigit right) { return left + right.Value; }
    public static int operator -(int left, IntegerDigit right) { return left - right.Value; }





    public static int[] Values { get; } = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

    public static char[] ValueCharacters { get; } = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

    public static Dictionary<int, char> ValCharDictionary { get; } = new()
    {
        { 0, '0' }, { 1, '1' }, { 2, '2' }, { 3, '3' }, { 4, '4' },
        { 5, '5' }, { 6, '6' }, { 7, '7' }, { 8, '8' }, { 9, '9' }
    };

    public static Dictionary<char, int> CharValDictionary { get; } = new()
    {
        { '0', 0 }, { '1', 1 }, { '2', 2 }, { '3', 3 }, { '4', 4 }, 
        { '5', 5 }, { '6', 6 }, { '7', 7 }, { '8', 8 }, { '9', 9 }
    };

    /// <summary>
    /// Gets you the character of the integer digit, or null if the value 
    /// passed is not in the range of 0 to 9.
    /// </summary>
    /// <param name="sourceValue">The value you want the character for</param>
    /// <returns>The character for the digit, or null.</returns>
    public static char? GetCharFromDigit(int sourceValue)
    {
        if (sourceValue < 0 || sourceValue >= ValCharDictionary.Count) { return null; }

        return ValCharDictionary[sourceValue];
    }

    public static char? GetCharFromDigit(IntegerDigit sourceValue)
    {
        return sourceValue._char;
    }


    public static IEnumerable<IntegerDigit> GetDigits(int sourceValue)
    {
        if (sourceValue < 0) { yield break; }

        if (sourceValue == 0) { yield return new IntegerDigit(0); yield break; }

        List<IntegerDigit> digitsFromOnesUp = new();
        int workVal = sourceValue;

        while (workVal > 0)
        {
            int currentDigit = workVal % 10;
            digitsFromOnesUp.Add(new IntegerDigit(currentDigit));
            workVal = (workVal - currentDigit) / 10;
        }

        int digitCount = digitsFromOnesUp.Count;

        for (int i = digitCount - 1; i >= 0; i--)
        {
            yield return digitsFromOnesUp[i];
        }
    }


    public static bool IsValidDigit(char testDigit)
    {
        return ValueCharacters.Contains(testDigit);
    }


}
