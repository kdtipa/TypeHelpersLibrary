using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace CustomTypes.NumberText;

public static class NumberWordLookUp
{
    public static Dictionary<CultureInfo, string> DecimalPointWord { get; } = new()
    {
        { CultureInfo.InvariantCulture, "Point" },
        { new CultureInfo("en-US"), "Point" },
        { new CultureInfo("de-DE"), "Punkt" }
    };

    public static Dictionary<CultureInfo, string> NegativeWord { get; } = new()
    {
        { CultureInfo.InvariantCulture, "Negative" },
        { new CultureInfo("en-US"), "Negative" },
        { new CultureInfo("de-DE"), "Negativ" }
    };

    public static Dictionary<NumberWordValueLanguageKey, string> SpecialNumberWords { get; } = new()
    {
        //{ new NumberWordValueLanguageKey(0, CultureInfo.InvariantCulture), "Zero" },
        //{ new NumberWordValueLanguageKey(1, CultureInfo.InvariantCulture), "One" },
        //{ new NumberWordValueLanguageKey(2, CultureInfo.InvariantCulture), "Two" },
        //{ new NumberWordValueLanguageKey(3, CultureInfo.InvariantCulture), "Three" },
        //{ new NumberWordValueLanguageKey(4, CultureInfo.InvariantCulture), "Four" },
        //{ new NumberWordValueLanguageKey(5, CultureInfo.InvariantCulture), "Five" },
        //{ new NumberWordValueLanguageKey(6, CultureInfo.InvariantCulture), "Six" },
        //{ new NumberWordValueLanguageKey(7, CultureInfo.InvariantCulture), "Seven" },
        //{ new NumberWordValueLanguageKey(8, CultureInfo.InvariantCulture), "Eight" },
        //{ new NumberWordValueLanguageKey(9, CultureInfo.InvariantCulture), "Nine" },
        //{ new NumberWordValueLanguageKey(10, CultureInfo.InvariantCulture), "Ten" },
        //{ new NumberWordValueLanguageKey(11, CultureInfo.InvariantCulture), "Eleven" },
        //{ new NumberWordValueLanguageKey(12, CultureInfo.InvariantCulture), "Twelve" },
        //{ new NumberWordValueLanguageKey(13, CultureInfo.InvariantCulture), "Thirteen" },
        //{ new NumberWordValueLanguageKey(14, CultureInfo.InvariantCulture), "Fourteen" },
        //{ new NumberWordValueLanguageKey(15, CultureInfo.InvariantCulture), "Fifteen" },
        //{ new NumberWordValueLanguageKey(16, CultureInfo.InvariantCulture), "Sixteen" },
        //{ new NumberWordValueLanguageKey(17, CultureInfo.InvariantCulture), "Seventeen" },
        //{ new NumberWordValueLanguageKey(18, CultureInfo.InvariantCulture), "Eighteen" },
        //{ new NumberWordValueLanguageKey(19, CultureInfo.InvariantCulture), "Nineteen" },

        { new NumberWordValueLanguageKey(0, English_UnitedStates), "Zero" },
        { new NumberWordValueLanguageKey(1, English_UnitedStates), "One" },
        { new NumberWordValueLanguageKey(2, English_UnitedStates), "Two" },
        { new NumberWordValueLanguageKey(3, English_UnitedStates), "Three" },
        { new NumberWordValueLanguageKey(4, English_UnitedStates), "Four" },
        { new NumberWordValueLanguageKey(5, English_UnitedStates), "Five" },
        { new NumberWordValueLanguageKey(6, English_UnitedStates), "Six" },
        { new NumberWordValueLanguageKey(7, English_UnitedStates), "Seven" },
        { new NumberWordValueLanguageKey(8, English_UnitedStates), "Eight" },
        { new NumberWordValueLanguageKey(9, English_UnitedStates), "Nine" },
        { new NumberWordValueLanguageKey(10, English_UnitedStates), "Ten" },
        { new NumberWordValueLanguageKey(11, English_UnitedStates), "Eleven" },
        { new NumberWordValueLanguageKey(12, English_UnitedStates), "Twelve" },
        { new NumberWordValueLanguageKey(13, English_UnitedStates), "Thirteen" },
        { new NumberWordValueLanguageKey(14, English_UnitedStates), "Fourteen" },
        { new NumberWordValueLanguageKey(15, English_UnitedStates), "Fifteen" },
        { new NumberWordValueLanguageKey(16, English_UnitedStates), "Sixteen" },
        { new NumberWordValueLanguageKey(17, English_UnitedStates), "Seventeen" },
        { new NumberWordValueLanguageKey(18, English_UnitedStates), "Eighteen" },
        { new NumberWordValueLanguageKey(19, English_UnitedStates), "Nineteen" },

        { new NumberWordValueLanguageKey(0, German), "Null" },
        { new NumberWordValueLanguageKey(1, German), "Ein" },
        { new NumberWordValueLanguageKey(2, German), "Zwei" },
        { new NumberWordValueLanguageKey(3, German), "Drei" },
        { new NumberWordValueLanguageKey(4, German), "Vier" },
        { new NumberWordValueLanguageKey(5, German), "Fünf" },
        { new NumberWordValueLanguageKey(6, German), "Sechs" },
        { new NumberWordValueLanguageKey(7, German), "Sieben" },
        { new NumberWordValueLanguageKey(8, German), "Acht" },
        { new NumberWordValueLanguageKey(9, German), "Neun" },
        { new NumberWordValueLanguageKey(10, German), "Zehn" },
        { new NumberWordValueLanguageKey(11, German), "Elf" },
        { new NumberWordValueLanguageKey(12, German), "Zwölf" },
        { new NumberWordValueLanguageKey(13, German), "Dreizehn" },
        { new NumberWordValueLanguageKey(14, German), "Vierzehn" },
        { new NumberWordValueLanguageKey(15, German), "Fünfzehn" },
        { new NumberWordValueLanguageKey(16, German), "Sechzehn" },
        { new NumberWordValueLanguageKey(17, German), "Siebzehn" },
        { new NumberWordValueLanguageKey(18, German), "Achtzehn" },
        { new NumberWordValueLanguageKey(19, German), "Neunzehn" },
        
    };


    public static Dictionary<NumberWordValueLanguageKey, string> TensNumberWords { get; } = new()
    {
        { new NumberWordValueLanguageKey(20, CultureInfo.InvariantCulture), "Twenty" },
        { new NumberWordValueLanguageKey(30, CultureInfo.InvariantCulture), "Thirty" },
        { new NumberWordValueLanguageKey(40, CultureInfo.InvariantCulture), "Forty" },
        { new NumberWordValueLanguageKey(50, CultureInfo.InvariantCulture), "Fifty" },
        { new NumberWordValueLanguageKey(60, CultureInfo.InvariantCulture), "Sixty" },
        { new NumberWordValueLanguageKey(70, CultureInfo.InvariantCulture), "Seventy" },
        { new NumberWordValueLanguageKey(80, CultureInfo.InvariantCulture), "Eighty" },
        { new NumberWordValueLanguageKey(90, CultureInfo.InvariantCulture), "Ninety" },

        { new NumberWordValueLanguageKey(20, English_UnitedStates), "Twenty" },
        { new NumberWordValueLanguageKey(30, English_UnitedStates), "Thirty" },
        { new NumberWordValueLanguageKey(40, English_UnitedStates), "Forty" },
        { new NumberWordValueLanguageKey(50, English_UnitedStates), "Fifty" },
        { new NumberWordValueLanguageKey(60, English_UnitedStates), "Sixty" },
        { new NumberWordValueLanguageKey(70, English_UnitedStates), "Seventy" },
        { new NumberWordValueLanguageKey(80, English_UnitedStates), "Eighty" },
        { new NumberWordValueLanguageKey(90, English_UnitedStates), "Ninety" },

        { new NumberWordValueLanguageKey(20, German), "Zwanzig" },
        { new NumberWordValueLanguageKey(30, German), "Dreizig" },
        { new NumberWordValueLanguageKey(40, German), "Vierzig" },
        { new NumberWordValueLanguageKey(50, German), "Fünfzig" },
        { new NumberWordValueLanguageKey(60, German), "Sechzig" },
        { new NumberWordValueLanguageKey(70, German), "Siebzig" },
        { new NumberWordValueLanguageKey(80, German), "Achtzig" },
        { new NumberWordValueLanguageKey(90, German), "Neunzig" },
    };


    public static Dictionary<NumberWordValueLanguageKey, string> BigNumberWords { get; } = new()
    {
        { new NumberWordValueLanguageKey(100, CultureInfo.InvariantCulture), "Hundred" },
        { new NumberWordValueLanguageKey(1000, CultureInfo.InvariantCulture), "Thousand" },
        { new NumberWordValueLanguageKey(1000000, CultureInfo.InvariantCulture), "Million" },
        { new NumberWordValueLanguageKey(1000000000, CultureInfo.InvariantCulture), "Billion" },

        { new NumberWordValueLanguageKey(100, English_UnitedStates), "Hundred" },
        { new NumberWordValueLanguageKey(1000, English_UnitedStates), "Thousand" },
        { new NumberWordValueLanguageKey(1000000, English_UnitedStates), "Million" },
        { new NumberWordValueLanguageKey(1000000000, English_UnitedStates), "Billion" },

        { new NumberWordValueLanguageKey(100, German), "Hundert" },
        { new NumberWordValueLanguageKey(1000, German), "Tausend" },
        { new NumberWordValueLanguageKey(1000000, German), "Million" },
        { new NumberWordValueLanguageKey(1000000000, German), "Milliarde" },

    };


    public static bool HasCultureInfo(CultureInfo checkValue)
    {
        return DefinedCultureInfos.Any(ci => ci == checkValue);
    }

    public static IEnumerable<CultureInfo> DefinedCultureInfos
    {
        get
        {
            yield return CultureInfo.InvariantCulture;
            yield return English_UnitedStates;
            yield return German;
        }
    }

    public static CultureInfo English_UnitedStates = new CultureInfo("en-US");
    public static CultureInfo German = new CultureInfo("de-DE");
}
