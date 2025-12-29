using CustomTypes.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTypes;

public class DataTree<T> where T : IEquatable<T>, new()
{
    public DataTree() { }

    public DataTree(List<DataTreeNode<T>> initialNodes)
    {
        foreach (var node in initialNodes) { _nodes.Add(node); }
        RefreshParentIndexes();
    }

    private List<DataTreeNode<T>> _nodes = new();

    private bool _parentIndexesAreCurrent = false;

    private void RefreshParentIndexes()
    {
        int count = Count;
        for (int i = 0; i < count; i++)
        {
            if (string.IsNullOrWhiteSpace(_nodes[i].ParentID))
            {
                _nodes[i].SetParentIndex(-1);
                continue;
            }

            _nodes[i].SetParentIndex(IndexByItemID(_nodes[i].ParentID));
        }
        _parentIndexesAreCurrent = true;
    }

    private int IndexByItemID(string itemID)
    {
        int count = Count;
        for (int i = 0; i < count; i++)
        {
            if (string.Equals(itemID, _nodes[i].ItemID)) { return i; }
        }
        return -1;
    }

    private int IndexByValue(T value)
    {
        int count = Count;
        for (int i = 0; i < count; i++)
        {
            if (_nodes[i].Value.Equals(value)) { return i; }
        }
        return -1;
    }

    private bool _healthyTree = true;
    private bool _healthyIsCurrent = true;

    public bool HealthyTree
    {
        get
        {
            RefreshHealthyTree();
            return _healthyTree;
        }
    }

    private void RefreshHealthyTree()
    {
        if (_healthyIsCurrent) return;
        RefreshParentIndexes(); // need to make sure the parent indexes are set right for this to work

        List<int> visitedIndexes = new List<int>();

        for (int i = 0; i < Count; i++)
        {
            visitedIndexes.Clear();
            int walker = i;

            while (walker != -1)
            {
                if (visitedIndexes.Contains(walker))
                {
                    // this is the case where we have a circular path... very bad
                    _healthyTree = false;
                    _healthyIsCurrent = true;
                    return;
                }

                visitedIndexes.Add(walker);
                walker = _nodes[walker].ParentIndex;
            }
        }

        _healthyTree = true;
        _healthyIsCurrent = true;
    }

    public IEnumerable<T> GetValueCollection()
    {
        foreach (var item in _nodes)
        {
            yield return item.Value;
        }
    }

    public IEnumerable<DataTreeNode<T>> GetNodeCollection()
    {
        foreach (var item in _nodes)
        {
            yield return item;
        }
    }

    /// <summary>
    /// Likely the reason for using this class, this method returns data that 
    /// has a tree structure in the order it would be displayed linearly like 
    /// in a folder tree for folders and files displayed in a file explorer.  
    /// The type for the enumeration includes information about what lines 
    /// would be needed drawn before the text.
    /// </summary>
    public IEnumerable<DataTreeDisplayInfo<T>> GetTextDisplayCollection()
    {
        // zero nodes means zero return
        if (_nodes.Count == 0) { yield break; }

        // one node means we don't need to sort at all
        if (_nodes.Count == 1) { yield return new DataTreeDisplayInfo<T>() { Value = _nodes[0].Value }; yield break; }

        // has an internal check to prevent unnecessary runs
        RefreshParentIndexes();

        // now we begin the recursion.  Anything with the parent index of -1 is a root, and we have no levels yet
        var orderedList = GetChildrenRecursively(-1, 0);

        // now we return the results...
        foreach (var item in orderedList) { yield return item; }
        
    }

    private List<DataTreeDisplayInfo<T>> GetChildrenRecursively(int parentIndex, int parentLevel)
    {
        var resultList = new List<DataTreeDisplayInfo<T>>();
        var childList = _nodes.Where(n => n.ParentIndex == parentIndex).OrderBy(n => n.SiblingSortValue).ToList();
        int childCount = childList.Count;

        
        for (int i = 0; i < childCount; i++)
        {
            // get our child
            var child = childList[i];

            // add the child to the result
            resultList.Add(new DataTreeDisplayInfo<T>() { Value = child.Value, TreeDisplayLines = BuildLineList(parentLevel, i == childCount - 1) });

            // now recursively add this child's children
            resultList.AddRange(GetChildrenRecursively(IndexByValue(child.Value), parentLevel + 1));
        }

        return resultList;
    }

    private List<DataTreeLineInfo> BuildLineList(int layersBefore, bool elbow)
    {
        List<DataTreeLineInfo> result = new();
        for (int l = 0; l < layersBefore; l++)
        {
            result.Add(DataTreeLineInfo.CreateVerticalLine());
        }

        if (elbow) { result.Add(DataTreeLineInfo.CreateElbow()); }
        else { result.Add(DataTreeLineInfo.CreateVerticalWithBranch()); }

        return result;
    }

    private bool IsRootNodeIndex(int index)
    {
        // if the parent ID is blank, it's a root
        if (string.IsNullOrWhiteSpace(_nodes[index].ParentID)) { return true; }

        // this checks so won't do it multiple times if it's already in order
        RefreshParentIndexes();

        // now the indexes are up to date so if it's a -1, it's a root
        return _nodes[index].ParentIndex == -1;

    }
    

    public int Count { get { return _nodes.Count; } }

    public bool Contains(T item) { return _nodes.Any(n => n.Value.Equals(item)); }

    public bool ContainsSpecificNode(DataTreeNode<T> item) { return _nodes.Any(n => n.Equals(item)); }

    /// <summary>
    /// In order to build the tree structure, we need some extra values provided that 
    /// our code can't know how to get from T, assuming it even has that information.  
    /// When putting data from your database query into this structure, provide the 
    /// item, its ID, the ID of the record that is considered its parent if it has 
    /// one, and the sibling sort value which tells us how to sort nodes under the 
    /// same parent node.
    /// </summary>
    /// <param name="item">The actual item you want to store and access</param>
    /// <param name="itemID">The unique ID of the record</param>
    /// <param name="siblingSortValue">A value to decide sort order of nodes under the same parent</param>
    /// <param name="parentID">The parent ID if it has a parent.  When null or empty, it is automatically a root node.</param>
    /// <returns>True if the node was added.  Will fail if a node with the same item ID is already there.</returns>
    public bool Add(T item, string itemID, string siblingSortValue, string? parentID = null)
    {
        var node = new DataTreeNode<T>(item, itemID, siblingSortValue, parentID);

        if (ContainsSpecificNode(node)) { return false; }

        _nodes.Add(node);
        _parentIndexesAreCurrent = false;
        _healthyIsCurrent = false;
        return true;
    }

    /// <summary>
    /// This version of add includes a check to be sure the new node wouldn't cause 
    /// a circular reference.  The normal add doesn't do this which makes it better 
    /// in high volume adding and where you have some assurance the data is clean.  
    /// This can help you build clean data, but is very slow when the structure has 
    /// a lot of nodes because checking for circular references is an order of 
    /// N-squared operation.
    /// </summary>
    /// <param name="item">The actual item you want to store and access</param>
    /// <param name="itemID">The unique ID of the record</param>
    /// <param name="siblingSortValue">A value to decide sort order of nodes under the same parent</param>
    /// <param name="parentID">The parent ID if it has a parent.  When null or empty, it is automatically a root node.</param>
    /// <returns>True if the node was added.  False if the node already exists or causes a circular reference.</returns>
    public bool SafeAdd(T item, string itemID, string siblingSortValue, string? parentID = null)
    {
        // need a variable representation of the parameters that can be added
        var node = new DataTreeNode<T>(item, itemID, siblingSortValue, parentID);

        // if the node exists in the list already, just leave
        if (ContainsSpecificNode(node)) { return false; }

        // here's the check for if the new node would cause a circular reference
        if (!SafeToAdd(node, out bool _, out bool? _)) { return false; }

        // if we got here, it's okay to add the node
        _nodes.Add(node);
        _parentIndexesAreCurrent = false;
        _healthyIsCurrent = false;
        return true;
    }

    /// <summary>
    /// A way to check to see if the inclusion of a node in the tree would 
    /// be a problem.  It's fast if the node already exists in the tree 
    /// because that check is order of N.  But if it doesn't already exist, 
    /// the method to do the order of N-squared operation to test for a 
    /// circular reference and has to use string equality in the checks for 
    /// IDs because this class uses strings to accomodate ints, GUIDs, and 
    /// literally any type of ID.  This is a beast over an operation.
    /// </summary>
    /// <param name="node">The fully filled out node</param>
    /// <param name="alreadyExists">Tells you if it is not safe to add because it already exists</param>
    /// <param name="causesCircularReference">
    /// Tells you if adding would fail because of a circular reference.  It comes back as 
    /// null to indicate unknown (since the method will return if the node already exists).  
    /// Otherwise it will return true or false as a specific answer.
    /// </param>
    /// <returns>True if the node can be added safely</returns>
    public bool SafeToAdd(DataTreeNode<T> node, out bool alreadyExists, out bool? causesCircularReference)
    {
        alreadyExists = ContainsSpecificNode(node);
        causesCircularReference = null;

        if (alreadyExists) { return false; }

        int count = Count;
        for (int i = 0; i < count; i++)
        {
            List<string> visitedParentIDs = new();
            string walker = _nodes[i].ParentID;
            bool foundRoot = false;

            while (!foundRoot)
            {
                // this is the case where we have a circular path... very bad
                if (visitedParentIDs.Contains(walker)) { return false; }

                // not circular yet, so add to visited list
                visitedParentIDs.Add(walker);

                int walkerIndex = IndexByItemID(walker);
                if (walkerIndex != -1) { walker = _nodes[walkerIndex].ParentID; }
                else if (string.Equals(node.ItemID, walker)) { walker = node.ParentID; }
                else { foundRoot = true; }
            }
        }

        return true;
    }

    public bool Remove(T item)
    {
        int targetIndex = IndexByValue(item);
        if (targetIndex >= 0 && targetIndex < Count)
        {
            _nodes.RemoveAt(targetIndex);
            return true;
        }
        return false;
    }
    
}

public struct DataTreeNode<T> : IEquatable<DataTreeNode<T>> where T : IEquatable<T>, new()
{
    public DataTreeNode() { }

    public DataTreeNode(T value, string itemID, string siblingSortValue, string? parentID = null)
    {
        Value = value;
        ItemID = itemID;
        SiblingSortValue = siblingSortValue;
        ParentID = parentID ?? string.Empty;
    }

    public void Set(T value, string itemID, string siblingSortValue, string? parentID = null)
    {
        Value = value;
        ItemID = itemID;
        SiblingSortValue = siblingSortValue;
        ParentID = parentID ?? string.Empty;
    }

    public void SetValue(T value) { Value = value; }
    public void SetItemID(string itemID) { ItemID = itemID; }
    public void SetParentID(string parentID) { ParentID = parentID; }
    public void SetSortValue(string siblingSortValue) { SiblingSortValue = siblingSortValue; }

    public void SetParentIndex(int index) { ParentIndex = index; }


    /// <summary>
    /// The actual value of the node
    /// </summary>
    public T Value { get; private set; } = new();

    /// <summary>
    /// The unique ID for this node.  It is required for the DataTree to function 
    /// correctly.  It is required so that parent ID can be used to determine the 
    /// tree relationships correctly.  It is a string because that way we can 
    /// accomodate integers, GUIDs, or whatever form the ID takes in the database 
    /// we're getting our information from.
    /// </summary>
    public string ItemID { get; private set; } = string.Empty;

    /// <summary>
    /// The ID of the node that is the parent of this node.  If left empty, it 
    /// means that in this collection of nodes, this is one of the roots as it 
    /// has no parent.  But it should be set even if the parent doesn't exist 
    /// yet in the collection because if you add the parent, the tree can then 
    /// recognize the relationship.  If the ParentID is set and the parent node 
    /// does NOT exist in the collection, this node will be treated as root level.
    /// </summary>
    public string ParentID { get; private set; } = string.Empty;

    /// <summary>
    /// This is the string that determines which siblings come before the others 
    /// when sorting.  If you have Hector, Gabriel, Arthur, and Melody as children 
    /// of Isaac, we know Isaac come first when getting the collection iteratively 
    /// (like if you want to display your tree), but need to know that our type T 
    /// generic object has the names of the kids that they do.  You can choose whatever 
    /// string you want as the sort value.
    /// </summary>
    public string SiblingSortValue { get; private set; } = string.Empty;

    /// <summary>
    /// You shouldn't fiddle with this property.  It's here for the Tree object to 
    /// store which index the parent is stored in to make traversal quick.
    /// </summary>
    public int ParentIndex { get; set; } = -1;

    public bool Equals(DataTreeNode<T> other)
    {
        return string.Equals(ItemID, other.ItemID);
    }

    public bool Equals(DataTreeNode<T> other, bool insistOnValue, bool insistOnID, bool insistOnParentID, bool insistOnSiblingSort)
    {
        if (insistOnValue && !Value.Equals(other.Value)) { return false; }
        if (insistOnID && !string.Equals(ItemID, other.ItemID)) { return false; }
        if (insistOnParentID && !string.Equals(ParentID, other.ParentID)) { return false; }
        if (insistOnSiblingSort && !string.Equals(SiblingSortValue,other.SiblingSortValue)) { return false; }

        return true;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) { return false; }

        if (obj is DataTreeNode<T> dtnObj) { return Equals(dtnObj); }

        if (obj is T tObj) { return tObj.Equals(Value); }

        return false;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    /// <summary>
    /// just returns the Sibling Sort Item since  
    /// that's our representation of this node.
    /// </summary>
    public override string ToString()
    {
        return SiblingSortValue;
    }

    /// <summary>
    /// Allows you to get more specific about what you want returned.  You 
    /// pass in a template for what you want the output to look like with 
    /// tokens denoted by a tilde [~] to specify when you want one of the 
    /// properties from this node.  If the token is not recognized, the 
    /// character after the tilde is just included, so if you want a tilde 
    /// in your result use "~~".
    /// </summary>
    /// <param name="resultTemplate">
    /// Use tilde [~] to mark the tokens.  The tokens are 
    /// NOT case-sensitive.  For the node Item, use V; 
    /// ItemID is I; ParentID is P; and SiblingSortValue 
    /// is S.  For example "[~I]: ~S" might return something 
    /// like "[457]: Wilson, John".
    /// </param>
    /// <returns>
    /// Uses your template and replaces any tokens found with 
    /// the value from this node and returns the result.
    /// </returns>
    public string ToString(string resultTemplate)
    {
        var result = new StringBuilder();
        bool foundToken = false;

        foreach (char c in resultTemplate)
        {
            if (foundToken)
            {
                switch (c)
                {
                    case 'V':
                    case 'v':
                        result.Append(Value.ToString());
                        break;
                    case 'I':
                    case 'i':
                        result.Append(ItemID);
                        break;
                    case 'P':
                    case 'p':
                        result.Append(ParentID);
                        break;
                    case 'S':
                    case 's':
                        result.Append(SiblingSortValue);
                        break;
                    default:
                        result.Append(c);
                        break;
                }
                foundToken = false;
            }
            else if (c == '~')
            {
                foundToken = true;
            }
            else
            {
                 result.Append(c);
            }
        }

        return result.ToString();
    }




    public static bool operator ==(DataTreeNode<T> left, DataTreeNode<T> right) { return left.Equals(right); }
    public static bool operator !=(DataTreeNode<T> left, DataTreeNode<T> right) { return !left.Equals(right); }
}


public struct DataTreeDisplayInfo<T> : IEquatable<DataTreeDisplayInfo<T>> where T : IEquatable<T>, new()
{
    public DataTreeDisplayInfo() { }


    public T Value { get; set; } = new();

    public List<DataTreeLineInfo> TreeDisplayLines { get; set; } = new();

    public string GetTreeDisplayLinesString()
    {
        var result = new StringBuilder();
        foreach (var line in TreeDisplayLines)
        {
            char c = line.GetBoxLineCharacter();
            result.Append(c);
        }
        return result.ToString();
    }

    public bool Equals(DataTreeDisplayInfo<T> other)
    {
        return TreeDisplayLines.SequenceEqual(other.TreeDisplayLines) && Value.Equals(other.Value);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is DataTreeDisplayInfo<T> dtdiObj) { return Equals(dtdiObj); }

        return false;
    }

    public override int GetHashCode() { return Value.GetHashCode(); }

    public override string ToString()
    {
        var result = new StringBuilder();
        result.Append(GetTreeDisplayLinesString());
        if (TreeDisplayLines.Count > 0) { result.Append(' '); }
        result.Append(Value.ToString());
        return result.ToString();
    }


}


public struct DataTreeLineInfo : IEquatable<DataTreeLineInfo>
{
    public DataTreeLineInfo() { }

    public DataTreeLineInfo(bool needUp, bool needRight, bool needDown, bool needLeft)
    {
        NeedDown = needDown;
        NeedLeft = needLeft;
        NeedRight = needRight;
        NeedUp = needUp;
    }

    public static DataTreeLineInfo CreateVerticalLine() { return new DataTreeLineInfo(true, false, true, false); }
    public static DataTreeLineInfo CreateVerticalWithBranch() { return new DataTreeLineInfo(true, true, true, false); }
    public static DataTreeLineInfo CreateElbow() { return new DataTreeLineInfo(true, true, false, false); }


    public bool NeedUp { get; set; } = false;
    public bool NeedDown { get; set; } = false;
    public bool NeedLeft { get; set; } = false;
    public bool NeedRight { get; set; } = false;

    /// <summary>
    /// Gets you the character to use in a text based UI to 
    /// draw the correct line character: │, ├, and so on. 
    /// Returns a space if all the properties are false.  And 
    /// in this context, we'll likely never need a left line
    /// </summary>
    /// <returns></returns>
    public char GetBoxLineCharacter()
    {
        return CharHelper.GetLineChar(
            NeedUp ? 1 : 0,
            NeedRight ? 1 : 0,
            NeedDown ? 1 : 0,
            NeedLeft ? 1 : 0) ?? ' ';
    }

    
    public bool IsVerticalLine { get { return NeedUp && NeedDown && !NeedLeft && !NeedRight; } }
    public bool IsVerticalWithBranch { get { return NeedUp && NeedDown && !NeedLeft && NeedRight; } }
    public bool IsElbow { get { return NeedUp && !NeedDown && !NeedLeft && NeedRight; } }


    public bool Equals(DataTreeLineInfo other)
    {
        return NeedUp == other.NeedUp && NeedDown == other.NeedDown && NeedLeft == other.NeedLeft && NeedRight == other.NeedRight;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is DataTreeLineInfo dtliObj) { return Equals(dtliObj); }

        return false;
    }

    public override int GetHashCode()
    {
        return 40000 + (NeedUp ? 1000 : 0) + (NeedRight ? 100 : 0) + (NeedDown ? 10 : 0) + (NeedLeft ? 1 : 0);
    }

    public override string ToString()
    {
        return GetBoxLineCharacter().ToString();
    }

    public static bool operator ==(DataTreeLineInfo a, DataTreeLineInfo b) { return a.Equals(b); }
    public static bool operator !=(DataTreeLineInfo a, DataTreeLineInfo b) { return !a.Equals(b); }



}