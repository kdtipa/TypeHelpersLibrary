using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomTypes.Helpers;

namespace CustomTypes;

public class TextDisplayTable
{

    public TextDisplayTableLine LineBorder { get; set; } = new(1);

    public TextDisplayTableLine LineLabelSeparator { get; set; } = new(1);

    public TextDisplayTableLine LineCellSeparator { get; set; } = new(1);


    public List<string> TopLabels { get; set; } = new();
    public bool ShowTopLabels { get; set; } = false;

    public List<string> LeftLabels { get; set; } = new();
    public bool ShowLeftLabels { get; set; } = false;

    /// <summary>
    /// First index is the row, and second is the column
    /// </summary>
    public List<List<string>> CellData { get; set; } = new();

    public bool IncludeBufferSpace { get; set; } = true;

    private int getColumnCount()
    {
        int result = TopLabels.Count;
        foreach (var row in CellData)
        {
            if (result < row.Count) { result = row.Count; }
        }
        return result;
    }

    /// <summary>
    /// A utility method to tell you how wide a column has to 
    /// be to support displaying the text without truncation.
    /// </summary>
    /// <param name="columnIndex">
    /// Which column do you want to know the width for?  Pass 
    /// -1 to get the LeftLabels result.
    /// </param>
    /// <param name="includeTopLabel">
    /// You might not have top labels or not want to display 
    /// them this time around, so you just want the answer 
    /// without the top label.  Pass true if you want the 
    /// method to look for a label for that column and include 
    /// it in the result.  Defaults to false.
    /// </param>
    /// <param name="includeBufferSpaces">
    /// The label "count" might need to be displayed as " count ", 
    /// so you want those buffer spaces.  Defaults to false.
    /// </param>
    /// <returns>
    /// The number of characters wide the given column needs to 
    /// have all its content displayed properly.  If the column 
    /// exists but has no content, you'll get 0.  If the column 
    /// does NOT exist (you passed a non-existent index), you'll 
    /// get a null.
    /// </returns>
    public int? RequiredColumnWidth(int columnIndex, bool? includeTopLabel = null)
    {
        bool itl = includeTopLabel ?? false;

        if (columnIndex < -1) { return null; }

        if (columnIndex == -1)
        {
            // this is the case where we're just looking at the left labels
            int longestLabel = 0;
            foreach (string lbl in LeftLabels)
            {
                if (lbl.Length > longestLabel) { longestLabel = lbl.Length; }
            }

            if (IncludeBufferSpace) { longestLabel += 2; }
            return longestLabel;
        }

        // now we need to figure out if there's anything to even look at.  
        int longestContent = 0;
        bool foundColumn = false;
        if (itl && TopLabels.Count > columnIndex)
        {
            // start with the label if the user wants that
            int lblLen = TopLabels[columnIndex].Length;
            if (lblLen > longestContent) { longestContent = lblLen; }
            foundColumn = true;
        }

        int rowCount = CellData.Count;
        for (int r = 0; r < rowCount; r++)
        {
            // see if the current row has cell data for the asked for column.
            if (CellData[r].Count > columnIndex)
            {
                int dataLen = CellData[r][columnIndex].Length;
                if (dataLen > longestContent) { longestContent = dataLen; }
                foundColumn = true;
            }
        }

        if (IncludeBufferSpace) { longestContent += 2; }
        if (!foundColumn) { return null; }
        return longestContent;
    }
    

    public string GenerateTable()
    {
        var wholeTable = new StringBuilder();

        foreach (string line in GenerateTableByLines())
        {
            wholeTable.AppendLine(line);
        }

        return wholeTable.ToString();
    }

    public IEnumerable<string> GenerateTableByLines()
    {
        List<int> colWidths = new();
        if (ShowLeftLabels)
        {
            // if the user wants to show left labels, we need a number here
            var reqLeftWidth = RequiredColumnWidth(-1, false);
            colWidths.Add(reqLeftWidth ?? (IncludeBufferSpace ? 2 : 0));
        }

        // now we build out list of column widths from the data for use in generating each line...
        int colCount = getColumnCount();
        for (int c = 0; c < colCount; c++) 
        { 
            var cw = RequiredColumnWidth(c, ShowTopLabels);
            if (cw is null) { continue; }
            colWidths.Add(cw.Value);
        }

        // let's make and return our top line...
        yield return GenerateTopLine(colWidths);

        if (ShowTopLabels)
        {
            // now the header text cells...
            yield return GenerateTopLabelLine(colWidths);

            // now the border between header labels and content section
            yield return GenerateLabelSeparatorLine(colWidths);
        }

        // now the content section
        // first we can generate the separator that goes between each row of data because it's the same for all...
        string contentLineSeparator = GenerateContentSeparatorLine(colWidths);

        // then we loop and fill in content and lines...
        int rowCount = CellData.Count;
        for (int r = 0; r < rowCount; r++)
        {
            if (r > 0) { yield return contentLineSeparator; }

            // content line
            yield return GenerateContentLine(colWidths, r);
        }

        // finally, bottom line...
        yield return GenerateBottomLine(colWidths);
    }

    
    private string GenerateTopLabelLine(List<int> cellWidths)
    {
        StringBuilder result = new StringBuilder();
        int cellCount = cellWidths.Count;
        if (cellCount == 0) { return string.Empty; }

        // first cell is empty if we're showing headers...
        string currentText = string.Empty;
        bool useLabelSep = ShowLeftLabels;

        char leftBorder = CharHelper.GetLineChar(LineBorder.Count, 0, LineBorder.Count, 0) ?? '|';
        char rightBorder = CharHelper.GetLineChar(LineBorder.Count, 0, LineBorder.Count, 0) ?? '|';
        char labelSeparator = CharHelper.GetLineChar(LineLabelSeparator.Count, 0, LineLabelSeparator.Count, 0) ?? '|';
        char cellSeparator = CharHelper.GetLineChar(LineCellSeparator.Count, 0, LineCellSeparator.Count, 0) ?? '+';
        char horizontalRule = CharHelper.GetLineChar(0, LineCellSeparator.Count, 0, LineCellSeparator.Count) ?? '-';

        result.Append(leftBorder);
        List<string> rowContent = TopLabels;
        if (ShowLeftLabels)
        {
            rowContent = TackOnLabelContent(string.Empty, TopLabels).ToList();
        }

        for (int c = 0; c < cellCount; c++)
        {
            // take care of separators after the first cell
            if (c == 1)
            {
                if (ShowLeftLabels) { result.Append(labelSeparator); }
                else { result.Append(cellSeparator); }
            }
            else if (c > 1) { result.Append(cellSeparator); }

            // handle the contents
            result.Append(rowContent[c].PadLeftIndent(cellWidths[c], 1, ' '));
        }

        result.Append(rightBorder);

        return result.ToString();
    }

    private string GenerateLabelSeparatorLine(List<int> cellWidths)
    {
        StringBuilder result = new StringBuilder();
        char hr = CharHelper.GetLineChar(0, LineLabelSeparator.Count, 0, LineLabelSeparator.Count) ?? '-';
        // the first junction might be different than the others, so we have to do this stupid crap
        char junc1 = CharHelper.GetLineChar(LineLabelSeparator.Count, LineLabelSeparator.Count, LineLabelSeparator.Count, LineLabelSeparator.Count) ?? '+';
        char junc = CharHelper.GetLineChar(LineCellSeparator.Count, LineLabelSeparator.Count, LineCellSeparator.Count, LineLabelSeparator.Count) ?? '+';

        // left edge
        result.Append(CharHelper.GetLineChar(LineBorder.Count, LineLabelSeparator.Count, LineBorder.Count, 0) ?? '|');


        for (int c = 0; c < cellWidths.Count; c++)
        {
            if (c == 1) { result.Append(junc1); }
            else if (c > 1) { result.Append(junc); }

            result.Append("".PadRight(cellWidths[c], hr));
        }

        // right edge
        result.Append(CharHelper.GetLineChar(LineBorder.Count, 0, LineBorder.Count, LineLabelSeparator.Count) ?? '|');

        return result.ToString();
    }

    private IEnumerable<string> TackOnLabelContent(string labelCellContent, List<string> rowData)
    {
        yield return labelCellContent;

        foreach (string row in rowData) { yield return row; }
    }

    private string GenerateTopLine(List<int> cellWidths)
    {
        StringBuilder tl = new StringBuilder();
        int cellCount = cellWidths.Count;

        // top left corner
        tl.Append(CharHelper.GetLineChar(0, LineBorder.Count, LineBorder.Count, 0));

        char sep = CharHelper.GetLineChar(0, LineBorder.Count, LineCellSeparator.Count, LineBorder.Count) ?? '+';
        char hrc = CharHelper.GetLineChar(0, LineBorder.Count, 0, LineBorder.Count) ?? '-';

        for (int c = 0; c < cellCount; c++)
        {
            // if it's not the first one, we add the separator character before the horizontal line
            if (c == 1)
            {
                if (ShowLeftLabels) { tl.Append(CharHelper.GetLineChar(0, LineBorder.Count, LineLabelSeparator.Count, LineBorder.Count) ?? '+'); }
                else { tl.Append(sep); }
            }
            else if (c > 1) { tl.Append(sep); }
            tl.Append("".PadRight(cellWidths[c], hrc));
        }

        // now the top right corner
        tl.Append(CharHelper.GetLineChar(0, 0, LineBorder.Count, LineBorder.Count));
        return tl.ToString();
    }

    private string GenerateBottomLine(List<int> cellWidths)
    {
        StringBuilder bl = new StringBuilder();
        int cellCount = cellWidths.Count;

        // bottom left corner
        bl.Append(CharHelper.GetLineChar(LineBorder.Count, LineBorder.Count, 0, 0));

        char sep = CharHelper.GetLineChar(LineCellSeparator.Count, LineBorder.Count, 0, LineBorder.Count) ?? '+';
        char hrc = CharHelper.GetLineChar(0, LineBorder.Count, 0, LineBorder.Count) ?? '-';

        for (int c = 0; c < cellCount; c++)
        {
            // if it's not the first one, we add the separator character before the horizontal line
            if (c == 1)
            {
                if (ShowLeftLabels) { bl.Append(CharHelper.GetLineChar(LineLabelSeparator.Count, LineBorder.Count, 0, LineBorder.Count) ?? '+'); }
                else { bl.Append(sep); }
            }
            else if (c > 1) { bl.Append(sep); }
            bl.Append("".PadRight(cellWidths[c], hrc));
        }

        // now the top right corner
        bl.Append(CharHelper.GetLineChar(LineBorder.Count, 0, 0, LineBorder.Count));
        return bl.ToString();
    }


    private string GenerateContentSeparatorLine(List<int> cellWidths)
    {
        StringBuilder result = new StringBuilder();
        char hr = CharHelper.GetLineChar(0, LineCellSeparator.Count, 0, LineCellSeparator.Count) ?? '-';
        // the first junction might be different than the others, so we have to do this stupid crap
        char junc1 = CharHelper.GetLineChar(LineLabelSeparator.Count, LineCellSeparator.Count, LineLabelSeparator.Count, LineCellSeparator.Count) ?? '+';
        char junc = CharHelper.GetLineChar(LineCellSeparator.Count, LineCellSeparator.Count, LineCellSeparator.Count, LineCellSeparator.Count) ?? '+';

        // left edge
        result.Append(CharHelper.GetLineChar(LineBorder.Count, LineCellSeparator.Count, LineBorder.Count, 0) ?? '|');


        for (int c = 0; c < cellWidths.Count; c++)
        {
            if (c == 1) { result.Append(junc1); }
            else if (c > 1) { result.Append(junc); }

            result.Append("".PadRight(cellWidths[c], hr));
        }

        // right edge
        result.Append(CharHelper.GetLineChar(LineBorder.Count, 0, LineBorder.Count, LineCellSeparator.Count) ?? '|');

        return result.ToString();
    }

    private string GenerateContentLine(List<int> cellWidths, int row)
    {
        var result = new StringBuilder();
        var cellContents = CellData[row];
        if (ShowLeftLabels) { cellContents = cellContents.Combine(LeftLabels[row], true).ToList(); }
        int cellCount = cellContents.Count;
        int colCount = cellWidths.Count;

        if (cellCount != colCount) { throw new Exception("table row contents don't match expectations"); }


        result.Append(CharHelper.GetLineChar(LineBorder.Count, 0, LineBorder.Count, 0) ?? '|');
        char cellSeparator = CharHelper.GetLineChar(LineCellSeparator.Count, 0, LineCellSeparator.Count, 0) ?? '|';

        for (int col = 0; col < colCount; col++)
        {
            if (col == 1)
            {
                if (ShowLeftLabels) { result.Append(CharHelper.GetLineChar(LineLabelSeparator.Count, 0, LineLabelSeparator.Count, 0) ?? '|'); }
                else { result.Append(cellSeparator); }
            }
            else if (col > 1) { result.Append(cellSeparator); }

            result.Append(cellContents[col].PadLeftIndent(cellWidths[col], 1, ' '));
        }

        result.Append(CharHelper.GetLineChar(LineBorder.Count, 0, LineBorder.Count, 0) ?? '|');



        return result.ToString();
    }


    private bool TryGet(int row, int col, [NotNullWhen(true)] out string? text)
    {
        // default cell contents
        text = null;

        // is there a row at that index?
        if (row < 0 || row > CellData.Count) { return false; }

        // now since there's a row, see if that particular row has a column at the index.
        if (col < 0 || col > CellData[row].Count) { return false; }

        // there's a row and column existing.  Time to return the contents...
        text = CellData[row][col];
        return true;
    }
}

public struct TextDisplayTableLine : IEquatable<TextDisplayTableLine>, IComparable<TextDisplayTableLine>
{
    public TextDisplayTableLine() { }

    public TextDisplayTableLine(int lineCount) { Count = lineCount; }

    private int _lineCount = 1;

    public int Count
    {
        get { return _lineCount; }
        set
        {
            if (value <= 0) { _lineCount = 0; }
            else if (value >= 2) { _lineCount = 2; }
            else { _lineCount = 1; }
        }
    }

    public static int NoLine { get; } = 0;
    public static int SingleLine { get; } = 1;
    public static int DoubleLine { get; } = 2;

    public int CompareTo(TextDisplayTableLine other)
    {
        return _lineCount.CompareTo(other._lineCount);
    }

    public bool Equals(TextDisplayTableLine other)
    {
        return _lineCount == other._lineCount;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is TextDisplayTableLine tdtlObj) { return Equals(tdtlObj); }

        if (obj is int iObj) { return iObj == _lineCount; }

        return false;
    }

    public override int GetHashCode()
    {
        return _lineCount;
    }

    public override string ToString()
    {
        switch (_lineCount)
        {
            case 0: return "No Line";
            case 1: return "Single Line";
            case 2: return "Double Line";
            default: return "Unknown";
        }
    }


    public static bool operator ==(TextDisplayTableLine a, TextDisplayTableLine b) { return a.Equals(b); }
    public static bool operator !=(TextDisplayTableLine a, TextDisplayTableLine b) { return !a.Equals(b); }

    public static bool operator ==(TextDisplayTableLine a, int b) { return a.Count == b; }
    public static bool operator !=(TextDisplayTableLine a, int b) { return a.Count == b; }

    public static bool operator ==(int a, TextDisplayTableLine b) { return b.Count == a; }
    public static bool operator !=(int a, TextDisplayTableLine b) { return b.Count != a; }


}
