//------------------------------------------------------------------------------
// <sourcefile name="Queue.cs" language="C#" begin="08/03/2005">
//
//     <author name="Giuseppe Greco" email="giuseppe.greco@agamura.com" />
//
//     <copyright company="Agamura" url="http://www.agamura.com">
//         Copyright (C) 2005 Agamura, Inc.  All rights reserved.
//     </copyright>
//
// </sourcefile>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using Gekkota.Utilities;

namespace Gekkota.Collections
{
    /// <summary>
    /// Implements a queue with sort support.
    /// </summary>
    /// <example>
    /// The following example shows how to create and initialize a
    /// sorted <c>Queue</c> and how to print out its content.
    /// <code>
    /// <![CDATA[
    /// using System;
    /// using System.Collections;
    /// using Gekkota.Collections;
    ///
    /// public class MyClass
    /// {
    ///     public static void Main()
    ///     {
    ///         //
    ///         // create and initialize a new prioritized Queue
    ///         //
    ///         Queue<string> queue = new Queue<string>(true);
    ///         queue.Enqueue("Red");
    ///         queue.Enqueue("Green");
    ///         queue.Enqueue("Blue");
    ///
    ///         //
    ///         // display the content of the Queue
    ///         //
    ///         Console.WriteLine("queue");
    ///         Console.WriteLine("\tCount:           {0}", queue.Count);
    ///         Console.WriteLine("\tIsReadOnly:      {0}", queue.IsReadOnly);
    ///         Console.WriteLine("\tIsSynchronized:  {0}", queue.IsSynchronized);
    ///         Console.WriteLine("\tSorted:          {0}", queue.Sorted);
    ///         Console.WriteLine("\tValues:");
    ///         foreach (string element in Queue) {
    ///             Console.WriteLine("\n\t\t{0}", element);
    ///         }
    ///
    ///         //
    ///         // removes all the elements from the Queue
    ///         //
    ///         queue.Clear();
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// The code above produces the following output:
    /// <![CDATA[
    /// queue
    ///     Count:           3
    ///     IsReadOnly:      false
    ///     IsSynchronized:  false
    ///     Sorted:          true
    ///     Values:
    ///         Blue
    ///         Green
    ///         Red
    /// ]]>
    /// </example>
    public class Queue<T> : ICloneable, ICollection, IEnumerable<T>
    {
        #region private fields
        private Collections.LinkedList<T> source;
        #endregion private fields

        #region public properties
        /// <summary>
        /// Gets or sets the comparer to use for the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" />.
        /// </summary>
        /// <value>
        /// An <see cref="System.Collections.Generic.IComparer&lt;T&gt;" /> that
        /// represents the comparer to use for the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" />.
        /// </value>
        /// <example>
        /// The following example shows how to customize the way elements in the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" /> are enqueued.
        /// <code>
        /// <![CDATA[
        /// string[] elements = { "Red", "Green", "Blue" };
        /// Queue<string> queue = new Queue<string>(true);
        /// queue.Comparer = new MyComparer();
        ///
        /// //
        /// // sort elements by length
        /// ///
        /// foreach (string element in elements) {
        ///     queue.Enqueue(element);
        /// }
        ///
        /// foreach (string element in queue) {
        ///     Console.WriteLine(element);
        /// }
        ///
        /// ...
        ///
        /// public class MyComparer : IComparer
        /// {
        ///     int IComparer.Compare(object x, object y)
        ///     {
        ///         int xLength = ((string) x).Length;
        ///         int yLength = ((string) y).Length;
        ///
        ///         if (xLength > yLength) {
        ///             return 1;
        ///         } else if (xLength < yLength) {
        ///             return -1;
        ///         }
        ///
        ///         return 0;
        ///   }
        /// }
        /// ]]>
        /// </code>
        /// The code above produces the following output:
        /// <![CDATA[
        /// Red
        /// Blue
        /// Green
        /// ]]>
        /// </example>
        public virtual IComparer<T> Comparer
        {
            get { return source.Comparer; }
            set { source.Comparer = value; }
        }

        /// <summary>
        /// Gets the number of elements contained in the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" />.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int32" /> that represents the number of elements
        /// contained in the <see cref="Gekkota.Collections.Queue&lt;T&gt;" />.
        /// </value>
        public virtual int Count
        {
            get { return source.Count; }
        }

        /// <summary>
        /// Specifies whether the <see cref="Gekkota.Collections.Queue&lt;T&gt;" />
        /// is read-only.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" /> is read-only;
        /// otherwise, <see langword="false" />.
        /// </value>
        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether or not access to the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" /> is
        /// synchronized.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if access to the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" /> is
        /// synchronized; otherwise, <see langword="false" />.
        /// </value>
        public virtual bool IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" /> is sorted.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" /> is sorted;
        /// otherwise, <see langword="false" />.
        /// </value>
        public virtual bool Sorted
        {
            get { return source.Sorted; }
            set { source.Sorted = value; }
        }

        /// <summary>
        /// Gets an <see cref="System.Object" /> that can be used to synchronize
        /// access to the <see cref="Gekkota.Collections.Queue&lt;T&gt;" />.
        /// </summary>
        /// <value>
        /// An <see cref="System.Object" /> that can be used to synchronize access
        /// to the <see cref="Gekkota.Collections.Queue&lt;T&gt;" />.
        /// </value>
        /// <example>
        /// The following example shows how to lock the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" /> using
        /// <c>SyncRoot</c> during the entire enumeration.
        /// <code>
        /// <![CDATA[
        /// Queue<string> queue = new Queue<string>();
        /// 
        /// ...
        /// 
        /// lock (queue.SyncRoot) {
        ///     foreach (string element in queue) {
        ///         //
        ///         // insert your code here
        ///         //
        ///     }
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public virtual object SyncRoot
        {
            get { return this; }
        }
        #endregion public properties

        #region public constructors
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" /> class.
        /// </summary>
        public Queue() : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" /> class with the
        /// specified boolean value indicating whether or not the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" /> is sorted.
        /// </summary>
        /// <param name="sorted">
        /// A <see cref="System.Boolean" /> value indicating whether or not the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" /> has to be sorted.
        /// </param>
        public Queue(bool sorted) : this(new LinkedList<T>(sorted)) {}
        #endregion public constructors

        #region private constructors
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" /> class with the
        /// specified <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <param name="source">
        /// A <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> that
        /// implements the <see cref="Gekkota.Collections.Queue&lt;T&gt;" />
        /// functionality.
        /// </param>
        private Queue(LinkedList<T> source)
        {
            this.source = source;
        }
        #endregion private constructors

        #region public methods
        /// <summary>
        /// Removes all the elements from the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" />.
        /// </summary>
        /// <exception cref="System.NotSupportedException">
        /// The <see cref="Gekkota.Collections.Queue&lt;T&gt;" /> is read-only.
        /// <para>-or</para>
        /// The <see cref="Gekkota.Collections.Queue&lt;T&gt;" /> has a fixed size.
        /// </exception>
        public virtual void Clear()
        {
            source.Clear();
        }

        /// <summary>
        /// Creates a shallow copy of the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" />.
        /// </summary>
        /// <returns>
        /// A shallow copy of the <see cref="Gekkota.Collections.Queue&lt;T&gt;" />.
        /// </returns>
        public virtual Queue<T> Clone()
        {
            return new Queue<T>(source.Clone());
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Determines whether or not the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" /> contains the
        /// specified element.
        /// </summary>
        /// <param name="value">
        /// The element to locate in the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" />.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="value" /> is found in the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" />; otherwise,
        /// <see langword="false" />.
        /// </returns>
        /// <remarks>
        /// The comparation algorithm can be overridden by setting the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;.Comparer" /> property
        /// to a customized implementation of the
        /// <see cref="System.Collections.IComparer" /> interface.
        /// </remarks>
        public virtual bool Contains(T value)
        {
            return source.Contains(value);
        }

        /// <summary>
        /// Copies the <see cref="Gekkota.Collections.Queue&lt;T&gt;" /> to the
        /// specified one-dimensional array, starting at the specified array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional array to which the elements of the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" /> are copied.
        /// </param>
        /// <param name="index">
        /// The zero-based index in <paramref name="array" /> at which copying
        /// begins.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="array" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="index" /> is equal to or greater than the length of
        /// <paramref name="array" />.
        /// <para>-or</para>
        /// The number of elements in the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" /> is greater than
        /// the available space from <paramref name="index" /> to the end of
        /// <paramref name="array" />.
        /// <para>-or-</para>
        /// <paramref name="array" /> is multidimensional.
        /// </exception>
        /// <example>
        /// The following example shows how to copy a
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" /> into a
        /// one-dimensional <see cref="System.Array" />.
        /// <code>
        /// <![CDATA[
        /// //
        /// // create and initialize a new Queue
        /// //
        /// Queue<string> queue = new Queue<string>();
        /// queue.Enqueue("Red");
        /// queue.Enqueue("Green");
        /// queue.Enqueue("Blue");
        ///
        /// string[] array = new string[queue.Length];
        /// queue.CopyTo(array, 0);
        ///
        /// for (int i = 0; i < array.Length; i++) {
        ///     Console.WriteLine(array[i]);
        /// }
        /// ]]>
        /// </code>
        /// The code above produces the following output:
        /// <![CDATA[
        /// Red
        /// Green
        /// Blue
        /// ]]>
        /// </example>
        public virtual void CopyTo(T[] array, int index)
        {
            source.CopyTo(array, index);
        }

        /// <summary>
        /// Copies the <see cref="Gekkota.Collections.Queue&lt;T&gt;" /> to the
        /// specified one-dimensional <see cref="System.Array" />, starting at
        /// the specified array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="System.Array" /> to which the elements
        /// of the <see cref="Gekkota.Collections.Queue&lt;T&gt;" /> are copied.
        /// </param>
        /// <param name="index">
        /// The zero-based index in <paramref name="array" /> at which copying
        /// begins.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="array" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="index" /> is equal to or greater than the length of
        /// <paramref name="array" />.
        /// <para>-or</para>
        /// The number of elements in the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" /> is greater than
        /// the available space from <paramref name="index" /> to the end
        /// of <paramref name="array" />.
        /// <para>-or-</para>
        /// <paramref name="array" /> is multidimensional.
        /// </exception>
        /// <example>
        /// The following example shows how to copy a
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" /> into a
        /// one-dimensional <see cref="System.Array" />.
        /// <code>
        /// <![CDATA[
        /// //
        /// // create and initialize a new Queue
        /// //
        /// Queue<string> queue = new Queue<string>();
        /// queue.Enqueue("Red");
        /// queue.Enqueue("Green");
        /// queue.Enqueue("Blue");
        ///
        /// Array array = Array.CreateInstance(typeof(string), queue.Length);
        /// queue.CopyTo(array, 0);
        ///
        /// for (int i = 0; i < array.Length; i++) {
        ///     Console.WriteLine(array.GetValue(i));
        /// }
        /// ]]>
        /// </code>
        /// The code above produces the following output:
        /// <![CDATA[
        /// Red
        /// Green
        /// Blue
        /// ]]>
        /// </example>
        void ICollection.CopyTo(Array array, int index)
        {
            BoundsChecker.Check("array", array, index, source.Count);

            for (LinkedList<T>.Node node = source.Head; node != null; node = node.Next) {
                array.SetValue(node.Value, index++);
            }
        }

        /// <summary>
        /// Returns an enumerator that can iterate through the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" />.
        /// </summary>
        /// <returns>
        /// A <see cref="Gekkota.Collections.NodeEnumerator&lt;T&gt;" />
        /// for the <see cref="Gekkota.Collections.Queue&lt;T&gt;" />.
        /// </returns>
        public virtual IEnumerator<T> GetEnumerator()
        {
            return source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Removes and returns the element at the beginning of the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" />.
        /// </summary>
        /// <returns>
        /// The element that is removed from the beginning of the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" />.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// The <see cref="Gekkota.Collections.Queue&lt;T&gt;" /> is empty.
        /// </exception>
        /// <example>
        /// The following example shows how to remove elements from a
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" />.
        /// <code>
        /// <![CDATA[
        /// //
        /// // create and initialize a new Queue
        /// //
        /// Queue<string> queue = new Queue<string>();
        /// queue.Enqueue("White");
        /// queue.Enqueue("Black");
        /// queue.Enqueue("Red");
        /// queue.Enqueue("Green");
        /// queue.Enqueue("Blue");
        ///
        /// queue.Dequeue();
        /// queue.Dequeue();
        ///
        /// foreach (string element in queue) {
        ///     Console.WriteLine(element);
        /// }
        /// ]]>
        /// </code>
        /// The code above produces the following output:
        /// <![CDATA[
        /// Red
        /// Green
        /// Blue
        /// ]]>
        /// </example>
        public virtual T Dequeue()
        {
            T value = source[0];
            source.RemoveAt(0);
            return value;
        }

        /// <summary>
        /// Adds the specified value to the end of the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" />.
        /// </summary>
        /// <param name="value">
        /// The value to add to the end of the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" />. The value can be
        /// <see langword="null" />.
        /// </param>
        public virtual void Enqueue(T value)
        {
            source.Add(value);
        }

        /// <summary>
        /// Returns the element at the beginning of the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" /> without removing
        /// it.
        /// </summary>
        /// <returns>
        /// The element at the beginning of the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" />.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// The <see cref="Gekkota.Collections.Queue&lt;T&gt;" /> is empty.
        /// </exception>
        public virtual T Peek()
        {
            return source[0];
        }

        /// <summary>
        /// Removes the first occurrence of the specified value from the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" />.
        /// </summary>
        /// <param name="value">
        /// The value to remove from the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" />.
        /// </param>
        /// <example>
        /// The following example shows how to remove elements from a
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" />.
        /// <code>
        /// <![CDATA[
        /// //
        /// // create and initialize a new Queue
        /// //
        /// Queue<string> queue = new Queue<string>();
        /// queue.Add("Red");
        /// queue.Add("Green");
        /// queue.Add("Blue");
        /// queue.Add("White");
        /// queue.Add("Black");
        ///
        /// queue.Remove("White");
        /// queue.Remove("Black");
        ///
        /// foreach (string element in queue) {
        ///     Console.WriteLine(element);
        /// }
        /// ]]>
        /// </code>
        /// The code above produces the following output:
        /// <![CDATA[
        /// Red
        /// Green
        /// Blue
        /// ]]>
        /// </example>
        public virtual bool Remove(T value)
        {
            return source.Remove(value);
        }

        /// <summary>
        /// Returns a synchronized wrapper for the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" />.
        /// </summary>
        /// <param name="queue">
        /// The <see cref="Gekkota.Collections.Queue&lt;T&gt;" /> to wrap.
        /// </param>
        /// <returns>
        /// A synchronized wrapper around the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="queue" /> is <see langword="null" />.
        /// </exception>
        /// <example>
        /// The following example shows how to create a synchronized wrapper
        /// around a <see cref="Gekkota.Collections.Queue&lt;T&gt;" />.
        /// <code>
        /// <![CDATA[
        /// Queue<string> queue = new Queue<string>();
        /// Console.WriteLine("queue is {0}.",
        ///     queue.IsSynchronized ? "synchronized" : "not synchronized");
        ///
        /// Queue<string> syncQueue = Queue<string>.Synchronized(queue);
        /// Console.WriteLine("queue is {0}.",
        ///     syncQueue.IsSynchronized ? "synchronized" : "not synchronized");
        /// ]]>
        /// </code>
        /// The code above produces the following output:
        /// <![CDATA[
        /// queue is not synchronized.
        /// queue is synchronized.
        /// ]]>
        /// </example>
        public static Queue<T> Synchronized(Queue<T> queue)
        {
            if (queue == null) {
                throw new ArgumentNullException ("queue");
            }

            if (queue.IsSynchronized) {
                return queue;
            }

            return new SyncQueue(queue);
        }

        /// <summary>
        /// Copies the elements of the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" /> to a new array.
        /// </summary>
        /// <returns>
        /// A new array containing elements copied from the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" />.
        /// </returns>
        /// <example>
        /// The following example shows how to copy a
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" /> into a
        /// one-dimensional array.
        /// <code>
        /// <![CDATA[
        /// //
        /// // create and initialize a new Queue
        /// //
        /// Queue<string> queue = new Queue<string>();
        /// queue.Enqueue("Red");
        /// queue.Enqueue("Green");
        /// queue.Enqueue("Blue");
        ///
        /// string[] array = queue.ToArray();
        ///
        /// for (int i = 0; i < array.Length; i++) {
        ///     Console.WriteLine(array[i]);
        /// }
        /// ]]>
        /// </code>
        /// The code above produces the following output:
        /// <![CDATA[
        /// Red
        /// Green
        /// Blue
        /// ]]>
        /// </example>
        public virtual T[] ToArray()
        {
            T[] array = new T[source.Count];
            CopyTo(array, 0);
            return array;
        }
        #endregion public methods

        #region inner classes
        /// <summary>
        /// Implements a synchronized wrapper for the
        /// <see cref="Gekkota.Collections.Queue&lt;T&gt;" />.
        /// </summary>
        private sealed class SyncQueue : Queue<T>
        {
            private Queue<T> queue;

            public override IComparer<T> Comparer
            {
                get {
                    lock (SyncRoot) {
                        return queue.Comparer;
                    }
                }

                set {
                    lock (SyncRoot) {
                        queue.Comparer = value;
                    }
                }
            }

            public override int Count
            {
                get {
                    lock (SyncRoot) {
                        return queue.Count;
                    }
                }
            }

            public override bool IsSynchronized
            {
                get { return true; }
            }

            public override bool Sorted
            {
                get {
                    lock (SyncRoot) {
                        return queue.Sorted;
                    }
                }

                set {
                    lock (SyncRoot) {
                        queue.Sorted = value;
                    }
                }
            }

            public override object SyncRoot
            {
                get { return queue.SyncRoot; }
            }

            public SyncQueue(Queue<T> queue)
            {
                this.queue = queue;
            }

            public override void Clear()
            {
                lock (SyncRoot) {
                    queue.Clear();
                }
            }

            public override Queue<T> Clone()
            {
                lock (SyncRoot) {
                    return new SyncQueue(queue.Clone());
                }
            }

            public override bool Contains(T value)
            {
                lock (SyncRoot) {
                    return queue.Contains(value);
                }
            }

            public override void CopyTo(T[] array, int index)
            {
                queue.CopyTo(array, index);
            }

            public override IEnumerator<T> GetEnumerator()
            {
                lock (SyncRoot) {
                    return queue.GetEnumerator();
                }
            }

            public override T Dequeue()
            {
                lock (SyncRoot) {
                    return queue.Dequeue();
                }
            }

            public override void Enqueue(T value)
            {
                lock (SyncRoot) {
                    queue.Enqueue(value);
                }
            }

            public override T Peek()
            {
                lock (SyncRoot) {
                    return queue.Peek();
                }
            }

            public override bool Remove(T value)
            {
                lock (SyncRoot) {
                    return queue.Remove(value);
                }
            }

            public override T[] ToArray()
            {
                lock (SyncRoot) {
                    return queue.ToArray();
                }
            }
        }
        #endregion inner classes
    }
}
