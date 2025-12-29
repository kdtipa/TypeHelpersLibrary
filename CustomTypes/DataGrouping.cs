using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTypes;

public struct DataGrouping<T> where T : IComparable<T>, IEquatable<T>
{
    public DataGrouping() { }

    public DataGrouping(params T[] initialDataSources)
    {
        foreach (var ids in initialDataSources)
        {
            AddNode(ids);
        }
    }

    private List<DataNode<T>> _nodes = new();
    public int Count { get; private set; } = 0;

    /// <summary>
    /// If the data already exists, it bumps up the count of that node.  
    /// If not, it adds the data to the collection at a count of 1.
    /// </summary>
    public void AddNode(T node)
    {
        AddNode(new DataNode<T>(node, 1));
    }

    /// <summary>
    /// If the data in your provided node is already in the collection, 
    /// it will add the counts together.  If the data doesn't exist in 
    /// the collection yet, it will simply add your node with count it 
    /// has.
    /// </summary>
    public void AddNode(DataNode<T> node)
    {
        for (int n = 0; n < Count; n++)
        {
            if (_nodes[n].Data.Equals(node.Data))
            {
                _nodes[n].AdjustCount(node.Count);
                return;
            }
        }

        _nodes.Add(node);
        Count += 1;
    }


    public void RemoveNode(T node)
    {
        _nodes.RemoveAll(n => n.Data.Equals(node));
        Count = _nodes.Count;
    }

    public void RemoveNode(DataNode<T> node)
    {
        RemoveNode(node.Data);
    }

    public DataNode<T> this[T n]
    {
        get { return _nodes.Where(item => item.Data.Equals(n)).FirstOrDefault(); }
    }
}
