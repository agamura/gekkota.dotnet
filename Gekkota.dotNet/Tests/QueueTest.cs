//------------------------------------------------------------------------------
// <sourcefile name="QueueTest.cs" language="C#" begin="08/16/2005">
//
//     <author name="Giuseppe Greco" email="giuseppe.greco@agamura.com" />
//
//     <copyright company="Agamura" url="http://www.agamura.com">
//         Copyright (C) 2005 Agamura, Inc.  All rights reserved.
//     </copyright>
//
// </sourcefile>
//------------------------------------------------------------------------------

#if DEBUG
using System;
using System.Text;
using System.Collections.Generic;
using Gekkota.Collections;
using NUnit.Framework;

namespace Gekkota.Tests
{
    [TestFixture]
    public class QueueTest
    {
        #region service methods
        [SetUp]
        public void Init() {}

        [TearDown]
        public void Clean() {}
        #endregion service methods

        #region unit test methods
        [Test]
        public void Clone()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            Collections.Queue<String> queue = new Collections.Queue<String>(true);
            
            foreach (string item in items) {
                queue.Enqueue(item);
            }

            queue = Collections.Queue<String>.Synchronized(queue);
            Collections.Queue<String> clone = queue.Clone();

            Assert.AreEqual(queue.Count, clone.Count);
            Assert.AreEqual(queue.Sorted, clone.Sorted);
            Assert.AreEqual(queue.Comparer, clone.Comparer);
            Assert.AreEqual(queue.IsSynchronized, clone.IsSynchronized);

            IEnumerator<String> enumerator1 = queue.GetEnumerator();
            IEnumerator<String> enumerator2 = clone.GetEnumerator();

            while (enumerator1.MoveNext() && enumerator2.MoveNext()) {
                Assert.AreEqual(enumerator1.Current, enumerator2.Current);
            }
        }

        [Test]
        public void Comparer()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            Collections.Queue<String> queue = new Collections.Queue<String>(true);
            queue.Comparer = new LengthComparer();

            foreach (string item in items) {
                queue.Enqueue(item);
            }

            int lastLength = 0;
            foreach (string item in queue) {
                Assert.IsTrue(item.Length >= lastLength);
                lastLength = item.Length;
            }
        }

        [Test]
        public void Count()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            Collections.Queue<string> queue = new Collections.Queue<string>();
            queue.Comparer = new LengthComparer();

            foreach (string item in items) {
                queue.Enqueue(item);
            }

            Assert.AreEqual(items.Length, queue.Count);
        }

        [Test]
        public void IsSynchronized()
        {
            Collections.Queue<object> queue = new Collections.Queue<object>();
            Assert.AreEqual(false, queue.IsSynchronized);

            queue = Collections.Queue<object>.Synchronized(queue);
            Assert.AreEqual(true, queue.IsSynchronized);
        }

        [Test]
        public void Sorted()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            Collections.Queue<string> queue = new Collections.Queue<string>();
            Assert.AreEqual(false, queue.Sorted);

            foreach (string item in items) {
                queue.Enqueue(item);
            }

            queue.Sorted = true;
            Assert.AreEqual(true, queue.Sorted);

            Assert.AreEqual(items[1], queue.Dequeue()); // "Cat"
            Assert.AreEqual(items[4], queue.Dequeue()); // "Cow"
            Assert.AreEqual(items[3], queue.Dequeue()); // "Dog"
            Assert.AreEqual(items[2], queue.Dequeue()); // "Gorilla"
            Assert.AreEqual(items[0], queue.Dequeue()); // "Horse"
        }

        [Test]
        public void SyncRoot()
        {
            Collections.Queue<object> queue = new Collections.Queue<object>();
            Assert.AreEqual(queue, queue.SyncRoot);
        }

        [Test]
        public void CreateInstance()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            Collections.Queue<string> queue = new Collections.Queue<string>();
            Assert.AreEqual(false, queue.Sorted);

            foreach (string item in items) {
                queue.Enqueue(item);
            }

            Assert.AreEqual(items[0], queue.Dequeue()); // "Horse"
            Assert.AreEqual(items[1], queue.Dequeue()); // "Cat"
            Assert.AreEqual(items[2], queue.Dequeue()); // "Gorilla"
            Assert.AreEqual(items[3], queue.Dequeue()); // "Dog"
            Assert.AreEqual(items[4], queue.Dequeue()); // "Cow"
        }

        [Test]
        public void CreateInstance_Sorted()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            Collections.Queue<string> queue = new Collections.Queue<string>(true);
            Assert.AreEqual(true, queue.Sorted);

            foreach (string item in items) {
                queue.Enqueue(item);
            }

            Assert.AreEqual(items[1], queue.Dequeue()); // "Cat"
            Assert.AreEqual(items[4], queue.Dequeue()); // "Cow"
            Assert.AreEqual(items[3], queue.Dequeue()); // "Dog"
            Assert.AreEqual(items[2], queue.Dequeue()); // "Gorilla"
            Assert.AreEqual(items[0], queue.Dequeue()); // "Horse"
        }

        [Test]
        public void Enqueue_Dequeue()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            Collections.Queue<string> queue = new Collections.Queue<string>();

            foreach (string item in items) {
                queue.Enqueue(item);
            }

            Assert.AreEqual(items.Length, queue.Count);
            for (int i = 0; i < items.Length; i++) {
                Assert.AreEqual(items[i], queue.Dequeue());
            }

            Assert.AreEqual(0, queue.Count);
        }

        [Test]
        public void GetEnumerator()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            Collections.Queue<string> queue = new Collections.Queue<string>();

            foreach (string item in items) {
                queue.Enqueue(item);
            }

            IEnumerator<string> enumerator = queue.GetEnumerator();

            int i = 0;
            while (enumerator.MoveNext()) {
                Assert.AreEqual(items[i++], enumerator.Current);
            }
        }

        [Test]
        public void Peek()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            Collections.Queue<string> queue = new Collections.Queue<string>();

            foreach (string item in items) {
                queue.Enqueue(item);
            }

            Assert.AreEqual(items[0], queue.Peek());
        }

        [Test]
        public void Synchronized()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            Collections.Queue<string> queue = new Collections.Queue<string>();

            foreach (string item in items) {
                queue.Enqueue(item);
            }

            Collections.Queue<string> syncQueue = Collections.Queue<string>.Synchronized(queue);

            Assert.AreEqual(queue.Count, syncQueue.Count);
            Assert.AreEqual(true, syncQueue.IsSynchronized);
            Assert.AreEqual(queue.SyncRoot, syncQueue.SyncRoot);
        }

        [Test]
        public void ToArray()
        {
            string[] items = { "Horse", "Cat", "Gorilla", "Dog", "Cow" };
            Collections.Queue<string> queue = new Collections.Queue<string>();

            foreach (string item in items) {
                queue.Enqueue(item);
            }

            object[] array = queue.ToArray();
            Assert.AreEqual(items.Length, array.Length);

            for (int i = 0; i < items.Length; i++) {
                Assert.AreEqual(items[i], array[i]);
            }
        }
        #endregion unit test methods

        #region inner classes
        private sealed class LengthComparer : IComparer<String>
        {
            int IComparer<String>.Compare(string x, string y)
            {
                return x.Length - y.Length;
            }
        }
        #endregion inner classes
    }
}
#endif
