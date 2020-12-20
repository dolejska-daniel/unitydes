using System.Collections.Generic;

namespace UnityDES.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class MinHeap<V> : Heap<V>
    {
        public MinHeap(IEnumerable<V> items = null, IComparer<V> comparer = null) : base(items, comparer)
        {
        }

        protected override bool IsGreaterThan(int indexA, int indexB)
        {
            return Comparer.Compare(Items[indexA], Items[indexB]) < 0;
        }
    }
}
