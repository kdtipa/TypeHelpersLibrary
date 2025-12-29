namespace TypeHelperExperiment;

using System;
using System.Linq;
using TypeHelperExperiment.cmds;

internal class Program
{
    static void Main(string[] args)
    {
        int argCount = args.Length;
        if (argCount > 0)
        {
            // we'll see if the user has entered a valid command and run that, and exit the program when we're done
            // ToDo: put in the code for that
            // return;

            // temporarily, just note that we got args
            string plural = argCount != 1 ? "s" : "";
            Console.WriteLine($"got {argCount} executable arg{plural}\n");
        }

        // create the list of commands...
        cmdList _commands = new();
        _commands.Add(new cmdStringIsInt());
        _commands.Add(new cmdIntGetFactors());
        _commands.Add(new cmdRomanNumerals());
        _commands.Add(new cmdShowMultTable());

        //special commands...
        cmdName cmdExit = new("exit", "x", "quit", "q");
        cmdName cmdHelp = new("help", "h");



        char[] argDelims = { ' ' };
        const string HorizontalRuleString = "============================================";
        bool keepGoing = true;
        while (keepGoing)
        {
            Console.Write(">> ");
            var userInput = Console.ReadLine() ?? "";
            var inputParts = userInput.Split(argDelims, StringSplitOptions.RemoveEmptyEntries);
            var inputCount = inputParts.Length;

            if (inputCount == 0) { continue; }

            // handle exit command
            if (cmdExit.IsMatch(inputParts[0]))
            {
                if (inputCount > 1 && cmdHelp.IsMatch(inputParts[1])) 
                {
                    // display help for the exit command
                    Console.WriteLine("Detailed Help for the Exit Command");
                    Console.WriteLine(HorizontalRuleString);

                    Console.WriteLine($"aliases: {cmdExit.ToString()}\n");
                    Console.WriteLine("usage: just type one of the exit aliases at the prompt; hit enter; and the program will exit.");

                    Console.WriteLine(HorizontalRuleString);
                    Console.WriteLine();
                    continue;
                }

                // if it wasn't help, we're running the exit
                keepGoing = false;
                break;
            }

            // handle help command
            if (cmdHelp.IsMatch(inputParts[0]))
            {
                if (inputCount > 1)
                {
                    // means the user might be looking for help about a command, so see if any of the extra args match a command
                    for (int i = 1; i < inputCount; i++)
                    {
                        
                    }
                }

                // show help stuff
                Console.WriteLine(Helper.GetHelpMenuText(ref _commands, ref cmdHelp, ref cmdExit, HorizontalRuleString));
                continue;
            }

            // make sure we have 1 match in our command list
            var matches = _commands.FindMatches(inputParts[0]).ToList();
            var matchCount = matches is null ? 0 : matches.Count;
            if (matchCount == 0)
            {
                Console.WriteLine($"No matches found for command {inputParts[0]}\n");
                continue;
            }
            
            if (matchCount > 1)
            {
                Console.WriteLine($"{matchCount} matches found.  Something is broken.");
                continue;
            }

            // if they typed the command name and then a help request, handle that


            // if we got here, we'll finally run the command
            matches![0].Run(inputParts);

        }

        Console.WriteLine("exiting...\n");


    }
}
