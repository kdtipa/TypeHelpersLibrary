using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomTypes;

/// <summary>
/// This class is meant for when the randomness is more important than how fast it goes.  
/// The built in Random class is used by this class, but varying thread sleeps are used 
/// to improve the randomness of the results.  This has been tested to show a better 
/// match to expected bell curve results for combinations of random numbers.  The sleep 
/// times range from 1 to 53 milliseconds.  If doing many random number generations, this 
/// really does give better results, but the time does add up too.
/// </summary>
public class SlowRandom
{
    public SlowRandom() { }

    public SlowRandom(int initialSleepMilliseconds)
    {
        if (initialSleepMilliseconds == 0 || initialSleepMilliseconds == int.MinValue) { _sleepTime = 1; }
        else if (initialSleepMilliseconds > 0) { _sleepTime = (initialSleepMilliseconds % 53) + 1; }
        else { _sleepTime = ((initialSleepMilliseconds * -1) % 53) + 1; }
    }

    private Random _rootRNG = new Random();
    private int _sleepTime = 11;

    /// <summary>
    /// This is a slightly more modern version of the Next methods that affords you more 
    /// flexibility with the parameters.  Without parameter values, it does what Next() 
    /// does which gives you a positive integer from 0 to int.MaxValue - 1.  But when 
    /// passing parameter values, it allows you to set the range as you want more 
    /// intuitively instead of shifting position of the parameters.
    /// </summary>
    /// <param name="minimumResult">defaults to 0, and zero can be included in the result</param>
    /// <param name="maximumLimit">defaults to int.MaxValue, and the highest result is one lower</param>
    /// <returns>a random positive integer within the range of 0 to (int.MaxValue - 1), or the range you set</returns>
    /// <exception cref="ArgumentException">If you pass negative parameters, it will throw an exception.</exception>
    public int GetNext(int? minimumResult = null, int? maximumLimit = null)
    {
        // get our non-nullable range values
        int minR = minimumResult ?? 0;
        int maxL = maximumLimit ?? int.MaxValue;

        // swap the min and max if the user messed up
        if (minR > maxL) { (minR, maxL) = (maxL, minR); }

        // now we need to make sure the user didn't pass negative values
        if (minR < 0 || maxL < minR) { throw new ArgumentException($"bad range values given for SlowRandom...\n- min: {minR}\n- max: {maxL}"); }

        Thread.Sleep(_sleepTime);
        int result = _rootRNG.Next(minR, maxL);
        _sleepTime = ((_sleepTime + (result % 97)) % 53) + 1;
        return result;
    }

    /// <summary>
    /// Behaves the same as the standard Random.Next(), but with our thread sleep randomness help
    /// </summary>
    /// <returns>a random positive integer from 0 to (int.MaxValue - 1)</returns>
    public int Next() { return GetNext(); }

    /// <summary>
    /// Behaves the same as the standard Random.Next(int maxValue), but with our thread sleep randomness help
    /// </summary>
    /// <param name="maxValue">This is the non-inclusive limit... for some reason they thought this was more intuitive.</param>
    /// <returns>a random positive integer from 0 to (maxValue - 1)</returns>
    public int Next(int maxValue) { return GetNext(0, maxValue); }

    /// <summary>
    /// Behaves the same as the standard Random.Next(int minValue, int maxValue), but with our thread sleep randomness help
    /// </summary>
    /// <param name="minValue">The minimum value that can be a result of this method</param>
    /// <param name="maxValue">This is the non-inclusive limit... for some reason they thought this was more intuitive.</param>
    /// <returns>a random positive integer from minValue to (maxValue - 1)</returns>
    public int Next(int minValue, int maxValue) { return GetNext(minValue, maxValue); }

    /// <summary>
    /// This is a way to get an enumeration of random integers.  You can specify the count, 
    /// but if you want to just keep going until the count is equal to int.MaxValue, you 
    /// can do that and let the calling code decide when to stop.  Because of the sleep time, 
    /// every 40 or so results would require about 1 second and 2400 would take about one 
    /// minute.  Don't use this method for anything where speed is important, but it can 
    /// give you excellent random numbers.
    /// </summary>
    /// <param name="countOfResults">
    /// Defaults to int.MaxValue with the expectation that the calling code will end the 
    /// enumeration when it has what it needs.  In general though, you should pass the 
    /// number of results you want.
    /// </param>
    /// <param name="minimumResult">
    /// The lowest value that can be returned as a result.  The range is 0 to (int.MaxValue - 2) 
    /// and we have to subtract 2 because the max parameter indicates the excluded upper limit of 
    /// the values that can be returned.  The max limit is int.MaxValue (so the max result is 
    /// int.MaxValue - 1), and we need at least 2 values the method can chose between for random 
    /// results.
    /// </param>
    /// <param name="maximumLimit">
    /// The excluded upper limit that is used for the random number.  The range is 2 to int.MaxValue.  
    /// Remember that if you want to flip a coin a bunch of times, so you want the values 1 or 2, you 
    /// pass in 1 as the minimumResult and 3 as the maximumLimit, but 3 is not included in the possible 
    /// results.  The built-in Random class does it this way to my great annoyance.
    /// </param>
    /// <returns>A collection of random numbers within the default or specified range.</returns>
    public IEnumerable<int> GetNextList(int? countOfResults = null, int? minimumResult = null, int? maximumLimit = null)
    {
        // need a positive number here
        if (countOfResults <= 0) { yield break; }

        // if you really want to go on forever, pass in null and you'll get a constant stream of random numbers
        int cor = countOfResults ?? int.MaxValue;

        // get our non-nullable range values
        int minR = minimumResult ?? 0;
        int maxL = maximumLimit ?? int.MaxValue;

        // another break condition.  Make sure we have at least 2 values that can be randomly chosen.
        if (minR >= maxL - 1) { yield break; }

        // swap the min and max if the user messed up
        if (minR > maxL) { (minR, maxL) = (maxL, minR); }

        // now we need to make sure the user didn't pass negative values
        if (minR < 0) { yield break; }

        // now loop and deliver results...
        for (int n = 1; n <= cor; n++)
        {
            Thread.Sleep(_sleepTime);
            int result = _rootRNG.Next(minR, maxL);
            _sleepTime = ((_sleepTime + (result % 97)) % 53) + 1;
            yield return result;
        }

    }



    public long GetNextLong(long? minimumResult = null, long? maximumLimit = null)
    {
        // get our non-nullable range values
        long minR = minimumResult ?? 0;
        long maxL = maximumLimit ?? long.MaxValue;

        // swap the min and max if the user messed up
        if (minR > maxL) { (minR, maxL) = (maxL, minR); }

        // now we need to make sure the user didn't pass negative values
        if (minR < 0 || maxL < minR) { throw new ArgumentException($"bad range values given for SlowRandom...\n- min: {minR}\n- max: {maxL}"); }

        Thread.Sleep(_sleepTime);
        long result = _rootRNG.NextInt64(minR, maxL);
        _sleepTime = ((_sleepTime + (int)(result % 97)) % 53) + 1;
        return result;
    }

    /// <summary>
    /// Gets you a random decimal from 0.0000 to 1.0000 (0.00% to 1.00%).  If you 
    /// provide parameter values, there must be at least 2 possible values for it 
    /// to choose between randomly, so they cannot be the same value.  Please note 
    /// that this method is different from the integer ones that do not include the 
    /// maximum parameter.  This one let's you define the range inclusively.
    /// </summary>
    /// <param name="minimumResult">
    /// Defaults to zero and it ranges from 0.0000 to 0.9999 (to leave room for 
    /// the maximum to be 1.0000).
    /// </param>
    /// <param name="maximumResult">
    /// Defaults to one and it ranges from 0.0001 to 1.0000 (to leave room for 
    /// the minimum to be 0.0000).
    /// </param>
    /// <returns>A random decimal that is effectively a percentage.</returns>
    public decimal GetNextPercent(decimal? minimumResult = null, decimal? maximumResult = null)
    {
        // get the min and max within range
        decimal minR = LimitValue(minimumResult ?? 0.0000m, 0.0000m, 0.9999m);
        decimal maxR = LimitValue(maximumResult ?? 1.0000m, 0.0001m, 1.0000m);

        // if the user screwed up and gave us == values, throw an exception
        if (minR == maxR) { throw new Exception("Cannot provide a random result when only one result is allowed."); }

        // if the user screwed up and swapped the values, we'll be nice and help them
        if (minR > maxR) { (minR, maxR) = (maxR, minR); }

        int intMin = (int)(10000.0000m * minR);
        int intMax = (int)(10000.0000m * maxR);

        int intResult = GetNext(intMin, intMax + 1); // +1 to include the max as a possible result

        return 0.0001m * intResult;
    }

    private decimal LimitValue(decimal sourceVal, decimal minAllowed, decimal maxAllowed)
    {
        if (sourceVal < minAllowed) { return minAllowed; }
        else if (sourceVal > maxAllowed) { return maxAllowed; }
        return sourceVal;
    }


    public static IEnumerable<T> RandomizeCollection<T>(IEnumerable<T> sourceCollection)
    {
        // have to use a list to store the items in the random order...
        List<T> randomList = new();

        // and of course we need out random number generator...
        SlowRandom sr = new();

        foreach (T srcItem in sourceCollection)
        {
            // get the count for a simple check
            int listCount = randomList.Count;

            // if we don't have any items yet, we don't need to go through the rest of the code...
            if (listCount == 0)
            {
                randomList.Add(srcItem);
                continue;
            }

            // if we have more than zero, we need an index among the existing possibilities
            int insertIndex = sr.GetNext(0, randomList.Count + 1);

            // put it in the list...
            randomList.Insert(insertIndex, srcItem);
        }

        // now that we have a randomized list, start returning the results...
        foreach (T item in randomList)
        {
            yield return item;
        }
    }

    public static IEnumerable<T> CreateRandomCollection<T>(int targetLength, params T[] possibleValues)
    {
        int possibleValCount = possibleValues.Length;
        if (targetLength < 1 || possibleValCount == 0) { yield break; }

        SlowRandom sr = new();
        
        for (int c = 1; c <= targetLength; c++)
        {
            int valIndex = sr.GetNext(0, possibleValCount);
            yield return possibleValues[valIndex];
        }
    }

    /// <summary>
    /// Gives a random result as if rolling dice.  It very intentionally calls for a random number separately 
    /// for each virtual die because of the bell curve of results.  For example, if you roll 3 six-sided dice, 
    /// the possible results are 3 to 18, but the likelihood of getting a 10 is much higher than the chance of 
    /// getting a 3 because only one combination of the dice yields a 3, while many combinations yield a 10.
    /// </summary>
    /// <param name="numberOfDice">
    /// How many dice do you want to roll?  Current range of options is 1 to 100.  Be careful with high numbers 
    /// for this parameter.  Because we're using the SlowRandom class, there's added time in the retrieval of 
    /// the random numbers.  Each die is done separately, so at the maximum of 100 dice, there's a probability 
    /// that it will take around 3 seconds to run.
    /// </param>
    /// <param name="sizeOfDice">
    /// How big are the dice?  The standard die is six-sided, so you'd pass a 6, but there are game dice with 
    /// sizes, 4, 8, 10, 12, and 20 for example.  However, this method does not limit you to those.  The method 
    /// allows for virtual size of dice from 2 to 100
    /// </param>
    /// <returns>Returns the sum of values rolled by all the dice.</returns>
    /// <exception cref="ArgumentException">
    /// If you pass a number or size of dice outside their ranges, you'll get an exception.
    /// </exception>
    public static int RollDiceSum(int numberOfDice, int sizeOfDice)
    {
        if (numberOfDice < 1) { throw new ArgumentException("number of dice must be at least 1."); }
        if (numberOfDice > 100) { throw new ArgumentException("this method only handles up to 100 dice at a time."); }
        if (sizeOfDice < 2) { throw new ArgumentException("to get a random value, we must have at least 2 possible values, so size of dice must be at least 2."); }
        if (sizeOfDice > 100) { throw new ArgumentException("biggest die this method handles is 100 sides."); }

        SlowRandom sr = new();
        int result = 0;
        for (int n = 1; n <= numberOfDice; n++)
        {
            result += sr.GetNext(1, sizeOfDice + 1);
        }

        return result;
    }

    /// <summary>
    /// Like RollDiceSum, this rolls a number of dice of chosen size, but instead of giving you the total, 
    /// it returns each result as a collection.
    /// </summary>
    /// <param name="numberOfDice">
    /// How many dice do you want to roll?  Current range of options is 1 to 100.  Be careful with high numbers 
    /// for this parameter.  Because we're using the SlowRandom class, there's added time in the retrieval of 
    /// the random numbers.  Each die is done separately, so at the maximum of 100 dice, there's a probability 
    /// that it will take around 3 seconds to run.
    /// </param>
    /// <param name="sizeOfDice">
    /// How big are the dice?  The standard die is six-sided, so you'd pass a 6, but there are game dice with 
    /// sizes, 4, 8, 10, 12, and 20 for example.  However, this method does not limit you to those.  The method 
    /// allows for virtual size of dice from 2 to 100
    /// </param>
    /// <returns>Returns a collection of the individual die results</returns>
    /// <exception cref="ArgumentException">
    /// If you pass a number or size of dice outside their ranges, you'll get an exception.
    /// </exception>
    public static IEnumerable<int> RollDiceResults(int numberOfDice, int sizeOfDice)
    {
        if (numberOfDice < 1) { throw new ArgumentException("number of dice must be at least 1."); }
        if (numberOfDice > 100) { throw new ArgumentException("this method only handles up to 100 dice at a time."); }
        if (sizeOfDice < 2) { throw new ArgumentException("to get a random value, we must have at least 2 possible values, so size of dice must be at least 2."); }
        if (sizeOfDice > 100) { throw new ArgumentException("biggest die this method handles is 100 sides."); }

        SlowRandom sr = new();
        for (int n = 1; n <= numberOfDice; n++)
        {
            yield return sr.GetNext(1, sizeOfDice + 1);
        }
    }

}
