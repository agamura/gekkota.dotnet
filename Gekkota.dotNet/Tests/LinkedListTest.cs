//------------------------------------------------------------------------------
// <sourcefile name="LinkedListTest.cs" language="C#" begin="05/05/2004">
//
//     <author name="Giuseppe Greco" email="giuseppe.greco@agamura.com" />
//
//     <copyright company="Agamura" url="http://www.agamura.com">
//         Copyright (C) 2004 Agamura, Inc.  All rights reserved.
//     </copyright>
//
// </sourcefile>
//------------------------------------------------------------------------------

#if DEBUG
using System;
using System.Collections.Generic;
using Gekkota.Collections;
using NUnit.Framework;

namespace Gekkota.Tests
{
    [TestFixture]
    public class LinkedListTest
    {
        #region service methods
        [SetUp]
        public void Init() {}

        [TearDown]
        public void Clean() {}
        #endregion service methods

        #region unit test methods
        [Test]
        public void Item()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            Collections.LinkedList<string> list = new Collections.LinkedList<string>();

            for (int i = 0; i < items.Length; i++) {
                list.Add(null);
                list[i] = items[i];
                Assert.AreEqual(items[i], list[i]);
            }
        }

        [Test]
        public void Comparer()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            Collections.LinkedList<string> list = new Collections.LinkedList<string>();
            list.Comparer = new LengthComparer();

            foreach (string item in items) {
                list.Add(item);
            }

            //
            // remove all the items with a length of 3 (see LengthComparer class)
            //
            for (int i = 0; i < items.Length; i++) {
                list.Remove(items[i]);
            }

            foreach (string item in list) {
                Assert.IsTrue(item.Length != 3);
            }
        }

        [Test]
        public void Count()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            Collections.LinkedList<string> list = new Collections.LinkedList<string>();

            foreach (string item in items) {
                list.Add(item);
            }

            Assert.AreEqual(items.Length, list.Count);
        }

        [Test]
        public void IsFixedSize()
        {
            Collections.LinkedList<string> list = new Collections.LinkedList<string>();
            Assert.AreEqual(false, list.IsFixedSize);

            list = Collections.LinkedList<string>.FixedSize(list);
            Assert.AreEqual(true, list.IsFixedSize);
        }

        [Test]
        public void IsReadOnly()
        {
            Collections.LinkedList<string> list = new Collections.LinkedList<string>();
            Assert.AreEqual(false, list.IsReadOnly);

            list = Collections.LinkedList<string>.ReadOnly(list);
            Assert.AreEqual(true, list.IsReadOnly);
        }

        [Test]
        public void IsSynchronized()
        {
            Collections.LinkedList<string> list = new Collections.LinkedList<string>();
            Assert.AreEqual(false, list.IsSynchronized);

            list = Collections.LinkedList<string>.Synchronized(list);
            Assert.AreEqual(true, list.IsSynchronized);
        }

        [Test]
        public void Sorted()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            Collections.LinkedList<string> list = new Collections.LinkedList<string>();
            Assert.AreEqual(false, list.Sorted);

            foreach (string item in items) {
                list.Add(item);
            }

            list.Sorted = true;
            Assert.AreEqual(true, list.Sorted);

            Assert.AreEqual(items[1], list[0]); // "Cat"
            Assert.AreEqual(items[4], list[1]); // "Cow"
            Assert.AreEqual(items[3], list[2]); // "Dog"
            Assert.AreEqual(items[2], list[3]); // "Gorilla"
            Assert.AreEqual(items[0], list[4]); // "Horse"
        }

        [Test]
        public void SyncRoot()
        {
            Collections.LinkedList<string> list = new Collections.LinkedList<string>();
            Assert.AreEqual(list, list.SyncRoot);
        }

        [Test]
        public void CreateInstance()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            Collections.LinkedList<string> list = new Collections.LinkedList<string>();
            Assert.AreEqual(false, list.Sorted);

            foreach (string item in items) {
                list.Add(item);
            }

            Assert.AreEqual(items[0], list[0]); // "Horse"
            Assert.AreEqual(items[1], list[1]); // "Cat"
            Assert.AreEqual(items[2], list[2]); // "Gorilla"
            Assert.AreEqual(items[3], list[3]); // "Dog"
            Assert.AreEqual(items[4], list[4]); // "Cow"
        }

        [Test]
        public void CreateInstance_Sorted()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            Collections.LinkedList<string> list = new Collections.LinkedList<string>(true);
            Assert.AreEqual(true, list.Sorted);

            foreach (string item in items) {
                list.Add(item);
            }

            Assert.AreEqual(items[1], list[0]); // "Cat"
            Assert.AreEqual(items[4], list[1]); // "Cow"
            Assert.AreEqual(items[3], list[2]); // "Dog"
            Assert.AreEqual(items[2], list[3]); // "Gorilla"
            Assert.AreEqual(items[0], list[4]); // "Horse"
        }

        [Test]
        public void Add()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            Collections.LinkedList<string> list = new Collections.LinkedList<string>();

            foreach (string item in items) {
                list.Add(item);
            }

            Assert.AreEqual(items.Length, list.Count);
            for (int i = 0; i < items.Length; i++) {
                Assert.AreEqual(items[i], list[i]);
            }
        }

        [Test]
        public void Clear()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            Collections.LinkedList<string> list = new Collections.LinkedList<string>();

            foreach (string item in items) {
                list.Add(item);
            }

            list.Clear();
            Assert.AreEqual(0, list.Count);
        }

        [Test]
        public void Clone()
        {
            //
            // clone an empty list
            //
            Collections.LinkedList<string> list = new Collections.LinkedList<string>();
            Collections.LinkedList<string> clone = list.Clone();
            
            Assert.AreEqual(0, clone.Count);
            Assert.AreEqual(false, clone.Sorted);
            Assert.AreEqual(null, clone.Comparer);

            //
            // clone a non-empty list
            //
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            foreach (string item in items) {
                list.Add(item);
            }

            clone = list.Clone();

            Assert.AreEqual(list.Count, clone.Count);
            Assert.AreEqual(list.Sorted, clone.Sorted);
            Assert.AreEqual(list.Comparer, clone.Comparer);

            for (int i = 0; i < list.Count; i++) {
                Assert.AreEqual(list[i], clone[i]);
            }

            //
            // clone a non-empty sorted list
            //
            list.Comparer = new LengthComparer();
            list.Sorted = true;
            clone = list.Clone();

            Assert.AreEqual(list.Sorted, clone.Sorted);
            Assert.AreEqual(list.Count, clone.Count);
            Assert.AreEqual(list.Comparer, clone.Comparer);

            for (int i = 0; i < list.Count; i++) {
                Assert.AreEqual(list[i], clone[i]);
            }

            //
            // clone a fixed-size, read-only, synchronized list
            //
            list = Collections.LinkedList<string>.ReadOnly(list);
            list = Collections.LinkedList<string>.Synchronized(list);
            clone = list.Clone();
            Assert.AreEqual(list.IsReadOnly, clone.IsReadOnly);
            Assert.AreEqual(list.IsFixedSize, clone.IsFixedSize);
            Assert.AreEqual(list.IsSynchronized, clone.IsSynchronized);
        }

        [Test]
        public void Contains()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            Collections.LinkedList<string> list = new Collections.LinkedList<string>();

            foreach (string item in items) {
                list.Add(item);
            }

            foreach (string item in items) {
                Assert.AreEqual(true, list.Contains(item));
            }   Assert.AreEqual(false, list.Contains("Bird"));
        }

        [Test]
        public void CopyTo()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            Collections.LinkedList<string> list = new Collections.LinkedList<string>();

            foreach (string item in items) {
                list.Add(item);
            }

            string[] array = new string[items.Length];
            list.CopyTo(array, 0);

            Assert.AreEqual(list.Count, array.Length);
            for (int i = 0; i < list.Count; i++) {
                Assert.AreEqual(list[i], array[i]);
            }
        }

        [Test]
        public void FixedSize()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            Collections.LinkedList<string> list = new Collections.LinkedList<string>();

            foreach (string item in items) {
                list.Add(item);
            }

            list = Collections.LinkedList<string>.Synchronized(list);
            Collections.LinkedList<string> fixedSizeList = Collections.LinkedList<string>.FixedSize(list);

            Assert.AreEqual(true, fixedSizeList.IsFixedSize);
            Assert.AreEqual(list.IsReadOnly, fixedSizeList.IsReadOnly);
            Assert.AreEqual(list.IsSynchronized, fixedSizeList.IsSynchronized);
            Assert.AreEqual(list.Count, fixedSizeList.Count);
            Assert.AreEqual(list.SyncRoot, fixedSizeList.SyncRoot);

            for (int i = 0; i < list.Count; i++) {
                Assert.AreEqual(list[i], fixedSizeList[i]);
                fixedSizeList[i] = "";
            }
        }


        [Test]
        public void GetEnumerator()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            Collections.LinkedList<string> list = new Collections.LinkedList<string>();

            foreach (string item in items) {
                list.Add(item);
            }

            IEnumerator<String> enumerator = list.GetEnumerator();

            int i = 0;
            while (enumerator.MoveNext()) {
                Assert.AreEqual(items[i++], enumerator.Current);
            }
        }

        [Test]
        public void IndexOf()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            Collections.LinkedList<string> list = new Collections.LinkedList<string>();

            foreach (string item in items) {
                list.Add(item);
            }

            for (int i = 0; i < items.Length; i++) {
                Assert.AreEqual(i, list.IndexOf(items[i]));
            }
        }

        [Test]
        public void Insert()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            Collections.LinkedList<string> list = new Collections.LinkedList<string>();

            list.Insert(0, items[0]); // insert "Horse" at position 0
            list.Insert(1, items[2]); // insert "Gorilla" at position 1
            list.Insert(1, items[1]); // insert "Cat" at position 1
            list.Insert(3, items[3]); // insert "Dog" at position 3
            list.Insert(4, items[4]); // insert "Cow" at position 4
            
            for (int i = 0; i < items.Length; i++) {
                Assert.AreEqual(items[i], list[i]);
            }
        }

        [Test]
        public void ReadOnly()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            Collections.LinkedList<string> list = new Collections.LinkedList<string>();

            foreach (string item in items) {
                list.Add(item);
            }

            list = Collections.LinkedList<string>.Synchronized(list);
            Collections.LinkedList<string> readOnlyList = Collections.LinkedList<string>.ReadOnly(list);

            Assert.AreEqual(true, readOnlyList.IsReadOnly);
            Assert.AreEqual(true, readOnlyList.IsFixedSize);
            Assert.AreEqual(list.IsSynchronized, readOnlyList.IsSynchronized);
            Assert.AreEqual(list.Count, readOnlyList.Count);
            Assert.AreEqual(list.SyncRoot, readOnlyList.SyncRoot);

            for (int i = 0; i < list.Count; i++) {
                Assert.AreEqual(list[i], readOnlyList[i]);
            }
        }

        [Test]
        public void Remove()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            int count = items.Length;
            Collections.LinkedList<string> list = new Collections.LinkedList<string>();

            foreach (string item in items) {
                list.Add(item);
            }

            //
            // remove "Gorilla"
            //
            list.Remove(items[2]);
            Assert.AreEqual(--count, list.Count);
            Assert.AreEqual(false, list.Contains(items[2]));

            //
            // remove "Horse" (head)
            //
            list.Remove(items[0]);
            Assert.AreEqual(--count, list.Count);
            Assert.AreEqual(false, list.Contains(items[0]));

            //
            // remove "Cow" (tail)
            //
            list.Remove(items[4]);
            Assert.AreEqual(--count, list.Count);
            Assert.AreEqual(false, list.Contains(items[4]));

            //
            // "Cat" and "Dog" should still be there
            //
            Assert.AreEqual(true, list.Contains(items[1]));
            Assert.AreEqual(true, list.Contains(items[3]));
        }

        [Test]
        public void RemoveAt()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            int count = items.Length;
            Collections.LinkedList<string> list = new Collections.LinkedList<string>();

            foreach (string item in items) {
                list.Add(item);
            }

            //
            // remove "Gorilla" from position 2
            //
            list.RemoveAt(2);
            Assert.AreEqual(--count, list.Count);
            Assert.AreEqual(false, list.Contains(items[2]));

            //
            // remove "Horse" from position 0 (head)
            //
            list.RemoveAt(0);
            Assert.AreEqual(--count, list.Count);
            Assert.AreEqual(false, list.Contains(items[0]));

            //
            // remove "Cow" from position --count (tail)
            //
            list.RemoveAt(--count);
            Assert.AreEqual(count, list.Count);
            Assert.AreEqual(false, list.Contains(items[4]));

            //
            // "Cat" and "Dog" should still be there
            //
            Assert.AreEqual(true, list.Contains(items[1]));
            Assert.AreEqual(true, list.Contains(items[3]));
        }

        [Test]
        public void Reverse()
        {
            Collections.LinkedList<int> list = new Collections.LinkedList<int>();

            //
            // reverse an empty list
            //
            list.Reverse();
            Assert.AreEqual(0, list.Count);

            //
            // reverse a list that contains just one element
            //
            list.Add(0);
            list.Reverse();
            Assert.AreEqual(0, list[0]);

            //
            // reverse a list that contains an even number of elements
            //
            for (int i = 1; i < 5; i++) {
                list.Add(i);
            }   list.Reverse();
            
            for (int i = list.Count -1, j = 0; j < list.Count; i--, j++) {
                Assert.AreEqual(i, list[j]);
            }

            //
            // reverse a list that contains an odd number of elements
            //
            list.Insert(0, list.Count);
            list.Reverse();

            for (int i = 0; i < list.Count; i++) {
                Assert.AreEqual(i, list[i]);
            }
        }

        [Test]
        public void Sort()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            Collections.LinkedList<string> list = new Collections.LinkedList<string>();

            foreach (string item in items) {
                list.Add(item);
            }

            //
            // sort the list twice: the first time to test the sorting of an
            // unsorted list, while the second time to test the sorting of an
            // already sorted list
            //
            for (int i = 0; i < 2; i++) {
                list.Sort();

                Assert.AreEqual(items[1], list[0]); // "Cat"
                Assert.AreEqual(items[4], list[1]); // "Cow"
                Assert.AreEqual(items[3], list[2]); // "Dog"
                Assert.AreEqual(items[2], list[3]); // "Gorilla"
                Assert.AreEqual(items[0], list[4]); // "Horse"
            }
        } 

        [Test]
        public void Sort_WithComparer()
        {
            int[] items = { 5, 4, 2, 1, 6, 3 };
            Collections.LinkedList<int> list = new Collections.LinkedList<int>();
            list.Comparer = new ValueComparer();

            foreach (int i in items) {
                list.Add(i);
            }

            //
            // sort the list twice: the first time to test the sorting of an
            // unsorted list, while the second time to test the sorting of an
            // already sorted list
            //
            for (int i = 0; i < 2; i++) {
                list.Sort();

                Assert.AreEqual(items[3], list[0]); // item "1"
                Assert.AreEqual(items[2], list[1]); // item "2"
                Assert.AreEqual(items[5], list[2]); // item "3"
                Assert.AreEqual(items[1], list[3]); // item "4"
                Assert.AreEqual(items[0], list[4]); // item "5"
                Assert.AreEqual(items[4], list[5]); // item "6"
            }
        }

        [Test]
        public void Synchronized()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            Collections.LinkedList<string> list = new Collections.LinkedList<string>();

            foreach (string item in items) {
                list.Add(item);
            }

            list = Collections.LinkedList<string>.FixedSize(list);
            Collections.LinkedList<string> syncList = Collections.LinkedList<string>.Synchronized(list);

            Assert.AreEqual(true, syncList.IsSynchronized);
            Assert.AreEqual(list.IsFixedSize, syncList.IsFixedSize);
            Assert.AreEqual(list.IsReadOnly, syncList.IsReadOnly);
            Assert.AreEqual(list.Count, syncList.Count);
            Assert.AreEqual(list.SyncRoot, syncList.SyncRoot);

            for (int i = 0; i < list.Count; i++) {
                Assert.AreEqual(list[i], syncList[i]);
                syncList[i] = "";
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetItem_EmptyList()
        {
            Collections.LinkedList<object> list = new Collections.LinkedList<object>();
            object item = list[0];
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetItem_EmptyList()
        {
            Collections.LinkedList<object> list = new Collections.LinkedList<object>();
            list[0] = null;
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetItem_NegativeIndex()
        {
            Collections.LinkedList<object> list = new Collections.LinkedList<object>();
            list.Add(null);
            object item = list[-1];
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetItem_NegativeIndex()
        {
            Collections.LinkedList<object> list = new Collections.LinkedList<object>();
            list.Add(null);
            list[-1] = null;
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetItem_IndexOutOfBounds()
        {
            Collections.LinkedList<object> list = new Collections.LinkedList<object>();
            list.Add(null);
            object item = list[1];
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void SetItem_IndexOutOfBounds()
        {
            Collections.LinkedList<object> list = new Collections.LinkedList<object>();
            list.Add(null);
            list[1] = null;
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void SetItem_ReadOnlyList()
        {
            Collections.LinkedList<object> list = new Collections.LinkedList<object>();
            list.Add(null);

            list = Collections.LinkedList<object>.ReadOnly(list);
            list[0] = null;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CopyTo_NullArray()
        {
            Collections.LinkedList<object> list = new Collections.LinkedList<object>();
            list.CopyTo(null, 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CopyTo_NegativeIndex()
        {
            Collections.LinkedList<object> list = new Collections.LinkedList<object>();
            object[] array = new object[2];
            list.CopyTo(array, -1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CopyTo_IndexOutOfBounds()
        {
            Collections.LinkedList<object> list = new Collections.LinkedList<object>();
            object[] array = new object[2];
            list.CopyTo(array, 2);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CopyTo_ArrayTooSmall()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            Collections.LinkedList<string> list = new Collections.LinkedList<string>();

            foreach (string item in items) {
                list.Add(item);
            }

            string[] array = new string[2];
            list.CopyTo(array, 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FixedSize_NullList()
        {
            Collections.LinkedList<object>.FixedSize(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Insert_NegativeIndex()
        {
            Collections.LinkedList<object> list = new Collections.LinkedList<object>();
            list.Add(null);
            list.Insert(-1, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Insert_IndexOutOfBounds()
        {
            Collections.LinkedList<object> list = new Collections.LinkedList<object>();
            list.Add(null);
            list.Insert(2, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Insert_EmptyListAndIndexNotZero()
        {
            Collections.LinkedList<object> list = new Collections.LinkedList<object>();
            list.Insert(1, null);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Insert_FixedSizeList()
        {
            Collections.LinkedList<object> list = new Collections.LinkedList<object>();
            list.Add(null);

            list = Collections.LinkedList<object>.FixedSize(list);
            list.Insert(0, null);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Insert_ReadOnlyList()
        {
            Collections.LinkedList<object> list = new Collections.LinkedList<object>();
            list.Add(null);

            list = Collections.LinkedList<object>.ReadOnly(list);
            list.Insert(0, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadOnly_NullList()
        {
            Collections.LinkedList<object>.ReadOnly(null);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Remove_FixedSizeList()
        {
            Collections.LinkedList<object> list = new Collections.LinkedList<object>();

            object obj = new object();
            list.Add(obj);

            list = Collections.LinkedList<object>.FixedSize(list);
            list.Remove(obj);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Remove_ReadOnlyList()
        {
            Collections.LinkedList<object> list = new Collections.LinkedList<object>();

            object obj = new object();
            list.Add(obj);

            list = Collections.LinkedList<object>.ReadOnly(list);
            list.Remove(obj);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RemoveAt_NegativeIndex()
        {
            Collections.LinkedList<object> list = new Collections.LinkedList<object>();
            list.Add(null);
            list.RemoveAt(-1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void RemoveAt_IndexOutOfBounds()
        {
            Collections.LinkedList<object> list = new Collections.LinkedList<object>();
            list.Add(null);
            list.RemoveAt(1);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RemoveAt_EmptyList()
        {
            Collections.LinkedList<object> list = new Collections.LinkedList<object>();
            list.RemoveAt(0);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Insert_Sorted()
        {
            Collections.LinkedList<object> list = new Collections.LinkedList<object>(true);
            list.Insert(0, null);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void RemoveAt_FixedSizeList()
        {
            Collections.LinkedList<object> list = new Collections.LinkedList<object>();
            list.Add(null);

            list = Collections.LinkedList<object>.FixedSize(list);
            list.RemoveAt(0);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void RemoveAt_ReadOnlyList()
        {
            Collections.LinkedList<object> list = new Collections.LinkedList<object>();
            list.Add(null);

            list = Collections.LinkedList<object>.ReadOnly(list);
            list.RemoveAt(0);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Reverse_ReadOnlyList()
        {
            Collections.LinkedList<object> list = new Collections.LinkedList<object>();
            list.Add(null);

            list = Collections.LinkedList<object>.ReadOnly(list);
            list.Reverse();
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Sort_ReadOnlyList()
        {
            Collections.LinkedList<object> list = new Collections.LinkedList<object>();
            list.Add(null);

            list = Collections.LinkedList<object>.ReadOnly(list);
            list.Sort();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Synchronized_NullList()
        {
            Collections.LinkedList<object>.Synchronized(null);
        }
        #endregion unit test methods

        #region inner classes
        private sealed class LengthComparer : IComparer<String>
        {
            int IComparer<String>.Compare(string x, string y)
            {
                if (x.Length > 3) {
                    return 1;
                } else if (x.Length < 3) {
                    return -1;
                }

                return 0;
            }
        }

        private sealed class ValueComparer : IComparer<int>
        {
            int IComparer<int>.Compare(int x, int y)
            {
                if (x > y) {
                    return 1;
                } else if (x < y) {
                    return -1;
                }

                return 0;
            }
        }
        #endregion inner classes
    }
}
#endif
