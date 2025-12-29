using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CustomTypes.NumberText;

public struct NumberWordValueLanguageKey : IEquatable<NumberWordValueLanguageKey>, IComparable<NumberWordValueLanguageKey>
{
    public NumberWordValueLanguageKey(int keyValue, CultureInfo? cultureInfo = null)
    {
        if (keyValue < 0) { throw new ArgumentOutOfRangeException(nameof(keyValue), "key for NumberWordValueLanguageKey must be zero or above."); }

        Culture = cultureInfo ?? CultureInfo.InvariantCulture;
    }

    public int Value { get; private set; } = 0;

    public CultureInfo Culture { get; private set; } = CultureInfo.InvariantCulture;

    public int CompareTo(NumberWordValueLanguageKey other)
    {
        if (Culture.Equals(other.Culture))
        {
            return Value.CompareTo(other.Value);
        }
        
        return Culture.Name.CompareTo(other.Culture.Name);
    }

    public bool Equals(NumberWordValueLanguageKey other)
    {
        return Value == other.Value && Culture.Equals(other.Culture);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is NumberWordValueLanguageKey other) { return Equals(other); }

        if (obj is KeyValuePair<int, CultureInfo> kvpObj) { return Equals(new NumberWordValueLanguageKey(kvpObj.Key, kvpObj.Value)); }

        if (obj is Tuple<int, CultureInfo> tObj) { return Equals(new NumberWordValueLanguageKey(tObj.Item1, tObj.Item2)); }

        if (obj is int iObj) { return Equals(iObj, CultureInfo.InvariantCulture); }

        if (obj is string sObj && TryParse(sObj, out var parseResult)) { return Equals(parseResult); }

        return false;
    }

    public override int GetHashCode()
    {
        return Culture.GetHashCode();
    }

    public override string ToString()
    {
        return $"{Value} [{Culture.Name}]";
    }

    public static bool TryParse(string sourceString, [NotNullWhen(true)] out NumberWordValueLanguageKey? parseResult)
    {
        parseResult = null;

        if (TrySplitNWVLK(sourceString, out var keyResult, out var langResult) && keyResult is not null && langResult is not null)
        {
            parseResult = new NumberWordValueLanguageKey(keyResult.Value, langResult);
            return true;
        }

        return false;
    }

    private static bool TrySplitNWVLK(string sourceString, out int? keyVal, out CultureInfo? langVal)
    {
        keyVal = null;
        langVal = null;

        if (string.IsNullOrWhiteSpace(sourceString)) { return false; }

        var matchVals = _NWVLKPattern.Match(sourceString);
        if (matchVals.Success && int.TryParse(matchVals.Groups[0].Value, out int kv))
        {
            string ciStr = matchVals.Groups[1].Value;
            try
            {
                CultureInfo ci = new CultureInfo(ciStr);
                keyVal = kv;
                langVal = ci;
                return true;
            }
            catch
            {
                return false;
            }
        }

        return false;
    }

    private static Regex _NWVLKPattern = new Regex("^([1-9][0-9]*)[ \\[]+(.+)[\\]]$|^([0])[ \\[]+(.+)[\\]]$", RegexOptions.Compiled);


    public static bool operator ==(NumberWordValueLanguageKey a, NumberWordValueLanguageKey b) { return a.Equals(b); }
    public static bool operator !=(NumberWordValueLanguageKey a, NumberWordValueLanguageKey b) { return !a.Equals(b); }



}
