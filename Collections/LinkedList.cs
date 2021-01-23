//------------------------------------------------------------------------------
// <sourcefile name="LinkedList.cs" language="C#" begin="05/31/2003">
//
//     <author name="Giuseppe Greco" email="giuseppe.greco@agamura.com" />
//
//     <copyright company="Agamura" url="http://www.agamura.com">
//         Copyright (C) 2003 Agamura, Inc.  All rights reserved.
//     </copyright>
//
// </sourcefile>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using Gekkota.Properties;
using Gekkota.Utilities;

namespace Gekkota.Collections
{
    /// <summary>
    /// Implements a doubly-linked list with sort support.
    /// </summary>
    /// <example>
    /// The following example shows how to create and initialize a
    /// <c>LinkedList</c> and how to print out its content.
    /// <code>
    /// <![CDATA[
    /// using System;
    /// using Gekkota.Collections;
    ///
    /// public class MyClass
    /// {
    ///     public static void Main()
    ///     {
    ///         //
    ///         // create and initialize a new LinkedList
    ///         //
    ///         LinkedList<string> list = new LinkedList<string>();
    ///         list.Add("Red");
    ///         list.Add("Green");
    ///         list.Add("Blue");
    ///
    ///         //
    ///         // sort the elements of the LinkedList using the IComparable
    ///         // implementation of each element
    ///         //
    ///         list.Sort();
    ///
    ///         //
    ///         // display the content of the LinkedList
    ///         //
    ///         Console.WriteLine("list");
    ///         Console.WriteLine("\tCount:           {0}", list.Count);
    ///         Console.WriteLine("\tIsFixedSize:     {0}", list.IsFixedSize);
    ///         Console.WriteLine("\tIsReadOnly:      {0}", list.IsReadOnly);
    ///         Console.WriteLine("\tIsSynchronized:  {0}", list.IsSynchronized);
    ///         Console.WriteLine("\tIsSorted:        {0}", list.IsSorted);
    ///         Console.WriteLine("\tSorted:          {0}", list.Sorted);
    ///         Console.WriteLine("\tValues:");
    ///         foreach (string element in list) {
    ///             Console.WriteLine("\n\t\t{0}", element);
    ///         }
    ///
    ///         //
    ///         // removes all the elements from the LinkedList
    ///         //
    ///         list.Clear();
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// The code above produces the following output:
    /// <![CDATA[
    /// list
    ///     Count:           3
    ///     IsFixedSize:     false
    ///     IsReadOnly:      false
    ///     IsSynchronized:  false
    ///     IsSorted:        true
    ///     Sorted:          false
    ///     Values:
    ///         Blue
    ///         Green
    ///         Red
    /// ]]>
    /// </example>
    public class LinkedList<T>
        : ICloneable, IList<T>, ICollection<T>, IEnumerable<T>
    {
        #region private fields
        private Node head;
        private Node middle;
        private Node tail;
        private int count;
        private IComparer<T> comparer;
        private bool sorted;
        private bool isSorted;
        #endregion private fields

        #region public properties
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the zero-based index
        /// of the element to get or set.
        /// </param>
        /// <value>
        /// The element at <paramref name="index" />.
        /// </value>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="index" /> is equal to or greater than
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Count" />.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> is empty.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> is
        /// read-only.
        /// </exception>
        public virtual T this[int index]
        {
            get { return OnGet(index, GetNodeAt(index).Value); }
            set {
                OnValidate(value);
                Node node = GetNodeAt(index);
                T currentValue = node.Value;
                OnSet(index, currentValue, value);
                node.Value = value;

                try {
                    OnSetComplete(index, currentValue, value);
                } catch {
                    node.Value = currentValue;
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets or sets the comparer to use for the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <value>
        /// An <see cref="System.Collections.Generic.IComparer&lt;T&gt;" /> that
        /// represents the comparer to use for the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </value>
        /// <example>
        /// The following example shows how to customize the way elements in the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> are compared.
        /// <code>
        /// <![CDATA[
        /// string[] elements = { "Red", "Green", "Blue", "White", "Black" };
        /// LinkedList<string> list = new LinkedList<string>();
        /// list.Comparer = new MyComparer();
        ///
        /// foreach (string element in elements) {
        ///     list.Add(element);
        /// }
        ///
        /// //
        /// // remove all the elements with a length of five
        /// //
        /// for (int i = 0; i < elements.Length; i++) {
        ///     list.Remove(elements[i]);
        /// }
        ///
        /// foreach (string element in list) {
        ///     Console.WriteLine(element);
        /// }
        ///
        /// ...
        ///
        /// public class MyComparer : IComparer
        /// {
        ///     int IComparer.Compare(object x, object y)
        ///     {
        ///         int length = ((string) x).Length;
        ///
        ///         if (length > 5) {
        ///             return 1;
        ///         } else if (length < 5) {
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
        /// ]]>
        /// </example>
        public virtual IComparer<T> Comparer
        {
            get { return comparer; }
            set {
                comparer = value;

                if (sorted) {
                    Sort();
                }
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int32" /> that represents the number of elements
        /// contained in the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </value>
        public virtual int Count
        {
            get { return count; }
        }

        /// <summary>
        /// Specifies whether the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />
        /// has a fixed size.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> has a fixed
        /// size; otherwise, <see langword="false" />.
        /// </value>
        public virtual bool IsFixedSize
        {
            get { return false; }
        }

        /// <summary>
        /// Specifies whether the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />
        /// is read-only.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> is read-only;
        /// otherwise, <see langword="false" />.
        /// </value>
        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the elements of the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> are sorted.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the elements of the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> are sorted;
        /// otherwise, <see langword="false" />.
        /// </value>
        /// <remarks>
        /// The <c>IsSorted</c> property is <see langword="true" /> if the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Sort" /> method
        /// has been explicitly invoked or the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Sorted" />
        /// property is <see langword="true" />.
        /// </remarks>
        public virtual bool IsSorted
        {
            get { return isSorted || Sorted; }
        }

        /// <summary>
        /// Gets a value indicating whether or not access to the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> is
        /// synchronized.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if access to the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> is
        /// synchronized; otherwise, <see langword="false" />.
        /// </value>
        public virtual bool IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> is sorted.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> is sorted;
        /// otherwise, <see langword="false" />.
        /// </value>
        /// <remarks>
        /// If the <c>Sorted</c> property is <see langword="true" /> the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Sort" /> method
        /// does nothing and the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.IsSorted" />
        /// property is always <see langword="true" />.
        /// </remarks>
        public virtual bool Sorted
        {
            get { return sorted; }
            set {
                if (value && !IsSorted) {
                    Sort();
                }

                sorted = value;
            }
        }

        /// <summary>
        /// Gets an <see cref="System.Object" /> that can be used to synchronize
        /// access to the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <value>
        /// An <see cref="System.Object" /> that can be used to synchronize access
        /// to the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </value>
        /// <example>
        /// The following example shows how to lock the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> using
        /// <c>SyncRoot</c> during the entire enumeration.
        /// <code>
        /// <![CDATA[
        /// LinkedList<string> list = new LinkedList<string>();
        /// 
        /// ...
        /// 
        /// lock (list.SyncRoot) {
        ///     foreach (string element in list) {
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

        #region internal properties
        /// <summary>
        /// Gets the first <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" />
        /// in the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <value>
        /// The first <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" />
        /// in the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </value>
        internal virtual Node Head
        {
            get { return head; }
        }

        /// <summary>
        /// Gets the last <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" />
        /// in the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <value>
        /// The last <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" />
        /// in the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </value>
        internal virtual Node Tail
        {
            get { return tail; }
        }
        #endregion internal properties

        #region public constructors
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> class.
        /// </summary>
        public LinkedList() : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> class with
        /// the specified boolean value indicating whether or not the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> is sorted.
        /// </summary>
        /// <param name="sorted">
        /// A <see cref="System.Boolean" /> value indicating whether or not the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> has to be
        /// sorted.
        /// </param>
        public LinkedList(bool sorted)
        {
            this.isSorted = this.sorted = sorted;
        }
        #endregion public constructors

        #region public methods
        /// <summary>
        /// Adds the specified element to the end of the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <value>
        /// The element to add to the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </value>
        /// <returns>
        /// An <see cref="System.Int32" /> that represents the position at which
        /// the new element has been added.
        /// </returns>
        /// <exception cref="System.NotSupportedException">
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> is
        /// read-only.
        /// <para>-or</para>
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> has a
        /// fixed size.
        /// </exception>
        public virtual void Add(T value)
        {
            InternalInsert(GetInsertionIndex(value), value);
        }

        /// <summary>
        /// Removes all the elements from the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <exception cref="System.NotSupportedException">
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> is
        /// read-only.
        /// <para>-or</para>
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> has a
        /// fixed size.
        /// </exception>
        public virtual void Clear()
        {
            OnClear();

            head = null;
            middle = null;
            tail = null;
            count = 0;

            OnClearComplete();
        }

        /// <summary>
        /// Creates a shallow copy of the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <returns>
        /// A shallow copy of the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </returns>
        public virtual LinkedList<T> Clone()
        {
            LinkedList<T> list = new LinkedList<T>();

            //
            // this method creates a shallow copy
            //
            for (Node node = head; node != null; node = node.Next) {
                list.InternalInsert(list.count, node.Value);
            }

            //
            // setting [Sorted] to true causes the LinkedList to perform a
            // QuickSort, which is faster than keeping the list sorted while
            // inserting new elements
            //
            list.Comparer = Comparer;
            list.Sorted = Sorted;

            return list;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Determines whether or not the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> contains the
        /// specified element.
        /// </summary>
        /// <param name="value">
        /// The element to locate in the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="value" /> is found in the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />; otherwise,
        /// <see langword="false" />.
        /// </returns>
        /// <remarks>
        /// The comparation algorithm can be overridden by setting the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Comparer" />
        /// property to a customized implementation of the
        /// <see cref="System.Collections.IComparer" /> interface.
        /// </remarks>
        public virtual bool Contains(T value)
        {
            int index = 0;
            return GetNode(value, out index) != null;
        }

        /// <summary>
        /// Copies the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> to
        /// the specified one-dimensional array, starting at the specified array
        /// index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="System.Array" /> to which the elements
        /// of the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> are
        /// copied.
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
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> is greater
        /// than the available space from <paramref name="index" /> to the end
        /// of <paramref name="array" />.
        /// <para>-or-</para>
        /// <paramref name="array" /> is multidimensional.
        /// </exception>
        /// <example>
        /// The following example shows how to copy a
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> into a
        /// one-dimensional <see cref="System.Array" />.
        /// <code>
        /// <![CDATA[
        /// //
        /// // create and initialize a new LinkedList
        /// //
        /// LinkedList<string> list = new LinkedList<string>();
        /// list.Add("Red");
        /// list.Add("Green");
        /// list.Add("Blue");
        ///
        /// string[] array = new string[list.Length];
        /// list.CopyTo(array, 0);
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
            BoundsChecker.Check("array", array, index, count);

            for (Node node = head; node != null; node = node.Next) {
                array[index++] = node.Value;
            }
        }

        /// <summary>
        /// Returns a fixed-size wrapper for the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <param name="list">
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> to wrap.
        /// </param>
        /// <returns>
        /// A fixed-size wrapper around the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="list" /> is <see langword="null" />.
        /// </exception>
        /// <example>
        /// The following example shows how to create a fixed-size wrapper
        /// around a <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// <code>
        /// <![CDATA[
        /// //
        /// // create and initialize a new LinkedList
        /// //
        /// LinkedList<string> list = new LinkedList<string>();
        /// list.Add("Red");
        /// list.Add("Green");
        /// list.Add("Blue");
        ///
        /// LinkedList<string> fixedSizeList = LinkedList<string>.FixedSize(list);
        ///
        /// //
        /// // replacing existing elements is allowed in the fixed-size LinkedList
        /// //
        /// fixedSizeList[2] = "White";
        ///
        /// //
        /// // sort and reverse is allowed in the fixed-size LinkedList
        /// //
        /// fixedSizeList.Sort();
        /// fixedSizeList.Reverse();
        ///
        /// //
        /// // adding or inserting elements to the fixed-size LinkedList throws an
        /// // exception
        /// //
        /// try {
        ///     fixedSizeList.Add("Black");
        /// } catch (NotSupportedException e) {
        ///     Console.WriteLine("Error: {0}", e.Message);
        /// }
        /// ]]>
        /// </code>
        /// The code above produces the following output:
        /// <![CDATA[
        /// Error: Cannot add or remove elements.
        /// ]]>
        /// </example>
        public static LinkedList<T> FixedSize(LinkedList<T> list)
        {
            if (list == null) {
                throw new ArgumentNullException("list");
            }

            if (list.IsFixedSize) {
                return list;
            }

            return new FixedSizeLinkedList(list);
        }

        /// <summary>
        /// Returns an enumerator that can iterate through the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <returns>
        /// A <see cref="Gekkota.Collections.NodeEnumerator&lt;T&gt;" />
        /// for the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </returns>
        public virtual IEnumerator<T> GetEnumerator()
        {
            return new NodeEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Determines the index of the specified element in the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <param name="value">
        /// The element to locate in the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </param>
        /// <returns>
        /// An <see cref="System.Int32" /> that represents the index of
        /// <paramref name="value" /> if found in the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />; otherwise, -1.
        /// </returns>
        /// <remarks>
        /// The comparation algorithm can be overridden by setting the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Comparer" />
        /// property to a customized implementation of the
        /// <see cref="System.Collections.IComparer" /> interface.
        /// </remarks>
        /// <example>
        /// The following example shows how to determine the index of a
        /// specified element.
        /// <code>
        /// <![CDATA[
        /// //
        /// // create and initialize a new LinkedList
        /// //
        /// LinkedList<string> list = new LinkedList<string>();
        /// list.Add("Red");
        /// list.Add("Green");
        /// list.Add("Blue");
        ///
        /// foreach (string element in list) {
        ///     Console.WriteLine("\"{0}\" is at index {1}.",
        ///         element, list.IndexOf(item);
        /// }
        /// ]]>
        /// </code>
        /// The code above produces the following output:
        /// <![CDATA[
        /// "Red" is at index 0.
        /// "Green" is at index 1.
        /// "Blue" is at index 2.
        /// ]]>
        /// </example>
        public virtual int IndexOf(T value)
        {
            int index = 0;
            GetNode(value, out index);

            return index;
        }

        /// <summary>
        /// Inserts the specified element into the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> at the
        /// specified position.
        /// </summary>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the zero-based index
        /// at which <paramref name="value" /> should be inserted.
        /// </param>
        /// <param name="value">
        /// The element to insert into the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </param>
        /// <exception cref="System.InvalidOperationException">
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> is sorted.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0;
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="index" /> is greater than
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Count" />.
        /// <para>-or-</para>
        /// <paramref name="index" /> is greater than 0 while the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> is empty.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> is
        /// read-only.
        /// <para>-or-</para>
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> has a
        /// fixed size.
        /// </exception>
        /// <example>
        /// The following example shows how to insert elements into the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// <code>
        /// <![CDATA[
        /// //
        /// // create and initialize a new LinkedList
        /// //
        /// LinkedList<string> list = new LinkedList<string>();
        /// list.Insert(0, "Red");
        /// list.Insert(1, "Green");
        /// list.Insert(2, "Blue");
        ///
        /// foreach (string element in list) {
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
        public virtual void Insert(int index, T value)
        {
            if (sorted) {
                throw new InvalidOperationException(
                    Resources.Error_CollectionSorted);
            }

            InternalInsert(index, value);
        }

        /// <summary>
        /// Returns a read-only wrapper for the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <param name="list">
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> to wrap.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="list" /> is <see langword="null" />.
        /// </exception>
        /// <returns>
        /// A read-only wrapper around the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </returns>
        /// <example>
        /// The following example shows how to create a read-only wrapper around
        /// a <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// <code>
        /// <![CDATA[
        /// //
        /// // create and initialize a new LinkedList
        /// //
        /// LinkedList<string> list = new LinkedList<string>();
        /// list.Add("Red");
        /// list.Add("Green");
        /// list.Add("Blue");
        ///
        /// LinkedList<string> readOnlyList = LinkedList<string>.ReadOnly(list);
        ///
        /// //
        /// // any attempt to modify a read-only LinkedList throws an exception
        /// //
        /// try {
        ///     readOnlyList.Sort();
        /// } catch (NotSupportedException e) {
        ///     Console.WriteLine("Error: {0}", e.Message);
        /// }
        /// ]]>
        /// </code>
        /// The code above produces the following output:
        /// <![CDATA[
        /// Error: Instance not modifiable.
        /// ]]>
        /// </example>
        public static LinkedList<T> ReadOnly(LinkedList<T> list)
        {
            if (list == null) {
                throw new ArgumentNullException("list");
            }

            if (list.IsReadOnly) {
                return list;
            }

            return new ReadOnlyLinkedList(list);
        }

        /// <summary>
        /// Removes the first occurrence of the specified value from the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <param name="value">
        /// The value to remove from the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </param>
        /// <exception cref="System.NotSupportedException">
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> is
        /// read-only.
        /// <para>-or-</para>
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> has a
        /// fixed size.
        /// </exception>
        /// <example>
        /// The following example shows how to remove elements from a
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// <code>
        /// <![CDATA[
        /// //
        /// // create and initialize a new LinkedList
        /// //
        /// LinkedList<string> list = new LinkedList<string>();
        /// list.Add("Red");
        /// list.Add("Green");
        /// list.Add("Blue");
        /// list.Add("White");
        /// list.Add("Black");
        ///
        /// list.Remove("White");
        /// list.Remove("Black");
        ///
        /// foreach (string element in list) {
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
            OnValidate(value);

            int index = 0;
            Node node = GetNode(value, out index);

            if (node == null) {
                return false;
            }

            Remove(node, index);
            return true;
        }

        /// <summary>
        /// Removes the element at the specified index from the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the zero-based index
        /// of the element to remove from the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="index" /> is equal to or greater than
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Count" />.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> is empty.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> is
        /// read-only.
        /// <para>-or-</para>
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> has a
        /// fixed size.
        /// </exception>
        /// <example>
        /// The following example shows how to remove elements at specified
        /// indexes from a <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// <code>
        /// <![CDATA[
        /// //
        /// // create and initialize a new LinkedList
        /// //
        /// LinkedList<string> list = new LinkedList<string>();
        /// list.Add("Red");
        /// list.Add("Green");
        /// list.Add("Blue");
        /// list.Add("White");
        /// list.Add("Black");
        ///
        /// list.RemoveAt(3);
        /// list.RemoveAt(4);
        ///
        /// foreach (string element in list) {
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
        public virtual void RemoveAt(int index)
        {
            Node node = GetNodeAt(index);

            OnValidate(node.Value);
            Remove(node, index);
        }

        /// <summary>
        /// Reverses the order of the elements in the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <exception cref="System.NotSupportedException">
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> is
        /// read-only.
        /// </exception>
        /// <example>
        /// The following example shows how to reverse the order of the elements
        /// in the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// <code>
        /// <![CDATA[
        /// //
        /// // create and initialize a new LinkedList
        /// //
        /// LinkedList<string> list = new LinkedList<string>();
        /// list.Add("Red");
        /// list.Add("Green");
        /// list.Add("Blue");
        /// list.Add("White");
        /// list.Add("Black");
        ///
        /// Console.WriteLine("The LinkedList before reversing:");
        /// foreach (string element in list) {
        ///     Console.WriteLine("\t{0}", element);
        /// }
        ///
        /// list.Reverse();
        ///
        /// Console.WriteLine("\nThe LinkedList after reversing:");
        /// foreach (string element in list) {
        ///     Console.WriteLine("\t{0}", element);
        /// }
        /// ]]>
        /// </code>
        /// The code above produces the following output:
        /// <![CDATA[
        /// The LinkedList before reversion:
        ///     Red
        ///     Green
        ///     Blue
        ///     White
        ///     Black
        ///
        /// The LinkedList after reversing:
        ///     Black
        ///     White
        ///     Blue
        ///     Green
        ///     Red
        /// ]]>
        /// </example>
        public virtual void Reverse()
        {
            if (count > 1) {
                for (Node first = head, last = tail;
                    first != middle;
                    first = first.Next, last = last.Prev) {
                    Swap(first, last);
                }
            }
        }

        /// <summary>
        /// Sorts the elements of the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> using the
        /// <see cref="System.IComparable" /> implementation of each element.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// One or more elements in the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> do not
        /// implement the <see cref="System.IComparable" /> interface.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> is
        /// read-only.
        /// </exception>
        /// <remarks>
        /// The <c>Sort</c> method uses the QuickSort algorithm.
        /// </remarks>
        /// <example>
        /// The following example shows how to sort the elements in the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// <code>
        /// <![CDATA[
        /// //
        /// // create and initialize a new LinkedList
        /// //
        /// LinkedList<string> list = new LinkedList<string>();
        /// list.Add("Red");
        /// list.Add("Green");
        /// list.Add("Blue");
        /// list.Add("White");
        /// list.Add("Black");
        ///
        /// Console.WriteLine("The LinkedList before sorting:");
        /// foreach (string element in list) {
        ///     Console.WriteLine("\t{0}", element);
        /// }
        ///
        /// list.Sort();
        ///
        /// Console.WriteLine("\nThe LinkedList after sorting:");
        /// foreach (string element in list) {
        ///     Console.WriteLine("\t{0}", element);
        /// }
        /// ]]>
        /// </code>
        /// The code above produces the following output:
        /// <![CDATA[
        /// The LinkedList before sorting:
        ///     Red
        ///     Green
        ///     Blue
        ///     White
        ///     Black
        ///
        /// The LinkedList after sorting:
        ///     Black
        ///     Blue
        ///     Green
        ///     Red
        ///     White
        /// ]]>
        /// </example>
        public virtual void Sort()
        {
            if (count > 1 && !Sorted) {
                QuickSort(head, tail, comparer);
            }

            isSorted = true;
        }

        /// <summary>
        /// Returns a synchronized wrapper for the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <param name="list">
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> to wrap.
        /// </param>
        /// <returns>
        /// A synchronized wrapper around the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="list" /> is <see langword="null" />.
        /// </exception>
        /// <example>
        /// The following example shows how to create a synchronized wrapper
        /// around a <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// <code>
        /// <![CDATA[
        /// LinkedList<string> list = new LinkedList<string>();
        /// Console.WriteLine("list is {0}.",
        ///     list.IsSynchronized ? "synchronized" : "not synchronized");
        ///
        /// LinkedList<string> syncList = LinkedList<string>.Synchronized(list);
        /// Console.WriteLine("list is {0}.",
        ///     syncList.IsSynchronized ? "synchronized" : "not synchronized");
        /// ]]>
        /// </code>
        /// The code above produces the following output:
        /// <![CDATA[
        /// list is not synchronized.
        /// list is synchronized.
        /// ]]>
        /// </example>
        public static LinkedList<T> Synchronized(LinkedList<T> list)
        {
            if (list == null) {
                throw new ArgumentNullException("list");
            }

            if (list.IsSynchronized) {
                return list;
            }

            return new SyncLinkedList(list);
        }
        #endregion public methods

        #region protected methods
        /// <summary>
        /// Performs additional custom processes before clearing the contents of
        /// the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <remarks>
        /// The default implementation of the <c>OnClear</c> method does
        /// nothing. Derived classes can override it to perform some action
        /// before the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> is
        /// cleared.
        /// </remarks>
        protected virtual void OnClear()
        {
        }

        /// <summary>
        /// Performs additional custom processes after clearing the contents of
        /// the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <remarks>
        /// The default implementation of the <c>OnClearComplete</c> method does
        /// nothing. Derived classes can override it to perform some action
        /// after the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> is
        /// cleared.
        /// </remarks>
        protected virtual void OnClearComplete()
        {
        }

        /// <summary>
        /// Performs additional custom processes before getting the element at
        /// the specified index in the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the zero-based index
        /// of the element to get.
        /// </param>
        /// <param name="currentValue">
        /// The current element at <paramref name="index" />.
        /// </param>
        /// <returns>
        /// The element at <paramref name="index" />.
        /// </returns>
        /// <remarks>
        /// The default implementation of the <c>OnGet</c> method returns
        /// <paramref name="currentValue" />. Derived classes can override it to
        /// perform additional action when the specified element is retrieved.
        /// </remarks>
        protected virtual T OnGet(int index, T currentValue)
        {
            return currentValue;
        }

        /// <summary>
        /// Performs additional custom processes before inserting a new element
        /// into the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the zero-based index
        /// at which <paramref name="value" /> is to be inserted.
        /// </param>
        /// <param name="value">
        /// The element to insert into the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </param>
        /// <remarks>
        /// The default implementation of the <c>OnInsert</c> method does
        /// nothing. Derived classes can override it to perform some action
        /// before the specified element is inserted.
        /// </remarks>
        protected virtual void OnInsert(int index, T value)
        {
        }

        /// <summary>
        /// Performs additional custom processes after inserting a new element
        /// into the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the zero-based index
        /// at which <paramref name="value" /> has been inserted.
        /// </param>
        /// <param name="value">
        /// The element inserted into the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </param>
        /// <remarks>
        /// The default implementation of the <c>OnInsertComplete</c> method
        /// does nothing. Derived classes can override it to perform some action
        /// after the specified element is inserted.
        /// </remarks>
        protected virtual void OnInsertComplete(int index, T value)
        {
        }

        /// <summary>
        /// Performs additional custom processes before removing an element from
        /// the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the zero-based index
        /// of the element to remove.
        /// </param>
        /// <param name="value">
        /// The element to remove from the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </param>
        /// <remarks>
        /// The default implementation of the <c>OnRemove</c> method does
        /// nothing. Derived classes can override it to perform some action
        /// before the specified element is removed.
        /// </remarks>
        protected virtual void OnRemove(int index, T value)
        {
        }

        /// <summary>
        /// Performs additional custom processes after removing an element from
        /// the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the zero-based index
        /// of the removed element.
        /// </param>
        /// <param name="value">
        /// The element removed from the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </param>
        /// <remarks>
        /// The default implementation of the <c>OnRemoveComplete</c> method
        /// does nothing. Derived classes can override it to perform some action
        /// after the specified element is removed.
        /// </remarks>
        protected virtual void OnRemoveComplete(int index, T value)
        {
        }

        /// <summary>
        /// Performs additional custom processes before setting an element in
        /// the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the zero-based index
        /// of the element to locate.
        /// </param>
        /// <param name="oldValue">
        /// The current element at <paramref name="index" />.
        /// </param>
        /// <param name="newValue">
        /// The element to set at <paramref name="index" />.
        /// </param>
        /// <remarks>
        /// The default implementation of the <c>OnSet</c> method does nothing.
        /// Derived classes can override it to perform some action before the
        /// specified element is set.
        /// </remarks>
        protected virtual void OnSet(int index, T oldValue, T newValue)
        {
        }

        /// <summary>
        /// Performs additional custom processes after setting an element in the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the zero-based index
        /// of the element to locate.
        /// </param>
        /// <param name="oldValue">
        /// The old element at <paramref name="index" />.
        /// </param>
        /// <param name="newValue">
        /// The new element at <paramref name="index" />.
        /// </param>
        /// <remarks>
        /// The default implementation of the <c>OnSetComplete</c> method does
        /// nothing. Derived classes can override it to perform some action
        /// after the specified element is set.
        /// </remarks>
        protected virtual void OnSetComplete(int index, T oldValue, T newValue)
        {
        }

        /// <summary>
        /// Performs additional custom processes when validating the specified
        /// element.
        /// </summary>
        /// <param name="value">
        /// The element to validate.
        /// </param>
        /// <remarks>
        /// The default implementation of the <c>OnValidate</c> method does
        /// nothing. Derived classes can override it to perform some action when
        /// the specified element is validated.
        /// </remarks>
        protected virtual void OnValidate(T value)
        {
        }
        #endregion protected methods

        #region internal methods
        /// <summary>
        /// Returns the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" />
        /// that contains the specified value.
        /// </summary>
        /// <param name="value">
        /// The value contained by the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" /> to
        /// locate in the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </param>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the zero-based index
        /// of the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" />
        /// that contains <paramref name="value" />, if found; otherwise, -1.
        /// </param>
        /// <returns>
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" /> that
        /// contains <paramref name="value" />, if found; otherwise,
        /// <see langword="null" />.
        /// </returns>
        internal virtual Node GetNode(T value, out int index)
        {
            index = 0;

            for (Node node = head; node != null; node = node.Next) {
                if (Compare(node.Value, value, comparer) == 0) {
                    return node;
                }   index++;
            }

            index = -1;
            return null;
        }

        /// <summary>
        /// Returns the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" />
        /// at the specified index.
        /// </summary>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the zero-based
        /// index of the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" />
        /// to return.
        /// </param>
        /// <returns>
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" /> at
        /// <paramref name="index" />.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="index" /> is equal to or greater than
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Count" />.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> is empty.
        /// </exception>
        internal virtual Node GetNodeAt(int index)
        {
            if (count == 0) {
                throw new InvalidOperationException(
                    Resources.Error_DatagramEmpty);
            }

            if (index < 0) {
                throw new ArgumentOutOfRangeException(
                    "index", index, Resources.Error_NonNegativeNumberRequired);
            }

            if (index >= count) {
                throw new ArgumentException(
                    Resources.Error_IndexOutOfBounds);
            }

            if (index == 0) return head;
            if (index == (count - 1)) return tail;

            int i2 = count >> 1;
            if (index == i2) return middle;

            Node node = null;
            int i = 0;
            int i4 = count >> 2;

            if ((index > i4 && index < i2) || (index > (i2 + i4))) {
                if (index < i2) {
                    node = middle.Prev;
                    i = i2 - 1;
                } else {
                    node = tail.Prev;
                    i = count - 2;
                }

                for (; i > index && node != null; i--) {
                    node = node.Prev;
                }
            } else {
                if (index > i2) {
                    node = middle.Next;
                    i = i2 + 1;
                } else {
                    node = head.Next;
                    i = 1;
                }

                for (; i < index && node != null; i++) {
                    node = node.Next;
                }
            }

            return node;
        }

        /// <summary>
        /// Inserts the specified element into the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> at the
        /// specified position.
        /// </summary>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the zero-based index
        /// at which <paramref name="value" /> should be inserted.
        /// </param>
        /// <param name="value">
        /// The element to insert into the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0;
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="index" /> is greater than
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Count" />.
        /// <para>-or-</para>
        /// <paramref name="index" /> is greater than 0 while the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> is empty.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> is
        /// read-only.
        /// <para>-or-</para>
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> has a
        /// fixed size.
        /// </exception>
        internal virtual void InternalInsert(int index, T value)
        {
            if (count == 0 && index > 0) {
                throw new ArgumentException(
                    Resources.Error_IndexOutOfBounds);
            }

            Node node = index == count ? null : GetNodeAt(index);
            Node newNode = null;

            OnValidate(value);
            OnInsert(index, value);

            if (node == null) {
                newNode = new Node(value, tail, null);
                if (tail != null) { tail.Next = newNode; }
                tail = newNode;
            } else {
                newNode = new Node(value, node.Prev, node);
                if (node != head) { node.Prev.Next = newNode; }
                node.Prev = newNode;
            }
            
            if (node == head) { head = newNode; }
            count++;

            if (middle == null) {
                middle = newNode;
            } else {
                int remainder = 0;
                int quotient = Math.DivRem(count, 2, out remainder);

                if (remainder > 0) {
                    if (quotient >= index && middle != head) { middle = middle.Prev; }
                } else {
                    if (quotient <= index && middle != tail) { middle = middle.Next; }
                }
            }

            try {
                OnInsertComplete(index, value);
            } catch {
                Remove(newNode, index);
                throw;
            }

            isSorted = false;
        }
        #endregion internal methods

        #region private methods
        /// <summary>
        /// Compares the specified values using the specified comparer.
        /// </summary>
        /// <param name="value1">
        /// The value to compare with <paramref name="value2" />.
        /// </param>
        /// <param name="value2">
        /// The value to compare with <paramref name="value1" />.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="System.Collections.IComparer" /> implementation to be
        /// used to compare <paramref name="value1" /> and
        /// <paramref name="value2" />.
        /// </param>
        /// <returns>
        /// Less than 0 if <paramref name="value1" /> is less than
        /// <paramref name="value2" />.
        /// <para>-or-</para>
        /// Zero if <paramref name="value1" /> equals
        /// <paramref name="value2" />.
        /// <para>-or-</para>
        /// Greater than 0 if <paramref name="value1" /> is greater than
        /// <paramref name="value2" />.
        /// </returns>
        private int Compare(T value1, T value2, IComparer<T> comparer)
        {
            if (value1 == null) {
                return value2 == null ? 0 : -1;
            } else if (value2 == null) {
                return 1;
            } else if (comparer == null) {
                return ((IComparable) value1).CompareTo(value2);
            } else {
                return comparer.Compare(value1, value2);
            }
        }

        /// <summary>
        /// Returns the position at which the specified value should be inserted
        /// into the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />
        /// </summary>
        /// <param name="value">
        /// The value being inserted.
        /// </param>
        /// <returns>
        /// An <see cref="System.Int32" /> that represents the position at
        /// which <paramref name="value" /> should be inserted.
        /// </returns>
        private int GetInsertionIndex(T value)
        {
            if (!sorted) { return count; }

            int left = 0;
            int right = count - 1;
            int guess = 0;

            while (left <= right) {
                guess = (left + right) >> 1;

                if (Compare(GetNodeAt(guess).Value, value, comparer) > 0) {
                    right = guess - 1;
                } else {
                    left = guess + 1;
                }
            }

            return left;
        }

        /// <summary>
        /// Implements the QuickSort algorithm.
        /// </summary>
        /// <param name="first">
        /// The first <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" />
        /// of the sequence to sort.
        /// </param>
        /// <param name="last">
        /// The last <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" />
        /// of the sequence to sort.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="System.Collections.IComparer" /> implementation to
        /// use when comparing
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" /> values.
        /// <para>-or-</para>
        /// <see langword="null" /> to use the <see cref="System.IComparable" />
        /// implementation of each
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" /> value.
        /// </param>
        /// <exception cref="System.InvalidOperationException">
        /// <paramref name="comparer" /> is <see langword="null" /> and one or
        /// more <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" />
        /// values in the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />
        /// do not implement the <see cref="System.IComparable" /> interface.
        /// </exception>
        private void QuickSort(Node first, Node last, IComparer<T> comparer)
        {
            if (first != last && first != null &&
                first.Next != null &&  last != null) {

                Node pivot = first;
                Node right = first;

                while (right != last) {
                    if (right.Next != null) {
                        if (Compare(right.Next.Value, first.Value, comparer) < 0) {
                            Swap(right.Next, pivot.Next);
                            pivot = pivot.Next;
                        }
                        right = right.Next;
                    } else { return; }
                }

                Swap(first, pivot);
                QuickSort(first, pivot.Prev, comparer);
                QuickSort(pivot.Next, last, comparer);
            }
        }

        /// <summary>
        /// Removes the specified
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" /> from the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <param name="node">
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" /> to
        /// remove.
        /// </param>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the zero-based index
        /// of <paramref name="node" />.
        /// </param>
        private void Remove(Node node, int index)
        {
            OnRemove(index, node.Value);

            if (count > 1) {
                if (node.Prev != null) { node.Prev.Next = node.Next; }
                if (node.Next != null) { node.Next.Prev = node.Prev; }

                if (node == head) { head = node.Next; }
                if (node == tail) { tail = node.Prev; }

                int remainder = 0;
                int quotient = Math.DivRem(count, 2, out remainder);

                if (remainder > 0) {
                    if (quotient >= index) { middle = middle.Next; }
                } else {
                    if (quotient <= index) { middle = middle.Prev; }
                }

                count--;
            } else { Clear(); }

            OnRemoveComplete(index, node.Value);
        }

        /// <summary>
        /// Swaps the specified nodes.
        /// </summary>
        /// <param name="node1">
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" /> to
        /// swap with <paramref name="node2" />.
        /// </param>
        /// <param name="node2">
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" /> to
        /// swap with <paramref name="node1" />.
        /// </param>
        private void Swap(Node node1, Node node2)
        {
            T value = node1.Value;
            node1.Value = node2.Value;
            node2.Value = value;
        }
        #endregion private methods

        #region inner classes
        /// <summary>
        /// Represents a node in the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        internal sealed class Node
        {
            /// <summary>
            /// The value of the current
            /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" />.
            /// </summary>
            public T Value;

            /// <summary>
            /// The previous <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" />.
            /// </summary>
            /// <remarks>
            /// The <c>Prev</c> field is <see langword="null" /> if the current
            /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" /> is
            /// the first one.
            /// </remarks>
            public Node Prev;

            /// <summary>
            /// The next node.
            /// </summary>
            /// <remarks>
            /// The <c>Next</c> field is <see langword="null" /> if the current
            /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" /> is
            /// the last one.
            /// </remarks>
            public Node Next;

            /// <summary>
            /// Initialized a new instance of the
            /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" />
            /// class.
            /// </summary>
            public Node()
            {
            }

            /// <summary>
            /// Initialized a new instance of the
            /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" />
            /// class with the specified value, previous node, and next node.
            /// </summary>
            /// <param name="value">
            /// The value of the node.
            /// </param>
            /// <param name="prev">
            /// The previous node.
            /// </param>
            /// <param name="next">
            /// The next node.
            /// </param>
            public Node(T value, Node prev, Node next)
            {
                Value = value;
                Prev = prev;
                Next = next;
            }
        }

        /// <summary>
        /// Implements a wrapper for the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        private class LinkedListWrapper : LinkedList<T>
        {
            private LinkedList<T> list;

            public override T this[int index]
            {
                get { return list[index]; }
                set { list[index] = value; }
            }

            public override IComparer<T> Comparer
            {
                get { return list.Comparer; }
                set { list.Comparer = value; }
            }

            public override int Count
            {
                get { return list.Count; }
            }

            public override bool IsFixedSize
            {
                get { return list.IsFixedSize; }
            }

            public override bool IsReadOnly
            {
                get { return list.IsReadOnly; }
            }

            public override bool IsSorted
            {
                get { return list.IsSorted; }
            }

            public override bool IsSynchronized
            {
                get { return list.IsSynchronized; }
            }

            public override bool Sorted
            {
                get { return list.Sorted; }
                set { list.Sorted = value; }
            }

            public override object SyncRoot
            {
                get { return list.SyncRoot; }
            }

            internal override Node Head
            {
                get { return list.Head; }
            }

            internal override Node Tail
            {
                get { return list.Tail; }
            }

            public LinkedListWrapper(LinkedList<T> list)
            {
                this.list = list;
            }

            public override void Add(T value)
            {
                list.Add(value);
            }

            public override void Clear()
            {
                list.Clear();
            }

            public override LinkedList<T> Clone()
            {
                return list.Clone();
            }

            public override bool Contains(T value)
            {
                return list.Contains(value);
            }

            public override void CopyTo(T[] array, int index)
            {
                list.CopyTo(array, index);
            }

            public override IEnumerator<T> GetEnumerator()
            {
                return list.GetEnumerator();
            }

            public override int IndexOf(T value)
            {
                return list.IndexOf(value);
            }

            public override void Insert(int index, T value)
            {
                list.Insert(index, value);
            }

            public override bool Remove(T value)
            {
                return list.Remove(value);
            }

            public override void RemoveAt(int index)
            {
                list.RemoveAt(index);
            }

            public override void Reverse()
            {
                list.Reverse();
            }

            public override void Sort()
            {
                list.Sort();
            }

            internal override Node GetNode(T value, out int index)
            {
                return list.GetNode(value, out index);
            }

            internal override Node GetNodeAt(int index)
            {
                return list.GetNodeAt(index);
            }
        }

        /// <summary>
        /// Implements a fixed-size wrapper for the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        private class FixedSizeLinkedList : LinkedListWrapper
        {
            public override bool IsFixedSize
            {
                get { return true; }
            }

            public FixedSizeLinkedList(LinkedList<T> list) : base(list) {}

            public override void Add(T value)
            {
                throw new NotSupportedException(
                    Resources.Error_CannotAddOrRemove);
            }

            public override void Clear()
            {
                throw new NotSupportedException(
                    Resources.Error_CannotAddOrRemove);
            }

            public override LinkedList<T> Clone()
            {
                return new FixedSizeLinkedList(base.Clone());
            }

            public override void Insert(int index, T value)
            {
                throw new NotSupportedException(
                    Resources.Error_CannotAddOrRemove);
            }

            public override bool Remove(T value)
            {
                throw new NotSupportedException(
                    Resources.Error_CannotAddOrRemove);
            }

            public override void RemoveAt(int index)
            {
                throw new NotSupportedException(
                    Resources.Error_CannotAddOrRemove);
            }
        }

        /// <summary>
        /// Implements a read-only wrapper for the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        private sealed class ReadOnlyLinkedList : FixedSizeLinkedList
        {
            public override T this[int index]
            {
                get { return base[index]; }
                set {
                    throw new NotSupportedException(
                        Resources.Error_InstanceNotModifiable);
                }
            }

            public override IComparer<T> Comparer
            {
                get { return base.Comparer; }
                set {
                    throw new NotSupportedException(
                        Resources.Error_InstanceNotModifiable);
                }
            }

            public override bool IsReadOnly
            {
                get { return true; }
            }

            public override bool Sorted
            {
                get { return base.Sorted; }
                set {
                    throw new NotSupportedException(
                        Resources.Error_InstanceNotModifiable);
                }
            }

            public ReadOnlyLinkedList(LinkedList<T> list) : base(list) {}

            public override LinkedList<T> Clone()
            {
                return new ReadOnlyLinkedList(base.Clone());
            }

            public override void Reverse()
            {
                throw new NotSupportedException(
                        Resources.Error_InstanceNotModifiable);
            }

            public override void Sort()
            {
                throw new NotSupportedException(
                        Resources.Error_InstanceNotModifiable);
            }
        }

        /// <summary>
        /// Implements a synchronized wrapper for the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        private sealed class SyncLinkedList : LinkedListWrapper
        {
            public override T this[int index]
            {
                get {
                    lock (SyncRoot) {
                        return base[index];
                    }
                }

                set {
                    lock (SyncRoot) {
                        base[index] = value;
                    }
                }
            }

            public override IComparer<T> Comparer
            {
                get {
                    lock (SyncRoot) {
                        return base.Comparer;
                    }
                }

                set {
                    lock (SyncRoot) {
                        base.Comparer = value;
                    }
                }
            }

            public override int Count
            {
                get {
                    lock (SyncRoot) {
                        return base.Count;
                    }
                }
            }

            public override bool IsSorted
            {
                get {
                    lock (SyncRoot) {
                        return base.IsSorted;
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
                        return base.Sorted;
                    }
                }

                set {
                    lock (SyncRoot) {
                        base.Sorted = value;
                    }
                }
            }

            internal override Node Head
            {
                get {
                    lock (SyncRoot) {
                        return base.Head;
                    }
                }
            }

            internal override Node Tail
            {
                get {
                    lock (SyncRoot) {
                        return base.Tail;
                    }
                }
            }

            public SyncLinkedList(LinkedList<T> list) : base(list) {}

            public override void Add(T value)
            {
                lock (SyncRoot) {
                    base.Add(value);
                }
            }

            public override void Clear()
            {
                lock (SyncRoot) {
                    base.Clear();
                }
            }

            public override LinkedList<T> Clone()
            {
                lock (SyncRoot) {
                    return new SyncLinkedList(base.Clone());
                }
            }

            public override void CopyTo(T[] array, int index)
            {
                lock (SyncRoot) {
                    base.CopyTo(array, index);
                }
            }

            public override bool Contains(T value)
            {
                lock (SyncRoot) {
                    return base.Contains(value);
                }
            }

            public override IEnumerator<T> GetEnumerator()
            {
                lock (SyncRoot) {
                    return base.GetEnumerator();
                }
            }

            public override int IndexOf(T value)
            {
                lock (SyncRoot) {
                    return base.IndexOf(value);
                }
            }

            public override void Insert(int index, T value)
            {
                lock (SyncRoot) {
                    base.Insert(index, value);
                }
            }

            public override bool Remove(T value)
            {
                lock (SyncRoot) {
                    return base.Remove(value);
                }
            }

            public override void RemoveAt(int index)
            {
                lock (SyncRoot) {
                    base.RemoveAt(index);
                }
            }

            public override void Reverse()
            {
                lock (SyncRoot) {
                    base.Reverse();
                }
            }

            public override void Sort()
            {
                lock (SyncRoot) {
                    base.Sort();
                }
            }

            internal override Node GetNode(T value, out int index)
            {
                lock (SyncRoot) {
                    return base.GetNode(value, out index);
                }
            }

            internal override Node GetNodeAt(int index)
            {
                lock (SyncRoot) {
                    return base.GetNodeAt(index);
                }
            }
        }
        #endregion inner classes
    }
}
