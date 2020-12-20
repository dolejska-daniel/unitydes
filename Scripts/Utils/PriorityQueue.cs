using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace UnityDES.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class PriorityQueue<K, V> : IEnumerable<V>, IReadOnlyCollection<V>
        where K : IComparable<K>
        where V : class
    {
        protected SortedDictionary<K, LinkedList<V>> itemLists;

        public int Count => itemLists.Sum(entry => entry.Value.Count);

        public V Dequeue()
        {
            var entries = itemLists.First().Value;
            var entry = entries.First();

            entries.RemoveFirst();
            return entry;
        }

        public void Enqueue(K key, V item)
        {
            var entries = itemLists[key];
            if (entries == null)
                entries = itemLists[key] = new LinkedList<V>();

            entries.AddLast(item);
        }

        public V Peek() => itemLists.First().Value.First();

        public K PeekKey() => itemLists.First().Key;

        public IEnumerator<V> GetEnumerator() => (IEnumerator<V>)itemLists.GetEnumerator().Current.Value;

        IEnumerator IEnumerable.GetEnumerator() => itemLists.GetEnumerator();
    }
}
