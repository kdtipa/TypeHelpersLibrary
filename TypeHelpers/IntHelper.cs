using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TypeHelpers;

public static class IntHelper
{
    /// <summary>
    /// Using ideal performance algorithm, calculates whether the given 
    /// integer is a prime number or not.  Keep in mind that negative 
    /// numbers cannot be prime.  A prime is a natural number greater 
    /// than 1 that is not a product of two smaller numbers.  For example, 
    /// 5 is a prime number (only factors are 1 and 5) and 6 is NOT a 
    /// prime number (factors include 1, 2, 3, and 6).
    /// </summary>
    public static bool IsPrime(this int value)
    {
        // get the early tests out of the way
        if (value <= 1) { return false; }
        if (value == 2) { return true; }
        if (value % 2 == 0) { return false; }

        // now on to the loop that just does odd numbers.  
        for (var x = 3; x <= value / x; x += 2)
        {
            if (value % x == 0) { return false; }
        }

        return true;
    }

    /// <summary>
    /// A square number is one whose factors include a number that can 
    /// be multiplied by itself to get the desired number.  So, 9 is a 
    /// square number because 3 times 3 is 9.  10 is NOT a square number 
    /// because there is no factor that multiplied by itself will give 
    /// you 10.
    /// </summary>
    public static bool IsSquare(this int value)
    {
        // early tests
        if (value < 0) { return false; } // must be positive
        if (value <= 1) { return true; } // 0 and 1 count

        var rootVal = 2;
        while (rootVal * rootVal <= value)
        {
            if (rootVal * rootVal == value) { return true; }
            rootVal++;
        }

        return false;
    }

    /// <summary>
    /// returns null if the square root isn't possible like for negative numbers, or if 
    /// the number doesn't have an integer square root.  Returns the root if there is 
    /// an integer root.
    /// </summary>
    public static int? SquareRoot(this int value)
    {
        if (value < 0) { return null; }
        if (value <= 1) { return value; }

        var rootVal = 2;
        while (value / rootVal >= rootVal)
        {
            if (rootVal * rootVal == value) { return rootVal; }
            rootVal++;
        }

        return null;
    }

    /// <summary>
    /// A more open method than the SquareRoot method presented here that gives you a 
    /// decimal result, meaning you'll get a result whether the answer is a whole 
    /// number or not.  Keep in mind that negative numbers can't have roots for even 
    /// powers, so if you pass an even rootBase for a negative starting value, you'll 
    /// get null.
    /// </summary>
    /// <param name="rootBase">
    /// The power you'd have to raise the return value to, to get your integer value.  
    /// It defaults to 2, so if you pass 2 or leave it null, you'll get the square 
    /// root of the number.  Also of note is that this method is not set up to handle 
    /// negative exponents.  Possible there will be a method set up for that later.  
    /// For now, this method will just return null for a negative rootBase.
    /// </param>
    /// <returns></returns>
    public static decimal? Root(this int value, int? rootBase = null)
    {
        var rb = 2; // default to square root
        if (rootBase is not null) { rb = rootBase.Value; } // if not null, use what the user asks for
        if (rb < 0) { return null; } // if the root value they're looking for is negative, we're not set up to handle that yet.

        if (value == 0) { return 0.00000m; } // The number that when multiplied by itself any number of times equals zero, is zero.
        if (value == 1) { return 1.00000m; } // The number that when multiplied by itself any number of times equals one, is one.
        if (value == -1 && rb % 2 != 0) { return -1.00000m; }

        if (value < 0 && rb % 2 == 0) { return null; } // a negative value can't be reached by multiplying any number an even number of times. -2 * -2 = 4

        // set up our allowed variance...
        var variance = 0.0001m;

        // set up our target value...
        decimal targetVal = Convert.ToDecimal(value);

        // at this point we know the integer value we're trying to find a root for is above 1, or is negative and the root base is an odd number.
        // let's start with the positive numbers.
        if (value > 0)
        {
            decimal calcRootVal = 1.00000m; // the variable that will have our answer
            decimal calcStepVal = 1.00000m; // the variable keep track of how much we adjust the calcRootVal each time through the loop
            decimal testVal = decimalPower(calcRootVal, rb);

            // this while loop will continue until the calcRoot results in something close enough to the start value
            while (testVal < targetVal - variance || testVal > targetVal + variance)
            {
                if (testVal > targetVal)
                {
                    // in this case our calculated root value is too high, so we back it down by the step; reduce the step; and bump it up from there
                    calcRootVal -= calcStepVal;
                    calcStepVal = calcStepVal / 10.0m;
                    calcRootVal += calcStepVal;
                }
                else
                {
                    // in this case out calculated root value is too low, so we just increase by the step.
                    calcRootVal += calcStepVal;
                }

                testVal = decimalPower(calcRootVal, rb);
            }

            // after the loop is complete, we have a base that when raised to the given power will give use at least very close to the original integer
            return calcRootVal;
        }
        else
        {
            // and here is our code block for that negative number with an odd exponent.
            // it should be similar to the positive number loop except we'll be going down through the negative numbers looking for the answer
            decimal calcRootVal = -1.00000m; // the variable that will have our answer
            decimal calcStepVal = 1.00000m; // the variable keep track of how much we adjust the calcRootVal each time through the loop
            decimal testVal = decimalPower(calcRootVal, rb);

            // this while loop will continue until the calcRoot results in something close enough to the start value
            while (testVal < targetVal - variance || testVal > targetVal + variance)
            {
                if (testVal > targetVal)
                {
                    // in this case our calculated root value is too low, so we back it up by the step; reduce the step; and bump it back from there
                    calcRootVal += calcStepVal;
                    calcStepVal = calcStepVal / 10.0m;
                    calcRootVal -= calcStepVal;
                }
                else
                {
                    // in this case out calculated root value is too high, so we just decrease by the step.
                    calcRootVal -= calcStepVal;
                }

                testVal = decimalPower(calcRootVal, rb);
            }

            // after the loop is complete, we have a base that when raised to the given power will give use at least very close to the original integer
            return calcRootVal;
        }

    }

    /// <summary>
    /// a private helper method to get a decimal power value used within the Root method, 
    /// and possibly others.  It doesn't check for wonky values in the params and assumes 
    /// you will only pass powers that are 0 or higher.
    /// </summary>
    private static decimal decimalPower(decimal baseVal, int power)
    {
        decimal calcVal = 1.00000m;

        for (int c = 1; c <= power; c++)
        {
            calcVal *= baseVal;
        }

        return calcVal;
    }


    /// <summary>
    /// A convenience method that does the modulus operation for you.
    /// </summary>
    public static bool IsEven(this int value)
    {
        return value % 2 == 0;
    }

    /// <summary>
    /// A convenience method that does the modulus operation for you.
    /// </summary>
    public static bool IsOdd(this int value)
    {
        return value % 2 != 0;
    }

    /// <summary>
    /// Uses the modulus operation to tell you if this integer has the 
    /// factor parameter as a factor.  So, if your integer is 24 and you 
    /// pass in 6, you'll get true.  But if you pass in 7, you'll get false.
    /// </summary>
    public static bool IsMultipleOf(this int value, int factor)
    {
        return value % factor == 0;
    }

    /// <summary>
    /// This method is essentially rounding the integer to the nearest multiple 
    /// of a given number.  If you want the nearest multiple of 10, you pass in 
    /// 10 as the targetFactor.  The method will find it and return that number.
    /// </summary>
    public static int? NearestMultipleOf(this int value, int targetFactor)
    {
        // handle some basics
        if (targetFactor == 0) { return null; }
        targetFactor = targetFactor.Absolute();
        if (targetFactor == 1 || value % targetFactor == 0) { return value; }


        // handle stuff too close to max or min of int
        if (int.MaxValue - targetFactor <= value || int.MinValue + targetFactor >= value) { return null; }


        // 10 - (16 % 10).Absolute() => 10 - 6 = 4 (distance from value to 20)
        // 10 - (-14 % 10).Absolute() => 10 - 4 = 6  (distance from value to -20)
        var distanceFromNextHigher = targetFactor - (value % targetFactor).Absolute();

        // (16 % 10).Absolute() = 6 (distance from value to 10)
        // (-14 % 10).Absolute() = 4 (distance from value to -10)
        var distanceFromNextLower = (value % targetFactor).Absolute();

        // since positive and negative numbers need to be handled differently, we'll use an if to separate them
        if (value > 0)
        {
            if (distanceFromNextHigher < distanceFromNextLower) { return value + distanceFromNextHigher; }
            else { return value - distanceFromNextLower; }
        }
        else
        {
            if (distanceFromNextHigher < distanceFromNextLower) { return value - distanceFromNextHigher; }
            else { return value + distanceFromNextLower; }
        }
        
    }

    public static IEnumerable<int> GetFactors(this int value)
    {
        // special case of zero
        if (value == 0)
        {
            yield return 0;
            yield break;
        }

        // create the list, add -1 if the number is negative, and add 1
        var factors = new List<int>();
        if (value < 0)
        {
            factors.Add(-1);
        }
        factors.Add(1);

        // now go through all the numbers up to half way
        var half = (value + (value % 2)) / 2; // (33 + (33 % 2)) / 2 => (33 + 1) / 2 => 34 / 2 = 17

        for (var fctr = 2; fctr <= half; fctr++)
        {
            if (value % fctr == 0) { factors.Add(fctr); }
        }

        factors.Add(value.Absolute());
        foreach (var factor in factors)
        {
            yield return factor;
        }
    }

    /// <summary>
    /// Gives you a division option that you can rely on to behave 
    /// the way you want it to.  Integer division doesn't have a 
    /// result that includes fractional parts.  This method allows 
    /// you to tell it how to round the result.
    /// </summary>
    /// <param name="value">
    /// The value within this variable which is the number being divided.
    /// </param>
    /// <param name="divisor">
    /// The number you want to divide your value by.  If you pass in a 
    /// value of zero (0), you'll get null as your result since you 
    /// cannot divide by zero.
    /// </param>
    /// <param name="roundHow"></param>
    /// <returns></returns>
    public static int? DivideBy(this int value, int divisor, IntRoundOption? roundHow = null)
    {
        if (divisor == 0) { return null; }

        if (value == 0) { return 0; }

        
        IntRoundOption roundOpt = roundHow is null ? IntRoundOption.PointFiveAwayFromZero : roundHow.Value;

        decimal betterResult = (decimal)value / divisor; // the complete correct answer
        bool isNegative = betterResult < 0.00000m; // do we need to account for it being negative when we round?
        decimal absMultiplier = isNegative ? -1.00000m : 1.00000m; // the multiplier to use when calculations need a positive number
        decimal fractionalPart = betterResult % 1.00000m * absMultiplier; // the positive fractional part of the result
        decimal reciprocalPart = 1.00000m - fractionalPart; // useful for rounding away from zero
        // is the fractional part at the 0.5 level or higher?  Disregards negativity.  Important for rounding decisions...
        bool isPointFiveOrGreater = isNegative ? (fractionalPart * -1.00000m) >= 0.50000m : fractionalPart >= 0.50000m;
        int result = 0;

        switch (roundOpt)
        {
            case IntRoundOption.RoundTowardZero:
                // always move the result toward zero in the rounding
                result = (int)(((betterResult * absMultiplier) - fractionalPart) * absMultiplier);
                break;
            case IntRoundOption.RoundAwayFromZero:
                // always move away from zero in the rounding
                result = (int)(((betterResult * absMultiplier) + reciprocalPart) * absMultiplier);
                break;
            case IntRoundOption.RoundUp:
                // always move to a higher integer value in the rounding (5.25 => 6.00 and -4.6 => -4)
                result = isNegative ? (int)(betterResult + fractionalPart) : (int)(betterResult + reciprocalPart);
                break;
            case IntRoundOption.RoundDown:
                // always move to a lower integer value in the rounding (5.8 => 5 and -7.1 => -8)
                result = isNegative ? (int)(betterResult - reciprocalPart) : (int)(betterResult - fractionalPart);
                break;
            default:
                // the default is also the rounding option where 0.5 or more means round away from zero and less than 0.5 means round toward zero
                if (fractionalPart >= 0.50000m)
                {
                    result = (int)(((betterResult * absMultiplier) + reciprocalPart) * absMultiplier);
                }
                else
                {
                    result = (int)(((betterResult * absMultiplier) - fractionalPart) * absMultiplier);
                }
                break;

        }

        return result;
    }


    


    /// <summary>
    /// Rather than using the Math library, you can just use this.  It will 
    /// throw an exception if you use it on int minimum value because its 
    /// absolute value can't be represented by an int.
    /// </summary>
    public static int Absolute(this int value)
    {
        if (value == int.MinValue)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Minimum value for an int cannot have its absolute value represented as an int.");
        }

        if (value < 0)
        {
            return value * -1;
        }

        return value;
    }



    /// <summary>
    /// Currently incapable of handling negative exponents, 
    /// so it will return null if it can't find an answer.
    /// </summary>
    public static int? RaisedTo(this int value, int exponent)
    {
        // early possible situations...
        if (exponent < 0) { return null; }
        if (exponent == 0) { return 1; }
        if (exponent == 1) { return value; }
        if (value == 0) { return 0; }
        if (value == 1) { return 1; }

        // now we do the math...
        var result = 1;

        for (var i = 1; i <= exponent; i++)
        {
            result *= value;
        }

        return result;
    }

    /// <summary>
    /// If you have a number you want split into even groups without decimal parts, this method 
    /// can help you.  With a number like 15 split into 3 groups, you'd get a list with values 
    /// 5, 5, and 5.  That's easy.  But what about 18 split into 4 even groups?  This method 
    /// will give you back 4, 5, 5, and 4.  How would you use it?  In a console app if you're 
    /// trying to create a fancy horizontal line with the characters -, <, =, >, and -... you 
    /// can run this method on an integer variable with the width of the line you want (let's 
    /// say 64 characters wide) and enter the count of your list of characters (in this example 
    /// it's 5).  The method will return a list with 13, 13, 12, 13, and 13.  You can then build 
    /// your horizontal line by looping through 0 to 4 and using the character from that index 
    /// and the count from that index to then use something like PadRight to build your line.  
    /// And of course, having this method means being able to build the line at run time so you 
    /// can accomodate differing widths.
    /// </summary>
    /// <param name="howManyGroups">Should be a number smalled than the source number.</param>
    public static List<int>? SplitIntoEvenGroups(this int value, int howManyGroups)
    {
        // early saves
        if (value < 1 || value < howManyGroups || howManyGroups < 1) { return null; }
        
        var result = new List<int>();

        // another early save
        if (howManyGroups == 1)
        {
            // not sure why they asked for this, but... we can do this
            result.Add(value);
            return result;
        }

        // an easy one to do if the value is evenly divisible by the number of groups...
        if (value % howManyGroups == 0)
        {
            int numberPerGroup = value / howManyGroups;
            for (int g = 1; g <= howManyGroups; g++) { result.Add(numberPerGroup); }
            return result;
        }

        //and now we're in the situation where we won't have a nicely divisible amount
        bool hmgIsOdd = howManyGroups % 2 != 0; // 4 groups, 4 % 2 is 0, result is false
        int middleIndex = (howManyGroups - (howManyGroups % 2)) / 2; // (4 - (4 % 2)) / 2 = 2 || (5 - (5 % 2)) / 2 = 2
        int remainder = value % howManyGroups; // 15 % 4 = 3
        bool rIsOdd = remainder % 2 == 0; // 3 % 2 is 1, result is false
        int baseGroupValue = (value - remainder) / howManyGroups; // (15 - 3) / 4 = 3

        // fill in the return value with base values
        for (int g = 1; g <= howManyGroups; g++) { result.Add(baseGroupValue); }


        // if we have an odd number of groups and the remainder is odd, we can make the rest a little easier
        if (hmgIsOdd && rIsOdd)
        {
            result[middleIndex] += 1; // bump up the middle value
            remainder -= 1; // reduce the remainder so it's even now
        }

        // now we have an even remainder.  If we have an even number of groups, we would prefer 4,5,5,4 over 4,5,4,5; so we have to do something different
        int offsetUp = hmgIsOdd ? 1 : 0;
        int offsetDown = 1;
        while (remainder > 0)
        {
            result[middleIndex + offsetUp] += 1;
            result[middleIndex - offsetDown] += 1;

            offsetUp += 1;
            offsetDown += 1;
            remainder -= 2;
        }

        return result;
    }

    /// <summary>
    /// Tells you how many digits this number has.  Does not count 
    /// the negative sign as a digit if the number is negative.
    /// </summary>
    /// <returns>an integer 1 or higher</returns>
    public static int DigitCount(this int value)
    {
        if (value == 0) { return 1; }

        var result = 0;

        var worker = value;
        if (worker == int.MinValue) 
        {
            worker += 1; // so that when we get the absolute value, it won't throw an exception, but we'll still get the right number of digits
        }

        // get absolute value
        if (worker < 0) { worker *= -1; }

        // loop through and trim the value down until we have the number of digits
        while (worker > 0) 
        {
            worker = (worker - (worker % 10)) / 10; // change 456 to 45 to 4 | 9 to 0 | 101 to 10 to 1
            result++;
        }

        return result;
    }

    /// <summary>
    /// Gets you a collection of the digits in order from highest place to ones place.  
    /// Does NOT use strings to accomplish this, so should be pretty performant.  If 
    /// the number is negative, the returned ints will not indicate it.  It will just 
    /// be the list of positive value digits and you'll have to keep aware of negative 
    /// value separately.
    /// </summary>
    public static IEnumerable<int> Digits(this int value)
    {
        // if it's zero, the only digit is zero
        if (value == 0)
        {
            yield return 0;
            yield break;
        }

        // set up my calculation variables
        var digitList = new List<int>();
        var worker = value;
        var parsedDigit = 0;
        var mod = 10;

        // loop through figuring out the digits from ones to highest place
        while (worker != 0)
        {
            parsedDigit = worker % mod;
            worker = worker.MoveTowardZero(parsedDigit) / mod;
            digitList.Add(parsedDigit.Absolute());
        }

        // return the digits from highest place to ones place
        var digitCount = digitList.Count;
        for (var d = digitCount - 1; d >= 0; d--)
        {
            yield return digitList[d];
        }
    }

    /// <summary>
    /// Essentially an increment or decrement method that doesn't care if 
    /// the number is positive or negative and will move the value of the 
    /// integer closer to zero.  If run on zero, it will return zero.  If 
    /// the starting value would be moved past zero, it stops at zero.  An 
    /// example of needing this method could be a modulus loop where the 
    /// starting value could be negative and you end the loop at zero.
    /// </summary>
    /// <param name="step">
    /// An option parameter that defaults to 1.  If you provide a negative 
    /// value, it will use the absolute value (or just return 0 if you gave 
    /// the minimum value).  If you try to set the step to 0, it will just 
    /// use the default of 1.
    /// </param>
    /// <returns>And integer that is closer to zero by the step value.</returns>
    public static int MoveTowardZero(this int value, int? step = null)
    {
        int safeStep = 1;
        if (step is not null && step.Value != 0)
        {
            safeStep = step.Value;
        }

        if (safeStep == int.MinValue)
        {
            //in this case, any movement of this size will result in zero
            return 0;
        }

        if (safeStep < 0)
        {
            // now we don't have to worry about absolute value because min value is already accounted for
            safeStep *= -1;
        }

        // we have a valid safe step now, so we can start calculating the result
        var result = 0;
        if (value < 0)
        {
            result = value + safeStep;
            if (result > 0)
            {
                // this means we went past zero, so we need to stop at zero
                result = 0;
            }
        }
        else if (value > 0)
        {
            result = value - safeStep;
            if (result < 0)
            {
                // this means we went past zero, so we need to stop at zero
                result = 0;
            }
        }

        return result;
    }

    /// <summary>
    /// If the number value is 0 to 9, it will return the 
    /// character of that number '0' to '9'.  Returns null 
    /// otherwise.
    /// </summary>
    public static char? DigitChar(this int value)
    {
        switch (value)
        {
            case 0: return '0';
            case 1: return '1';
            case 2: return '2';
            case 3: return '3';
            case 4: return '4';
            case 5: return '5';
            case 6: return '6';
            case 7: return '7';
            case 8: return '8';
            case 9: return '9';
            default: return null;
        }
    }


    /// <summary>
    /// This returns ALL the Fibonacci sequence up to the highest one within int.MaxValue 
    /// unless you provide at least an end point.  The method will try to fix up your values 
    /// if you get them mixed up or the start is effectively negative.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<int> FibonacciSequence(int? startingPoint = null, int? endingPoint = null)
    {
        int retVal = 0;
        int nextVal = 1;
        int workVal;

        int startAt = startingPoint ?? 0;
        int endAt = endingPoint ?? int.MaxValue;

        // if the user is dumb, help them
        if (startAt > endAt)
        {
            (startAt, endAt) = (endAt, startAt);
        }

        // if the end doesn't make sense, just get out
        if (endAt < 0) { yield break; }

        // if the start is negative, just set it to zero
        if (startAt < 0) { startAt = 0; }


        // go until we reach the end point
        while (endAt - nextVal >= retVal)
        {
            // only return if it's greater than or equal to where the user wants to start
            if (retVal >= startAt)
            {
                yield return retVal;
            }

            workVal = retVal + nextVal; // this will be the new next value
            retVal = nextVal; // move the current next value into the return value
            nextVal = workVal; // store the new next value
        }
    }

    public static IEnumerable<int> BinarySequence(int? startingPoint = null, int? endingPoint = null)
    {
        int lowestValue = startingPoint ?? 1; // the lowest we can return
        if (lowestValue < 1) { lowestValue = 1; } // can't be negative

        int highestValue = endingPoint ?? int.MaxValue; // the highest value we can return
        if (highestValue < lowestValue) { highestValue = lowestValue; } // can't be lower than the start

        int endPower = (int)Math.Sqrt(highestValue);
        

        for (int x = 0; x <= endPower; x++)
        {
            int workVal = 2 ^ x;
            if (workVal >= lowestValue) { yield return workVal; }
        }
    }

    /// <summary>
    /// If the passed in object is null, a number type with value outside int range, a string that 
    /// doesn't parse to an int, or a type we aren't testing for, it passes back 0.  Otherwise, it 
    /// gives you the integer value that results from casting or TryParse.
    /// </summary>
    public static int GetInt(object? sourceValue)
    {
        if (sourceValue is null) { return 0; }

        if (sourceValue is int iVal) { return iVal; }

        if (sourceValue is long lVal && lVal <= int.MaxValue && lVal >= int.MinValue) { return (int)lVal; }

        if (sourceValue is short shVal) { return shVal; }

        if (sourceValue is ulong ulVal && ulVal <= int.MaxValue) { return (int)ulVal; }

        if (sourceValue is uint uiVal && uiVal <= int.MaxValue) { return (int)uiVal; }

        if (sourceValue is double dVal && dVal <= int.MaxValue && dVal >= int.MinValue) { return (int)dVal; }

        if (sourceValue is float fVal && fVal <= int.MaxValue && fVal >= int.MinValue) { return (int)fVal; }

        if (sourceValue is decimal decVal && decVal <= int.MaxValue && decVal >= int.MinValue) { return (int)decVal; }

        if (sourceValue is string sVal && int.TryParse(sVal, out int parsedIVal)) { return parsedIVal; }

        // the long shot...
        try
        {
            int lsVal = Convert.ToInt32(sourceValue);
            return lsVal;
        }
        catch { return 0; }

    }


    public static char[] IntegerDigitCharacters { get; } = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

    public static int? TranslateCharToIntValue(char digitValue) 
    { 
        if (IntegerDigitCharacters.Contains(digitValue)) { return digitValue - 48; }
        return null;
    }

    public static char? TranslateIntToChar(int digitValue)
    {
        if (digitValue >= 0 && digitValue < IntegerDigitCharacters.Length) { return IntegerDigitCharacters[digitValue]; }
        return null;
    }

    public static string RemovePaddedZeroes(string integerRepresentation)
    {
        if (string.IsNullOrWhiteSpace(integerRepresentation)) { return string.Empty; }

        string workString = integerRepresentation.Trim();

        bool isNegative = false;
        if (workString.StartsWith('-')) 
        { 
            isNegative = true;
            workString = workString.TrimStart(['-', ' ']);
        }

        var digitsOnly = new List<char>();
        foreach (char c in workString)
        {
            if (!IntegerDigitCharacters.Contains(c)) { return integerRepresentation; } // the case where they didn't pass an integer

            digitsOnly.Add(c);
        }

        if (digitsOnly.Count == 0 || digitsOnly.All(d => d == '0'))
        {
            return "0"; // if they are all zero like "00000000000000", we just need to return zero.
        }

        var cleanString = new StringBuilder();
        bool foundNonZero = false;

        foreach (char d in digitsOnly)
        {
            if (d != '0') { foundNonZero = true; }
            if (foundNonZero) { cleanString.Append(d); }
        }

        if (isNegative) { cleanString.Insert(0, '-'); }

        return cleanString.ToString();
        
    }

}

/// <summary>
/// Used for our integer division method that lets you 
/// pick which way to round the result for division that 
/// does not divide evenly.
/// </summary>
public enum IntRoundOption
{
    PointFiveAwayFromZero = 0, 
    RoundTowardZero = 1, 
    RoundAwayFromZero = 2, 
    RoundDown = 3, 
    RoundUp = 4
}
