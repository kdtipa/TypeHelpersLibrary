using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTypes;

public struct DataNode<T> where T : IComparable<T>, IEquatable<T>
{
    public DataNode(T sourceData, int? initialCount = null)
    {
        Count = initialCount is null || initialCount < 0 ? 1 : initialCount.Value;
        Data = sourceData;
    }

    public T Data { get; }

    public int Count { get; private set; } = 1;

    public void AdjustCount(int adjustmentAmount = 1)
    {
        Count += adjustmentAmount;
        if (Count < 0) { Count = 0; }
    }

    public void SetCount(int newCount)
    {
        Count = newCount;
        if (Count < 0) { Count = 0; }
    }


}
