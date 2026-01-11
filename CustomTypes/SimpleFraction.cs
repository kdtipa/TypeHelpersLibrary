using CustomTypes.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CustomTypes;

public struct SimpleFraction : IEquatable<SimpleFraction>, IComparable<SimpleFraction>
{
    public SimpleFraction() { }

    public SimpleFraction(int numerator, int denominator, bool? autoReduceFraction = null)
    {
        Numerator = numerator;
        Denominator = denominator;
        
        if (autoReduceFraction is not null && autoReduceFraction.Value == true)
        {
            Reduce();
        }
    }

    public SimpleFraction(decimal desiredValue)
    {
        SetByDecimal(desiredValue, 4);
    }

    public void Reduce()
    {
        int workN = Numerator;
        int absN = workN.Absolute();
        int workD = Denominator;
        int absD = workD.Absolute();

        int maxCheck = absN < absD ? workN.MaxFactor() : workD.MaxFactor();

        foreach (var val in IntHelper.GetPrimeNumbers(2, maxCheck))
        {
            if (val > workN || val > workD) { break; } // done because the original maxCheck limit may be higher than one of the reduced values

            while (workN % val == 0 && workD % val == 0)
            {
                workN /= val;
                workD /= val;
            }
        }

        Numerator = workN * (Numerator < 0 ? -1 : 1);
        Denominator = workD * (Denominator < 0 ? -1 : 1);

        // put the negative in the numerator if possible, or cancel out double negative
        if (Denominator < 0) { Numerator *= -1; Denominator *= -1; }

    }



    public int Numerator { get; set; } = 0;

    public int Denominator { get; set; } = 1;

    public decimal? Value
    {
        get
        {
            if (Denominator == 0) { return null; }

            if (Numerator == 0) { return 0.00000m; }

            if (Denominator == 1) { return Numerator; }

            if (Numerator == Denominator) { return 1; }

            return (decimal)Numerator / Denominator;
        }
    }

    public bool HasValue { get { return Denominator != 0; } }

    /// <summary>
    /// Does the math to find the closest approximation for the 
    /// decimal value you provided.  If you pass in 0.753, you 
    /// might get back the fraction 3/4 depending on how accurate 
    /// you demand.
    /// </summary>
    /// <param name="value">the value you want to approximate as a fraction.</param>
    /// <param name="accuracyToThisManyDigits">
    /// defaults to 4 places, but could be from 0 to 12.  Zero 
    /// would be kind of pointless though, since you'll end up 
    /// with a fraction for zero or one for any decimal between 
    /// zero and one.
    /// </param>
    public void SetByDecimal(decimal value, int? accuracyToThisManyDigits = null)
    {
        if (value == 0.00000m)
        {
            Numerator = 0;
            Denominator = 1;
            return;
        }

        int accDigits = accuracyToThisManyDigits ?? 4;
        if (accDigits < 1) { accDigits = 1; }
        decimal mult = decimal.One;
        for (int ad = 1; ad <= accDigits; ad++) { mult /= 10; }

        bool isNegative = value < 0.00000m;
        int N = isNegative ? -1 : 1;
        int D = 1;
        decimal testVal = (decimal)N / D;

        while (!value.ApproxEquals(testVal, mult))
        {
            if (isNegative)
            {
                if (testVal < value) { D++; }
                else if (testVal > value) { N--; } // have to subtract to keep incrementing the negative numerator
            }
            else
            {
                if (testVal > value) { D++; }
                else if (testVal < value) { N++; }
            }

            testVal = (decimal)N / D;
        }
    }

    public int CompareTo(SimpleFraction other)
    {
        // use these variables so we don't have to do the calculations inside the property repeatedly
        var thisVal = Value;
        var otherVal = other.Value;

        if (thisVal is null && otherVal is null) { return 0; } // both are null, so they're technically equal

        if (thisVal is null) { return -1; }

        if (otherVal is null) { return 1; }

        // now we know they're not null

        return thisVal.Value.CompareTo(otherVal.Value);
    }

    public bool Equals(SimpleFraction other)
    {
        return Value.Equals(other.Value);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is SimpleFraction sf) { return Equals(sf); }

        if (obj is decimal dObj) { return Value is not null && Value == dObj; }

        return false;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return $"{Numerator}/{Denominator}";
    }

    public bool TrySet(string sourceString)
    {
        var match = FractionPattern.Match(sourceString);
        if (match.Success 
         && int.TryParse(match.Groups[1].Value.RemoveLeadingZeroes(), out int nval) 
         && int.TryParse(match.Groups[2].Value.RemoveLeadingZeroes(), out int dval))
        {
            Numerator = nval;
            Denominator = dval;
            return true;
        }

        return false;
    }

    public static bool TryParse(string sourceString, out SimpleFraction? parseResult)
    {
        parseResult = null;

        SimpleFraction testVal = new();
        if (testVal.TrySet(sourceString))
        {
            parseResult = testVal;
            return true;
        }

        return false;
    }

    public static Regex FractionPattern { get; } = new Regex("/(\\-?[0-9]+)[\\/\\\\]+([0-9]+)/gm", RegexOptions.Compiled);

    public static int LowestCommonDenominator(params SimpleFraction[] fractions)
    {
        if (fractions.Length == 0) { return 0; }
        if (fractions.Length == 1) { return fractions[0].Denominator; }

        // we need the numerators because while ?/4 and ?/16 have 4 as a lowest common denomenator, if it's 7/16,
        // the smallest value you can use is 16 because 7 doesn't reduce.  In that case the LCD is 16.
        List<int> numerators = new();
        List<int> denominators = new();

        long bigDenominator = 1;
        List<int> usedDenominators = new();

        foreach (var fraction in fractions)
        {
            if (fraction.Denominator >= 0)
            {
                numerators.Add(fraction.Numerator);
                denominators.Add(fraction.Denominator);
                if (!usedDenominators.Contains(fraction.Denominator))
                {
                    bigDenominator *= fraction.Denominator;
                    usedDenominators.Add(fraction.Denominator);
                }
            }
            else
            {
                // need all denominators to be positive
                numerators.Add(fraction.Numerator * -1);
                int absDenominator = fraction.Denominator * -1;
                denominators.Add(absDenominator);
                if (!usedDenominators.Contains(absDenominator))
                {
                    bigDenominator *= absDenominator;
                    usedDenominators.Add(absDenominator);
                }
            }
        }

        // now we have lists of the numerators and denominators, and the biggest common denominator,
        // so we should be able to go through the numerators and figure out their values compared to that big denominator
        List<long> bigNumerators = new();
        int count = numerators.Count;
        for (int i = 0; i < count; i++)
        {
            long multiplier = bigDenominator / denominators[i];
            bigNumerators.Add(numerators[i] * multiplier);
        }

        // start reducing biggest fractions by 2
        while (bigNumerators.All(n => n % 2 == 0) && bigDenominator % 2 == 0)
        {
            for (int i = 0; i < count; i++) { bigNumerators[i] /= 2; }
            bigDenominator /= 2;
        }

        // now have removed as many factors of 2 as possible.  next we need to start trying to divide by odd numbers below the square root of the lowest value
        long maxDiv = bigNumerators.Min().ClosestSquareRoot(true);
        long denomDiv = bigDenominator.ClosestSquareRoot(true);
        if (denomDiv < maxDiv) { maxDiv = denomDiv; }

        for (int div = 3; div <= maxDiv; div += 2)
        {
            while (bigNumerators.All(n => n % div == 0) && bigDenominator % div == 0)
            {
                for (int i = 0; i < count; i++) { bigNumerators[i] /= div; }
                bigDenominator /= div;
            }
        }

        // we got here.  We have the answer
        if (bigDenominator > int.MaxValue) { throw new ArgumentOutOfRangeException("denominator", $"The fractions provided produce a denominator above integer max value: {bigDenominator:#,###}"); }


        return Convert.ToInt32(bigDenominator);
    }


    public static SimpleFraction Add(SimpleFraction left, SimpleFraction right)
    {
        left.Reduce();
        right.Reduce();

        int lcd = LowestCommonDenominator(left, right);
        int lMult = lcd / left.Denominator;
        int rMult = lcd / right.Denominator;

        int lN = left.Numerator * lMult;
        int rN = right.Numerator * rMult;

        return new SimpleFraction(lN + rN, lcd, true);
    }

    public static SimpleFraction Subtract(SimpleFraction left, SimpleFraction right)
    {
        left.Reduce();
        right.Reduce();

        int lcd = LowestCommonDenominator(left, right);
        int lMult = lcd / left.Denominator;
        int rMult = lcd / right.Denominator;

        int lN = left.Numerator * lMult;
        int rN = right.Numerator * rMult;

        return new SimpleFraction(lN - rN, lcd, true);
    }

    public static SimpleFraction Multiply(SimpleFraction left, SimpleFraction right)
    {
        left.Reduce();
        right.Reduce();

        return new SimpleFraction(left.Numerator * right.Numerator, left.Denominator * right.Denominator, true);
    }

    public static SimpleFraction Divide(SimpleFraction left, SimpleFraction right)
    {
        left.Reduce();
        right.Reduce();

        return new SimpleFraction(left.Numerator * right.Denominator, left.Denominator * right.Numerator, true);
    }


    public static SimpleFraction operator +(SimpleFraction left, SimpleFraction right) { return Add(left, right); }
    public static SimpleFraction operator -(SimpleFraction left, SimpleFraction right) { return Subtract(left, right); }
    public static SimpleFraction operator *(SimpleFraction left, SimpleFraction right) { return Multiply(left, right); }
    public static SimpleFraction operator /(SimpleFraction left, SimpleFraction right) { return Divide(left, right); }


    public static bool operator ==(SimpleFraction left, SimpleFraction right) { return left.Equals(right); }
    public static bool operator !=(SimpleFraction left, SimpleFraction right) { return !left.Equals(right); }

    public static bool operator >(SimpleFraction left, SimpleFraction right) { return left.HasValue && right.HasValue && left.Value > right.Value; }
    public static bool operator <(SimpleFraction left, SimpleFraction right) { return left.HasValue && right.HasValue && left.Value < right.Value; }

    public static bool operator >=(SimpleFraction left, SimpleFraction right) { return left.HasValue && right.HasValue && left.Value >= right.Value; }
    public static bool operator <=(SimpleFraction left, SimpleFraction right) { return left.HasValue && right.HasValue && left.Value <= right.Value; }

}
