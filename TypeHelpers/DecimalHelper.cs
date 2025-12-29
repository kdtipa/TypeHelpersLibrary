using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeHelpers;

public static class DecimalHelper
{
    /// <summary>
    /// Allows you to compare decimal values out to a particular number of decimal 
    /// digits.  So, if you have 6.123456 and 6.12345189 you can tell the method 
    /// to only care about 0 to 5 digits and it'll return true.  If you want the 
    /// fifth digit in that example to take into account that the first one would 
    /// round up and the second would not, you should probably use 4 digits or just 
    /// use the version of this method that gives a level of accuracy with a decimal 
    /// value for how different the values can be and still count as equal.
    /// </summary>
    /// <param name="compare">The value you want to compare this decimal to.</param>
    /// <param name="decimalDigits">
    /// An optional number of digits to use for the comparison.  If left blank/null, 
    /// it will default to 2 digits.  The lowest value you can pass is 0, which means 
    /// you only care about the integer part.  The highest value you can pass is 12 
    /// which is pretty fine detail beyond which you might as well just ask for normal 
    /// equality.  Yes, it's arbitrary.
    /// </param>
    /// <returns>
    /// Returns true if the decimal value is the same as the compare value out to at 
    /// least the number of digits specified (or defaulted) by decimalDigits.
    /// </returns>
    public static bool ApproximateEquals(this decimal value, 
        decimal compare, 
        int? decimalDigits = null)
    {
        // start with figuring out number of digits...
        int decDigits = 2; // default to 2 places
        if (decimalDigits is not null && decimalDigits.Value >= 0 && decimalDigits.Value <= 12)
        {
            decDigits = decimalDigits.Value;
        }

        // let's get our modulus value
        var mod = 1.0m;
        var divCount = 0;
        while (divCount < decDigits)
        {
            mod = mod / 10.0m;
            divCount++;
        }

        // now get the decimal values out to only the specified number of digits
        var trimVal = value - (value % mod);
        var trimCmp = compare - (compare % mod);

        // return the comparison
        return trimVal == trimCmp;
    }

    /// <summary>
    /// A newer version of an equality check between decimals that is approximate.  You 
    /// provide the number to compare to and the number of decimal digits to want to compare.  
    /// For example, if you compare 5.6789 to 5.6711111 and the decimal digits are 0, 1, or 2, 
    /// it will come back as true.  But if you want 3 digits or more, it will come back false.
    /// </summary>
    /// <param name="value">this object's value</param>
    /// <param name="compare">the value to compare to</param>
    /// <param name="numberOfDecimalDigits">
    /// The number of decimal digits to compare to.  If you pass null, it defaults to 2.  Can 
    /// be a value from 0 (essentially integer part equality) to 10.
    /// </param>
    /// <returns>true if the decimals are equal to the specified number of digits</returns>
    public static bool ApproxEquals(this decimal value, decimal compare, int? numberOfDecimalDigits = null)
    {
        int nodd = 2;
        if (numberOfDecimalDigits is not null && numberOfDecimalDigits >= 0 && numberOfDecimalDigits <= 10)
        {
            nodd = numberOfDecimalDigits.Value;
        }

        // check the integer parts first
        decimal vIntPart = value - (value % 1.0000000000m);
        decimal cIntPart = compare - (compare % 1.0000000000m);

        if (vIntPart != cIntPart) { return false; }

        // now remove the integer part so we don't have to worry about overflowing the decimals
        decimal vWorker = value - vIntPart;
        decimal cWorker = compare - cIntPart;

        // now we're going to shift the number of decimal digits we want to compare to the integer side
        for (int m = 1; m <= nodd; m++)
        {
            vWorker *= 10.0000000000m;
            cWorker *= 10.0000000000m;
        }

        // now we subtract the remaining decimal parts
        vWorker = vWorker - (vWorker % 1.0000000000m);
        cWorker = cWorker - (cWorker % 1.0000000000m);

        // so now we are effectively comparing the decimal digits to the right number of places
        if (vWorker != cWorker) { return false; }

        return true;
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
        if (v.ApproximateEquals(0.00000m, 4))
        {
            numerator = 0;
            denominator = 1;
            return;
        }


        // handle the 1 case
        if (v.ApproximateEquals(1.00000m, 4))
        {
            numerator = 1;
            denominator = 1;
            return;
        }

        // if the value is greater than 1, we can separate the integer part from the fractional part before running our loop
        decimal vIntPart = v - (v % 1.00000m);
        v = v - vIntPart;

        // now that we have a number from 0.00001 to 0.99999, we need to find a set of numbers that divide to that number
        while (!v.ApproximateEquals(calcVal, 4))
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

    public static int GetCountOfDecimalPlaces(this decimal value)
    {
        decimal decPart = value % 1.0m;
        int placeCount = 0;

        while (decPart > 0.0m)
        {
            decPart = (decPart * 10.0m) % 1.0m;
            placeCount++;
        }

        return placeCount;
    }


    


}
