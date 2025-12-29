using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTypes;

public class SpreadSheetTable<T> where T : struct, IEquatable<T>
{

    private int _colCount = 0;
    private int _rowCount = 0;

    private List<string> _colLabels = new();
    private List<string> _rowLabels = new();

    private List<List<T>> _values = new();

    public int RowCount { get { return _rowCount; } }

    public int ColCount { get { return _colCount; } }

    public void AddRow(string label, List<T> values)
    {
        _rowLabels.Add(label);
        _values.Add(values);

        if (_rowLabels.Count > _rowCount) { _rowCount = _rowLabels.Count; }
        if (values.Count > _colCount) { _colCount = values.Count; }
    }

    private void UpdateCounts()
    {
        int valRowCount = _values.Count;
        int labelRowCount = _rowLabels.Count;

        // if there are not rows in the values, columns is zero technically
    }

    /// <summary>
    /// Be careful with the indexer... it's not written with checks 
    /// to keep from trying to access something that doesn't exist.
    /// </summary>
    /// <param name="row">Which row of the table by index?</param>
    /// <param name="col">Which column of the row by index?</param>
    /// <returns>The value at that location</returns>
    public T this[int row, int col]
    {
        get { return _values[row][col]; }
        set { _values[row][col] = value; }
    }

    private bool _padCells = true;

    /// <summary>
    /// only run this once, after the values are all set, if possible.  
    /// Lots of string operations are possible, so don't do this often.
    /// </summary>
    /// <returns></returns>
    private int getMaxValueLength()
    {
        if (_values.Count == 0) { return 0; }

        int foundCount = 0;
        foreach (List<T> row in _values)
        {
            foreach (T val in row)
            {
                var stringVal = val.ToString();
                if (!string.IsNullOrEmpty(stringVal) && stringVal.Length > foundCount) { foundCount = stringVal.Length; }
            }
        }

        return foundCount;
    }




}
