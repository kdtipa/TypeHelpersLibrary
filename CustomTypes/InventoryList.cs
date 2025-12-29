using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTypes;

public struct InventoryList<T> : IEnumerable<InventoryItem<T>>, IEquatable<InventoryList<T>> where T : IEquatable<T>
{
    public InventoryList() { }


    private Dictionary<T, int> _inventory = new();

    /// <summary>
    /// This is the number of different items in the inventory
    /// </summary>
    public readonly int ItemTypeCount => _inventory.Count;

    /// <summary>
    /// This is the total number of items in the inventory, 
    /// summing the amounts for each item.
    /// </summary>
    public int ItemTotalCount { get { return _inventory.Sum(item => item.Value); } }

    /// <summary>
    /// This gives you the inventory count for the specific item.  Will 
    /// return zero if the item is not in the inventory, or if it is 
    /// stored and the count for that item is zero.
    /// </summary>
    public int Count(T item)
    {
        if (!_inventory.ContainsKey(item)) { return 0; }
        return _inventory[item];
    }

    public void AddIfNeeded(T item, int? startingQuantity = null)
    {
        int sq = startingQuantity ?? 0;
        if (sq < 0) { sq = 0; }

        if (!_inventory.ContainsKey(item)) { _inventory.Add(item, sq); }
    }

    public void AddOrIncrement(T item)
    {
        if (_inventory.ContainsKey(item)) { _inventory[item]++; }
        else { _inventory.Add(item, 1); }
    }

    public bool SetCount(T item, int count, bool? addItemIfNeeded = null)
    {
        // if the item doesn't already exist...
        if (!_inventory.ContainsKey(item)) 
        {
            // if the user asked us to add it, do that with the correct count and return
            if (addItemIfNeeded ?? false) { _inventory.Add(item, count); return true; }

            // the key doesn't exist and we aren't supposed to add it
            return false;
        }

        // if the key does exist, we just need to set the count...
        _inventory[item] = count;
        return true;
    }

    public bool RemoveItemEntirely(T item)
    {
        return _inventory.Remove(item);
    }

    public bool Contains(T item) { return _inventory.ContainsKey(item); }

    /// <summary>
    /// Clears the whole inventory
    /// </summary>
    public void Clear() { _inventory.Clear(); }

    /// <summary>
    /// Removes any items in the inventory that have a zero count
    /// </summary>
    public void ClearZeroes() 
    {
        _inventory = _inventory
            .Where(item => item.Value != 0)
            .ToDictionary();
    }

    /// <summary>
    /// This method allows you to set all quantities back to zero, 
    /// or if you provide a value, to whatever amount you'd like
    /// </summary>
    /// <param name="itemCount"></param>
    public void ResetInventory(int? itemCount = null)
    {
        int ic = itemCount ?? 0;
        if (ic < 0) { ic = 0; }

        foreach (T item in _inventory.Keys)
        {
            _inventory[item] = ic;
        }
    }

    public IEnumerator<InventoryItem<T>> GetEnumerator()
    {
        foreach (var item in _inventory)
        {
            yield return new InventoryItem<T>(item.Key, item.Value);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool Equals(InventoryList<T> other)
    {
        if (_inventory.Count != other._inventory.Count) { return false; }

        foreach (var item in _inventory.Keys)
        {
            if (!other.Contains(item) && _inventory[item] != other._inventory[item])
            {
                return false;
            }
        }

        return true;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is InventoryList<T> ilObj) { return Equals(ilObj); }

        return false;
    }

    public override int GetHashCode()
    {
        return _inventory.GetHashCode();
    }

    public override string ToString()
    {
        if (_inventory.Count == 0) { return string.Empty; }

        List<string> itemStrings = new();

        foreach (var item in _inventory.Select(item => new InventoryItem<T>(item.Key, item.Value)))
        {
            itemStrings.Add(item.ToString());
        }

        int maxLen = itemStrings.Max(item => item.Length);
        bool haveLineBreaks = itemStrings.Any(item => item.Contains('\n'));

        string insertVal = string.Empty;
        string joinVal = ", ";
        if (maxLen >= 12 || haveLineBreaks) { insertVal = " - "; joinVal = "\n - "; }

        return string.Join(joinVal, itemStrings).Insert(0, insertVal);
    }


    public static bool operator ==(InventoryList<T> a, InventoryList<T> b) { return a.Equals(b); }
    public static bool operator !=(InventoryList<T> a, InventoryList<T> b) { return !a.Equals(b); }

}


public struct InventoryItem<T> : IEquatable<InventoryItem<T>> where T : IEquatable<T>
{
    public InventoryItem(T item, int? initialCount = null)
    {
        Item = item;
        Count = initialCount ?? 0;
    }

    public T Item;

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

    public bool Equals(InventoryItem<T> other)
    {
        return Item.Equals(other.Item) && Count == other.Count;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is InventoryItem<T> iiObj) { return Equals(iiObj); }

        if (obj is KeyValuePair<T, int> kvpObj) { return Item.Equals(kvpObj.Key) && Count == kvpObj.Value; }

        return false;
    }

    public override int GetHashCode() { return Item.GetHashCode(); }

    public override string ToString()
    {
        return $"{Item.ToString()} [{Count}]";
    }


    public static bool operator ==(InventoryItem<T> a, InventoryItem<T> b) { return a.Equals(b); }
    public static bool operator !=(InventoryItem<T> a, InventoryItem<T> b) { return a.Equals(b); }
}
