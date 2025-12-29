using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTypes.Helpers;

/// <summary>
/// Only works in English with this implementation.  Maybe someday I'll 
/// use this to figure out how to write code that pays attention to the 
/// region.  For now, this should serve my needs though.  This class is 
/// here to help with things like tranlating "One Hundred Four" to 104.
/// </summary>
public static class NumberTextHelper
{

    #region Word Source Properties
    //=================================================================================================================

    /// <summary>
    /// The words that represent the possible digit names of numbers, 
    /// zero through nine in this case.
    /// </summary>
    public static Dictionary<int, string> OnesWords { get; } = new()
    {
        { 0, "Zero" }, { 1, "One" }, { 2, "Two" }, { 3, "Three" }, { 4, "Four" },
        { 5, "Five" }, { 6, "Six" }, { 7, "Seven" }, { 8, "Eight" }, { 9, "Nine" }
    };

    /// <summary>
    /// The words for Eleven to Nineteen since our language is stupid
    /// </summary>
    public static Dictionary<int, string> TeensWords { get; } = new()
    {
        { 11, "Eleven" }, { 12, "Twelve" }, { 13, "Thirteen" }, { 14, "Fourteen" }, { 15, "Fifteen" },
        { 16, "Sixteen" }, { 17, "Seventeen" }, { 18, "Eighteen" }, { 19, "Nineteen" }
    };

    /// <summary>
    /// Words for the multiples of ten.
    /// </summary>
    public static Dictionary<int, string> TensWords { get; } = new()
    {
        { 10, "Ten" }, { 20, "Twenty" }, { 30, "Thirty" }, { 40, "Forty" }, { 50, "Fifty" },
        { 60, "Sixty" }, { 70, "Seventy" }, { 80, "Eighty" }, { 90, "Ninety" }
    };

    /// <summary>
    /// Words for number groups like Thousand.
    /// </summary>
    public static Dictionary<int, string> GroupWords { get; } = new()
    {
        { 100, "Hundred" }, { 1000, "Thousand" }, { 1000000, "Million" }, { 1000000000, "Billion" }
    };

    public static List<string> NegativeWords { get; } = new()
    {
        "Negative", "Minus"
    };

    /// <summary>
    /// In case I decide to allow parsing of decimal numbers too, here are the 
    /// first three words for decimal places like Hundredths.
    /// </summary>
    public static Dictionary<decimal, string> DecimalPlaceWords { get; } = new()
    {
        { 0.1m, "Tenths" }, { 0.01m, "Hundredths" }, { 0.001m, "Thousandths" }
    };

    /// <summary>
    /// Words that equate to the decimal point in case I allow for things like 
    /// parsing "twelve and seventeen hundredths"
    /// </summary>
    public static List<string> DecimalWords { get; } = new()
    {
        "Decimal", "Point", "Dot", "And"
    };

    //=================================================================================================================
    #endregion


    /// <summary>
    /// A convoluted method that gets you the clean words that equal a given integer.  
    /// This is one of the primary methods of this class.
    /// </summary>
    /// <param name="value">The source value</param>
    /// <param name="resultCase">
    /// Tells the method whether to leave the words as proper case (default) or change 
    /// the case to lower or upper.
    /// </param>
    /// <returns>The string representation of the given integer</returns>
    public static string GetWordsFromInt(int value, TargetCaseForNumberText? resultCase = null)
    {
        // make sure we have a default for the text case
        if (resultCase is null) { resultCase = TargetCaseForNumberText.ProperCase; }

        // if it's a small number from -9 to 9, we can return that easily
        var isNegative = value < 0;
        var workerVal = value;
        if (value >= -9 && value <= 9) 
        { 
            if (isNegative)
            {
                workerVal *= -1;
                return $"{NegativeWords[0]} {OnesWords[workerVal]}";
            }
            else
            {
                return OnesWords[workerVal];
            }
        }

        // we should check the next easy ones... 11, 12, and the teens
        if (value >= -19 && value <= -11)
        {
            // negative
            workerVal *= -1;
            return $"{NegativeWords[0]} {TeensWords[workerVal]}";
        }
        else if (value >= 11 && value <= 19)
        {
            return TeensWords[value];
        }


        // now we do a little juggling just in case we're working with int.MinVal
        var absoluteValue = 0;
        
        var isMinVal = value == int.MinValue;
        var addOne = false;

        if (isMinVal)
        {
            // we already know it's negative and it started at min val... 
            // so, just set it to max value, and add one to the ones digit 
            // when we translate it to text
            absoluteValue = int.MaxValue;
            addOne = true;
        }
        else if (isNegative)
        {
            absoluteValue = value * -1;
        }
        else
        {
            absoluteValue = value;
        }

        var resultString = new StringBuilder();
        var currentGroup = 1;
        while (absoluteValue > 0) 
        {
            var smallGroup = absoluteValue % 1000; // gets us the current hundreds
            if (addOne)
            {
                // this should NOT move us over 999 since it only happens with int min val
                smallGroup++;
                addOne = false;
            }
            absoluteValue = (absoluteValue - smallGroup) / 1000; // removes that much from the worker and then moves us to the next group

            // now that we've done all the record keeping, let's get the words for this group of numbers
            var hasValue = smallGroup > 0;
            var onesPart = smallGroup % 10;
            smallGroup -= onesPart;
            var tensPart = smallGroup % 100;
            var hundredsPart = smallGroup - tensPart;
            

            var chunkStr = string.Empty;

            // hundreds part is easy
            if (hundredsPart > 0)
            {
                chunkStr += $"{OnesWords[hundredsPart / 100]} {GroupWords[100]} ";
            }

            // tens and ones parts are hard since we have to figure out if it's a teen
            if (tensPart == 10 && onesPart > 0)
            {
                chunkStr += TeensWords[tensPart + onesPart];
            }
            else
            {
                if (tensPart > 0)
                {
                    chunkStr += TensWords[tensPart];
                }

                if (tensPart > 0 && onesPart > 0)
                {
                    chunkStr += '-';
                }

                if (onesPart > 0)
                {
                    chunkStr += OnesWords[onesPart];
                }
            }

            if (hasValue && currentGroup > 1)
            {
                chunkStr += $" {GroupWords[currentGroup]} ";
            }

            resultString.Insert(0, chunkStr);


            // okay, one more record keeping since the next group is 1000 times greater
            currentGroup *= 1000;
        }

        if (isNegative)
        {
            resultString.Insert(0, $"{NegativeWords[0]} ");
        }

        var returnVal = resultString.ToString().Trim();
        while (returnVal.Contains("  ")) { returnVal = returnVal.Replace("  ", " "); }

        if (resultCase == TargetCaseForNumberText.LowerCase)
        {
            returnVal = returnVal.ToLower();
        }
        else if (resultCase == TargetCaseForNumberText.Uppercase)
        {
            returnVal = returnVal.ToUpper();
        }

        return returnVal;
    }



    /// <summary>
    /// One of the main methods of this class, it attempts to read a string 
    /// that you provide and give you the integer value represented.  So, you 
    /// could pass the string "four thousand five hundred sixty-seven" and 
    /// get back the result 4567.  Could fail if you give a string that is 
    /// not written grammatically correctly for numbers like if you forget 
    /// the dash between numbers from twenty-one to ninety-nine or if you 
    /// include extra stuff like an "and" in "five hundred and sixty-seven" 
    /// since the "and" is supposed to be a break between the integer part 
    /// and the decimal part.
    /// </summary>
    public static int? GetIntValueFromWords(string sourceString)
    {
        // with no source, return null
        if (string.IsNullOrWhiteSpace(sourceString)) { return null; }

        // break up the string, and if there are no parts, return null
        var parts = CleanNumberStringParts(sourceString);
        var partCount = parts.Count;
        if (partCount == 0) { return null; }

        // now finally we loop through the parts and add anything that translates to a 
        // number to our worker variable and also check to see if the first part means 
        // the number is a negative number.
        int calculatedVal = 0;
        int startIndex = 0;
        var isNegative = false;
        if (partCount > 1 && RepresentsNegative(parts[0]))
        {
            startIndex = 1;
            isNegative = true;
        }

        var foundSomething = false;
        char[] spaceDelim = { ' ' };
        for (var i = startIndex; i < partCount; i++)
        {
            // if it has a space, there's a group word, so we need the number and the value of the group word
            if (parts[i].Contains(' '))
            {
                string[] subParts = parts[i].Split(spaceDelim);
                var subPartCount = subParts.Length;
                if (subPartCount == 2)
                {
                    var baseVal = GetChunkValue(subParts[0]);
                    var multVal = GroupWords.Where(gw => string.Equals(gw.Value, subParts[1], StringComparison.OrdinalIgnoreCase)).Select(gw => gw.Key);
                    if (baseVal is not null && multVal.Any())
                    {
                        calculatedVal += baseVal.Value * multVal.First();
                        foundSomething = true;
                    }
                }
            }
            // with no space, we are only looking for a number from 0 to 99
            else
            {
                var chunkVal = GetChunkValue(parts[i]);
                if (chunkVal is not null)
                {
                    calculatedVal += chunkVal.Value;
                    foundSomething = true;
                }
            }

        }

        if (foundSomething)
        {
            if (calculatedVal != 0 && isNegative) { calculatedVal *= -1; }
            return calculatedVal;
        }


        //if we got here, return null;
        return null;
    }


    /// <summary>
    /// This method takes your source string, translates it to a number if possible, 
    /// and then translates it back to words that are properly formatted and cased.
    /// </summary>
    /// <param name="sourceString"></param>
    /// <param name = "resultCase" >
    /// Tells the method whether to leave the words as proper case (default) or change 
    /// the case to lower or upper.
    /// </param>
    /// <returns>the clean word representation of the integer</returns>
    public static string GetCleanWordsFromWords(string sourceString, TargetCaseForNumberText? resultCase = null)
    {
        // get out fast if the source is empty
        if (string.IsNullOrWhiteSpace(sourceString)) { return string.Empty; }

        // use our already defined method to get an integer value from the words given
        var intVal = GetIntValueFromWords(sourceString);
        if (intVal is null) { return string.Empty; }

        // if we get here, we have a valid integer and just need to get the clean words
        return GetWordsFromInt(intVal.Value, resultCase);
    }

    /// <summary>
    /// This method breaks up an integer into separate integers for each 
    /// digit in the number.  The one thing to be aware of is that if the 
    /// source number is negative, only the first digit in the list will 
    /// be negative as the indication that the whole number is.
    /// </summary>
    public static List<int> GetDigitValueList(int sourceValue)
    {
        var isNegative = sourceValue < 0;
        var resultList = new List<int>();

        if (sourceValue == 0)
        {
            resultList.Add(0);
            return resultList;
        }

        var workValue = sourceValue;
        var addOne = sourceValue == int.MinValue;
        if (isNegative)
        {
            if (addOne)
            {
                workValue = int.MaxValue;
            }
            else
            {
                workValue *= -1;
            }
        }

        

        while (workValue > 0) 
        {
            var currentDigit = workValue % 10;
            resultList.Insert(0, currentDigit);
            workValue = (workValue - currentDigit) / 10;
        }

        if (isNegative)
        {
            resultList[0] *= -1;
        }


        return resultList;
    }




    /// <summary>
    /// This does NOT guarantee that the strings represent numbers, but it does 
    /// break up the string into logical groups.  "nine thousand fifty-one" would 
    /// give you a list with two items: "nine thousand" and "fifty-one".  If that's 
    /// all you wanted, then you're all set.  This method is actually here as a 
    /// helper method though, so you can get either an integer value or a clean 
    /// string using the other methods in this class.
    /// </summary>
    public static List<string> CleanNumberStringParts(string sourceString)
    {
        var resultList = new List<string>();
        if (string.IsNullOrWhiteSpace(sourceString)) { return resultList; }

        // we know we have SOMETHING.  So, we'll clean that and see if we have anything left
        var cleanString = GetCleanSourceString(sourceString);
        if (string.IsNullOrWhiteSpace(cleanString)) { return resultList; }

        // now we should be able to break it up by spaces
        char[] spaceDelim = { ' ' };
        string[] parts = cleanString.Split(spaceDelim);

        // Now that we have a clean source and have it split into chunks that SHOULD be complete parseable bits...
        foreach (var part in parts)
        {
            resultList.Add(part.Replace("_", " ")); // switch the space back in if the underscore was there keeping text together
        }

        return resultList;
    }



    /// <summary>
    /// This is a helper method.  You give it a string that doesn't have 
    /// spaces in it to get a number from 0 to 99.  Anything higher than 
    /// that would have another word after it, like "hundred".  Will 
    /// return null if outside that range.  Also keep in mind that a 
    /// dash is used between tens and ones for most of those numbers, 
    /// like "forty-two".
    /// </summary>
    private static int? GetChunkValue(string sourceString)
    {
        var cleanSrc = sourceString.Trim();

        if (string.IsNullOrWhiteSpace(cleanSrc) || cleanSrc.Contains(' ')) { return null; }

        // let's see if it's zero through nine first
        var linqWorker = OnesWords.Where(w => string.Equals(w.Value, cleanSrc, StringComparison.OrdinalIgnoreCase));
        if (linqWorker.Any())
        {
            return linqWorker.First().Key;
        }

        // now let's check eleven through nineteen
        linqWorker = TeensWords.Where(w => string.Equals(w.Value, cleanSrc, StringComparison.OrdinalIgnoreCase));
        if (linqWorker.Any())
        {
            return linqWorker.First().Key;
        }

        // now we'll check for pure multiples of ten, just in case
        linqWorker = TensWords.Where(w => string.Equals(w.Value, cleanSrc, StringComparison.OrdinalIgnoreCase));
        if (linqWorker.Any())
        {
            return linqWorker.First().Key;
        }

        // finally, the most complicate case, words with tens and ones digits
        char[] dashDelim = { '-' };
        if (cleanSrc.Count(c => c == '-') == 1)
        {
            string[] parts = cleanSrc.Split(dashDelim);
            if (parts.Length == 2)
            {
                // it really should be 2 after our first check
                var calcVal = 0;

                // get tens value first
                linqWorker = TensWords.Where(w => string.Equals(w.Value, parts[0], StringComparison.OrdinalIgnoreCase));
                if (linqWorker.Any())
                {
                    calcVal += linqWorker.First().Key;

                    // now see if the second part is a ones digit
                    linqWorker = OnesWords.Where(w => string.Equals(w.Value, parts[1], StringComparison.OrdinalIgnoreCase));
                    if (linqWorker.Any())
                    {
                        calcVal += linqWorker.First().Key;
                        return calcVal;
                    }
                }
            }
        }

        // If we didn't return from within one of those logic blocks, just return null.
        return null;
    }

    /// <summary>
    /// This helper method basically runs trims and reduction 
    /// code to make sure spacing is okay and then groups things 
    /// together with underscores to make splitting easier.  For 
    /// example, "seven thousand" would be "seven_thousand".
    /// </summary>
    private static string GetCleanSourceString(string sourceString)
    {
        var cleanString = sourceString.Trim();

        // get rid of any characters that aren't letters, spaces, or dashes
        var strBuilder = new StringBuilder();
        foreach (var c in cleanString)
        {
            if (c == ' ' || c == '-' || (c >= 65 && c <= 90) || (c >= 97 && c <=122))
            {
                strBuilder.Append(c);
            }
        }
        cleanString = strBuilder.ToString();

        // squishes multiple spaces down to one
        while (cleanString.Contains("  ")) { cleanString = cleanString.Replace("  ", " "); }

        // gets rid of spaces before or after dashes
        while (cleanString.Contains(" -")) { cleanString = cleanString.Replace(" -", "-"); }
        while (cleanString.Contains("- ")) { cleanString = cleanString.Replace("- ", "-"); }

        // should be sure they didn't put in multiple dashes together
        while (cleanString.Contains("--")) { cleanString = cleanString.Replace("--", "-"); }

        // just in case one of those dashes is at the beginning... we're not treating "-two" as "negative two"
        if (cleanString.StartsWith('-') && cleanString.Length > 1) { cleanString = cleanString[1..]; }

        // for ease of splitting later, stick in underscores
        foreach (var groupStr in GroupWords)
        {
            cleanString = cleanString.Replace($" {groupStr}", $"_{groupStr}");
        }

        // and just in case a space ended up at the beginning or end, we'll do another trim()
        return cleanString.Trim();
    }

    /// <summary>
    /// A helper to tell us if something matches a word for negative
    /// </summary>
    private static bool RepresentsNegative(string sourceString)
    {
        return NegativeWords.Where(nw => string.Equals(nw, sourceString, StringComparison.OrdinalIgnoreCase)).Any();
    }



}

public enum TargetCaseForNumberText
{
    LowerCase = -1, 
    ProperCase = 0, 
    Uppercase = 1
}
