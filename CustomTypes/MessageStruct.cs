using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTypes;

/// <summary>
/// Meant to help in situations where you don't know if you will need only one 
/// message string or a list of them.  Provides some convenience methods and a 
/// convenience property for the first message.  Equals, ==, and != are defined, 
/// though only the Equals method can allow for case insensitive comparison.  The 
/// most likely use case would be as part of a command return value when you need 
/// to pass messages for how the command performed.  This struct also defines the 
/// enumerator so it can be used in a foreach loop.
/// </summary>
public struct MessageStruct : IEnumerable<string>, IEquatable<MessageStruct>
{
    public MessageStruct() { }

    public MessageStruct(string initialMessage)
    {
        _messages.Add(initialMessage);
    }

    public MessageStruct(params string[] initialMessages)
    {
        _messages.AddRange(initialMessages);
    }

    public MessageStruct(List<string> initialMessages)
    {
        _messages.AddRange(initialMessages);
    }

    private List<string> _messages = new();


    /// <summary>
    /// This is specifically the message at index 0 in the list.  If the list is 
    /// empty, it will return an empty string.  If you set this property, it will 
    /// insert the value at index 0, moving existing messages down the list.
    /// </summary>
    public string Message
    {
        get 
        { 
            if (_messages.Count == 0) { return string.Empty; }
            else { return _messages[0]; }
        }
        set
        {
            if (_messages.Count == 0) { _messages.Add(value); }
            else { _messages.Insert(0, value); }
        }
    }

    /// <summary>
    /// How many items are in the message list.
    /// </summary>
    public int Count { get { return _messages.Count; } }

    /// <summary>
    /// Accessing by index allows you to walk the list and set the value at a given 
    /// index.  The one special case is that if you set the value at the index that 
    /// matches count of the list, it will add that value to the end of the list.
    /// </summary>
    public string this[int index]
    {
        get 
        { 
            int msgCount = _messages.Count;
            if (msgCount == 0) { throw new InvalidOperationException("This MessageStruct is empty, so no index is valid."); }
            if (index < 0 || index >= msgCount) { throw new IndexOutOfRangeException($"Index for this MessageStruct must be 0 to {msgCount} but received index {index}"); }
            else { return _messages[index]; }
        }
        set
        {
            if (index < 0) { throw new IndexOutOfRangeException($"index must be 0 or above but received {index}"); }
            else if (index > _messages.Count) { throw new IndexOutOfRangeException($"index must be 0 to {_messages.Count} but received {index}"); }
            else if (index == _messages.Count) { _messages.Add(value); }
            else { _messages[index] = value; }
        }
    }

    public IEnumerator<string> GetEnumerator()
    {
        foreach (var msg in _messages) { yield return msg; }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Adds a message to the end of the list.
    /// </summary>
    /// <param name="msg"></param>
    public void Add(string msg)
    {
        _messages.Add(msg);
    }

    /// <summary>
    /// Adds a range of messages to the end of the list.
    /// </summary>
    /// <param name="msgs"></param>
    public void AddRange(List<string> msgs)
    {
        _messages.AddRange(msgs);
    }

    /// <summary>
    /// Adds a range of messages to the end of the list.
    /// </summary>
    /// <param name="msgs"></param>
    public void AddRange(params string[] msgs)
    {
        _messages.AddRange(msgs);
    }

    /// <summary>
    /// PUts the new message at the given index and moves the existing 
    /// messages to indexes 1 higher than before.
    /// </summary>
    public void Insert(int index, string msg)
    {
        if (index < 0 || index > _messages.Count) { throw new IndexOutOfRangeException($"index must be 0 to {_messages.Count} but received {index}"); }
        else { _messages.Insert(index, msg); }
    }

    /// <summary>
    /// If an exact match exists, removes the first occurance and returns true.
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    public bool Remove(string msg)
    {
        return _messages.Remove(msg);
    }

    /// <summary>
    /// If the index exists, removes the message at that index and returns true.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool RemoveAt(int index)
    {
        if (index < 0 || index >= _messages.Count) { return false; }
        _messages.RemoveAt(index);
        return true;
    }

    public bool Contains(string searchFor, StringComparison? comparison = null)
    {
        StringComparison cmp = comparison ?? StringComparison.Ordinal;
        return _messages.Any(m => m.Equals(searchFor, cmp));
    }

    public void Clear()
    {
        _messages.Clear();
    }

    public bool Equals(MessageStruct other)
    {
        if (Count != other.Count) { return false; }

        int msgCount = Count;
        for (int m = 0; m < msgCount; m++)
        {
            if (!string.Equals(_messages[m], other._messages[m])) { return false; }
        }

        return true;
    }

    public bool Equals(MessageStruct other, StringComparison comparison)
    {
        if (Count != other.Count) { return false; }

        int msgCount = Count;
        for (int m = 0; m < msgCount; m++)
        {
            if (!string.Equals(_messages[m], other._messages[m], comparison)) { return false; }
        }

        return true;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) { return false; }
        if (obj is MessageStruct msObj) { return Equals(msObj); }

        if (obj is List<string> listObj)
        {
            var lComp = new MessageStruct(listObj);
            return Equals(lComp);
        }

        if (obj is string[] arrObj)
        {
            var aComp = new MessageStruct(arrObj);
            return Equals(aComp);
        }

        return false;
    }

    public bool Equals(object obj, StringComparison comparison)
    {
        if (obj is MessageStruct msObj) { return Equals(other:msObj, comparison); }

        if (obj is List<string> listObj)
        {
            var lComp = new MessageStruct(listObj);
            return Equals(other:lComp, comparison);
        }

        if (obj is string[] arrObj)
        {
            var aComp = new MessageStruct(arrObj);
            return Equals(other:aComp, comparison);
        }

        return false;
    }

    /// <summary>
    /// Returns the hash code for the first message in the list, or 0 if the list is empty, 
    /// mostly useful for grouping since it doesn't account fo anything but the first item.
    /// </summary>
    public override int GetHashCode()
    {
        if (Count == 0) { return 0; }

        return _messages[0].GetHashCode();
    }

    /// <summary>
    /// If there's only one message, it returns that message.  Multiple messages are returned 
    /// as a dash-bullet list with items on separate lines.
    /// </summary>
    public override string ToString()
    {
        if (Count == 0) { return string.Empty; }
        else if (Count == 1) { return _messages[0]; }
        else
        {
            StringBuilder sb = new StringBuilder();
            for (int m = 0; m < Count; m++)
            {
                sb.AppendLine($"- {_messages[m]}");
            }
            return sb.ToString();
        }
    }


    public static bool operator ==(MessageStruct left, MessageStruct right) { return left.Equals(right); }
    public static bool operator !=(MessageStruct left, MessageStruct right) { return !left.Equals(right); }
    public static bool operator ==(MessageStruct left, List<string> right) { return left.Equals(right); }
    public static bool operator !=(MessageStruct left, List<string> right) { return !left.Equals(right); }
    public static bool operator ==(MessageStruct left, string[] right) { return left.Equals(right); }
    public static bool operator !=(MessageStruct left, string[] right) { return !left.Equals(right); }





}
