namespace TypeHelpers;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public static class StringHelper
{
    /// <summary>
    /// Gives a convenient way to only use a portion of a long string.  Useful 
    /// when you want to display to a user a portion of a string but don't 
    /// feel like checking initial string length yourself to see if you need 
    /// a SubString().
    /// </summary>
    /// <param name="characterCountLimit">
    /// The maximum number of characters you want to allow in your output.  
    /// Defaults to 24 and will use that default if you enter a number 
    /// lower than 1 or null.
    /// </param>
    /// <param name="endWithEllipsis">
    /// If set to true, will try to make the last three characters of the 
    /// return string into an ellipsis in the form of "...".  Won't do that 
    /// if the character count limit is less than 10 or if the string itself 
    /// is less long than the limit.
    /// </param>
    public static string LimitString(this string srcStr, int? characterCountLimit = null, bool? endWithEllipsis = null)
    {
        // if the source is nothing, return an empty string right away
        if (string.IsNullOrEmpty(srcStr)) { return string.Empty; }

        // see if the user wants an ellipse
        var ewEl = false;
        if (endWithEllipsis is not null && endWithEllipsis == true) { ewEl = true; }

        // figure out how long a string we're trying to make
        var ccLimit = 24;
        if (characterCountLimit is not null && characterCountLimit >= 1) { ccLimit = characterCountLimit.Value; }

        // if the limit is long enough and the user wants an ellipse, subtract 3 from the limit so we have room for the ellipse
        if (ccLimit > 9 && ewEl) { ccLimit -= 3; }
        
        // figure out the length of the source
        var srcLen = srcStr.Length;

        // if the source is shorter than the limit, we can just return the source
        if (srcLen <= ccLimit) { return srcStr; }

        // if we don't want an ellipse, we just return the substring of the correct length
        if (!ewEl) { return srcStr.Substring(0, ccLimit); }

        // if we got here, we want an ellipse, so return substring with ellipse
        return $"{srcStr.Substring(0, ccLimit)}...";
    }

    /// <summary>
    /// Tells you how many occurences of the specified character are 
    /// in the string.  If it doesn't exist in the string, you'll get 
    /// zero back.  
    /// </summary>
    public static int CountChar(this string srcStr, char characterToCount)
    {
        var count = 0;

        foreach (char c in srcStr)
        {
            if (c == characterToCount) { count++; }
        }

        return count;
    }



    public static char[] WordBreakCharacters { get; } = [ ' ', '.', ',', '?', '!', '(', ')', '+', '=', '{', '}', '[', ']', '|', '"', ';', '<', '>', (char)9, (char)10, (char)13 ];

    public static char[] SentenceBreakCharacters { get; } = ['.', '?', '!'];

    public static int WordCount(this string srcString)
    {
        return srcString.Split(WordBreakCharacters, StringSplitOptions.RemoveEmptyEntries).Length;
    }

    public static IEnumerable<string> Words(this string srcString)
    {
        if (string.IsNullOrWhiteSpace(srcString)) { yield break; }

        var words = srcString.Split(WordBreakCharacters, StringSplitOptions.RemoveEmptyEntries);

        if (words is null || words.Length == 0) { yield break; }

        foreach (var word in words)
        {
            yield return word;
        }
    }

    /// <summary>
    /// Breaks up the string by periods, question marks, and excalamtion points.  You'll get weird 
    /// results if there are line breaks because the original string might have used them to separate 
    /// paragraphs.  You'll also get weird results if people use end-of-sentence punctuation inside 
    /// quotation marks where the user might expect those to not affect the sentence break, but the 
    /// code for this method isn't that complex.  You lose the punctuation in the effort because we're 
    /// doing a Split on those characters.
    /// </summary>
    public static IEnumerable<string> Sentences(this string srcString)
    {
        if (string.IsNullOrWhiteSpace(srcString)) { yield break; }

        var sentences = srcString.Split(SentenceBreakCharacters, StringSplitOptions.RemoveEmptyEntries);

        if (sentences is null || sentences.Length == 0) { yield break; }

        foreach (var sentence in sentences)
        {
            yield return sentence;
        }

    }

    /// <summary>
    /// This method is because I refuse the ridiculous lack of double spaces after periods, 
    /// question marks, and exclamation points to end sentences.  I find single spacing in 
    /// that scenario to be obnoxious.  The extra space helps my eyes parse what they're 
    /// reading.  To avoid putting space where it shouldn't go, those characters needs at 
    /// least one space after them in order for it to be normalized that way.  We don't 
    /// want to add space to something like "$6.99".  
    /// </summary>
    public static string FixEndOfSentenceSpacing(this string srcString)
    {
        var fixedString = new StringBuilder();
        var foundCloser = false;

        foreach (var c in srcString)
        {
            if (SentenceBreakCharacters.Contains(c))
            {
                fixedString.Append($"{c}  ");
                foundCloser = true;
            }
            else if (c == ' ' && !foundCloser)
            {
                fixedString.Append(c);
            }
            else if (c != ' ')
            {
                fixedString.Append(c);
                foundCloser = false;
            }
        }

        return fixedString.ToString();
    }

    /// <summary>
    /// Gets you a string of a length equal to the count you passed in 
    /// and made up of the character you pass in repeated for the whole 
    /// length of the string.
    /// </summary>
    /// <param name="count">length of the string you want</param>
    /// <param name="repeatedCharacter">character to use</param>
    /// <returns>if you pass 10 and '=', you'll get the string "=========="</returns>
    public static string RepeatChars(int count, char repeatedCharacter)
    {
        if (count <= 0) { return string.Empty; }

        var result = new StringBuilder(count);
        for (var c = 1; c <= count; c++)
        {
            result.Append(repeatedCharacter);
        }
        return result.ToString();
    }

    /// <summary>
    /// Allows you to create fancier horizontal rules for text based interfaces.  For 
    /// example, you could pass a line length of 12 with characters '<', '=', and '>', 
    /// and you would get back a string like "<<<<====>>>>".
    /// </summary>
    /// <param name="lineLength">The length of the string you want returned</param>
    /// <param name="charactersToBuildLineWith">the collection of characters to use</param>
    /// <returns>
    /// a string of the specified length with the characters passed making up parts of 
    /// the string in groups, like 16 with '|', '/', '-', '\' giving you a pattern like 
    /// "||||////----\\\\"
    /// </returns>
    public static string? CreateLineOfGroupedCharacters(int lineLength, params char[] charactersToBuildLineWith)
    {
        if (lineLength <= 0 || charactersToBuildLineWith.Length == 0) { return null; }

        var result = new StringBuilder();
        int charCount = charactersToBuildLineWith.Length;
        var groupCounts = lineLength.SplitIntoEvenGroups(charCount);
        if (groupCounts is null) { return null; }

        for (int ci = 0; ci < charCount; ci++)
        {
            result.Append("".PadRight(groupCounts[ci], charactersToBuildLineWith[ci]));
        }

        return result.ToString();
    }

    /// <summary>
    /// Allows you to create a fancier horizontal rule for text based interfaces.  For 
    /// example, you could pass a line length of 20 with characters '/', '\', '_', ' ' 
    /// to get a string like "/\_ /\_ /\_ /\_ /\_ ".
    /// </summary>
    /// <param name="lineLength">the length of the string you want returned</param>
    /// <param name="charactersToBuildLineWith">
    /// the collection of characters to use when building the string.
    /// </param>
    /// <returns>
    /// a string of the specified length with the characters passed making up the 
    /// pattern that is repeated, like 15 with '.', 'o', 'O', 'o', '.' to get a 
    /// string like ".oOo..oOo..oOo.".
    /// </returns>
    public static string? CreateLineOfCycledCharacters(int lineLength, params char[] charactersToBuildLineWith)
    {
        if (lineLength <= 0 || charactersToBuildLineWith.Length == 0) { return null; }

        var result = new StringBuilder();
        int charCount = charactersToBuildLineWith.Length;
        int currentChar = 0;

        while (result.Length < lineLength)
        {
            result.Append(charactersToBuildLineWith[currentChar]);
            currentChar++;
            if (currentChar >= charCount) { currentChar = 0; }
        }

        return result.ToString();
    }

    /// <summary>
    /// As long as it has basic letters and they are all 
    /// upper case, this will return true.  Returns false 
    /// if there are any lower case letters or no letters.
    /// </summary>
    public static bool IsAllUpperCase(this string srcString)
    {
        var hasUpperLetter = false;

        foreach (char c in srcString)
        {
            if (c >= 65 && c <= 90)
            {
                hasUpperLetter = true;
                continue;
            }

            if (c >= 97 && c <= 122)
            {
                return false;
            }
        }

        return hasUpperLetter;
    }

    /// <summary>
    /// As long as it has basic letters and they are all 
    /// lower case, this will return true.  Returns false 
    /// if there are any upper case letters or no letters.
    /// </summary>
    public static bool IsAllLowerCase(this string srcString)
    {
        var hasLowerLetter = false;

        foreach (char c in srcString)
        {
            if (c >= 97 && c <= 122)
            {
                hasLowerLetter = true;
                continue;
            }

            if (c >= 65 && c <= 90)
            {
                return false;
            }
        }

        return hasLowerLetter;
    }

    /// <summary>
    /// A variation of IndexOf that gets you the index for every 
    /// instance of what you're looking for within the string.
    /// </summary>
    /// <param name="findStr">The string you're looking for.</param>
    /// <returns>
    /// A list of indices where your find string exists.  If it 
    /// does NOT exist in the base string, the List is empty.
    /// </returns>
    public static List<int> IndexesOf(this string srcString, string? findStr)
    {
        var results = new List<int>();

        if (string.IsNullOrEmpty(srcString) 
         || string.IsNullOrEmpty(findStr) 
         || srcString.Length < findStr.Length 
         || !srcString.Contains(findStr))
        {
            return results;
        }

        var srcLen = srcString.Length;
        var fndLen = findStr.Length;

        for (int i = 0; i < srcLen - fndLen; i++)
        {
            var srcTest = srcString.Substring(i, fndLen);
            if (string.Equals(srcTest, findStr, StringComparison.OrdinalIgnoreCase))
            {
                results.Add(i);
            }
        }

        return results;
    }

    /// <summary>
    /// This method loops through all the characters in the string and builds 
    /// the dictionary each time you call the method, so it is recommended that 
    /// you run the method once and store the results.
    /// </summary>
    /// <returns>
    /// Gives you a dictionary of the characters that exist in this string 
    /// with a count of each character.
    /// </returns>
    public static Dictionary<char, int> GetCharacterInventory(this string srcString)
    {
        var returnVal = new Dictionary<char, int>();

        // loop through all the characters...
        foreach (char c in srcString)
        {
            if (returnVal.ContainsKey(c))
            {
                returnVal[c]++;
            }
            else
            {
                returnVal.Add(c, 1);
            }
        }

        return returnVal;
    }


    public static IEnumerable<string> Split(this string srcString, 
        string delimiter = " ", 
        bool? ignoreCase = null, 
        bool? skipEmptyEntries = null, 
        bool? trimEntries = null)
    {
        // if the source string is null or empty, return the empty string and then break from the method
        if (string.IsNullOrEmpty(srcString)) 
        { 
            yield return string.Empty; 
            yield break;
        }

        // if the delimiter is empty, the normal split breaks it up by character, so, I guess we'll do that.
        if (string.IsNullOrEmpty(delimiter))
        {
            foreach (char c in srcString)
            {
                yield return c.ToString();
            }
            yield break;
        }

        bool ic = ignoreCase ?? false;
        StringComparison sc = ic ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

        // if the src string does not contain the delimiter... just pass back the string.
        if (!srcString.Contains(delimiter, sc))
        {
            yield return srcString;
            yield break;
        }

        /* At this point we know we have a source string that contains the delimiter
         * so we'll do the annoying complicated stuff */

        // now let's tidy up the other parameters
        var skip = false;
        if (skipEmptyEntries is not null && skipEmptyEntries.Value == true) { skip = true; }

        var trim = false;
        if (trimEntries is not null && trimEntries.Value == true) { trim = true; }

        // set up a couple tracking variables
        var delimLen = delimiter.Length;
        var strLen = srcString.Length;
        var previousPosition = 0;
        var currentPosition = 0;
        var testString = ic ? srcString.ToLower() : srcString;
        var testDelim = ic ? delimiter.ToLower() : delimiter;

        while (currentPosition + delimLen < strLen)
        {
            // if the current position has the delimiter, return the string from the previous position (+delim len) to the current position
            // if we did return something, update the previous position to this one
            var testChunk = testString.Substring(currentPosition, delimLen);
            if (string.Equals(testDelim, testChunk))
            {
                var resultStr = srcString.Substring(previousPosition, currentPosition);
                if (resultStr.Length > 0) { yield return resultStr; }
                else if (!skip) { yield return string.Empty; }
                
                previousPosition = currentPosition + delimLen;
            }


            // increment the current position
            currentPosition++;
        }

    }

    /// <summary>
    /// Allows you to replace a set of characters with a single other character while 
    /// traversing the string only once as opposed to multiple Replace calls.
    /// </summary>
    /// <param name="desiredChar">This is the character you want in the result</param>
    /// <param name="replaceChars">These are the characters you want replaced</param>
    /// <returns>The original string with replaceChars replaced by desiredChar</returns>
    public static string ReplaceMany(this string srcString, bool ignoreCase, char desiredChar, params char[] replaceChars)
    {
        if (replaceChars.Length == 0 || string.IsNullOrEmpty(srcString)) 
        { 
            return srcString; 
        }

        var result = new StringBuilder();

        foreach (char c in srcString)
        {
            if (CharHelper.Contains(replaceChars, c, ignoreCase)) 
            { 
                if (desiredChar != '\0')
                {
                    result.Append(desiredChar);
                }
            }
            else 
            { 
                result.Append(c); 
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// Allows you to replace a set of character with a string of characters while 
    /// traversing the string only once as opposed to multiple Replace calls.
    /// </summary>
    /// <param name="desiredString">This is the string you want put into the result</param>
    /// <param name="replaceChars">These are the characters you want replaced</param>
    /// <returns>The original string with replaceChars replaced by desiredString</returns>
    public static string ReplaceMany(this string srcString, bool ignoreCase, string desiredString, params char[] replaceChars)
    {
        if (replaceChars.Length == 0 || string.IsNullOrEmpty(srcString))
        {
            return srcString;
        }

        var result = new StringBuilder();

        foreach (char c in srcString)
        {
            if (CharHelper.Contains(replaceChars, c, ignoreCase)) { result.Append(desiredString); }
            else { result.Append(c); }
        }

        return result.ToString();
    }

    public static string ReplaceMany(this string srcString, bool ignoreCase, char desiredChar, params string[] replaceStrings)
    {
        return ReplaceMany(srcString, ignoreCase, desiredChar.ToString(), replaceStrings);
    }

    public static string ReplaceMany(this string srcString, bool ignoreCase, string desiredString, params string[] replaceStrings)
    {
        if (replaceStrings.Length == 0 || string.IsNullOrEmpty(srcString))
        {
            return srcString;
        }

        var result = new StringBuilder();
        var dStrEmpty = string.IsNullOrEmpty(desiredString); // tells us if what we're including in the result is nothing in place of the replace strings
        var srcLen = srcString.Length;
        var dLen = desiredString.Length;
        var rLenMin = replaceStrings.Min(rs => rs.Length);
        var rLenMax = replaceStrings.Max(rs => rs.Length);
        var checkLen = srcLen - rLenMin;
        var compareType = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

        for (var i = 0; i < srcLen; i++)
        {
            // if the index is past the point where the smallest replace string can fit, we can just tack on the current character
            if (i > checkLen) { result.Append(srcString[i]); continue; }

            var compareChunk = srcString.Substring(i, rLenMax);
        }


        return result.ToString();
    }

    public static bool Contains(this string srcString, IEnumerable<string> findCollection, bool? ignoreCase = null)
    {
        if (string.IsNullOrEmpty(srcString) || findCollection.Count() == 0) { return false; }

        var ic = ignoreCase is not null && ignoreCase == true;

        foreach (var findStr in findCollection)
        {
            if (ic && srcString.Contains(findStr, StringComparison.OrdinalIgnoreCase)) { return true; }
            else if (!ic && srcString.Contains(findStr)) { return true; }
        }

        return false;
    }

    public static bool EqualsOneOf(this string srcString, IEnumerable<string> compareList, StringComparison? comparisonType = null)
    {
        if (string.IsNullOrEmpty(srcString) || compareList.Count() == 0) { return false; }

        StringComparison compareType = comparisonType is null ? StringComparison.OrdinalIgnoreCase : comparisonType.Value;

        foreach (string compareStr in compareList)
        {
            if (string.Equals(srcString, compareStr, compareType)) { return true; }
        }

        return false;

    }

    public static bool StartsWithOneOf(this string srcString, IEnumerable<string> compareList, StringComparison? comparisonType = null)
    {
        if (string.IsNullOrEmpty(srcString) || compareList.Count() == 0) { return false; }

        StringComparison compareType = comparisonType is null ? StringComparison.OrdinalIgnoreCase : comparisonType.Value;

        foreach (string compareStr in compareList)
        {
            if (srcString.StartsWith(compareStr, compareType)) { return true; }
        }

        return false;
    }


    public static IEnumerable<string> Split2(this string srcString, 
        bool? removeEmptyItems = null, 
        bool? trimEachItem = null, 
        bool? ignoreCase = null,
        params string[] splitOn)
    {
        // if there's nothing to split, just return an empty string
        if (string.IsNullOrEmpty(srcString)) { yield return string.Empty; yield break; }

        // if there's nothing to split on, do the default behavior of returning each character separately
        if (splitOn.Length == 0)
        {
            foreach (char c in srcString) { yield return c.ToString(); }
            yield break;
        }

        // get non-nullable versions of the options
        bool rei = removeEmptyItems ?? false;
        bool tei = trimEachItem ?? false;
        bool ic = ignoreCase ?? false;

        // now we go through and find matches to split on
        var retVal = new StringBuilder();
        int srcLen = srcString.Length;
        for (int i = 0; i < srcLen; i++)
        {
            foreach (var splitTerm in splitOn)
            {
                if (srcString[i..].StartsWith(splitTerm, ic, CultureInfo.InvariantCulture))
                {
                    // this means we found a spot to split on, so return whatever is in the string builder, clear it, and increment i
                    var retStr = retVal.ToString();
                    if (tei) { retStr = retStr.Trim(); } // if they want trimmed return items, do that
                    if (!rei || !string.IsNullOrEmpty(retStr)) // if we ignore empty items or the item is not empty, return it
                    {
                        yield return retStr;
                    }
                    i += splitTerm.Length; // move the index up by the length of the split term
                    retVal.Clear();
                    break; // don't need to keep running through the list of split terms
                }
            }

            // if we got here, we didn't find a split term, so add the current character to the string builder
            retVal.Append(srcString[i]);
        }

        // after the for loop is done, we might still have characters in the string builder because there wasn't a split term after those we stored
        if (retVal.Length > 0)
        {
            var retStr = retVal.ToString();
            if (tei) { retStr = retStr.Trim(); } // if they want trimmed return items, do that
            if (!rei || !string.IsNullOrEmpty(retStr)) // if we ignore empty items or the item is not empty, return it
            {
                yield return retStr;
            }
        }

    }



    public static string CleanFancyChars(this string srcString, 
        bool? replaceDoubleQuotes = null, 
        bool? replaceSingleQuotes = null, 
        bool? replaceEllipses = null, 
        bool? replaceLongDashes = null)
    {
        if (string.IsNullOrWhiteSpace(srcString)) { return srcString; }

        // now set up our preference parameter values...
        bool rDQ = replaceDoubleQuotes is null || replaceDoubleQuotes == true;
        bool rSQ = replaceSingleQuotes is null || replaceSingleQuotes == true;
        bool rE  = replaceEllipses is null || replaceEllipses == true;
        bool rLD = replaceLongDashes is null || replaceLongDashes == true;

        // set up our return value...
        string resultStr = srcString;

        // handle double quotes
        if (replaceDoubleQuotes is null || replaceDoubleQuotes == true)
        {
            resultStr = resultStr.Replace(CharHelper.OpenDoubleQuote, '"').Replace(CharHelper.CloseDoubleQuote, '"');
        }





        return resultStr;
    }


    public static string GetCharacterHorizontalRule(
        int length, 
        CharacterHorizontalRulePattern pattern, 
        params char[] characters)
    {
        int len = length < 5 ? 5 : length;
        char defaultChar = '-';
        char[] chars;

        if (characters is null || characters.Length == 0)
        {
            chars = [ defaultChar ];
        }
        else { chars = characters; }

        var retVal = new StringBuilder();

        int charIndex = -1;
        int charCount = chars.Length;
        int groupSize = len / charCount;
        Random ri = new Random();


        while (retVal.Length < len)
        {
            if (pattern == CharacterHorizontalRulePattern.CycleInOrder)
            {
                // go through the characters in order and repeat from 0 so "abc" would result in "abcabcabc"
                charIndex++;
                if (charIndex >= charCount) { charIndex = 0; }
            }
            else if (pattern == CharacterHorizontalRulePattern.GroupInOrder)
            {
                // divide the length by the number of characters and each segment is composed of a grouping 
                // of the character so "abc" might result in a horizontal rule like "aaabbbccc".
                charIndex = 0;
            }
            else if (pattern == CharacterHorizontalRulePattern.RandomOrder)
            {
                // "abc" might end up like "baccacbaaccbabbaa"
                charIndex = ri.Next(0, charCount);
            }
            else
            {
                // default situation where 
                charIndex = 0;
            }

            retVal.Append(chars[charIndex]);
        }

        return retVal.ToString();
    }





    /// <summary>
    /// This method calls the one to get the character inventory, so it loops through 
    /// all the characters in the string.  It is advisable to run this method once 
    /// and store the result.
    /// </summary>
    /// <returns>
    /// Gives you two objects: a list of the characters tied for most common and the 
    /// number of times those characters each appear in the string.
    /// </returns>
    public static (List<char>? mostCommonChars, int numberOfOccurrences) GetMostCommonCharacters(this string srcString)
    {
        var inventory = srcString.GetCharacterInventory();

        var mccCount = inventory.Max(item => item.Value);
        var mccList = inventory.Where(item => item.Value == mccCount).Select(item => item.Key).ToList();

        return (mccList, mccCount);
    }


    public static bool IsInt(this string srcString, out int parsedValue)
    {
        parsedValue = 0;

        if (string.IsNullOrWhiteSpace(srcString)) { return false; }

        var workString = srcString.Replace(" ", ""); // remove all the spaces in case there are any

        while (workString.Contains("--")) { workString = workString.Replace("--", "-"); }
        if (workString.Length > 1 && workString.StartsWith('+')) { workString = workString[1..]; }
        

        // we've cleaned up the string a little.  Anything we might manually check at this point is checked by the parse attempt
        if (int.TryParse(workString, out parsedValue)) { return true; }

        return false;
    }

    public static bool IsDateTime(this string srcString, out DateTime parsedValue)
    {
        parsedValue = DateTime.MinValue;

        if (string.IsNullOrWhiteSpace(srcString)) { return false; }

        if (DateTime.TryParse(srcString, out parsedValue)) { return true; }

        return false;
    }


    /// <summary>
    /// This method searches through the string to pull out every 
    /// integer it can find.  The one case you should be aware of 
    /// is that decimal points are just considered breaks in the 
    /// string, so a string like "costs $5.79 on sale" will return 
    /// 5 and 79.
    /// </summary>
    /// <returns></returns>
    public static List<int> ScrapeInts(this string sourceString)
    {
        var result = new List<int>();

        if (string.IsNullOrWhiteSpace(sourceString)) { return result; }

        var pattern = new Regex("(-?[0-9]+)");
        var matchResult = pattern.Matches(sourceString);
        char[] trimChars = ['0'];

        foreach (Match match in matchResult)
        {
            // should always be a success or it wouldn't be in the collection, but can't hurt to check
            if (match.Success && int.TryParse(match.Value.RemoveLeadingZeroes(), out int parsedValue))
            {
                result.Add(parsedValue);
            }
        }

        return result;
    }

    /// <summary>
    /// Only necessary in place of TrimStart with '0' because it has the ability to remove 
    /// ALL the zeroes leaving an empty string.  We want "00000" to give us "0" and want 
    /// "00304" to give us "304"
    /// </summary>
    private static string RemoveLeadingZeroes(this string sourceString)
    {
        var workString = sourceString;
        while (workString.Length > 1 && workString[0] == '0')
        {
            workString = workString[1..];
        }
        return workString;
    }



    public static int? HexadecimalToIntValue(this string sourceString)
    {
        if (!IsHexadecimalString(sourceString)) { return null; }

        var workStr = sourceString.Trim();
        bool isNegative = false;
        if (workStr.StartsWith('-'))
        {
            isNegative = true;
            workStr = workStr.Substring(1).Trim();
        }

        // now we step through the digits starting at the "ones" place
        int digitMult = 1;
        int result = 0;
        int srcLen = workStr.Length;

        for (int i = srcLen - 1; i >= 0; i--)
        {
            char workChar = workStr[i];
            if (HexadecimalDigitsChar.ContainsKey(workChar))
            {
                result += HexadecimalDigitsChar[workChar] * digitMult;
            }
            else
            {
                return null; // this means we somehow found a digit that got past the test for whether it's a Hexadecimal number
            }

            digitMult *= 16;
        }

        return result * (isNegative ? -1 : 1);
    }

    public static bool IsHexadecimalString(this string sourceString)
    {
        if (string.IsNullOrWhiteSpace(sourceString)) { return false; }
        var workStr = sourceString.Trim();
        if (workStr.StartsWith('-'))
        {
            workStr = workStr.Substring(1).Trim();
        }

        foreach (char c in workStr)
        {
            if (!HexadecimalDigitsChar.ContainsKey(c))
            {
                return false;
            }
        }

        return workStr.Length > 0;
    }


    public static Dictionary<char, int> IntegerDigitsChar = new()
    {
        { '0', 0 }, { '1', 1 }, { '2', 2 }, { '3', 3 }, { '4', 4 },
        { '5', 5 }, { '6', 6 }, { '7', 7 }, { '8', 8 }, { '9', 9 }
    };

    public static Dictionary<string, int> IntegerDigitsString = new()
    {
        { "0", 0 }, { "1", 1 }, { "2", 2 }, { "3", 3 }, { "4", 4 },
        { "5", 5 }, { "6", 6 }, { "7", 7 }, { "8", 8 }, { "9", 9 }
    };

    public static Dictionary <char, int> HexadecimalDigitsChar = new()
    {
        { '0', 0 }, { '1', 1 }, { '2', 2 }, { '3', 3 }, { '4', 4 },
        { '5', 5 }, { '6', 6 }, { '7', 7 }, { '8', 8 }, { '9', 9 },
        { 'A', 10 }, { 'a', 10 },
        { 'B', 11 }, { 'b', 11 },
        { 'C', 12 }, { 'c', 12 },
        { 'D', 13 }, { 'd', 13 },
        { 'E', 14 }, { 'e', 14 },
        { 'F', 15 }, { 'f', 15 }
    };

    public static Dictionary<string, int> HexadecimalDigitsString = new()
    {
        { "0", 0 }, { "1", 1 }, { "2", 2 }, { "3", 3 }, { "4", 4 },
        { "5", 5 }, { "6", 6 }, { "7", 7 }, { "8", 8 }, { "9", 9 },
        { "A", 10 }, { "a", 10 },
        { "B", 11 }, { "b", 11 },
        { "C", 12 }, { "c", 12 },
        { "D", 13 }, { "d", 13 },
        { "E", 14 }, { "e", 14 },
        { "F", 15 }, { "f", 15 }
    };
}

public enum CharacterHorizontalRulePattern
{
    GroupInOrder = 0, 
    CycleInOrder = 1, 
    RandomOrder = 2
}
