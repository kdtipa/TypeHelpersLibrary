using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomTypes.Helpers;

namespace TypeHelperExperiment.cmds
{
    internal class cmdIntGetFactors : ICmd
    {
        public cmdName Name { get; } = new("getfactors", "gf", "factors");

        public string HelpMenuText { get; } = "gets you the list of integer factors of a number or numbers you supply.";

        public string SpecificHelpText { get; } = @"GetFactors command
=======================
aliases: getfactors, gf, factors

usage... 
  getfactors 14
  gf 6 7 10 60
  factors 23
=======================";

        public cmdResult Run(params string[] arguments)
        {
            foreach (var arg in arguments)
            {
                handleArg(arg);
            }

            return new cmdResult();
        }

        private void handleArg(string arg)
        {
            if (arg.Contains(' '))
            {
                // this means we got passed things we need to split, and check each one individually
                char[] delims = { ' ', ',', '\'', '"', '\t' };
                var chunks = arg.Split(delims, StringSplitOptions.RemoveEmptyEntries);
                foreach (var chunk in chunks)
                {
                    if (int.TryParse(chunk, out int result))
                    {
                        var factors = result.GetFactors();
                        if (factors is not null)
                        {
                            Console.Write($"factors of {result} = ");
                            Console.WriteLine(string.Join(", ", factors));
                        }
                    }
                }
            }
            else
            {
                // this means we got a normal single chunk of text we can try parsing
                if (int.TryParse(arg.Trim(), out int result))
                {
                    var factors = result.GetFactors();
                    if (factors is not null)
                    {
                        Console.Write($"factors of {result} = ");
                        Console.WriteLine(string.Join(", ", factors));
                    }
                }
            }
        }
    }
}
