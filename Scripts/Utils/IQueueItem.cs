using System.Collections.Generic;

namespace UnityDES.Utils
{
    /// <summary>
    /// The mandatory interface of any queueable item.
    /// </summary>
    /// 
    /// <typeparam name="TKey">Type of the key of the item</typeparam>
    public interface IQueueItem<TKey>
    {
        /// <summary>
        /// The key of the item in the queue.
        /// This property will determine the item's position within the queue.
        /// </summary>
        TKey QueueKey { get; }
    }
}
