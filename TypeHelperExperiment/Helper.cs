using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeHelperExperiment;

internal static class Helper
{
    public static string GetHelpMenuText(ref cmdList cmdList, ref cmdName helpNames, ref cmdName exitNames, string? horizontalRule = null)
    {
        var result = new StringBuilder();
        string hr = "====================================================";
        if (!string.IsNullOrWhiteSpace(horizontalRule))
        {
            hr = horizontalRule;
        }

        result.AppendLine("Help Menu");
        result.AppendLine(hr);
        result.Append(exitNames.ToString());
        result.AppendLine(" = The command to end the program.");
        result.Append(helpNames.ToString());
        result.AppendLine(" = The command to get this help menu or get help with specific commands");

        foreach (var cmd in cmdList.Commands)
        {
            result.AppendLine($"\n{cmd.Name.ToString()} = {cmd.HelpMenuText}");
        }

        result.AppendLine(hr);

        return result.ToString();
    }
}
