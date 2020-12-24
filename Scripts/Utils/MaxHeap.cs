using System.Collections.Generic;

namespace UnityDES.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class MaxHeap<V> : Heap<V>
    {
        public MaxHeap(IEnumerable<V> items = null, IComparer<V> comparer = null)
            : base(items, comparer)
        {
        }

        protected override bool IsGreaterThan(int indexA, int indexB)
        {
            return Comparer.Compare(Items[indexA], Items[indexB]) > 0;
        }
    }
}
