using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTypes.EquationTypes;

public static class EquationHelper
{
    public static decimal CalculateResult(string equation)
    {
        List<string> parts = BreakApart(equation).ToList();



        return 0.0000m;
    }



    public static IEnumerable<string> BreakApart(string sourceString)
    {
        var worker = new StringBuilder();
        string cleanSrc = CleanGroupSymbols(sourceString);

        foreach (char c in cleanSrc)
        {
            if (c == '(')
            {
                if (worker.Length > 0) 
                { 
                    yield return worker.ToString(); 
                    worker.Clear(); 
                }

                worker.Append(c);
            }
            else if (c == ')')
            {
                worker.Append(c);

                yield return worker.ToString();

                worker.Clear();
            }
            else
            {
                worker.Append(c);
            }
        }
    }


    public static string CleanGroupSymbols(string sourceString)
    {
        var result = new StringBuilder();

        foreach (char c in sourceString)
        {
            if (GroupSymbols.ContainsKey(c)) { result.Append(GroupSymbols[c]); }
            else { result.Append(c); }
        }

        return result.ToString();
    }

    public static Dictionary<char, char> GroupSymbols { get; } = new Dictionary<char, char>()
    {
        { '(', '(' }, { ')', ')' },
        { '[', '(' }, { ']', ')' },
        { '{', '(' }, { '}', ')' }
    };

    public static char[] RecognizedOperators { get; } = { '+', '-', '*', '/' };
}
