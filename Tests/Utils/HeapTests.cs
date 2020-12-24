using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using UnityDES.Utils;

namespace Utils
{
    public class HeapTests
    {
        public class PublicMaxHeap : MaxHeap<int>
        {
            public new List<int> Items { get => base.Items; set => base.Items = value; }

            public new Dictionary<int, int> ItemIndices { get => base.ItemIndices; set => base.ItemIndices = value; }

            public new void SiftDown(int nodeIndex) => base.SiftDown(nodeIndex);

            public new void SiftDown(int nodeIndex, int nodeCount) => base.SiftDown(nodeIndex, nodeCount);

            public PublicMaxHeap(IEnumerable<int> items, IComparer<int> comparer = null)
            {
                Items = new List<int>(items);
                ItemIndices = new Dictionary<int, int>(Items.Capacity);
                Comparer = (comparer ?? Comparer<int>.Default) ?? throw new ArgumentNullException(nameof(comparer));
            }
        }

        public class KeyValObj<K, V> : IEqualityComparer<K>
        {
            public K Key { get; }

            public V Value { get; set; }

            public static readonly Comparer<KeyValObj<K, V>> KeyComparer = Comparer<KeyValObj<K, V>>.Create((a, b) => (dynamic)a.Value - b.Value);

            public static readonly Comparer<KeyValObj<K, V>> ValueComparer = Comparer<KeyValObj<K, V>>.Create((a, b) => (dynamic)a.Value - b.Value);

            public KeyValObj(K key, V value)
            {
                Key = key;
                Value = value;
            }

            public override string ToString() => $"{Key}: {Value} ({GetHashCode()})";

            public override int GetHashCode() => GetHashCode(Key);

            public override bool Equals(object obj) => Equals(obj as KeyValObj<K, V>);

            public bool Equals(KeyValObj<K, V> obj) => obj != null && Equals(obj.Key, Key);

            public bool Equals(K x, K y) => x.GetHashCode() == y.GetHashCode();

            public int GetHashCode(K obj) => obj.GetHashCode();
        }

        static void AssertSortedSame<T>(IEnumerable<T> a, IEnumerable<T> b)
        {
            Assert.IsTrue(a.Count() == b.Count());

            var x = "";
            foreach (var s in a)
                x += ", " + s.ToString();

            for (var i = 0; i < a.Count(); i++)
            {
                Assert.AreEqual(b.ElementAt(i), a.ElementAt(i), x);
            }
        }

        [Test]
        public void ItemCount()
        {
            PublicMaxHeap heap;

            heap = new PublicMaxHeap(new[] { 1 });
            Assert.AreEqual(1, heap.Count);
            heap.RemoveAt(0);
            Assert.AreEqual(0, heap.Count);

            heap = new PublicMaxHeap(new[] { 1, 2, 3 });
            Assert.AreEqual(3, heap.Count);
            heap.RemoveAt(0);
            Assert.AreEqual(2, heap.Count);

            heap = new PublicMaxHeap(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            Assert.AreEqual(10, heap.Count);
            heap.RemoveAt(0);
            Assert.AreEqual(9, heap.Count);
        }

        [Test]
        public void SiftDown()
        {
            PublicMaxHeap heap;

            heap = new PublicMaxHeap(new[] { 1, 2, 3 });
            Assert.AreEqual(1, heap.Items[0]);
            heap.SiftDown(0);
            Assert.AreEqual(3, heap.Items[0]);

            heap = new PublicMaxHeap(new[] { 1, 3, 2 });
            Assert.AreEqual(1, heap.Items[0]);
            heap.SiftDown(0);
            Assert.AreEqual(3, heap.Items[0]);

            heap = new PublicMaxHeap(new[] { 3, 2, 1 });
            Assert.AreEqual(3, heap.Items[0]);
            heap.SiftDown(0);
            Assert.AreEqual(3, heap.Items[0]);

            heap = new PublicMaxHeap(new[] { 1, 3 });
            Assert.AreEqual(1, heap.Items[0]);
            heap.SiftDown(0);
            Assert.AreEqual(3, heap.Items[0]);

            heap = new PublicMaxHeap(new[] { 1, 2, 3, 4, 5, 6, 7 });
            Assert.AreEqual(3, heap.Items[2]);
            heap.SiftDown(2);
            Assert.AreEqual(7, heap.Items[2]);
            Assert.AreEqual(2, heap.Items[1]);
            heap.SiftDown(1);
            Assert.AreEqual(5, heap.Items[1]);
            Assert.AreEqual(1, heap.Items[0]);
            heap.SiftDown(0);
            Assert.AreEqual(7, heap.Items[0]);
        }

        [Test]
        public void SiftDownLimited()
        {
            PublicMaxHeap heap;

            heap = new PublicMaxHeap(new[] { 1, 2, 3 });
            Assert.AreEqual(1, heap.Items[0]);
            heap.SiftDown(0, 2);
            Assert.AreEqual(2, heap.Items[0]);

            heap = new PublicMaxHeap(new[] { 1, 3, 2 });
            Assert.AreEqual(1, heap.Items[0]);
            heap.SiftDown(0, 2);
            Assert.AreEqual(3, heap.Items[0]);

            heap = new PublicMaxHeap(new[] { 3, 2, 1 });
            Assert.AreEqual(3, heap.Items[0]);
            heap.SiftDown(0, 2);
            Assert.AreEqual(3, heap.Items[0]);

            heap = new PublicMaxHeap(new[] { 1, 3 });
            Assert.AreEqual(1, heap.Items[0]);
            heap.SiftDown(0, 1);
            Assert.AreEqual(1, heap.Items[0]);

            heap = new PublicMaxHeap(new[] { 1, 2, 3, 4, 5, 6, 7 });
            Assert.AreEqual(3, heap.Items[2]);
            heap.SiftDown(2, 5);
            Assert.AreEqual(3, heap.Items[2]);
            Assert.AreEqual(2, heap.Items[1]);
            heap.SiftDown(1, 5);
            Assert.AreEqual(5, heap.Items[1]);
            Assert.AreEqual(1, heap.Items[0]);
            heap.SiftDown(0, 5);
            Assert.AreEqual(5, heap.Items[0]);
        }

        [Test]
        public void Heapify()
        {
            PublicMaxHeap heap;

            heap = new PublicMaxHeap(new[] { 5, 2, 4, 6, 1, 3, 8 });
            Assert.AreEqual(5, heap.Items[0]);
            heap.Heapify();
            Assert.AreEqual(8, heap.Items[0]);
            Assert.AreEqual(6, heap.Items[1]);
            Assert.AreEqual(5, heap.Items[2]);

            heap = new PublicMaxHeap(new[] { 5, 2, 4, 6, 1, 8, 3 });
            Assert.AreEqual(5, heap.Items[0]);
            heap.Heapify();
            Assert.AreEqual(8, heap.Items[0]);
            Assert.AreEqual(6, heap.Items[1]);
            Assert.AreEqual(5, heap.Items[2]);

            heap = new PublicMaxHeap(new[] { 7, 6, 1, 4, 8, 3, 0 });
            Assert.AreEqual(7, heap.Items[0]);
            heap.Heapify();
            Assert.AreEqual(8, heap.Items[0]);
            Assert.AreEqual(7, heap.Items[1]);
            Assert.AreEqual(3, heap.Items[2]);

            heap = new PublicMaxHeap(new[] { 7, 6, 1, 8, 4, 3, 0 });
            Assert.AreEqual(7, heap.Items[0]);
            heap.Heapify();
            Assert.AreEqual(8, heap.Items[0]);
            Assert.AreEqual(7, heap.Items[1]);
            Assert.AreEqual(3, heap.Items[2]);

            heap = new PublicMaxHeap(new[] { 6, 4, 8, 2, 1, 3, 5 });
            Assert.AreEqual(6, heap.Items[0]);
            heap.Heapify();
            Assert.AreEqual(8, heap.Items[0]);
            Assert.AreEqual(4, heap.Items[1]);
            Assert.AreEqual(6, heap.Items[2]);

            heap = new PublicMaxHeap(new[] { 6, 8, 4, 2, 1, 3, 5 });
            Assert.AreEqual(6, heap.Items[0]);
            heap.Heapify();
            Assert.AreEqual(8, heap.Items[0]);
            Assert.AreEqual(6, heap.Items[1]);
            Assert.AreEqual(5, heap.Items[2]);

            heap = new PublicMaxHeap(new[] { 8, 4, 6, 7, 1, 2, 0 });
            Assert.AreEqual(8, heap.Items[0]);
            heap.Heapify();
            Assert.AreEqual(8, heap.Items[0]);
            Assert.AreEqual(7, heap.Items[1]);
            Assert.AreEqual(6, heap.Items[2]);
        }

        [Test]
        public void Sorting()
        {
            PublicMaxHeap heap;

            heap = new PublicMaxHeap(new[] { 5, 2, 4, 6, 1, 3, 8 });
            heap.Heapify();
            heap.Sort();
            AssertSortedSame(heap.Items, new[] { 5, 2, 4, 6, 1, 3, 8 }.OrderBy(i => i));

            heap = new PublicMaxHeap(new[] { 7, 6, 1, 4, 8, 3, 0 });
            heap.Heapify();
            heap.Sort();
            AssertSortedSame(heap.Items, new[] { 7, 6, 1, 4, 8, 3, 0 }.OrderBy(i => i));

            heap = new PublicMaxHeap(new[] { 6, 4, 8, 2, 1, 3, 5 });
            heap.Heapify();
            heap.Sort();
            AssertSortedSame(heap.Items, new[] { 6, 4, 8, 2, 1, 3, 5 }.OrderBy(i => i));

            heap = new PublicMaxHeap(new[] { 8, 4, 6, 7, 1, 2, 0 });
            heap.Heapify();
            heap.Sort();
            AssertSortedSame(heap.Items, new[] { 8, 4, 6, 7, 1, 2, 0 }.OrderBy(i => i));
        }

        [Test]
        public void Add()
        {
            PublicMaxHeap heap;

            heap = new PublicMaxHeap(new[] { 5, 2, 4, 6, 1, 3, 8 });
            heap.Heapify();
            Assert.AreEqual(7, heap.Items.Count);
            Assert.AreEqual(7, heap.ItemIndices.Count);
            heap.Add(9);
            Assert.AreEqual(9, heap.Items[0]);
            Assert.AreEqual(8, heap.Items.Count);
            Assert.AreEqual(8, heap.ItemIndices.Count);

            heap = new PublicMaxHeap(new[] { 8, 4, 6, 7, 1, 2, 0 });
            heap.Heapify();
            Assert.AreEqual(7, heap.Items.Count);
            Assert.AreEqual(7, heap.ItemIndices.Count);
            heap.Add(9);
            Assert.AreEqual(9, heap.Items[0]);
            Assert.AreEqual(8, heap.Items.Count);
            Assert.AreEqual(8, heap.ItemIndices.Count);

            heap = new PublicMaxHeap(new[] { 7, 6, 1, 4, 8, 3, 0 });
            heap.Heapify();
            Assert.AreEqual(7, heap.Items.Count);
            Assert.AreEqual(7, heap.ItemIndices.Count);
            heap.Add(9);
            Assert.AreEqual(9, heap.Items[0]);
            Assert.AreEqual(8, heap.Items.Count);
            Assert.AreEqual(8, heap.ItemIndices.Count);

            heap = new PublicMaxHeap(new[] { 5, 2, 4, 6, 1, 3, 8 });
            heap.Heapify();
            Assert.AreEqual(7, heap.Items.Count);
            Assert.AreEqual(7, heap.ItemIndices.Count);
            heap.Add(0);
            Assert.AreEqual(8, heap.Items[0]);
            Assert.AreEqual(8, heap.Items.Count);
            Assert.AreEqual(8, heap.ItemIndices.Count);

            heap = new PublicMaxHeap(new[] { 8, 4, 6, 7, 1, 2, 0 });
            heap.Heapify();
            Assert.AreEqual(7, heap.Items.Count);
            Assert.AreEqual(7, heap.ItemIndices.Count);
            heap.Add(3);
            Assert.AreEqual(8, heap.Items[0]);
            Assert.AreEqual(8, heap.Items.Count);
            Assert.AreEqual(8, heap.ItemIndices.Count);

            heap = new PublicMaxHeap(new[] { 7, 6, 1, 4, 8, 3, 0 });
            heap.Heapify();
            Assert.AreEqual(7, heap.Items.Count);
            Assert.AreEqual(7, heap.ItemIndices.Count);
            heap.Add(5);
            Assert.AreEqual(8, heap.Items[0]);
            Assert.AreEqual(8, heap.Items.Count);
            Assert.AreEqual(8, heap.ItemIndices.Count);
        }


        [Test]
        public void Remove()
        {
            PublicMaxHeap heap;

            heap = new PublicMaxHeap(new[] { 5, 2, 4, 6, 1, 3, 8 });
            heap.Heapify();
            Assert.AreEqual(7, heap.Items.Count);
            Assert.AreEqual(7, heap.ItemIndices.Count);
            heap.Remove(8);
            Assert.AreEqual(6, heap.Items[0]);
            Assert.AreEqual(6, heap.Items.Count);
            Assert.AreEqual(6, heap.ItemIndices.Count);

            heap = new PublicMaxHeap(new[] { 8, 4, 6, 7, 1, 2, 0 });
            heap.Heapify();
            Assert.AreEqual(7, heap.Items.Count);
            Assert.AreEqual(7, heap.ItemIndices.Count);
            heap.Remove(7);
            Assert.AreEqual(4, heap.Items[1]);
            Assert.AreEqual(8, heap.Items[0]);
            Assert.AreEqual(6, heap.Items.Count);
            Assert.AreEqual(6, heap.ItemIndices.Count);

            heap = new PublicMaxHeap(new[] { 7, 6, 1, 4, 8, 3, 5 });
            heap.Heapify();
            Assert.AreEqual(7, heap.Items.Count);
            Assert.AreEqual(7, heap.ItemIndices.Count);
            heap.Remove(8);
            heap.Remove(6);
            Assert.AreEqual(5, heap.Items[2]);
            Assert.AreEqual(4, heap.Items[1]);
            Assert.AreEqual(7, heap.Items[0]);
            Assert.AreEqual(5, heap.Items.Count);
            Assert.AreEqual(5, heap.ItemIndices.Count);
        }

        [Test]
        public void ExtractTop()
        {
            PublicMaxHeap heap;

            heap = new PublicMaxHeap(new[] { 5, 2, 4, 6, 1, 3, 8 });
            heap.Heapify();
            Assert.AreEqual(7, heap.Items.Count);
            Assert.AreEqual(7, heap.ItemIndices.Count);

            Assert.AreEqual(8, heap.ExtractTop());
            Assert.AreEqual(6, heap.ExtractTop());
            Assert.AreEqual(5, heap.ExtractTop());
            Assert.AreEqual(4, heap.ExtractTop());
            Assert.AreEqual(3, heap.Items.Count);
            Assert.AreEqual(3, heap.ItemIndices.Count);

            Assert.AreEqual(3, heap.ExtractTop());
            Assert.AreEqual(2, heap.ExtractTop());
            Assert.AreEqual(1, heap.ExtractTop());
            Assert.AreEqual(0, heap.Items.Count);
            Assert.AreEqual(0, heap.ItemIndices.Count);
        }

        [Test]
        public void Update()
        {
            var item1 = new KeyValObj<int, int>(1, 1);
            var item2 = new KeyValObj<int, int>(2, 2);
            var item3 = new KeyValObj<int, int>(3, 3);

            var items = new[] { item1, item2, item3 };
            var heap = new MaxHeap<KeyValObj<int, int>>(items, KeyValObj<int, int>.ValueComparer);

            Assert.AreSame(item3, heap.Peek());
            item3.Value = 0;
            Assert.AreEqual(item3.Value, heap.Peek().Value);
            Assert.IsTrue(heap.Update(item3));
            Assert.AreSame(item2, heap.Peek());

            item2.Value = 10;
            Assert.AreSame(item2, heap.Peek());
            Assert.AreEqual(item2.Value, heap.Peek().Value);

            Assert.AreSame(item2, heap.ExtractTop());
            Assert.AreSame(item1, heap.ExtractTop());
            Assert.AreSame(item3, heap.ExtractTop());
        }
    }
}