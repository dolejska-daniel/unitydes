using System.Collections;
using System.Collections.Generic;

namespace UnityDES.Utils
{
    /// <summary>
    /// The queue in which the items are ordered by their keys.
    /// </summary>
    public class PriorityQueue<TItem, TKey> : IQueue<TItem, TKey>
        where TItem : IQueueItem<TKey>
    {
        /// <summary>
        /// The items in the queue.
        /// </summary>
        protected MinHeap<TItem> ItemHeap;

        /// <summary>
        /// Count of the items currently in the queue.
        /// </summary>
        public int Count => ItemHeap.Count;

        public PriorityQueue(IComparer<TItem> comparer = null)
        {
            ItemHeap = new MinHeap<TItem>(null, comparer);
        }

        public void Enqueue(TItem item) => ItemHeap.Add(item);

        public TItem Dequeue() => ItemHeap.ExtractTop();

        public TItem Peek() => ItemHeap.Peek();

        public IEnumerator<TItem> GetEnumerator() => ItemHeap.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ItemHeap.GetEnumerator();
    }
}
