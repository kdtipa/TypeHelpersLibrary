using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeHelpers;

public static class CharHelper
{
    public static char FancyApostrophe { get; } = '’'; // likely the same as CloseSingleQuote, but just in case it's different and it's a convenience

    public static char Ellipse { get; } = '…';

    public static char LongHyphen { get; } = '–';

    public static char OpenDoubleQuote { get; } = '“';

    public static char CloseDoubleQuote { get; } = '”';

    public static char OpenSingleQuote { get; } = '‘';

    public static char CloseSingleQuote { get; } = '’';

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
            if (ic && c.ToUpper() == findChar.ToUpper()) { return true; }
            else if (!ic && c == findChar) { return true; }
        }

        return false;
    }


}
