using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTypes.Helpers;

public static class CharHelper
{
    public static char FancyApostrophe { get; } = '’'; // likely the same as CloseSingleQuote, but just in case it's different and it's a convenience

    public static char Ellipse { get; } = '…';

    public static char LongHyphen { get; } = '–';

    public static char OpenDoubleQuote { get; } = '“';

    public static char CloseDoubleQuote { get; } = '”';

    public static char OpenSingleQuote { get; } = '‘';

    public static char CloseSingleQuote { get; } = '’';

    public static char Eszett { get; } = 'ß';

    public static char Pi { get; } = 'π';

    public static char Infinity { get; } = '∞';

    public static char PlusOrMinus { get; } = '±';

    public static char CurrencyDollar { get; } = '$';
    public static char CurrencyPound { get; } = '£';
    public static char CurrencyYen { get; } = '¥';
    public static char CurrencyCent { get; } = '¢';


    /// <summary>
    /// The keys are the weird fancy characters Microsoft Word likes to replace the basic characters 
    /// with, and the value is the basic character.  So, “ can be replaced with ".  Note that the 
    /// value must be a string because of the ellipse that has to go from … to ...
    /// </summary>
    public static Dictionary<char, string> BasicVersion { get; } = new()
    {
        { '…', "..." },
        { '–', "-" },
        { '“', "\"" },
        { '”', "\"" },
        { '‘', "'" },
        { '’', "'" }
    };

    /// <summary>
    /// A method that uses the BasicVersion dictionary to clean up text to avoid using the fancy 
    /// versions of characters that Microsoft Word likes to replace the basic characters with.  
    /// Note that the return value must be a string for the ellipse.
    /// </summary>
    public static string GetBasicVersion(this char c)
    {
        if (BasicVersion.TryGetValue(c, out var result))
        {
            return result;
        }

        return c.ToString();
    }



    public static char ToUpper(this char c)
    {
        if (c >= 97 && c <= 122)
        {
            return (char)(c - 32);
        }

        return c;
    }

    public static bool IsUpper(this char c)
    {
        return c >= 65 && c <= 90;
    }

    public static char ToLower(this char c) 
    { 
        if (c >= 65 && c <= 90)
        {
            return (char)(c + 32);
        }

        return c;
    }

    public static bool IsLower(this char c)
    {
        return c >= 97 && c <= 122;
    }

    /// <summary>
    /// Tells you if the character is one of the original 128 characters and 
    /// is one that is visible when output like the letters, numbers, and 
    /// punctuation.  Does not include white space characters even though they 
    /// affect the output.
    /// </summary>
    public static bool IsVisiblePrintableCharacter(this char c)
    {
        if (c >= 33 && c <= 126) { return true; }
        return false;
    }

    /// <summary>
    /// Tells you if the character is a line feed, form feed, carriage return, 
    /// horizontal tab, vertical tab, or space.
    /// </summary>
    public static bool IsWhiteSpaceCharacter(this char c)
    {
        if (c == 9 || c == 10 || c == 11 || c == 12 || c == 13 || c == 32) { return true; }
        return false;
    }

    /// <summary>
    /// Tells you if the character is one of the visible printable characters 
    /// within the original 128, or one of the whitespace characters that can 
    /// be used in output.
    /// </summary>
    public static bool IsPrintableCharacter(this char c)
    {
        return c.IsVisiblePrintableCharacter() || c.IsWhiteSpaceCharacter();
    }


    private static char[] _punctuationChars = { '!', '"', '\'', ',', '.', ':', ';', '?', '`' };
    public static bool IsPunctuationCharacter(this char c)
    {
        return _punctuationChars.Any(x => x == c);
    }

    public static IEnumerable<char> PunctuationChars 
    { 
        get
        {
            foreach (char c in _punctuationChars) { yield return c; }
        } 
    }

    public static IEnumerable<KeyValuePair<char, int>> DigitCharacters
    {
        get
        {
            yield return new KeyValuePair<char, int>('0', 0);
            yield return new KeyValuePair<char, int>('1', 1);
            yield return new KeyValuePair<char, int>('2', 2);
            yield return new KeyValuePair<char, int>('3', 3);
            yield return new KeyValuePair<char, int>('4', 4);
            yield return new KeyValuePair<char, int>('5', 5);
            yield return new KeyValuePair<char, int>('6', 6);
            yield return new KeyValuePair<char, int>('7', 7);
            yield return new KeyValuePair<char, int>('8', 8);
            yield return new KeyValuePair<char, int>('9', 9);
        }
    }

    public static int? DigitValue(this char c)
    {
        if (c >= '0' && c <= '9') { return DigitCharacters.Where(k => k.Key == c).First().Value; }
        return null;
    }

    /// <summary>
    /// Just a gathering of brackets and parentheses where the key is the opening 
    /// character and the value is the closing character.  Hopefully helpful.
    /// </summary>
    public static Dictionary<char, char> ContainerCharacters { get; } = new()
    {
        { '(', ')' }, { '[', ']' }, { '<', '>' }, { '{', '}' }
    };


    public static bool Contains(IEnumerable<char> checkCollection, char findChar, bool? ignoreCase = null)
    {
        bool ic = ignoreCase is not null && ignoreCase.Value == true;

        foreach (char c in checkCollection)
        {
            if ((ic && c.ToUpper() == findChar.ToUpper()) || (!ic && c == findChar)) { return true; }
        }

        return false;
    }


    public static string ConvertToString(this IEnumerable<char> charCollection)
    {
        var result = new StringBuilder();

        foreach (char c in charCollection) { result.Append(c); }

        return result.ToString();
    }



    /// <summary>
    /// This dictionary has the line characters used to make frames/boxes 
    /// with characters for displays like you'd find in a Console.  The 
    /// keys arebased on the directions the lines in the given symbols go.  
    /// With four digits, it is top, right, bottom, left.  0 means no line, 
    /// 1 means a single line, and 2 means a double line.  To make it easier, 
    /// you can just use the GetLineChar method from this same class.
    /// </summary>
    public static Dictionary<int, char> LineCharacters = new()
    {
        { 1010, '│' }, { 101, '─' }, { 1111, '┼' },
        { 1011, '┤' }, { 1110, '├' }, { 1101, '┴' }, { 111, '┬' },
        { 11, '┐' }, { 1001, '┘' }, { 110, '┌'}, { 1100, '└' },

        { 2020, '║' }, { 202, '═' }, { 2222, '╬' },
        { 2022, '╣' }, { 2220, '╠' }, { 2202, '╩' }, { 222, '╦' },
        { 22, '╗' }, { 2002, '╝' }, { 220, '╔' }, { 2200, '╚' },

        { 2121, '╫' }, { 1212, '╪' },
        { 1012, '╡' }, { 2021, '╢' }, { 1210, '╞' }, { 2120, '╟' },
        { 1202, '╧' }, { 2101, '╨' }, { 212, '╤' }, { 121, '╥' },
        { 21, '╖' }, { 12, '╕' }, { 2001, '╜' }, { 1002, '╛' },
        { 2100, '╙' }, { 1200, '╘' }, { 120, '╓' }, { 210, '╒' }
    };

    /// <summary>
    /// This will get you the character for a given part of a frame or 
    /// box for the special characters like ╠ which would be retrieved 
    /// with parameter values of 2, 2, 2, 0.  You can use this method 
    /// to help you draw boxes in character driven UIs like you might 
    /// find in a Console interface.  There must be at least two values 
    /// above 0, and the ones across from each other must be the same 
    /// if above zero (so top and bottom cannot be 1 and 2).  If the 
    /// values pass fail the requirements, you'll get null.
    /// </summary>
    /// <param name="up">0 for none, 1 for single line, 2 for double line on the top</param>
    /// <param name="right">0 for none, 1 for single line, 2 for double line on the right</param>
    /// <param name="down">0 for none, 1 for single line, 2 for double line on the bottom</param>
    /// <param name="left">0 for none, 1 for single line, 2 for double line on the left</param>
    /// <returns>the actual character to be displayed</returns>
    public static char? GetLineChar(int up, int right, int down, int left)
    {
        // first let's see if the numbers are all in range of 0 to 2.
        bool goodNumbers = Limit(up, 0, 2, out int lu);
        goodNumbers &= Limit(right, 0, 2, out int lr);
        goodNumbers &= Limit(down, 0, 2, out int ld);
        goodNumbers &= Limit(left, 0, 2, out int ll);
        if (!goodNumbers) { return null; }

        // next we need to be sure we have at least two values that are not zero
        int directionCount = (lu > 0 ? 1 : 0) + (lr > 0 ? 1 : 0) + (ld > 0 ? 1 : 0) + (ll > 0 ? 1 : 0);
        if (directionCount < 2) { return null; }

        // next we need to make sure the ones on the same axis are valid compared to each other.  Can't 
        // have an Up of 1 and a Down of 2 for example because the lines can't change mid symbol.  We 
        // have access to '│' (1, 1) or '║' (2, 2), but can't represent a top of 1 and a bottom of 2.
        if (lu != 0 && ld != 0 && lu != ld) { return null; } // top and bottom
        if (lr != 0 && ll != 0 && lr != ll) { return null; } // left and right

        // now we know we have a valid combination, so let's build our key for the dictionary...
        int k = (lu * 1000) + (lr * 100) + (ld * 10) + ll;
        return LineCharacters[k];
    }

    private static bool Limit(int sourceInt, int lowerLimit, int upperLimit, out int closestInRange)
    {
        // make sure limits are okay
        if (lowerLimit > upperLimit) { (lowerLimit, upperLimit) = (upperLimit, lowerLimit); }

        // ideal scenario
        if (sourceInt >= lowerLimit && sourceInt <= upperLimit)
        {
            closestInRange = sourceInt;
            return true;
        }
        else if (sourceInt < lowerLimit) { closestInRange = lowerLimit; }
        else { closestInRange = upperLimit; }

        return false;
    }



    public static List<CharacterVariants> AccentedCharacters { get; } = new()
    {
        new CharacterVariants('a', umlaut: 'ä', acute: 'á', grave: 'à', circumflex: 'â', cedilla: null, ring: 'å'),
        new CharacterVariants('A', umlaut: 'Ä', acute: 'Á', grave: 'À', circumflex: 'Â', cedilla: null, ring: 'Å')
        // ToDo: finish AccentedCharacters list
    };

}



public enum CharacterAccents
{
    None = 0,
    Umlaut = 1, 
    Acute = 2, 
    Grave = 3, 
    Circumflex = 4,
    Cedilla = 5,
    Ring = 6
}

public struct CharacterVariants : IEquatable<CharacterVariants>, IComparable<CharacterVariants>
{
    public CharacterVariants(
        char normal, 
        char? umlaut = null, 
        char? acute = null, 
        char? grave = null, 
        char? circumflex = null, 
        char? cedilla = null, 
        char? ring = null)
    {
        Normal = normal;
        Umlaut = umlaut;
        Acute = acute;
        Grave = grave;
        Circumflex = circumflex;
        Cedilla = cedilla;
        Ring = ring;
    }

    public char Normal { get; }
    public char? Umlaut { get; }
    public char? Acute { get; }
    public char? Grave { get; }
    public char? Circumflex { get; }
    public char? Cedilla { get; }
    public char? Ring { get; }

    public IEnumerable<char> Values
    {
        get
        {
            yield return Normal;
            if (Umlaut is not null) { yield return Umlaut.Value; }
            if (Acute is not null) { yield return Acute.Value; }
            if (Grave is not null) { yield return Grave.Value; }
            if (Circumflex is not null) { yield return Circumflex.Value; }
            if (Cedilla is not null) { yield return Cedilla.Value; }
            if (Ring is not null) { yield return Ring.Value; }
        }
    }

    public char? CharValue(CharacterAccents? whichForm = null)
    {
        CharacterAccents wf = whichForm ?? CharacterAccents.None;

        switch (wf)
        {
            case CharacterAccents.Umlaut:
                return Umlaut;
            case CharacterAccents.Acute:
                return Acute;
            case CharacterAccents.Grave:
                return Grave;
            case CharacterAccents.Circumflex:
                return Circumflex;
            case CharacterAccents.Cedilla:
                return Cedilla;
            case CharacterAccents.Ring:
                return Ring;
            default:
                return Normal;
        }
    }

    public bool Equals(CharacterVariants other)
    {
        return Normal == other.Normal
            && Umlaut == other.Umlaut
            && Acute == other.Acute
            && Grave == other.Grave
            && Circumflex == other.Circumflex
            && Cedilla == other.Cedilla
            && Ring == other.Ring;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is CharacterVariants cvObj) { return Equals(cvObj); }

        if (obj is char cObj) { return IsVariant(cObj); }

        return false;
    }

    public bool IsVariant(char possibleVariant)
    {
        return Values.Any(c => c == possibleVariant);
    }

    public override int GetHashCode()
    {
        return Normal.GetHashCode();
    }

    public override string ToString()
    {
        return string.Join(", ", Values);
    }

    public int CompareTo(CharacterVariants other)
    {
        return Normal.CompareTo(other.Normal);
    }


    public static bool operator ==(CharacterVariants left, CharacterVariants right) { return left.Equals(right); }
    public static bool operator !=(CharacterVariants left, CharacterVariants right) { return !left.Equals(right); }


    public static bool operator >(CharacterVariants left, CharacterVariants right) { return left.CompareTo(right) > 0; }
    public static bool operator <(CharacterVariants left, CharacterVariants right) { return left.CompareTo(right) < 0; }


    public static bool operator >=(CharacterVariants left, CharacterVariants right) { return left.CompareTo(right) >= 0; }
    public static bool operator <=(CharacterVariants left, CharacterVariants right) { return left.CompareTo(right) <= 0; }


}
