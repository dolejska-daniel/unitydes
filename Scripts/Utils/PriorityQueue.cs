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

        public bool Dequeue(TItem item) => ItemHeap.Remove(item);

        public TItem Dequeue() => ItemHeap.ExtractTop();

        /// <summary>
        /// Updates the position of the provided item (<paramref name="item"/>) within the queue.
        /// </summary>
        /// 
        /// <param name="item">Item to be updated</param>
        /// <returns><c>True</c> if the update of the queue has been successful, <c>False</c> otherwise</returns>
        public bool Update(TItem item) => ItemHeap.Update(item);

        /// <summary>
        /// Checks whether the provided item (<paramref name="item"/>) is in the queue.
        /// </summary>
        /// 
        /// <param name="item">Item to be checked</param>
        /// <returns><c>True</c> if the item is in the queue, <c>False</c> otherwise</returns>
        public bool Queued(TItem item) => ItemHeap.Contains(item);

        public TItem Peek() => ItemHeap.Peek();

        public IEnumerator<TItem> GetEnumerator() => ItemHeap.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ItemHeap.GetEnumerator();
    }
}
