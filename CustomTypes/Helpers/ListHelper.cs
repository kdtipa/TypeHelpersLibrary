using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomTypes.Helpers;


public static class ListHelper
{
    /// <summary>
    /// An equality method that compares lists by value including number of occurences 
    /// of each item.  DOES care what order the items are in.
    /// </summary>
    public static bool ExactSameByValueAndOrder<T>(this List<T> list, List<T> compareList)
        where T : IEquatable<T>
    {
        int listCount = list.Count;
        int compareCount = compareList.Count;

        // if they're not the same size, we know they're not the same
        if (listCount != compareCount) { return false; }

        // if they're the same size and that size is zero, they're technically the same
        if (listCount == 0) { return true; }

        // now do our comparisons...
        for (int i = 0; i < listCount; i++) { if (!list[i].Equals(compareList[i])) { return false; } }

        return true;
    }

    /// <summary>
    /// An equality method that compares lists by value including number of occurences 
    /// of each item.  Does NOT care what order they're in.
    /// </summary>
    public static bool ExactSameByValueOnly<T>(this List<T> list, List<T> compareList)
        where T : IEquatable<T>
    {
        int listCount = list.Count;
        int compareCount = compareList.Count;

        // if they're not the same size, we know they're not the same
        if (listCount != compareCount) { return false; }

        // if they're the same size and that size is zero, they're technically the same
        if (listCount == 0) { return true; }

        // now to build our inventory...
        Dictionary<T, int> inventory = new();
        foreach (T item in list)
        {
            if (inventory.ContainsKey(item)) { inventory[item] += 1; }
            else { inventory.Add(item, 1); }
        }

        // now subtract from the inventory based on what's in the compareList
        foreach (T item in compareList)
        {
            // if it's not the in dictionary, they're not the same
            if (!inventory.ContainsKey(item)) { return false; }

            // it's in there, so we subtract 1 and see if it went below zero
            inventory[item] -= 1;
            if (inventory[item] < 0) { return false; }
        }

        // everything in the inventory should have been cleared if the lists have the same values
        return inventory.All(i => i.Value == 0);
    }

    /// <summary>
    /// Tells you if this list has the same items as the compare list and accounts for 
    /// duplicate entries so that 1, 1, 2, 3 is not the same as 1, 2, 3, 3.
    /// </summary>
    /// <param name="compareList">The list to compare to</param>
    /// <param name="sameOrder">set true if you want the order to matter</param>
    /// <returns>true if the items are the same and if necessary if the order is the same</returns>
    public static bool EqualByValue<T>(this List<T> list, List<T> compareList, bool? sameOrder = null)
        where T : IEquatable<T>
    {
        if (sameOrder ?? false)
        {
            return list.ExactSameByValueAndOrder(compareList);
        }
        else
        {
            return list.ExactSameByValueOnly(compareList);
        }
    }

    /// <summary>
    /// This method checks to see if the values of a smaller list are contained within a larger 
    /// list.  1, 2, 3, 4, 5 contains 2, 5 but not 2, 2 and not 1, 6.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="containingList"></param>
    /// <param name="searchForList"></param>
    /// <returns></returns>
    public static bool ContainsOtherListByValue<T>(this List<T> containingList, List<T> searchForList)
        where T : IEquatable<T>
    {
        var containerLen = containingList.Count;
        var itemLen = searchForList.Count;

        if (containerLen < itemLen) { return false; }

        if (containerLen == itemLen)
        {
            return containingList.ExactSameByValueOnly(searchForList);
        }

        // unfortunately, we need to do an inventory method to be sure duplicate values are matched correctly
        var inventoryList = new List<T>();
        foreach (var item in searchForList)
        {
            inventoryList.Add(item);
        }

        foreach (var item in containingList)
        {
            if (inventoryList.Contains(item))
            {
                inventoryList.Remove(item);
            }
        }

        return inventoryList.Count == 0;
    }

    /// <summary>
    /// If you want to get the items in your list in a random order, this method is one way to 
    /// do that.  It does also allow you to use the SlowRandom class by passing true to the 
    /// BetterButSlowerRandomness parameter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sourceList"></param>
    /// <param name="BetterButSlowerRandomness"></param>
    /// <returns></returns>
    public static IEnumerable<T> GetItemsInRandomOrder<T>(this List<T> sourceList, bool? BetterButSlowerRandomness = null)
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

        // create the containingList of indexes in random order
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

    /// <summary>
    /// If the list might contain duplicate items, you only want one instance of 
    /// each item, this method gets you that collection.
    /// </summary>
    public static IEnumerable<T> GetUniqueItems<T>(this List<T> sourceList) where T : IEquatable<T>
    {
        List<T> sentItems = new List<T>();

        foreach (var item in sourceList)
        {
            if (!sentItems.Any(si => si.Equals(item)))
            {
                sentItems.Add(item);
                yield return item;
            }
        }
    }

    public static bool TryAccessIndex<T>(this List<T> sourceList, int i, [NotNullWhen(true)] out T? value) where T : notnull 
    {
        value = default;

        if (i < 0 || i > sourceList.Count - 1) { return false; }

        value = sourceList[i];
        return true;
    }


    public static IEnumerable<T> Combine<T>(this List<T> sourceList, List<T> additionalValues, bool? addToFront = null)
    {
        if (addToFront is null || addToFront == false)
        {
            foreach (var item in sourceList) { yield return item; }
            foreach (var item in additionalValues) { yield return item; }
        }
        else
        {
            foreach (var item in additionalValues) { yield return item; }
            foreach (var item in sourceList) { yield return item; }
        }
    }

    public static IEnumerable<T> Combine<T>(this List<T> sourceList, T additionalValue, bool? addToFront)
    {
        if (addToFront is null || addToFront == false)
        {
            foreach (var item in sourceList) { yield return item; }
            yield return additionalValue;
        }
        else
        {
            yield return additionalValue;
            foreach (var item in sourceList) { yield return item; }
        }
    }

    public static IEnumerable<T> Combine<T>(params List<T>[] listsInOrder)
    {
        foreach (var list in listsInOrder)
        {
            foreach (T item in list) { yield return item; }
        }
    }

    public static IEnumerable<T> FilterByIndex<T>(this List<T> sourceList, params int[] includeIndices)
    {
        if (includeIndices is null || includeIndices.Length == 0) { yield break; }

        foreach (int index in includeIndices)
        {
            yield return sourceList[index];
        }
    }

    public static IEnumerable<T> FilterByRange<T>(this List<T> sourceList, int startIndex, int endIndex)
    {
        if (sourceList.Count == 0) { yield break; }

        (startIndex, endIndex) = IntHelper.FixRange(startIndex, endIndex, 0, sourceList.Count - 1);

        for (int i = startIndex; i <= endIndex; i++)
        {
            yield return sourceList[i];
        }

    }

    /// <summary>
    /// Allows you to enumerate over a list while having the index of each item.  This also means 
    /// you can use LINQ on the result of this method to filter the list while retaining the index 
    /// so that you don't have to rely on searches of the list to re-locate the item and if there 
    /// are duplicates in the list, you have a reference to the specific one.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sourceList"></param>
    /// <returns></returns>
    public static IEnumerable<ListItemWithIndex<T>> EnumerateWithIndex<T>(this List<T> sourceList)
    {
        int srcLen = sourceList.Count;
        for (int i = 0; i < srcLen; i++)
        {
            yield return new ListItemWithIndex<T>(sourceList[i], i);
        }
    }





}


public struct GroupedList<T> : IEnumerable<ItemWithCount<T>> where T : IEquatable<T>
{

    public GroupedList() { }


    private List<ItemWithCount<T>> _items = new();

    public List<T> Items
    {
        get
        {
            return _items.Select(itm => itm.Value).ToList();
        }
    }

    public List<ItemWithCount<T>> ItemsWithCounts
    {
        get
        {
            List<ItemWithCount<T>> returnVal = new();
            foreach (var item in _items) { returnVal.Add(item); }
            return returnVal;
        }
    }


    public ItemWithCount<T> this[T item]
    {
        get
        {
            foreach (var itm in _items) 
            { 
                if (itm.Match(item)) { return itm; }
            }

            throw new IndexOutOfRangeException("Given item does not exist in this list.");
        }
    }

    public ItemWithCount<T> this[int i]
    {
        get
        {
            return _items[i];
        }
        set
        {
            _items[i] = value; 
        }
    }
    

    /// <summary>
    /// Count of the uniqe items
    /// </summary>
    public int Count { get { return _items.Count; } }

    /// <summary>
    /// Tells you how many of that item exist in our records.  Will return 
    /// zero if the count is set to zero or if the item doesn't exist in 
    /// the list.  Minimum Count is zero as normal.
    /// </summary>
    /// <param name="item">The item to find and get a count of</param>
    /// <returns></returns>
    public int CountOfItem(T item)
    {
        foreach (var itm in _items)
        {
            if (itm.Match(item)) { return itm.Count; }
        }
        return 0;
    }

    public bool Contains(T item)
    {
        foreach (var itm in _items)
        {
            if (itm.Match(item)) { return true; }
        }
        return false;
    }


    /// <summary>
    /// If the item already exists in the List, it will increment the count 
    /// for that item.  By default it will incremenet by 1.  You can set the 
    /// addCount though, which will adjust the item's count by that much.  If 
    /// it's not in the list, it'll add the item to the list, defaulting to 1, 
    /// or using the value provided.  Minimum count is zero.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="addCount"></param>
    public void Add(T item, int? addCount = null)
    {
        int ac = addCount ?? 1;

        if (!Contains(item))
        {
            // in this case, minimum is 0
            if (ac < 0) { ac = 0; }
            _items.Add(new ItemWithCount<T>(item, ac));
            return;
        }

        int itemCount = _items.Count;
        for (int i = 0; i < itemCount; i++)
        {
            if (_items[i].Match(item)) { _items[i].AddToCount(ac); return; }
        }
    }

    public void AddRange(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            Add(item, 0);
        }
    }

    public void Clear() { _items.Clear(); }

    public void ClearCounts()
    {
        for (int i = 0; i < _items.Count; i++)
        {
            _items[i].SetCount(0);
        }
    }


    public bool Remove(T item)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].Match(item)) { _items.RemoveAt(i); return true; }
        }
        return false;
    }

    public int RemoveAll(params T[] items)
    {
        int removedCount = 0;
        for (int i = _items.Count - 1; i >= 0; i--)
        {
            foreach (var itm in items)
            {
                if (_items[i].Match(itm))
                {
                    _items.RemoveAt(i);
                    removedCount++;
                    break;
                }
            }
        }
        return removedCount;
    }

    public int IndexOf(T item)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].Match(item)) { return i; }
        }
        return -1;
    }



    public IEnumerator<ItemWithCount<T>> GetEnumerator()
    {
        foreach (var item in _items)
        {
            yield return item; 
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public struct ItemWithCount<T> where T : IEquatable<T>
{
    public ItemWithCount(T value, int? count = null) 
    { 
        Value = value;
        Count = count ?? 0;
    }

    public T Value { get; private set; }

    private int _count = 0;

    public int Count
    {
        get { return _count; } 
        set
        {
            if (value <= 0) { _count = 0; }
            else { _count = value; }
        }
    }

    public bool Match(T item) { return Value.Equals(item); }

    public void AddToCount(int val)
    {
        Count += val;
    }

    public void SetCount(int val) { Count = val; }
}