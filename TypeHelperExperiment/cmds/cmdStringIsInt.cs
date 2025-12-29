using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomTypes.Helpers;

namespace TypeHelperExperiment.cmds;

public class cmdStringIsInt : ICmd
{
    public cmdName Name { get; } = new cmdName("isint", "is_int", "isnum", "is_num", "isnumber", "is_number");
    public string HelpMenuText { get; } = "Pass in a string and this command will tell you if it represents an integer or not";
    public string SpecificHelpText { get; } = @"StringIsInt command
=======================
aliases: isint, is_int, isnum, is_num, isnumber, is_number

usage... 
  isint 579
  is_num hello world
  isnumber 1970 was the best year!
=======================";

    public cmdResult Run(params string[] arguments)
    {
        List<string> results = new();
        bool isFirst = true;

        foreach (string arg in arguments)
        {
            if (isFirst && Name.IsMatch(arg))
            {
                isFirst = false;
                continue;
            }

            if (arg.IsInt(out int val))
            {
                Console.WriteLine($"  {val} is an integer");
            }
            else
            {
                Console.WriteLine($"  {arg} is NOT an integer");
            }
        }

        Console.WriteLine();

        return new cmdResult();
    }
}
