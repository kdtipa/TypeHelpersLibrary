using CustomTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeHelperExperiment.cmds;

public class cmdShowMultTable : ICmd
{
    public cmdName Name { get; } = new("mult", "m", "table");

    public string HelpMenuText { get; } = "run this command to test out the table display.";

    public string SpecificHelpText { get; } = @"This multiplication table display command has lots of arguments. If you 
don't specific parameter names, the order is [max multiplier], [min 
multiplier], [outside border], [label separator], and finally the [cell 
borders].  When specifying name, use = and no spaces like [max=7] and 
don't type the braces... they're just here to show there are no spaces.

    max : the maximum multiplier to show on the table.  defaults to 9 and tops out at 25.
    min : the low end multiplier to show on the table.  defaults to 2 and bottoms out at 2.
    out : number from 0 to 2 that tells how many lines to use in the table's outside border.
    lbl : number from 0 to 2 that tells how many lines to use between labels and content cells.
    cll : number from 0 to 2 that tells how many lines to use between content cells.
";

    public cmdResult Run(params string[] arguments)
    {
        // checks for arguments or a help request.  Check isHelpRequest before displaying table.
        ParseArguments(arguments);

        if (isHelpRequest)
        {
            Console.WriteLine(SpecificHelpText);
            return new cmdResult(true);
        }

        Console.WriteLine($"Running the multiplication table from {min} to {max} with {outside}{lbl}{cell} borders.\n");

        TextDisplayTable tdt = new()
        {
            ShowLeftLabels = true,
            ShowTopLabels = true 
        };

        tdt.LineBorder = new(2);
        tdt.LineLabelSeparator = new(2);

        int currentRow = 0;
        for (int n = min; n <= max; n++)
        {
            string nStr = n.ToString();
            tdt.TopLabels.Add(nStr);
            tdt.LeftLabels.Add(nStr);
            tdt.CellData.Add(new());

            for (int n2 = min; n2 <= max; n2++)
            {
                tdt.CellData[currentRow].Add((n * n2).ToString());
            }

            currentRow++;
        }

        foreach (string line in tdt.GenerateTableByLines())
        {
            Console.WriteLine(line);
        }

        return new(true);
    }

    private int max = 9;
    private int min = 2;
    private int outside = 2;
    private int lbl = 2;
    private int cell = 1;
    private bool isHelpRequest = false;

    public void ResetArguments()
    {
        max = 9;
        min = 2;
        outside = 2;
        lbl = 2;
        cell = 1;
        isHelpRequest = false;
    }

    private void ParseArguments(string[] arguments)
    {
        ResetArguments();
        if (arguments is null || arguments.Length == 0) { return; }
        
        char[] spaceDelim = { ' ' };
        char[] equalDelim = { '=', ':' };

        string[] parts;

        // in case we didn't get a split up array we'll see if the first argument has anything useful.
        if (arguments.Length == 1 && arguments[0].Contains(' '))
        {
            parts = arguments[0].Split(spaceDelim, StringSplitOptions.RemoveEmptyEntries);
        }
        else
        {
            parts = arguments;
        }

        // now we have a thing we can parse for integers or parameter names equalling integers
        // we should look for specified values first...
        List<string> remaining = new();
        bool[] foundParts = { false, false, false, false, false };  // max, min, out, lbl, cell
        foreach (string part in parts)
        {
            if (string.Equals("help", part, StringComparison.OrdinalIgnoreCase) || string.Equals("h", part, StringComparison.OrdinalIgnoreCase))
            {
                // run the help instead of the table display
                isHelpRequest = true;
                return;
            }
            else if (part.Contains('='))
            {
                // do the parsing for a parameter
                string[] kvp = part.Split(equalDelim, StringSplitOptions.RemoveEmptyEntries);
                if (kvp.Length == 2)
                {
                    // we might have found something useful.
                    // the second part must be a number though, so let's start there...
                    if (int.TryParse(kvp[1], out int paramVal))
                    {
                        if (string.Equals("max", kvp[0], StringComparison.OrdinalIgnoreCase) && !foundParts[0])
                        {
                            max = paramVal;
                            foundParts[0] = true;
                        }
                        else if (string.Equals("min", kvp[0], StringComparison.OrdinalIgnoreCase) && !foundParts[1])
                        {
                            min = paramVal;
                            foundParts[1] = true;
                        }
                        else if (string.Equals("out", kvp[0], StringComparison.OrdinalIgnoreCase) && !foundParts[2])
                        {
                            outside = paramVal;
                            foundParts[2] = true;
                        }
                        else if (string.Equals("lbl", kvp[0], StringComparison.OrdinalIgnoreCase) && !foundParts[3])
                        {
                            outside = paramVal;
                            foundParts[3] = true;
                        }
                        else if (string.Equals("cll", kvp[0].Replace("e", ""), StringComparison.OrdinalIgnoreCase) && !foundParts[4])
                        {
                            cell = paramVal;
                            foundParts[4] = true;
                        }
                    }
                }
            }
            else
            {
                // store the value for after so we can apply the order to anything we didn't find
                remaining.Add(part);
            }
        }

        // now we should have some values if they user passed any with parameter names
        // and anything left will be in the remaining list.
        foreach (var part in remaining)
        {
            if (int.TryParse(part, out int unknownPart))
            {
                if (!foundParts[0]) { max = unknownPart; foundParts[0] = true; }
                else if (!foundParts[1]) { min = unknownPart; foundParts[1] = true; }
                else if (!foundParts[2]) { outside = unknownPart; foundParts[2] = true; }
                else if (!foundParts[3]) { lbl = unknownPart; foundParts[3] = true; }
                else if (!foundParts[4]) { cell = unknownPart; foundParts[4] = true; }
                else { break; }
            }
        }

        // now we've used all the arguments, but we need to make sure none of the values are dumb.
        if (max < 0) { max = 0; } else if (max > 25) { max = 25; }
        if (min < 0) { min = 0; } else if (min > 25) { min = 25; }

        // swap if needed
        if (min > max) { (min, max) = (max, min); }

        // limit the borders
        if (outside < 0) { outside = 0; } else if (outside > 2) { outside = 2; }
        if (lbl < 0) { lbl = 0; } else if (lbl > 2) { lbl = 2; }
        if (cell < 0) { cell = 0; } else if (cell > 2) { cell = 2; }

        // and we're done
    }


}
