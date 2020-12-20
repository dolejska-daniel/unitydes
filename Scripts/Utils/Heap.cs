using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityDES.Utils
{
    /// <summary>
    /// The base implementation of the heap.
    /// The order of the items on the heap must be defined in derived class.
    /// </summary>
    public abstract class Heap<TItem> : ICollection<TItem>, IEnumerable<TItem>
    {
        /// <summary>
        /// The items on the heap.
        /// </summary>
        protected List<TItem> Items;

        /// <summary>
        /// Item to index mapping.
        /// Allows constant time item lookup and existence checking.
        /// </summary>
        protected Dictionary<TItem, int> ItemIndices;

        /// <summary>
        /// Comparer of the items in the queue.
        /// </summary>
        protected IComparer<TItem> Comparer { get; set; }

        public bool IsReadOnly => false;

        /// <summary>
        /// Count of the items currently on the heap.
        /// </summary>
        public int Count => Items.Count;

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
                // move the highest item to the back
                SwapNodes(0, nodeIndexLast);
                // promote new highest item so far
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
        /// Promotes highest value from the children to the provided parent node (<paramref name="nodeIndex"/>), if greater than the parent's value.
        /// Works down recursively - from parent node to its children and so on.
        /// </summary>
        /// <remarks>
        /// Will run until the end of the array or is unable to swap values.
        /// The complexity of this method is O(log n).
        /// </remarks>
        /// 
        /// <param name="nodeIndex">Index of the parent node</param>
        protected void SiftDown(int nodeIndex) => SiftDown(nodeIndex, Count);

        /// <summary>
        /// Promotes highest value from the children to the provided parent node (<paramref name="nodeIndex"/>), if greater than the parent's value.
        /// Works down recursively - from parent node to its children and so on.
        /// </summary>
        /// <remarks>
        /// Will run until it reaches a node with index higher than provided <paramref name="nodeCount"/> or is unable to swap values.
        /// The complexity of this method is O(log n).
        /// </remarks>
        /// 
        /// <param name="nodeIndex">Index of the parent node</param>
        /// <param name="nodeCount">Number of items in the tree</param>
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

        /// <summary>
        /// Promotes highest value from the children to the provided parent node (<paramref name="nodeIndex"/>), if greater than the parent's value.
        /// Works up recursively - from child node to its parent and so on.
        /// </summary>
        /// <remarks>
        /// Will run until it reaches root or is unable to swap values.
        /// The complexity of this method is O(log n).
        /// </remarks>
        /// 
        /// <param name="nodeIndex">Index of the parent node</param>
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

        /// <summary>
        /// Adds item (<paramref name="item"/>) to the heap and ensures .
        /// </summary>
        /// <remarks>
        /// The complexity of this method is O(log n).
        /// </remarks>
        /// 
        /// <param name="item">Item to be added</param>
        public void Add(TItem item)
        {
            Items.Add(item);
            SiftUp(Count - 1);
        }

        /// <summary>
        /// Updates the heap structure after the provided item's (<paramref name="item"/>) key has changed.
        /// </summary>
        /// 
        /// <param name="item">Item which's key has changed</param>
        /// <returns><c>True</c> if update of the tree has been successful, <c>False</c> otherwise</returns>
        public bool Update(TItem item)
        {
            var nodeIndex = IndexOf(item);
            if (nodeIndex == -1)
                return false;
            
            SiftUp(nodeIndex);
            SiftDown(nodeIndex);
            return true;
        }

        /// <summary>
        /// Removes the provided item (<paramref name="item"/>) from the heap.
        /// </summary>
        /// 
        /// <param name="item">Item to be removed</param>
        /// <returns><c>True</c> if removal from the tree has been successful, <c>False</c> otherwise</returns>
        public bool Remove(TItem item)
        {
            var nodeIndex = IndexOf(item);
            if (nodeIndex == -1)
                return false;

            RemoveAt(nodeIndex);
            return true;
        }

        /// <summary>
        /// Removes all the items from the heap.
        /// </summary>
        public void Clear() => Items.Clear();

        /// <summary>
        /// Checks whether the provided item (<paramref name="item"/>) is on the heap.
        /// </summary>
        /// 
        /// <param name="item">Item to be checked</param>
        /// <returns><c>True</c> if the item is on the heap, <c>False</c> otherwise</returns>
        public bool Contains(TItem item) => ItemIndices.ContainsKey(item);

        /// <summary>
        /// Returns the node index of the provided item (<paramref name="item"/>).
        /// </summary>
        /// 
        /// <param name="item">Item which's node index is to be found</param>
        /// <returns>The index of the node</returns>
        public int IndexOf(TItem item) => Contains(item) ? ItemIndices[item] : -1;

        /// <summary>
        /// Removes the item contained by the node at given index (<paramref name="nodeIndex"/>) from the heap.
        /// </summary>
        /// 
        /// <param name="nodeIndex">Index of the node</param>
        public void RemoveAt(int nodeIndex)
        {
            var lastIndex = Count - 1;
            SwapNodes(nodeIndex, lastIndex);

            var item = Items[lastIndex];
            ItemIndices.Remove(item);
            Items.RemoveAt(lastIndex);

            SiftDown(nodeIndex);
        }

        /// <summary>
        /// Returns topmost item on the heap.
        /// </summary>
        /// 
        /// <returns>Topmost item</returns>
        public TItem Peek() => Count > 0 ? Items[0] : default;

        /// <summary>
        /// Returns topmost item on the heap, removes it, and fixes the structure of the heap.
        /// </summary>
        /// <remarks>
        /// The complexity of this method is O(log n).
        /// </remarks>
        /// 
        /// <returns>Topmost item</returns>
        public TItem ExtractTop()
        {
            if (Count == 0)
                throw new InvalidOperationException("Tried extracting topmost item from empty heap.");

            var item = Items[0];
            RemoveAt(0);

            return item;
        }

        /// <summary>
        /// Swaps values of nodes at two given indices (<paramref name="indexA"/>, <paramref name="indexB"/>).
        /// </summary>
        /// 
        /// <param name="indexA">Index of the first node</param>
        /// <param name="indexB">Index of the second node</param>
        protected void SwapNodes(int indexA, int indexB)
        {
            // save value of the first node
            var tmp = Items[indexA];

            // move value of the second node to the first node
            Items[indexA] = Items[indexB];
            // update index mapping of the item currently in the first node
            ItemIndices[Items[indexA]] = indexA;

            // move value of the first node to the second node
            Items[indexB] = tmp;
            // update index mapping of the item currently in the second node
            ItemIndices[Items[indexB]] = indexB;
        }

        /// <summary>
        /// Compares values of two nodes at given indices (<paramref name="indexA"/>, <paramref name="indexB"/>).
        /// </summary>
        /// <remarks>
        /// The implementation of this method determines the order of the items on the heap.
        /// </remarks>
        /// 
        /// <param name="indexA">Index of the first node</param>
        /// <param name="indexB">Index of the second node</param>
        /// <returns><c>&gt;0</c> if value in node at <paramref name="indexA"/> is greater, <c>&lt;0</c> if the value is smaller or <c>0</c> if the values are equal</returns>
        protected abstract bool IsGreaterThan(int indexA, int indexB);

        public void CopyTo(TItem[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);

        public IEnumerator<TItem> GetEnumerator() => (IEnumerator<TItem>)Items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
    }
}
