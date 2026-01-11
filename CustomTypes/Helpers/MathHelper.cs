using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTypes.Helpers;

public static class MathHelper
{
    public static long getWholePart(decimal sourceValue)
    {
        return Convert.ToInt64(sourceValue - (sourceValue % 1.00000m));
    }

    public static bool TryGetWholePart(decimal sourceValue, out int wholePart)
    {
        try
        {
            wholePart = Convert.ToInt32(sourceValue - (sourceValue % 1.00000m));
            return true;
        }
        catch
        {
            wholePart = 0;
            return false;
        }
    }

    /// <summary>
    /// The Math library has a power method that takes doubles.  This set of 
    /// methods just does the conversions for us and returns the more reasonable 
    /// decimal type.  This does mean that the inaccuracy of doubles is still 
    /// potentially present.
    /// </summary>
    /// <param name="baseVal">The root value for this exponentiation</param>
    /// <param name="powerVal">The exponent</param>
    /// <returns>The decimal result of raising the base value to the power value</returns>
    public static decimal RaiseToPower(long baseVal, long powerVal)
    {
        return Convert.ToDecimal(Math.Pow(baseVal, powerVal));
    }

    /// <summary>
    /// The Math library has a power method that takes doubles.  This set of 
    /// methods just does the conversions for us and returns the more reasonable 
    /// decimal type.  This does mean that the inaccuracy of doubles is still 
    /// potentially present.
    /// </summary>
    /// <param name="baseVal">The root value for this exponentiation</param>
    /// <param name="powerVal">The exponent</param>
    /// <returns>The decimal result of raising the base value to the power value</returns>
    public static decimal RaiseToPower(decimal baseVal, decimal powerVal)
    {
        return Convert.ToDecimal(Math.Pow(
            Convert.ToDouble(baseVal), 
            Convert.ToDouble(powerVal)));
    }

    /// <summary>
    /// The Math library has a power method that takes doubles.  This set of 
    /// methods just does the conversions for us and returns the more reasonable 
    /// decimal type.  This does mean that the inaccuracy of doubles is still 
    /// potentially present.
    /// </summary>
    /// <param name="baseVal">The root value for this exponentiation</param>
    /// <param name="powerVal">The exponent</param>
    /// <returns>The decimal result of raising the base value to the power value</returns>
    public static decimal RaiseToPower(long baseVal, decimal powerVal)
    {
        return Convert.ToDecimal(Math.Pow(
            Convert.ToDouble(baseVal),
            Convert.ToDouble(powerVal)));
    }

    /// <summary>
    /// The Math library has a power method that takes doubles.  This set of 
    /// methods just does the conversions for us and returns the more reasonable 
    /// decimal type.  This does mean that the inaccuracy of doubles is still 
    /// potentially present.
    /// </summary>
    /// <param name="baseVal">The root value for this exponentiation</param>
    /// <param name="powerVal">The exponent</param>
    /// <returns>The decimal result of raising the base value to the power value</returns>
    public static decimal RaiseToPower(decimal baseVal, long powerVal)
    {
        return Convert.ToDecimal(Math.Pow(
            Convert.ToDouble(baseVal),
            Convert.ToDouble(powerVal)));
    }

    public static decimal? Root(this int sourceVal, int exponent)
    {
        if (sourceVal < 0) { return null; }
        if (exponent < 1) { return null; }

        if (sourceVal == 0) { return decimal.Zero; }
        if (sourceVal == 1) { return decimal.One; }

        decimal worker = decimal.One;
        decimal adjust = 0.100m;
        decimal accuracy = 0.05m;

        bool keepGoing = true;

        while (keepGoing)
        {
            decimal testVal = worker.GetPower(exponent);
            decimal diff = sourceVal - testVal;
            if (diff < 0) { diff *= -1; }

            if (diff <= accuracy) { return worker; }

            if (testVal < sourceVal) { worker += adjust; }
            else if (testVal > sourceVal)
            {
                worker -= adjust;
                adjust /= 10.0000m;
                worker += adjust;
            }
        }

        return null;
    }

    /// <summary>
    /// Only handles exponents zero and above.  Returns null if you 
    /// pass a negative exponent.
    /// </summary>
    /// <param name="sourceVal"></param>
    /// <param name="exponent"></param>
    /// <returns></returns>
    public static int? Power(this int sourceVal, int exponent)
    {
        if (exponent < 0) { return null; }
        int worker = 1;

        for (int m = 1; m <= exponent; m++)
        {
            worker *= sourceVal;
        }
        return worker;
    }

    /// <summary>
    /// Only handles exponents zero and above.  Returns null if you 
    /// pass a negative exponent.
    /// </summary>
    /// <param name="sourceVal"></param>
    /// <param name="exponent"></param>
    /// <returns></returns>
    public static decimal? Power(this decimal sourceVal, int exponent)
    {
        if (exponent < 0) { return null; }
        decimal worker = 1;

        for (int m = 1; m <= exponent; m++)
        {
            worker *= sourceVal;
        }
        return worker;
    }

    public static bool TryPower(this decimal sourceVal, int exponent, [NotNullWhen(true)] out decimal? result)
    {
        result = null;
        if (exponent < 0) { return false; }

        decimal worker = decimal.One;
        for (int m = 1; m <= exponent; m++)
        {
            worker *= sourceVal;
        }
        result = worker;
        return true;
    }

    public static decimal GetPower(this decimal sourceVal, int exponent)
    {
        if (exponent < 0) { throw new ArgumentException("Exponent cannot be negative for this method"); }

        decimal worker = decimal.One;
        for (int m = 1; m <= exponent; m++)
        {
            worker *= sourceVal;
        }
        return worker;
    }

    /// <summary>
    /// Makes a judgement call about how close to zero the fractional part 
    /// of the decimal is.  If it's close enough, it essentially rounds the 
    /// decimal to the nearest whole number.  If the fractional part gets 
    /// closer to 0.5, the method will return false.
    /// </summary>
    public static bool DecimalEqualsInt(decimal decVal, int intVal)
    {
        decimal fPart = decVal % decimal.One;
        decimal worker = decVal - fPart;

        // the number is too far to be rounded
        if ((fPart > 0.05m && fPart < 0.95m) || (fPart < -0.05m && fPart > -0.95m)) { return false; }

        if (fPart >= 0.95m)
        {
            // in this case we want to round up the positive number...
            worker += decimal.One;
        }
        else if (fPart <= -0.95m)
        {
            // in this case we want to round doen the negative number
            worker -= decimal.One;
        }

        int iWorker = Convert.ToInt32(worker);
        return intVal == iWorker;

    }


}
