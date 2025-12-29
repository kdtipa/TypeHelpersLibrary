using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomTypes;

/// <summary>
/// This class is essentially a convenient way to get your items 
/// out of the list in random order without you having to write 
/// code to do that.
/// </summary>
public class RandomList<T>
{
    public RandomList()
    {
        _list = new List<T>();
        _method = RandomListMethod.MoreRandom;
    }

    public RandomList(List<T> sourceList, RandomListMethod? RandomizationMethod = null)
    {
        _list = sourceList;

        if (RandomizationMethod is not null && RandomizationMethod.Value == RandomListMethod.Faster)
        {
            _method = RandomListMethod.Faster;
        }
        else
        {
            _method = RandomListMethod.MoreRandom;
        }
    }



    private readonly List<T> _list;
    private RandomListMethod _method;
    private int _pause = 19; // magic number to start off the better randomization that uses thread sleeps
    private const int _maxPause = 97; // also a magic number that acts as the cap for the pause so the thread sleeps don't take TOO long


    public IEnumerable<T> OriginalOrderItems()
    {
        foreach (var item in _list)
        {
            yield return item;
        }
    }

    public IEnumerable<T> RandomOrderItems()
    {
        // get the count because we might not need to do anything
        var sourceCount = _list.Count;

        // exit if it makes sense...
        if (sourceCount == 0) { yield break; }
        else if (sourceCount == 1) 
        {
            yield return _list[0];
            yield break;
        }

        // build a list of indices in random order
        var randomIndices = new List<int>();
        var insertIndex = new Random();
        for (var i = 0; i < sourceCount; i++)
        {
            var ii = insertIndex.Next(0, randomIndices.Count);
            randomIndices.Insert(ii, i);
            if (_method == RandomListMethod.MoreRandom) 
            {
                Thread.Sleep(_pause);
                _pause = ((_pause + ii) % _maxPause) + 2; // +2 to make sure we don't land on zero.
            }
        }

        // now return the items in that random order...
        foreach (var ri in randomIndices)
        {
            yield return _list[ri];
        }
    }



}


public enum RandomListMethod
{
    MoreRandom = 0, 
    Faster = 1
}
