using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TypeHelpers;


public static class ListHelper
{
    /// <summary>
    /// Checks the values contained in the list to see if the lists are the same by value 
    /// instead of by reference (the default comparison method)
    /// </summary>
    /// <param name="sameOrder">
    /// Tell the method if you want to consider it equal only if the lists are in the same 
    /// order.  Defaults to null which to the method means false.
    /// </param>
    /// <returns>
    /// true if both lists contain the same values (in the same order if sameOrder set to true).  
    /// Also both lists being empty count as equal by value.
    /// </returns>
    public static bool EqualToListByValue<T>(List<T> list, List<T> compare, bool? sameOrder = null)
    {
        var listLen = list.Count;
        var compareLen = compare.Count;

        if (listLen == 0 && compareLen == 0) { return true; } // lists are the same if empty

        if (listLen != compareLen) { return false; } // lists can't have the same values if they have a different number of values

        if (sameOrder is null) { sameOrder = false; } // set the default for caring about order of the lists

        // comparison methods are different depending on if we care about order, so... 
        // we'll start with the default of not caring what order...
        if (!sameOrder.Value)
        {
            // fill the inventory
            var valueInventory = new List<T>();
            foreach (var item in list)
            {
                valueInventory.Add(item);
            }

            // remove from inventory as the item is found
            foreach (var item in compare)
            {
                var currentLength = valueInventory.Count;
                for (var i = 0; i < currentLength; i++)
                {
                    if (valueInventory[i]!.Equals(item!))
                    {
                        valueInventory.RemoveAt(i);
                        break;
                    }
                }
            }

            return valueInventory.Count == 0;
        }

        // now we do the in-order version and we know they have the same length at this point
        for (var i = 0; i < listLen; i++)
        {
            if (list[i]!.Equals(compare[i]!))
            {
                return false;
            }
        }

        return true;
    }


    public static bool ContainsOtherListByValue<T>(List<T> list, List<T> otherList)
    {
        var listLen = list.Count;
        var otherLen = otherList.Count;

        if (listLen < otherLen) { return false; }

        if (listLen == otherLen)
        {
            return EqualToListByValue(list, otherList, false);
        }

        // unfortunately, we need to do an inventory method to be sure duplicate values are matched correctly
        var inventoryList = new List<T>();
        foreach (var item in otherList)
        {
            inventoryList.Add(item);
        }

        foreach (var item in list)
        {
            if (inventoryList.Contains(item))
            {
                inventoryList.Remove(item);
            }
        }

        return inventoryList.Count == 0;
    }


    public static IEnumerable<T> GetItemsInRandomOrder<T>(List<T> sourceList, bool? BetterButSlowerRandomness = null)
    {
        // get the count
        var sourceCount = sourceList.Count;

        // return if there isn't enough to be random
        if (sourceCount == 0) { yield break; }
        else if (sourceCount == 1)
        {
            yield return sourceList[0];
            yield break;
        }

        //figure out what method they want us to use...
        bool BetterRandomness = true;
        if (BetterButSlowerRandomness is not null && BetterButSlowerRandomness.Value == false)
        {
            BetterRandomness = false;
        }

        // create the list of indexes in random order
        var RandIndexes = new List<int>();
        int SleepMS = 13;
        int MaxSleep = 97;
        var RandGen = new Random();
        for (int i = 0; i < sourceCount; i++)
        {
            var RandInsert = RandGen.Next(0, i);
            RandIndexes.Insert(RandInsert, i);
            if (BetterRandomness)
            {
                Thread.Sleep(SleepMS);
                SleepMS = ((SleepMS + RandInsert) % MaxSleep) + 2; // plus two to avoid zero.
            }
        }

        // return the results
        foreach (var ri in RandIndexes)
        {
            yield return sourceList[ri];
        }
    }





}
