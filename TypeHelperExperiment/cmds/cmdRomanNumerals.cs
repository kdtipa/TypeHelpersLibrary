using CustomTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeHelperExperiment.cmds;

public class cmdRomanNumerals : ICmd
{
    public cmdName Name { get; } = new cmdName("RomanNumerals", "RNs", "RN");

    public string HelpMenuText => "use this command to enter a mode that lets you translate from integers to Roman Numerals and RNs to integers.";

    public string SpecificHelpText => "from the RN command prompt, type roman numeral string to get the integer value.\nOr type an integer to get the additive roman numeral.\nOr type an integer followed by - to get the subtractive roman numeral.";

    public cmdResult Run(params string[] arguments)
    {
        Console.WriteLine("Welcome to the Roman Numeral translator.\nType in a value to translate to the other form.\nType in 'exit' to close this command.");

        cmdName exit = new cmdName("exit", "quit", "q");
        bool keepGoing = true;
        char[] trimEnd = { '-', '+', ' ' };

        while (keepGoing) 
        {
            Console.Write("rn translate: ");
            var userInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userInput)) { continue; }

            if (exit.IsMatch(userInput)) { keepGoing = false; continue; }

            userInput = userInput.Trim();
            bool requestSubtractive = false;
            if (userInput.EndsWith('-')) { requestSubtractive = true; }
            userInput = userInput.TrimEnd(trimEnd);

            if (int.TryParse(userInput, out int parseResult))
            {
                if (parseResult >= RomanNumeral.MinValue && parseResult <= RomanNumeral.MaxValue)
                {
                    var rn = new RomanNumeral(parseResult);
                    Console.WriteLine($"Roman Numeral = {rn.ToString(requestSubtractive)}");
                }
                else
                {
                    Console.WriteLine($"{parseResult} is outside range of {RomanNumeral.MinValue} to {RomanNumeral.MaxValue}");
                }
            }
            else if (RomanNumeral.TryParse(userInput, out var rnResult) && rnResult is not null)
            {
                Console.WriteLine($"Integer Value = {rnResult.Value.Value.ToString()}");
            }
            else
            {
                Console.WriteLine("Input seems to be neither an integer in range or a valid roman numeral");
            }
        }


        Console.WriteLine("Exiting Roman Numeral translator back to main prompt...\n");
        return new cmdResult(true);
    }
}
