using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TypeHelperExperiment;

public class cmdName
{
    public cmdName() { }

    public cmdName(params string[] initialNames)
    {
        AddNames(initialNames);
    }


    private List<string> _names = new();

    private char[] stripChars = { ' ', '-', '~' };

    private string GetCleanName(string srcName)
    {
        return srcName.ToLower().TrimStart(stripChars).TrimEnd();
    }

    /// <summary>
    /// The collection of _names that work to call this command
    /// </summary>
    public IEnumerable<string> ValidNames
    {
        get
        {
            foreach (var name in _names) { yield return name; }
        }
    }

    public string MainName { get { return _names.Any() ? _names[0] : string.Empty; } }

    public override string ToString()
    {
        return string.Join(", ", _names);
    }

    public int NameCount { get { return _names.Count; } }

    public void AddNames(params string[] addNames)
    {
        foreach (var name in addNames)
        {
            var cleanName = GetCleanName(name);

            if (!_names.Any(n => string.Equals(n, cleanName)))
            {
                _names.Add(cleanName);
            }
        }
    }

    public void ClearNames()
    {
        _names.Clear();
    }


    public bool IsMatch(string checkName)
    {
        var cleanCompare = GetCleanName(checkName);

        return _names.Any(n => string.Equals(n, cleanCompare, StringComparison.OrdinalIgnoreCase));
    }

    public bool Overlaps(cmdName compareCN)
    {
        foreach (string compareName in compareCN._names)
        {
            if (IsMatch(compareName)) { return true; }
        }

        return false;
    }

    /// <summary>
    /// Tells you which _names overlap between this command name and the 
    /// parameter command name.  Most likely useful if checking a command 
    /// to see if it's reasonable to try adding it to the command list.
    /// </summary>
    public IEnumerable<string> GetOverlaps(cmdName compareCN)
    {
        foreach (string compareName in compareCN._names)
        {
            if (IsMatch(compareName)) { yield return compareName; }
        }
    }


}
