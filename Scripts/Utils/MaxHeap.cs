using System.Collections.Generic;

namespace UnityDES.Utils
{
    /// <summary>
    /// Max version of heap structure (and sort).
    /// Topmost items on the heap will be items with the largest values.
    /// </summary>
    public class MaxHeap<V> : Heap<V>
    {
        public MaxHeap(IEnumerable<V> items = null, IComparer<V> comparer = null)
            : base(items, comparer)
        {
        }

        protected override bool ShouldBeParentOf(int indexA, int indexB)
        {
            return Comparer.Compare(Items[indexA], Items[indexB]) > 0;
        }
    }
}
