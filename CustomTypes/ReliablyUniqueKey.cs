using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomTypes;

public struct ReliablyUniqueKey : IComparable<ReliablyUniqueKey>, IEquatable<ReliablyUniqueKey>
{
    /// <summary>
    /// The primary constructor for the ReliablyUniqueKey.  It can be run 
    /// without a parameter, but allows you to pass true if you want the 
    /// constructor to also run the code to generate a key for this instance.  
    /// If you leave it blank or pass false, the Item will start out empty.  
    /// You can generate a key with the InitializeValue method.
    /// </summary>
    public ReliablyUniqueKey(bool? GenerateInitialValue = null)
    {
        if (GenerateInitialValue is not null && GenerateInitialValue.Value == true)
        {
            // set the value
            _value = GenerateKey();
            _isEmpty = false;
        }
    }

    /// <summary>
    /// This constructor is available in case you got the key from another source that 
    /// doesn't have access to this type class
    /// </summary>
    /// <param name="KeyValue"></param>
    public ReliablyUniqueKey(string KeyValue)
    {
        if (IsValid(KeyValue))
        {
            _value = KeyValue;
            _isEmpty = false;
        }
    }


    private string _value = string.Empty;
    public string Value
    {
        get { return _value; }
    }

    private bool _isEmpty = true;
    public bool IsEmpty { get { return _isEmpty; } }

    public bool HasValue { get { return !_isEmpty; } }

    /// <summary>
    /// Only does something if the value is empty.  If the 
    /// value is not empty, it does nothing but return false.  
    /// If the value IS empty, it will load a new key into 
    /// the value and return true.
    /// </summary>
    public bool InitializeValue()
    {
        if (_isEmpty)
        {
            _value = GenerateKey();
            _isEmpty = false;
            return true; // to say we set the key
        }

        return false; // to say there was already a key and we didn't need to set it
    }

    /// <summary>
    /// A static convenience method to get you and instance 
    /// of a ReliablyUniqueKey that has a value generated.
    /// </summary>
    public static ReliablyUniqueKey Create()
    {
        return new ReliablyUniqueKey(true);
    }

    /// <summary>
    /// Given that GUID gives you an Empty static property, I'm doing it here too.  But 
    /// you can use the "IsEmpty" property too if all you want to do is check to see if 
    /// a given variable is empty or not.  And the normal contructor without a parameter 
    /// also gives you an empty ReliablyUniqueKey if that's what you're looking for.
    /// </summary>
    public static ReliablyUniqueKey Empty
    {
        get
        {
            return new ReliablyUniqueKey();
        }
    }

    /// <summary>
    /// Tells you if the key you gave is a valid string to 
    /// represent one of these keys.  It must be composed 
    /// of characters for 0-9, a-z, and A-Z (each one of 
    /// those characters representing a value from 0 to 35).  
    /// The code must also be ## characters long.
    /// </summary>
    public static bool IsValid(string checkValue)
    {
        // can't be valid if it's blank or too short
        if (string.IsNullOrWhiteSpace(checkValue) || checkValue.Length < 10) { return false; }

        // make sure all the characters in the string exist in the digit list
        foreach (char c in checkValue)
        {
            if (!_digits.Any(d => d == c)) { return false; }
        }

        return true;
    }



    private static char[] _digits =
    {
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 
        'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 
        'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 
        'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 
        'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 
        'Y', 'Z'
    };




    private static string GenerateKey(int? seed = null)
    {
        // get our random number generator
        Random rng;
        if (seed is not null)
        {
            rng = new Random(seed.Value);
        }
        else
        {
            rng = new Random(DateTime.Now.Millisecond);
            Thread.Sleep(rng.Next(7, 16));
        }

        // build part 1 of the key in number form
        var part1 = new StringBuilder(12);

        part1.Append(DateTime.Now.ToString("yyyymmfffHH"));
        part1.Append(rng.Next(0, 10));

        Thread.Sleep(rng.Next(7, 16));

        // build part 2 of the key in number form
        var part2 = new StringBuilder(12);

        part2.Append(rng.Next(10, 100)); // make sure first digit is at least 1
        Thread.Sleep(rng.Next(7, 16));
        part2.Append(DateTime.Now.ToString("ssMMfffdd"));
        part2.Append(rng.Next(0, 10));

        // translate the numbers to our base 36 code
        var part1Key = base10_to_base36(part1.ToString());
        var part2Key = base10_to_base36(part2.ToString());

        // now that I have two parts, shove them together
        return part1Key + part2Key;
    }

    private static string base10_to_base36(long b10Val)
    {
        var keyDigits = new List<long>();
        var mod = 36;
        var workVal = b10Val;
        long parsedDigit;

        // split the number into "digits" where each digit is 0 to 35
        // order in the list will be ones digit to highest digit
        while (workVal > 0)
        {
            parsedDigit = workVal % mod;
            workVal = (workVal - parsedDigit) / mod;
            keyDigits.Add(parsedDigit);
        }

        // now since the first item in the list is the ones place, 
        // walk down the list backward to get the final code.
        var digitCount = keyDigits.Count;
        var result = new StringBuilder(digitCount);
        for (var d = digitCount - 1; d >= 0; d--)
        {
            result.Append(_digits[keyDigits[d]]);
        }

        return result.ToString();
    }

    private static string base10_to_base36(string b10Str)
    {
        if (long.TryParse(b10Str, out var b10))
        {
            return base10_to_base36(b10);
        }

        return string.Empty;
    }







    public override string ToString()
    {
        return _value;
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is ReliablyUniqueKey ruk) { return Equals(ruk); }

        if (obj is string str && IsValid(str))
        {
            return Equals(new ReliablyUniqueKey(str));
        }

        return false;
    }


    public int CompareTo(ReliablyUniqueKey other)
    {
        return string.Compare(_value, other._value);
    }

    public bool Equals(ReliablyUniqueKey other)
    {
        return string.Equals(_value, other._value);
    }



    public static bool operator ==(ReliablyUniqueKey left, ReliablyUniqueKey right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ReliablyUniqueKey left, ReliablyUniqueKey right)
    {
        return !left.Equals(right);
    }

    public static bool operator ==(ReliablyUniqueKey left, string right)
    {
        if (string.IsNullOrEmpty(right) || IsValid(right))
        {
            return string.Equals(left._value, right);
        }
        return false;
    }

    public static bool operator !=(ReliablyUniqueKey left, string right)
    {
        if (string.IsNullOrEmpty(right) || IsValid(right))
        {
            return !string.Equals(left._value, right);
        }
        return true;
    }

    public static bool operator ==(string left, ReliablyUniqueKey right)
    {
        if (string.IsNullOrEmpty(left) || IsValid(left))
        {
            return string.Equals(left, right._value);
        }
        return false;
    }

    public static bool operator !=(string left, ReliablyUniqueKey right)
    {
        if (string.IsNullOrEmpty(left) || IsValid(left))
        {
            return !string.Equals(left, right._value);
        }
        return true;
    }



}
