using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTypes.EquationTypes;

public static class MathHelper
{
    public static long getWholePart(decimal sourceValue)
    {
        return Convert.ToInt64(sourceValue - (sourceValue % 1.00000m));
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
}
