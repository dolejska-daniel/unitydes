using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityDES.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Heap<TItem> : ICollection<TItem>, IEnumerable<TItem>
    {
        protected List<TItem> Items;

        protected Dictionary<TItem, int> ItemIndices;

        public int Count => Items.Count;

        public bool IsReadOnly => false;

        protected IComparer<TItem> Comparer { get; set; }

        /// <summary>
        /// Initializes new heapsorted list of items.
        /// </summary>
        /// 
        /// <param name="items">Initial items of the list</param>
        /// <param name="comparer">Instance of the item comparer</param>
        public Heap(IEnumerable<TItem> items = null, IComparer<TItem> comparer = null)
        {
            Items = new List<TItem>(items ?? new TItem[100]);
            ItemIndices = new Dictionary<TItem, int>(Items.Capacity);
            Comparer = (comparer ?? Comparer<TItem>.Default) ?? throw new ArgumentNullException(nameof(comparer));

            Heapify();
        }

        /// <summary>
        /// Calculates index of the parent node of given child node index (<paramref name="childIndex"/>).
        /// </summary>
        /// 
        /// <param name="childIndex">Index of the child node</param>
        /// <returns>Index of the parent node</returns>
        protected int ParentIndex(int childIndex) => (childIndex - 1) / 2;

        /// <summary>
        /// Calculates index of the left child node of given parent node index (<paramref name="parentIndex"/>).
        /// </summary>
        /// 
        /// <param name="parentIndex">Index of the parent node</param>
        /// <returns>Index of the child node</returns>
        protected int LeftChildIndex(int parentIndex) => 2 * parentIndex + 1;

        /// <summary>
        /// Calculates index of the right child node of given parent node index (<paramref name="parentIndex"/>).
        /// </summary>
        /// 
        /// <param name="parentIndex">Index of the parent node</param>
        /// <returns>Index of the child node</returns>
        protected int RightChildIndex(int parentIndex) => 2 * parentIndex + 2;

        /// <summary>
        /// Sorts the items on the heap.
        /// </summary>
        /// <remarks>
        /// This method expects heap to be correctly built already.
        /// The complexity of this method is O(n log n).
        /// This operation <b>will destroy</b> the heap structure.
        /// </remarks>
        public void Sort()
        {
            // sort the items
            var nodeIndexLast = Count - 1;
            while (nodeIndexLast > 0)
            {
                // move the greatest item to the back
                SwapNodes(0, nodeIndexLast);
                // promote new greatest item so far
                SiftDown(0, nodeIndexLast--);
            }
        }

        /// <summary>
        /// Rebuilds the tree structure of the heap.
        /// </summary>
        /// <remarks>
        /// The complexity of this method is O(n).
        /// </remarks>
        public void Heapify()
        {
            var nodeIndex = ParentIndex(Count - 1);
            while (nodeIndex >= 0)
                SiftDown(nodeIndex--);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeIndex"></param>
        protected void SiftDown(int nodeIndex) => SiftDown(nodeIndex, Count);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// The complexity of this method is O(log n).
        /// </remarks>
        /// 
        /// <param name="nodeIndex"></param>
        /// <param name="nodeCount"></param>
        protected void SiftDown(int nodeIndex, int nodeCount)
        {
            var nextNodeIndex = nodeIndex;

            var left = LeftChildIndex(nodeIndex);
            if (left < nodeCount && IsGreaterThan(left, nextNodeIndex))
            {
                // the value of the left subnode is greater than the value of the root
                nextNodeIndex = left;
            }

            var right = RightChildIndex(nodeIndex);
            if (right < nodeCount && IsGreaterThan(right, nextNodeIndex))
            {
                // the value of the right subnode is greater than both the value of the left subnode and the root
                nextNodeIndex = right;
            }

            if (nextNodeIndex != nodeIndex)
            {
                // swap the values - largest value will be promoted to root node
                SwapNodes(nodeIndex, nextNodeIndex);

                // continue in the modified subtree
                SiftDown(nextNodeIndex, nodeCount);
            }
        }

        protected void SiftUp(int nodeIndex)
        {
            if (nodeIndex == 0)
                return;

            var parentNodeIndex = ParentIndex(nodeIndex);
            if (IsGreaterThan(nodeIndex, parentNodeIndex))
            {
                SwapNodes(nodeIndex, parentNodeIndex);
                SiftUp(parentNodeIndex);
            }
        }

        public void Add(TItem item)
        {
            Items.Add(item);
            SiftUp(Count - 1);
        }

        public bool Update(TItem item)
        {
            var nodeIndex = IndexOf(item);
            if (nodeIndex == -1)
                return false;
            
            SiftUp(nodeIndex);
            SiftDown(nodeIndex);
            return true;
        }

        public bool Remove(TItem item)
        {
            var nodeIndex = IndexOf(item);
            if (nodeIndex == -1)
                return false;

            RemoveAt(nodeIndex);
            return true;
        }

        public void Clear() => Items.Clear();

        public bool Contains(TItem item) => ItemIndices.ContainsKey(item);

        public int IndexOf(TItem item) => Contains(item) ? ItemIndices[item] : -1;

        public void RemoveAt(int nodeIndex)
        {
            var lastIndex = Count - 1;
            SwapNodes(nodeIndex, lastIndex);

            var item = Items[lastIndex];
            ItemIndices.Remove(item);
            Items.RemoveAt(lastIndex);

            SiftDown(nodeIndex);
        }

        public TItem Peek() => Count > 0 ? Items[0] : default;

        public TItem PeekEnd() => Count > 0 ? Items[Count - 1] : default;

        public TItem ExtractTop()
        {
            if (Count == 0)
                throw new InvalidOperationException("Tried extracting top value on an empty heap.");

            var item = Items[0];
            RemoveAt(0);

            return item;
        }

        protected void SwapNodes(int indexA, int indexB)
        {
            var tmp = Items[indexA];

            Items[indexA] = Items[indexB];
            ItemIndices[Items[indexA]] = indexA;

            Items[indexB] = tmp;
            ItemIndices[Items[indexB]] = indexB;
        }

        protected abstract bool IsGreaterThan(int indexA, int indexB);

        public void CopyTo(TItem[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);

        public IEnumerator<TItem> GetEnumerator() => (IEnumerator<TItem>)Items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
    }
}
