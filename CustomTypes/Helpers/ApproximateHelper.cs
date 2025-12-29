using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace CustomTypes.Helpers;

public static class ApproximateHelper
{
    /// <summary>
    /// This method is meant to make it easier to compare decimals that are close enough.  3.14 and 3.14159 
    /// are probably close enough for some purposes.  To be clear, 5.25 is not approximately equal to 7.25.  
    /// Also keep in mind that this does not round the values so if you want 1 place of accuracy and compare 
    /// 2.50 to 2.49 it will not round.  If you want that kind of comparison, use the Math.Round or 
    /// Decimal.Round methods on your values before you compare them.
    /// </summary>
    /// <param name="srcValue">this number</param>
    /// <param name="compareValue">the number to compare to</param>
    /// <param name="digitsAccuracy">
    /// how many decimal places do you want to match exactly?  Defaults to 2, and can be 0 (essentially just 
    /// comparing the whole number parts) to 20 (decimals have the potential to reach 29 decimal digits, but 
    /// since this is an approximate equality, 20 digits should be enough for most purposes).  If you pass a 
    /// negative number, it will default to 0.  If you pass a number above 20, it will default to 20.
    /// </param>
    /// <returns>True if the numbers are equal to the decimal accuracy chosen</returns>
    public static bool ApproximateMatch(this decimal srcValue, decimal compareValue, int? digitsAccuracy = null)
    {
        decimal srcF = srcValue % 1.00000m;
        decimal srcI = srcValue - srcF;

        decimal cmpF = compareValue % 1.00000m;
        decimal cmpI = compareValue - cmpF;

        if (srcI != cmpI) { return false; } // nice easy one if the whole number part is different

        int acc = digitsAccuracy ?? 2;
        if (digitsAccuracy < 0) { acc = 0; }
        else if (digitsAccuracy > 20) { acc = 20; }

        if (digitsAccuracy == 0) { return true; } // if we got here, it means the whole number part is the same, and we're not comparing the fractional part

        // now we know the accuracy being tested is at least 1, so we need to loop and keep comparing decimal digits until we find a break or reach the accuracy
        while (acc > 0)
        {
            srcF *= 10.00000m; // so the whole part is the next digit to compare
            srcI = srcF - (srcF % 1.00000m); // the whole part

            cmpF *= 10.00000m;
            cmpI *= cmpI - (cmpF % 1.00000m);

            if (srcI != cmpI) { return false; } // if that whole part isn't the same, the decimal part isn't close enough

            acc--; // count down the accuracy digits
        }

        return true; // if we got through the while loop
    }



    /// <summary>
    /// This method is meant to make it easier to compare doubles that are close enough.  3.14 and 3.14159 
    /// are probably close enough for some purposes.  To be clear, 5.25 is not approximately equal to 7.25.  
    /// Also keep in mind that this does not round the values so if you want 1 place of accuracy and compare 
    /// 2.50 to 2.49 it will not round.  If you want that kind of comparison, use the Math.Round method on 
    /// your values before you compare them.
    /// </summary>
    /// <param name="srcValue">this number</param>
    /// <param name="compareValue">the number to compare to</param>
    /// <param name="digitsAccuracy">
    /// how many double places do you want to match exactly?  Defaults to 2, and can be 0 (essentially just 
    /// comparing the whole number parts) to 20.  If you pass a negative number, it will default to 0.  If 
    /// you pass a number above 20, it will default to 20.
    /// </param>
    /// <returns>True if the numbers are equal to the double accuracy chosen</returns>
    public static bool ApproximateMatch(this double srcValue, double compareValue, int? digitsAccuracy = null)
    {
        double srcF = srcValue % 1.00000;
        double srcI = srcValue - srcF;

        double cmpF = compareValue % 1.00000;
        double cmpI = compareValue - cmpF;

        if (srcI != cmpI) { return false; } // nice easy one if the whole number part is different

        int acc = digitsAccuracy ?? 2;
        if (digitsAccuracy < 0) { acc = 0; }
        else if (digitsAccuracy > 20) { acc = 20; }

        if (digitsAccuracy == 0) { return true; } // if we got here, it means the whole number part is the same, and we're not comparing the fractional part

        // now we know the accuracy being tested is at least 1, so we need to loop and keep comparing double digits until we find a break or reach the accuracy
        while (acc > 0)
        {
            srcF *= 10.00000; // so the whole part is the next digit to compare
            srcI = srcF - (srcF % 1.00000); // the whole part

            cmpF *= 10.00000;
            cmpI *= cmpI - (cmpF % 1.00000);

            if (srcI != cmpI) { return false; } // if that whole part isn't the same, the double part isn't close enough

            acc--; // count down the accuracy digits
        }

        return true; // if we got through the while loop
    }


    /// <summary>
    /// Rounds this number and the compare number and then tells you if the rounded values are equal.  Basically 
    /// a convenience method so you don't have to write out the code to do it.
    /// </summary>
    /// <param name="srcValue">this number</param>
    /// <param name="compareValue">the number to compare to</param>
    /// <param name="roundingDigits">
    /// defaults to 2; minimum 0; maximum 20; and will adjust numbers outside the range to the nearest allowed.
    /// </param>
    /// <returns>true if the rounded values are equal</returns>
    public static bool ApproximateEquals(this decimal srcValue, decimal compareValue, int? roundingDigits = null)
    {
        int rd = roundingDigits ?? 2;
        if (rd < 0) { rd = 0; }
        else if (rd > 20) { rd = 20; }

        decimal roundedSrc = Decimal.Round(srcValue, rd);
        decimal roundedComp = Decimal.Round(compareValue, rd);

        return roundedSrc == roundedComp;
    }


    /// <summary>
    /// Rounds this number and the compare number and then tells you if the rounded values are equal.  Basically 
    /// a convenience method so you don't have to write out the code to do it.
    /// </summary>
    /// <param name="srcValue">this number</param>
    /// <param name="compareValue">the number to compare to</param>
    /// <param name="roundingDigits">
    /// defaults to 2; minimum 0; maximum 20; and will adjust numbers outside the range to the nearest allowed.
    /// </param>
    /// <returns>true if the rounded values are equal</returns>
    public static bool ApproximateEquals(this double srcValue, double compareValue, int? roundingDigits = null)
    {
        int rd = roundingDigits ?? 2;
        if (rd < 0) { rd = 0; }
        else if (rd > 20) { rd = 20; }

        double roundedSrc = Math.Round(srcValue, rd);
        double roundedComp = Math.Round(compareValue, rd);

        return roundedSrc == roundedComp;
    }


    /// <summary>
    /// Fundamentally inefficient because of string operations but gives you a way to check for whether two strings 
    /// are essentially the same despite minor differences.  "Home Work" could come back as the same as "homework!" 
    /// depending on the options you select.
    /// </summary>
    /// <param name="srcValue">this string</param>
    /// <param name="compareValue">the string you want to compare to</param>
    /// <param name="ignoreCase">defaults to false meaning it will only return true if the letter casing is the same.</param>
    /// <param name="ignoreWhiteSpace">defaults to false meaning it will make sure the white space is the same.</param>
    /// <param name="ignoreSymbols">defaults to false meaning it will make sure punctuation and other symbols will match.</param>
    /// <returns>True if the strings count as the same given the parameter choices made.</returns>
    public static bool ApproximateEquals(this string srcValue, 
        string compareValue, 
        bool? ignoreCase = null, 
        bool? ignoreWhiteSpace = null, 
        bool? ignoreSymbols = null)
    {
        string srcWorker = srcValue;
        string compareWorker = compareValue;

        StringComparison sc = (ignoreCase ?? false) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        bool iws = ignoreWhiteSpace ?? false;
        bool isym = ignoreSymbols ?? false;

        // now adjust the workers based on the parameters
        if (iws)
        {
            srcWorker = srcWorker.RemoveWhiteSpace();
            compareWorker = compareWorker.RemoveWhiteSpace();
        }

        if (isym)
        {
            srcWorker = srcWorker.RemoveSymbols();
            compareWorker = compareWorker.RemoveSymbols();
        }

        // finally, run the normal string equals on our modified strings
        return string.Equals(srcWorker, compareWorker, sc);
    }




}
