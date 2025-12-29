using CustomTypes.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTypes;

public struct CycleList<T> : IEnumerable<T>, IEquatable<CycleList<T>> where T : IEquatable<T>
{
    public CycleList() { }

    public CycleList(List<T> sourceList) { foreach (var item in sourceList) { _list.Add(item); } }

    public CycleList(params T[] source) { foreach (var item in source) { _list.Add(item); } }

    private readonly List<T> _list = new();
    private int _currentIndex = 0;

    /// <summary>
    /// Let's you track the current index so if you need to, you can check between 
    /// uses of the Next() method.  Also allows you to set the current index in case 
    /// you want to start somewhere specific, but it prevents you from setting it 
    /// out of range.
    /// </summary>
    public int CurrentIndex
    {
        get { return _currentIndex; }
        set
        {
            if (value < 0) { _currentIndex = 0; }
            else if (value > _list.Count) { _currentIndex = _list.Count - 1; }
            else { _currentIndex = value; }
        }
    }

    /// <summary>
    /// This is the primary method of this class.  It allows you to continuously loop 
    /// through the list of items by going back to the beginning after reaching the end.  
    /// </summary>
    /// <returns>The item at the current index in the list.</returns>
    public T Next()
    {
        T retVal = _list[_currentIndex];
        _currentIndex++;
        if (_currentIndex >= _list.Count) { _currentIndex = 0; }
        return retVal;
    }

    /// <summary>
    /// Allows you to access the list by index as normal for a list.
    /// </summary>
    public T this[int i]
    {
        get { return _list[i]; }
        set { _list[i] = value; }
    }

    public int Count { get { return _list.Count; } }

    public int IndexOf(T item) { return _list.IndexOf(item); }

    public bool Contains(T item) { return _list.Contains(item); }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (T item in _list) { yield return item; }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


    public void Add(T item) { _list.Add(item); }

    public void AddRange(List<T> items) { _list.AddRange(items); }

    public void AddRange(params T[] items) { _list.AddRange(items); }

    /// <summary>
    /// set the desired index and the item, and it'll try to 
    /// put the item in the list at that point.  The only way 
    /// it can fail is if you provide a negative index.
    /// </summary>
    /// <returns>True if the insert worked.  Only returns false with a negative index.</returns>
    public bool Insert(int desiredIndex, T item)
    {
        if (desiredIndex < 0) { return false; }

        if (desiredIndex > _list.Count - 1) 
        { 
            _list.Add(item); 
            return true;
        }

        _list.Insert(desiredIndex, item);
        return true;
    }

    public bool InsertRange(int desiredIndex, IEnumerable<T> items)
    {
        if (desiredIndex < 0) { return false; }

        if (desiredIndex > _list.Count - 1)
        {
            _list.AddRange(items);
            return true;
        }

        _list.InsertRange(desiredIndex, items);
        return true;
    }

    public void Clear() { _list.Clear(); }

    public bool Remove(T item) { return _list.Remove(item); }

    public void RemoveAt(int index) { _list.RemoveAt(index); }

    public int RemoveAll(List<T> items)
    {
        int removed = 0;
        foreach (T item in items)
        {
            if (_list.Remove(item)) { removed++; }
        }
        return removed;
    }

    public int RemoveAll(params T[] items)
    {
        int removed = 0;
        foreach (T item in items)
        {
            if (_list.Remove(item)) { removed++; }
        }
        return removed;
    }

    public void RemoveRange(int startIndex, int count)
    {
        _list.RemoveRange(startIndex, count);
    }

    public bool Equals(CycleList<T> other)
    {
        return _list.ExactSameByValueAndOrder(other._list);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is CycleList<T> clObj) { return Equals(clObj); }

        if (obj is List<T> listObj) { return _list.ExactSameByValueAndOrder(listObj); }

        if (obj is IEnumerable<T> eObj) { return _list.ExactSameByValueAndOrder(eObj.ToList()); }

        if (obj is T tObj && _list.Count == 1) { return _list[0].Equals(tObj); }
        
        return false;
    }

    public override int GetHashCode()
    {
        return _list.GetHashCode();
    }

    public override string ToString()
    {
        return string.Join(", ", _list);
    }


    public static bool operator ==(CycleList<T> a, CycleList<T> b) { return a.Equals(b); }
    public static bool operator !=(CycleList<T> a, CycleList<T> b) { return !a.Equals(b); }
}
