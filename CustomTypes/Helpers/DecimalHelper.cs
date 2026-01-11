using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CustomTypes.Helpers;

public static class DecimalHelper
{
    /// <summary>
    /// This method give a way to round a decimal to a specific place, like 
    /// the ones-place to give an integer; the tenths-place (like 0.1); or 
    /// any place represented by a decimal with a 1 in the important spot 
    /// with zeroes in the other.  If you pass something other than that, 
    /// it will take the biggest value you passed if it was 1 or above, 
    /// and the smallest place if it was below 1.  45.845 would give you 
    /// 10.0 and 0.345 would give you 0.001 for which place to focus on.  
    /// This does the useful kind of rounding where 2.5 => 3.0 and 3.5 => 
    /// 4.0 instead of that silly odds and evens thing.  Defaults the 
    /// place to 1
    /// </summary>
    public static decimal Round(this decimal value, decimal? place = null)
    {
        decimal desiredPlace = ForceOne(place ?? decimal.One);

        decimal removePart = value % desiredPlace; // the remainder if we were to divide by the desiredPlace, so 345.34 => 45.34
        decimal startingValue = value - removePart; // something like 345.34 => 300.0

        if (value >= decimal.Zero)
        {
            // positive value where we want to figure out whether we should round up...
            if (removePart * 2.00000m > desiredPlace) { return startingValue + desiredPlace; }
            else { return startingValue; }
        }
        else
        {
            // negative value where we need to figure out if we should round down, like -3.7 => -4.00
            if (removePart * -2.00000m > desiredPlace) { return startingValue - desiredPlace; }
            else { return startingValue; }
        }
    }

    /// <summary>
    /// This method is meant to ensure we have one digit that is not zero 
    /// and that it is a one.  So, something like 0.2345 would turn into 
    /// 0.0001.  This is useful for figuring out what place we want to 
    /// focus on and was written for use with the rounding method.  As a 
    /// note: 32.1 would go the other way and focus on the whole numbers, 
    /// giving the return value of 10 in that example.  Zero will return 
    /// a 1 since the relevant value is expressed by the ones-place.
    /// </summary>
    private static decimal ForceOne(decimal sourceVal)
    {
        if (sourceVal == decimal.Zero) { return decimal.One; }

        decimal result = decimal.One;
        decimal worker = sourceVal;

        if (sourceVal < 1.00000m && sourceVal > -1.00000m)
        {
            // This means we have only the fractional part to worry about
            while (worker != decimal.Zero)
            {
                worker = (worker * 10.00000m) % decimal.One; // 0.45 => 4.5 => 0.5
                result *= 0.10000m; // 1.0000 => 0.1000
            }
        }
        else
        {
            // If we got here, it's the integer part we need to count digits for.
            // and we first need to get rid of any fractional part...
            worker = worker - (worker % decimal.One);

            // now the lovely loop
            while (worker != decimal.Zero)
            {
                worker = worker / 10.00000m; // shift the decimal place over one
                worker = worker - (worker % decimal.One); // remove the fractional part each time
                result *= 10.00000m;
            }
        }

        return result;
    }

    /// <summary>
    /// Tells you if two decimal values are close enough that they can be considered 
    /// equal for whatever work you're doing.  3.14 might be close enough to 3.14159 
    /// to count as equal.
    /// </summary>
    /// <param name="compareVal">The number to compare the parent value to</param>
    /// <param name="howClose">How close do they need to be?  Defaults to 0.05</param>
    /// <returns>true if the compared numbers are within the how close value difference to each other.</returns>
    public static bool ApproxEquals(this decimal sourceVal, decimal compareVal, decimal? howClose = null)
    {
        if (sourceVal == compareVal) { return true; } // take care of the easy case first
        
        decimal accuracy = howClose ?? 0.005m;
        accuracy = accuracy % decimal.One; // trim any whole parts
        if (accuracy < decimal.Zero) { accuracy *= -1; } // make sure it's a positive number

        decimal diff = sourceVal - compareVal;
        if (diff < decimal.Zero) { diff *= -1; }

        return diff <= accuracy;
    }


    public static decimal CreateDecimal(int sourceNumbers, int shiftDecimalPlaces)
    {
        int sdp = shiftDecimalPlaces;
        if (sdp < 0) { sdp = 0; }
        if (sdp > 15) { sdp = 15; }

        decimal multiplier = 1.00000m;
        for (int p = 0; p < sdp; p++)
        {
            multiplier *= 0.10000m;
        }

        return multiplier * sourceNumbers;
    }

    /// <summary>
    /// Uses loops to figure out the lowest numerator and denominator that is close to 
    /// this decimal value.  Will go out to 5 decimal places to try to get integer 
    /// numerator and denominator that get you a close value.
    /// </summary>
    public static void GetFraction(this decimal value, out int numerator, out int denominator)
    {
        decimal n = 0.00000m;
        decimal d = 1.00000m;
        decimal calcVal = 0.0m;

        bool isNegative = false;
        decimal v = value;
        if (v < 0.0m)
        {
            isNegative = true;
            v *= -1.0m;
        }

        // handle the 0 case
        if (v.ApproxEquals(decimal.Zero, 0.0001m))
        {
            numerator = 0;
            denominator = 1;
            return;
        }


        // handle the 1 case
        if (v.ApproxEquals(decimal.One, 0.0001m))
        {
            numerator = 1;
            denominator = 1;
            return;
        }

        // if the value is greater than 1, we can separate the integer part from the fractional part before running our loop
        decimal vIntPart = v - (v % 1.00000m);
        v = v - vIntPart;

        // now that we have a number from 0.00001 to 0.99999, we need to find a set of numbers that divide to that number
        while (!v.ApproxEquals(calcVal, 0.0001m))
        {
            if (calcVal > v)
            {
                // the fraction is too big, so we need to increase the denominator
                d += 1.0m;
            }
            else if (calcVal < v)
            {
                // the fraction is too small, so we need to increase the numerator
                n += 1.0m;
            }

            calcVal = n / d;
        }

        // re-integrate the integer part
        n += vIntPart * d;

        // deal with negative if necessary
        if (isNegative) { n *= -1.0m; }

        // fille the out parameter values
        numerator = Convert.ToInt32(n);
        denominator = Convert.ToInt32(d);
    }

    
    /// <summary>
    /// Gives you the number of digits of the fractional part of this number.  For 
    /// example, 3.14159 woule return 5.
    /// </summary>
    public static int DecimalPlaceDigits(this decimal value)
    {
        decimal decPart = value % 1.00000m;
        int placeCount = 0;

        while (decPart != 0.00000m)
        {
            placeCount++; // bump up our counter
            decPart *= 10.00000m; // shift the value over...
            decPart = decPart % 1.00000m; // ... so we can strip off the whole number part
        }

        return placeCount;
    }


    public static Regex DecimalPattern { get; } = new Regex("/([+-]?[0-9]*\\.?[0-9]*)/gm", RegexOptions.Compiled);

    /// <summary>
    /// Gets you all the decimal values it can find withint a source string.  If the complete string is a decimal value, 
    /// it will just return that.
    /// </summary>
    public static IEnumerable<decimal> TryParseDecimalsWithRegex(this string sourceString)
    {
        var matches = DecimalPattern.Matches(sourceString);

        if (matches is null || matches.Count == 0) { yield break; }

        foreach (Match match in matches.Where(m => m.Success))
        {
            if (decimal.TryParse(match.Value, out decimal result)) { yield return result; }
        }
    }





}
