using System.Collections;
using System.Collections.Generic;

namespace UnityDES.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class PriorityQueue<TItem> : IQueue<TItem, int>
        where TItem : IQueueItem<int>
    {
        /// <summary>
        /// The items in the queue.
        /// </summary>
        protected MinHeap<TItem> items;

        /// <summary>
        /// Count of the items currently in the queue.
        /// </summary>
        public int Count => items.Count;

        public void Enqueue(TItem item) => items.Add(item);

        public TItem Dequeue() => items.ExtractTop();

        public TItem Peek() => items.Peek();

        public IEnumerator<TItem> GetEnumerator() => items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();
    }
}
