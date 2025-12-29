using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeHelperExperiment;

public class cmdList
{
    public cmdList() { }

    public cmdList(ICmd[] commands)
    {
        _ = AddMany(commands);
    }

    private List<ICmd> _commands = new();

    public IEnumerable<ICmd> Commands
    {
        get
        {
            foreach (var cmd in _commands)
            {
                yield return cmd;
            }
        }
    }

    public IEnumerable<ICmd> FindMatches(string userGivenCommandText)
    {
        foreach (var cmd in _commands)
        {
            if (cmd.Name.IsMatch(userGivenCommandText))
            {
                yield return cmd;
            }
        }
    }

    public void Clear()
    {
        _commands.Clear();
    }

    public int Count { get { return _commands.Count; } }

    /// <summary>
    /// Checks to see if the command _names/aliases overlap any already in the 
    /// list, and if not, it adds the command.  If there is overlap, it does 
    /// not add the command, returns false, and gives the first overlap it found.
    /// </summary>
    /// <param name="command">The command you want to add</param>
    /// <param name="overlapName">
    /// If an overlap is found, it gives you the first one it found.
    /// </param>
    /// <returns>True if it successfully added the command to the list.</returns>
    public bool Add(ICmd command)
    {
        if (_commands.Any(cmd => cmd.Name.Overlaps(command.Name)))
        {
            return false;
        }

        _commands.Add(command);
        return true;
    }

    public IEnumerable<bool> AddMany(ICmd[] commands)
    {
        foreach (var cmd in commands)
        {
            yield return Add(cmd);
        }
    }

}
