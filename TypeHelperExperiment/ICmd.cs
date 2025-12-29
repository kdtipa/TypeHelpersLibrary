using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeHelperExperiment;

public interface ICmd
{
    /// <summary>
    /// The complex name of the command which includes the aliases, and a 
    /// built-in method for checking to see if a string matches any of the 
    /// _names for this command.
    /// </summary>
    public cmdName Name { get; }

    /// <summary>
    /// The text to be shown for the help menu along with this command
    /// </summary>
    public string HelpMenuText { get; }

    /// <summary>
    /// The text to be shown for the command if the specific help is 
    /// request for the specific command.  It is commonly longer than 
    /// the blurb given for the help menu.
    /// </summary>
    public string SpecificHelpText { get; }

    /// <summary>
    /// The main method of the command that does what you need
    /// </summary>
    /// <param name="arguments">
    /// The correctly broken up list of arguments for the command, 
    /// if there are any.  Most likely, if the command needs info 
    /// from the user, it should have prompts and allow input.  You 
    /// could get fancy and allow the user to enter commands either 
    /// way.  It's essentially up to the person who writes the 
    /// implementation of the Run method.
    /// </param>
    /// <returns>
    /// Returns a command result which is a bool that tells the 
    /// main program whether the running of the command should 
    /// be considered a success or not, and an optional list of 
    /// messages which could be shown to the user.
    /// </returns>
    public cmdResult Run(params string[] arguments);
}
