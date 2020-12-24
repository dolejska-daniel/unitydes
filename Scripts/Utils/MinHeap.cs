using System.Collections.Generic;

namespace UnityDES.Utils
{
    /// <summary>
    /// Min version of heap structure (and sort).
    /// Topmost items on the heap will be items with the smallest values.
    /// </summary>
    public class MinHeap<V> : Heap<V>
    {
        public MinHeap(IEnumerable<V> items = null, IComparer<V> comparer = null)
            : base(items, comparer)
        {
        }

        protected override bool ShouldBeParentOf(int indexA, int indexB)
        {
            return Comparer.Compare(Items[indexA], Items[indexB]) < 0;
        }
    }
}
