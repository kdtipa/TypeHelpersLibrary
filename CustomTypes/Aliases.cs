using CustomTypes.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTypes;

/// <summary>
/// This struct is meant to help in situations where something might have multiple 
/// words associated as a name.  For example, if you have a console app, and you 
/// want a command that closes the application, you might use an Aliases object to 
/// give it the names "Exit", "X", "Quit", "Q", "Close", "C", and so on.  And if 
/// the user types any of those (case insensitive), it would be a match.  Just use 
/// the IsMatch method.  This code also removes leading and trailing spaces, dashes, 
/// underscores, periods, tabs, and line breaks to try to keep the command words as 
/// clean as possible, but this is done in the matching too, so if someone types 
/// "-x", it would still count as a match in our example above.
/// </summary>
public struct Aliases : IEquatable<Aliases>
{
    /// <summary>
    /// An empty Aliases object.  Use AddAlias or AddAliases to 
    /// put some values in.
    /// </summary>
    public Aliases() { }

    /// <summary>
    /// Defines the object with an initial set of values.  You can 
    /// still use AddAlias and RemoveAlias to adjust.
    /// </summary>
    /// <param name="aliases"></param>
    public Aliases(params string[] aliases)
    {
        foreach (var alias in aliases)
        {
            _ = AddAlias(alias);
        }
    }

    private List<string> _aliases = new();

    /// <summary>
    /// Let's you poke through the list in case you want to.  In 
    /// general though, using the IsMatch method should cover most 
    /// needs for seeing what's in the List compared to a string.
    /// </summary>
    public IEnumerable<string> AliasList
    {
        get { foreach (var alias in _aliases) { yield return alias; } }
    }

    /// <summary>
    /// The number of Aliases represented
    /// </summary>
    public int Count { get { return _aliases.Count; } }

    /// <summary>
    /// Cleans up the string and tries to add it to the 
    /// List of aliases.  If it is already in the list, 
    /// it returns false because it didn't add anything.  
    /// If the add happened, it returns true.
    /// </summary>
    public bool AddAlias(string alias)
    {
        string worker = alias.Trim(TrimChars);
        if (_aliases.Any(a => a.Equals(worker, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        _aliases.Add(worker);
        return true;
    }

    /// <summary>
    /// Uses the AddAlias method to try adding your list.  
    /// It returns the number actually added (which leaves 
    /// out any that were already in the list or were 
    /// duplicates in the parameters).
    /// </summary>
    public int AddAliases(params string[] aliases)
    {
        int addedCount = 0;

        foreach (var alias in aliases)
        {
            if (AddAlias(alias)) { addedCount++; }
        }

        return addedCount;
    }

    /// <summary>
    /// Looks for a match and removes it if found.  In that 
    /// case, it returns true.  If not found, it returns false.
    /// </summary>
    public bool RemoveAlias(string alias)
    {
        string worker = alias.Trim(TrimChars);
        for (int i = 0; i < _aliases.Count; i++)
        {
            if (_aliases[i].Equals(worker, StringComparison.OrdinalIgnoreCase))
            {
                _aliases.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// The most important method of this struct.  It tells you if the compare string 
    /// matches one of the aliases.  As a related note, == and != are defined for an 
    /// object of this type and a string to function as this method, IsMatch.
    /// </summary>
    /// <param name="compare">The string you want to look for a match for.</param>
    /// <returns>true if the compare string matches one of the aliases.</returns>
    public bool IsMatch(string compare)
    {
        string worker = compare.Trim(TrimChars);
        return _aliases.Any(a => a.Equals(worker));
    }

    /// <summary>
    /// Tells you if two Aliases objects have overlapping strings.  For example, if 
    /// your open file command and find file command both allow "f" as a match, this 
    /// method can warn you of it.
    /// </summary>
    public bool Overlaps(Aliases other)
    {
        foreach (var check in _aliases)
        {
            if (other.IsMatch(check)) { return true; }
        }
        return false;
    }

    /// <summary>
    /// The pure equality method that tells you if the two aliases are the same, 
    /// including all aliases stored.
    /// </summary>
    public bool Equals(Aliases other)
    {
        if (_aliases.Count != other._aliases.Count) { return false; }

        foreach (var alias in _aliases)
        {
            if (!other.IsMatch(alias)) { return false; }
        }
        return true;
    }

    /// <summary>
    /// A general equals method which can handle Aliases objects, Lists of strings, and 
    /// single strings.  For single strings, it functions as the IsMatch method.  For 
    /// the others, it looks for an exact match of the lists.
    /// </summary>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is Aliases aObj) { return Equals(aObj); }

        if (obj is List<string> lsObj)
        {
            // make sure each item is a match
            foreach (var ls in lsObj)
            {
                if (!IsMatch(ls)) { return false; }
            }
            return true;
        }

        if (obj is string sObj) { return IsMatch(sObj); }

        return false;
    }

    public override int GetHashCode()
    {
        return _aliases.GetHashCode();
    }

    /// <summary>
    /// Gives you the comma separate list of aliases.
    /// </summary>
    public override string ToString()
    {
        return string.Join(", ", _aliases);
    }

    public static char[] TrimChars = { ' ', '-', '_', '.', '+', (char)9, (char)10, (char)13 };

    public static bool operator ==(Aliases a, Aliases b) { return a.Equals(b); }
    public static bool operator !=(Aliases a, Aliases b) { return !a.Equals(b); }

    public static bool operator ==(Aliases a, string b) { return a.Equals(b); }
    public static bool operator !=(Aliases a, string b) { return !a.Equals(b); }

    public static bool operator ==(string a, Aliases b) { return b.Equals(a); }
    public static bool operator !=(string a, Aliases b) { return !b.Equals(a); }

    public static Aliases operator +(Aliases a, Aliases b)
    {
        var result = new Aliases();

        foreach (var aa in a.AliasList) { _ = result.AddAlias(aa); }
        foreach (var ba in b.AliasList) { _ = result.AddAlias(ba); }

        return result;
    }

    public static Aliases operator -(Aliases a, Aliases b)
    {
        var result = new Aliases();

        foreach (var aa in a.AliasList) { _ = result.AddAlias(aa); }
        foreach (var ba in b.AliasList) { _ = result.RemoveAlias(ba); }

        return result;
    }

    public static Aliases operator +(Aliases a, string b)
    {
        var result = new Aliases();

        foreach (var aa in a.AliasList) { _ = result.AddAlias(aa); }
        _ = result.AddAlias(b);

        return result;
    }

    public static Aliases operator -(Aliases a, string b)
    {
        var result = new Aliases();

        foreach (var aa in a.AliasList) { _ = result.AddAlias(aa); }
        _ = result.RemoveAlias(b);

        return result;
    }

}
