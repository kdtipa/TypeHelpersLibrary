using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTypes.Helpers;

/// <summary>
/// Sometimes when you want to use a foreach loop but need the index inside the loop, you have to 
/// use a normal for-loop instead.  This allows you to use the ListHelper method EnumerateWithIndex 
/// to loop over your list and still have the index.  The Equals, ==, and != methods are defined so 
/// that they can be equated with others of this struct or the type of T.
/// </summary>
/// <typeparam name="T"></typeparam>
public struct ListItemWithIndex<T>
{
    public ListItemWithIndex(T item, int index)
    {
        Item = item;
        Index = index;
        if (Index < -1) { Index = -1; }
    }

    /// <summary>
    /// The index of the item withint the list it is being found in.  Can 
    /// be set to -1, suggesting it is not in a list.
    /// </summary>
    public int Index { get; private set; } = -1;

    /// <summary>
    /// The item from the list
    /// </summary>
    public T Item { get; private set; }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is ListItemWithIndex<T> liwiObj) 
        { 
            return Index == liwiObj.Index 
                && ((Item is not null && liwiObj.Item is not null && Item.Equals(liwiObj.Item)) 
                 || (Item is null && liwiObj.Item is null)); 
        }

        if (obj is T tObj) 
        {
            return (tObj is not null && Item is not null && Item.Equals(tObj))
                || (tObj is null && Item is null);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return Item is null ? 0 : Item.GetHashCode();
    }

    public override string ToString()
    {
        if (Item is null)
        {
            return $"null [{Index}]";
        }
        
        return $"{Item.ToString()} [{Index}]";
    }


    public static bool operator ==(ListItemWithIndex<T> a, object? b) { return a.Equals(b); }
    public static bool operator !=(ListItemWithIndex<T> a, object? b) { return !a.Equals(b); }



}
