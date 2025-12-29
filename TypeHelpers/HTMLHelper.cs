using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeHelpers;

public static class HTMLHelper
{

    private static char[] LineBreakChars = { (char)10, (char)13 };

    /// <summary>
    /// This method takes a long string that has line breaks that separate paragraphs, and adds the HTML 
    /// tags to make them show in HTML as the paragraph implied by the string.  Allows you to include a 
    /// CSS class or inline CSS style or both.
    /// </summary>
    /// <param name="longTextWithLineBreaks"></param>
    /// <param name="CSSClassName">A CSS class name</param>
    /// <param name="CSSInlineStyle">A CSS style</param>
    /// <returns></returns>
    public static IEnumerable<string> HTMLParagraphs(
        string longTextWithLineBreaks, 
        string? CSSClassName = null, 
        string? CSSInlineStyle = null, 
        bool? tagsOnOwnLines = null)
    {
        // if it's empty, we return nothing
        if (string.IsNullOrWhiteSpace(longTextWithLineBreaks)) { yield break; }

        // create the correct thing to add to separate the <p> tags from the content
        string tagBreak = tagsOnOwnLines is null || tagsOnOwnLines == false ? "" : "\n";

        // get our collection of paragraphs
        var paragraphs = longTextWithLineBreaks.Split(LineBreakChars, StringSplitOptions.RemoveEmptyEntries);

        // build the complete tag we'll use to open our paragraphs including CSS and a line break if the user asked for separate lines
        var pTagBuilder = new StringBuilder();
        pTagBuilder.Append("<p");
        if (!string.IsNullOrWhiteSpace(CSSClassName)) { pTagBuilder.Append($" class=\"{CSSClassName}\""); }
        if (!string.IsNullOrWhiteSpace(CSSInlineStyle)) { pTagBuilder.Append($" style=\"{CSSInlineStyle}\""); }
        pTagBuilder.Append('>');
        pTagBuilder.Append(tagBreak);
        var pTag = pTagBuilder.ToString();

        // create our closing tag including a line break if the user asked for separate lines
        string endPTag = $"{tagBreak}</p>";

        // loop through our paragraphs and return the string with the paragraph tags added
        foreach (string p in paragraphs)
        {
            yield return $"{pTag}{p}{endPTag}";
        }
        
    }


    



}
