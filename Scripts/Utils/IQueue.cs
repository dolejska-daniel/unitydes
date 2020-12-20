using System.Collections.Generic;

namespace UnityDES.Utils
{
    /// <summary>
    /// The mandatory interface of any queue implementation.
    /// </summary>
    /// 
    /// <typeparam name="TItem">Type of the items in the queue</typeparam>
    public interface IQueue<TItem, TKey> : IEnumerable<TItem>, IReadOnlyCollection<TItem>
        where TItem : IQueueItem<TKey>
    {
        /// <summary>
        /// Adds provided item (<paramref name="item"/>) to the queue.
        /// </summary>
        /// 
        /// <param name="item"></param>
        void Enqueue(TItem item);

        /// <summary>
        /// Removes an item from the front of the queue.
        /// Basically pop operation.
        /// </summary>
        /// 
        /// <returns>Item from the front of the queue</returns>
        TItem Dequeue();

        /// <summary>
        /// Returns the item from the front of the queue without removing it.
        /// </summary>
        /// 
        /// <returns>Item from the front of the queue</returns>
        TItem Peek();
    }
}
