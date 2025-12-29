using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CustomTypes.Helpers;

namespace TypeHelperExperiment;

public class cmdResult
{
    public cmdResult() { Succeeded = true; }

    public cmdResult(bool succeeded)
    {
        Succeeded = succeeded;
    }

    public cmdResult(bool succeeded, params string[] messages)
    {
        foreach (var message in messages) { msgs.Add(message); }
    }


    public bool Succeeded { get; private set; }


    private List<string> msgs = new();
    public IEnumerable<string>? Messages 
    {
        get
        {
            foreach (var msg in msgs) { yield return msg; }
        }
    }

    public void AddMessage(string msg)
    {
        msgs.Add(msg);
    }

    public void ClearMessages()
    {
        msgs.Clear();
    }



    public override bool Equals(object? obj)
    {
        if (obj is null) { return false; }

        if (obj is cmdResult cr)
        {
            return cr.Succeeded == Succeeded && msgs.ExactSameByValueOnly(cr.msgs);
        }

        if (obj is bool bCompare)
        {
            return Succeeded == bCompare;
        }

        return false;
    }

    public override int GetHashCode()
    {
        if (Succeeded)
        {
            return 1 + msgs.Count > 0 ? 1 : 0;
        }
        else
        {
            return -1 + msgs.Count > 0 ? -1 : 0;
        }
    }

    public override string ToString()
    {
        return Succeeded.ToString();
    }

    public string ToString(string label, bool? listMessages = null, string? listBullet = null)
    {
        var result = new StringBuilder();
        bool lm = listMessages is not null && listMessages.Value == true;
        string lb = " - ";
        if (!string.IsNullOrEmpty(listBullet) && listBullet.Length <= 10)
        {
            lb = listBullet;
        }

        if (!string.IsNullOrEmpty(label))
        {
            result.Append(label);
        }

        result.Append(Succeeded.ToString());

        if (lm && msgs.Any())
        {
            result.AppendLine();
            foreach (var msg in msgs)
            {
                result.AppendLine($"{lb}{msg}");
            }
        }

        return result.ToString();
    }


    public static bool operator ==(cmdResult cr, bool compareVal)
    {
        return cr.Succeeded == compareVal;
    }

    public static bool operator !=(cmdResult cr, bool compareVal)
    {
        return cr.Succeeded != compareVal;
    }

    public static bool operator ==(bool compareVal, cmdResult cr)
    {
        return cr.Succeeded == compareVal;
    }

    public static bool operator !=(bool compareVal, cmdResult cr)
    {
        return cr.Succeeded != compareVal;
    }
}
