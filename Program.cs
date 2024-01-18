// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;

var cacheComp = new UsedCache<string, int>(3);

cacheComp.evictEntry += (sender, evictedItem) =>
{
      Console.WriteLine($"Evicted : Key: {evictedItem.Item1}, Value: {evictedItem.Item2}");

};
Console.WriteLine("befofre Inserting");
cacheComp.Insert("key1", 1);
cacheComp.Insert("key2", 2);
cacheComp.Insert("key3", 3);
int val;
cacheComp.TryGetValue("key1", out val);

cacheComp.Insert("key4", 4);
cacheComp.TryGetValue("key2", out val);
cacheComp.Insert("key5", 5);



public class UsedCache<TKey, TValue>
{
    
    private readonly int _thresholdValue;
    private readonly Dictionary<TKey, LinkedListNode<(TKey, TValue)>> _cacheDict;
    private readonly LinkedList<(TKey, TValue)> _cacheUsedList;

    public event EventHandler<(TKey Key, TValue Value)> evictEntry;

    public UsedCache(int limit)
    {
        _thresholdValue = limit;
        _cacheDict = new Dictionary<TKey, LinkedListNode<(TKey, TValue)>>();
        _cacheUsedList = new LinkedList<(TKey, TValue)>();
    }

    public void Insert(TKey key, TValue value)
    {
        if(_cacheDict.TryGetValue(key, out LinkedListNode<(TKey, TValue)> node))
        {
            _cacheUsedList.Remove(node);
            _cacheUsedList.AddFirst(node);
            node.Value = (key, value);
        }
        else
        {
            if(_cacheDict.Count >= _thresholdValue)
            {
                removefromCache();
            }

            var newNode = new LinkedListNode<(TKey, TValue)>((key, value));
            _cacheUsedList.AddFirst(newNode);
            _cacheDict[key] = newNode;

        }
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if(_cacheDict.TryGetValue(key, out LinkedListNode<(TKey, TValue)> node))
        {
            value = node.Value.Item2;
            _cacheUsedList.Remove(node);
            _cacheUsedList.AddFirst(node);
            return true;

        }
        value = default;
        return false;

    }

    private void removefromCache()
    {
        LinkedListNode<(TKey, TValue)> last = _cacheUsedList.Last;
        _cacheUsedList.RemoveLast();
        _cacheDict.Remove(last.Value.Item1);
        evictEntry?.Invoke(this, last.Value);

    }
}

