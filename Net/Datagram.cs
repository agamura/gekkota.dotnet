//------------------------------------------------------------------------------
// <sourcefile name="Datagram.cs" language="C#" begin="06/02/2003">
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
using System.Collections.Generic;
using Gekkota.Collections;
using Gekkota.Properties;

namespace Gekkota.Net
{
    /// <summary>
    /// Represents a datagram.
    /// </summary>
    /// <example>
    /// The following example shows how to create and initialize a <c>Datagram</c>
    /// and how to print out its content.
    /// <code>
    /// <![CDATA[
    /// using System;
    /// using Gekkota.Net;
    ///
    /// public class MyClass
    /// {
    ///   public static void Main()
    ///   {
    ///     //
    ///     // create and initialize a new Datagram
    ///     //
    ///     Datagram datagram = new Datagram();
    ///     datagram.Add(new Field(1, "Red"));
    ///     datagram.Add(new Field(2, "Green"));
    ///     datagram.Add(new Field(3, "Blue"));
    ///
    ///     //
    ///     // display the content of the Datagram
    ///     //
    ///     Console.WriteLine("datagram");
    ///     Console.WriteLine("\tCount:           {0}", datagram.Count);
    ///     Console.WriteLine("\tIsFixedSize:     {0}", datagram.IsFixedSize);
    ///     Console.WriteLine("\tIsReadOnly:      {0}", datagram.IsReadOnly);
    ///     Console.WriteLine("\tIsSynchronized:  {0}", datagram.IsSynchronized);
    ///     Console.WriteLine("\tSize:            {0}", datagram.Size);
    ///     Console.WriteLine("\tFields:");
    ///     foreach (Field field in datagram) {
    ///       Console.WriteLine("\n\t\t{0} {1}", field.Id, field.ValueAsString);
    ///     }
    ///   }
    /// }
    /// ]]>
    /// </code>
    /// The code above produces the following output:
    /// <![CDATA[
    /// datagram
    ///     Count:           3
    ///     IsFixedSize:     false
    ///     IsReadOnly:      false
    ///     IsSynchronized:  false
    ///     Size:            15
    ///     Fields:
    ///         1 Red
    ///         2 Green
    ///         3 Blue
    /// ]]>
    /// </example>
    /// <seealso cref="Gekkota.Collections.LinkedList&lt;T&gt;" />
    /// <seealso cref="Gekkota.Net.Field" />
    public class Datagram : Collections.LinkedList<Field>
    {
        #region private fields
        private int size;
        #endregion private fields

        #region public properties
        /// <summary>
        /// Gets or sets the <see cref="Gekkota.Net.Field" /> at the specified
        /// index.
        /// </summary>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the zero-based index of
        /// the <see cref="Gekkota.Net.Field" /> to get or set.
        /// </param>
        /// <value>
        /// The <see cref="Gekkota.Net.Field" /> at <paramref name="index" />.
        /// </value>
        /// <exception cref="System.ArgumentNullException">
        /// The specified value is <see langword="null" />.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="index" /> is equal to or greater than
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Count" />.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// The <see cref="Gekkota.Net.Datagram" /> is empty.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// The <see cref="Gekkota.Net.Datagram" /> is read-only.
        /// </exception>
        public new virtual Field this[int index]
        {
            get { return (Field) base[index]; }
            set { base[index] = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="Gekkota.Net.Field" /> described by the
        /// specified <see cref="Gekkota.Net.Metafield" />.
        /// </summary>
        /// <param name="metafield">
        /// A <see cref="Gekkota.Net.Metafield" /> that describes the
        /// <see cref="Gekkota.Net.Field" /> to get or set.
        /// </param>
        /// <value>
        /// The <see cref="Gekkota.Net.Field" /> described by
        /// <paramref name="metafield" />.
        /// </value>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="metafield" /> or the specified value is
        /// <see langword="null" />.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <see cref="Gekkota.Net.Field" /> described by
        /// <paramref name="metafield" /> does not exist in the
        /// <see cref="Gekkota.Net.Datagram" />.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// The <see cref="Gekkota.Net.Datagram" /> is empty.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// The <see cref="Gekkota.Net.Datagram" /> is read-only.
        /// </exception>
        public virtual Field this[Metafield metafield]
        {
            get {
                if (Count == 0) {
                    throw new InvalidOperationException(
                        Resources.Error_DatagramEmpty);
                }

                int index = 0;
                Node node = GetNode(metafield, out index);

                if (node == null) {
                    throw new ArgumentException(String.Format(
                        Resources.Error_FieldDoesNotExist, metafield.Id));
                }

                return (Field) node.Value;
            }

            set {
                OnValidate(value);

                if (Count == 0) {
                    throw new InvalidOperationException(
                        Resources.Error_CollectionEmpty);
                }

                int index = 0;
                Node node = GetNode(metafield, out index);

                if (node == null) {
                    throw new ArgumentException(String.Format(
                        Resources.Error_FieldDoesNotExist, metafield.Id));
                }

                node.Value = value;
            }
        }

        /// <summary>
        /// Gets the size of the <see cref="Gekkota.Net.Datagram" />.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int32" /> that represents the size of the
        /// <see cref="Gekkota.Net.Datagram" />, in bytes.
        /// </value>
        public virtual int Size
        {
            get { return size; }
        }
        #endregion public properties

        #region operators
        /// <summary>
        /// Determines whether or not the specified
        /// <see cref="Gekkota.Net.Datagram" /> objects are equal.
        /// </summary>
        /// <param name="datagram1">
        /// The <see cref="Gekkota.Net.Datagram" /> to compare with
        /// <paramref name="datagram2" />.
        /// </param>
        /// <param name="datagram2">
        /// The <see cref="Gekkota.Net.Datagram" /> to compare with
        /// <paramref name="datagram1" />.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="datagram1" /> is equal
        /// to <paramref name="datagram2" />; otherwise, <see langword="false" />.
        /// </returns>
        public static bool operator ==(Datagram datagram1, Datagram datagram2)
        {
            return Equals(datagram1, datagram2);
        }

        /// <summary>
        /// Determines whether or not the specified
        /// <see cref="Gekkota.Net.Datagram" /> objects are not equal.
        /// </summary>
        /// <param name="datagram1">
        /// The <see cref="Gekkota.Net.Datagram" /> to compare with
        /// <paramref name="datagram2" />.
        /// </param>
        /// <param name="datagram2">
        /// The <see cref="Gekkota.Net.Datagram" /> to compare with
        /// <paramref name="datagram1" />.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="datagram1" /> is not equal
        /// to <paramref name="datagram2" />; otherwise, <see langword="false" />.
        /// </returns>
        public static bool operator !=(Datagram datagram1, Datagram datagram2)
        {
            return !Equals(datagram1, datagram2);
        }
        #endregion operators

        #region public constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Gekkota.Net.Datagram" />
        /// class.
        /// </summary>
        public Datagram() : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gekkota.Net.Datagram" />
        /// class with the specified boolean value indicating whether or not the
        /// <see cref="Gekkota.Net.Datagram" /> is sorted.
        /// </summary>
        /// <param name="sorted">
        /// A <see cref="System.Boolean" /> value indicating whether or not the
        /// <see cref="Gekkota.Net.Datagram" /> has to be sorted.
        /// </param>
        public Datagram(bool sorted) : base(sorted)
        {
            base.Comparer = new DefaultComparer();
        }
        #endregion public constructors

        #region public methods
        /// <summary>
        /// Creates a shallow copy of the <see cref="Gekkota.Net.Datagram" />.
        /// </summary>
        /// <returns>
        /// A shallow copy of the <see cref="Gekkota.Net.Datagram" />.
        /// </returns>
        public new virtual Datagram Clone()
        {
            Datagram datagram = new Datagram();

            //
            // this method creates a shallow copy
            //
            for (Node node = Head; node != null; node = node.Next) {
                datagram.InternalInsert(datagram.Count, (Field) node.Value);
            }

            //
            // setting [Sorted] to true causes the LinkedList to perform a
            // QuickSort, which is faster than keeping the list sorted while
            // inserting new elements
            //
            datagram.Comparer = Comparer;
            datagram.Sorted = Sorted;

            return datagram;
        }

        /// <summary>
        /// Determines whether or not the <see cref="Gekkota.Net.Datagram" />
        /// contains the <see cref="Gekkota.Net.Field" /> described by the
        /// specified <see cref="Gekkota.Net.Metafield" />.
        /// </summary>
        /// <param name="metafield">
        /// The <see cref="Gekkota.Net.Metafield" /> that describes the
        /// <see cref="Gekkota.Net.Field" /> to locate in the
        /// <see cref="Gekkota.Net.Datagram" />.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if the <see cref="Gekkota.Net.Field" />
        /// described by <paramref name="metafield" /> is found in the
        /// <see cref="Gekkota.Net.Datagram" />; otherwise, <see langword="false" />.
        /// </returns>
        public virtual bool Contains(Metafield metafield)
        {
            OnValidate(metafield);
            return base.Contains(new Field(null, metafield));
        }

        /// <summary>
        /// Determines whether or not the specified
        /// <see cref="Gekkota.Net.Datagram" /> objects are equal.
        /// </summary>
        /// <param name="datagram1">
        /// The <see cref="Gekkota.Net.Datagram" /> to compare with
        /// <paramref name="datagram2" />.
        /// </param>
        /// <param name="datagram2">
        /// The <see cref="Gekkota.Net.Datagram" /> to compare with
        /// <paramref name="datagram1" />.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="datagram1" /> is equal to
        /// <paramref name="datagram2" />; otherwise, <see langword="false" />.
        /// </returns>
        public static bool Equals(Datagram datagram1, Datagram datagram2)
        {
            if ((datagram1 as object) == (datagram2 as object)) {
                return true;
            }

            if ((datagram1 as object) == null || (datagram2 as object) == null) {
                return false;
            }

            if (datagram1.Size != datagram2.Size ||
                datagram1.Count != datagram2.Count) {
                return false;
            }

            for (int i = 0; i < datagram1.Count; i++) {
                if (!Field.Equals(datagram1[i], datagram2[i])) {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether or not the specified
        /// <see cref="Gekkota.Net.Datagram" /> is equal to the current
        /// <see cref="Gekkota.Net.Datagram" />.
        /// </summary>
        /// <param name="datagram">
        /// The <see cref="Gekkota.Net.Datagram" /> to compare with the current
        /// <see cref="Gekkota.Net.Datagram" />.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="datagram" /> is equal to
        /// the current <see cref="Gekkota.Net.Datagram" />; otherwise,
        /// <see langword="false" />.
        /// </returns>
        public virtual bool Equals(Datagram datagram)
        {
            return Equals(this, datagram);
        }

        /// <summary>
        /// Determines whether or not the specified
        /// <see cref="System.Object" /> is equal to the current
        /// <see cref="Gekkota.Net.Datagram" />.
        /// </summary>
        /// <param name="datagram">
        /// The <see cref="System.Object" /> to compare with the current
        /// <see cref="Gekkota.Net.Datagram" />.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="datagram" /> is equal to
        /// the current <see cref="Gekkota.Net.Datagram" />; otherwise,
        /// <see langword="false" />.
        /// </returns>
        public override bool Equals(object datagram)
        {
            return Equals(this, datagram as Datagram);
        }

        /// <summary>
        /// Returns a fixed-size wrapper for the
        /// <see cref="Gekkota.Net.Datagram" />.
        /// </summary>
        /// <param name="datagram">
        /// The <see cref="Gekkota.Net.Datagram" /> to wrap.
        /// </param>
        /// <returns>
        /// A fixed-size wrapper around the <see cref="Gekkota.Net.Datagram" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="datagram" /> is <see langword="null" />.
        /// </exception>
        /// <example>
        /// The following example shows how to create a fixed-size wrapper
        /// around a <see cref="Gekkota.Net.Datagram" />.
        /// <code>
        /// <![CDATA[
        /// //
        /// // create and initialize a new Datagram
        /// //
        /// Datagram datagram = new Datagram();
        /// datagram.Add(new Field(1, "Red"));
        /// datagram.Add(new Field(2, "Green"));
        /// datagram.Add(new Field(3, "Blue"));
        ///
        /// Datagram fixedSizeDatagram = Datagram.FixedSize(datagram);
        ///
        /// //
        /// // replacing existing fields is allowed in the fixed-size Datagram
        /// //
        /// fixedSizeDatagram[2] = new Field(4, "White");
        ///
        /// //
        /// // sort and reverse is allowed in the fixed-size Datagram
        /// //
        /// fixedSizeDatagram.Sort();
        /// fixedSizeDatagram.Reverse();
        ///
        /// //
        /// // adding or inserting elements to the fixed-size Datagram throws an
        /// // exception
        /// //
        /// try {
        ///   fixedSizeDatagram.Add(new Field(5, "Black"));
        /// } catch (NotSupportedException e) {
        ///   Console.WriteLine("Error: {0}", e.Message);
        /// }
        /// ]]>
        /// </code>
        /// The code above produces the following output:
        /// <![CDATA[
        /// Error: Cannot add or remove elements.
        /// ]]>
        /// </example>
        public static Datagram FixedSize(Datagram datagram)
        {
            if (datagram == null) {
                throw new ArgumentNullException("datagram");
            }

            if (datagram.IsFixedSize) {
                return datagram;
            }

            return new FixedSizeDatagram(datagram);
        }

        /// <summary>
        /// Returns an enumerator that can iterate through the
        /// <see cref="Gekkota.Net.Datagram" />.
        /// </summary>
        /// <returns>
        /// A <see cref="Gekkota.Net.FieldEnumerator" /> for the
        /// <see cref="Gekkota.Net.Datagram" />.
        /// </returns>
        public new virtual FieldEnumerator GetEnumerator()
        {
            return new FieldEnumerator(this);
        }

        /// <summary>
        /// Determines the index of the <see cref="Gekkota.Net.Field" />
        /// described by the specified <see cref="Gekkota.Net.Metafield" /> in
        /// the <see cref="Gekkota.Net.Datagram" />.
        /// </summary>
        /// <param name="metafield">
        /// A <see cref="Gekkota.Net.Metafield" /> that describes the
        /// <see cref="Gekkota.Net.Field" /> to locate in the
        /// <see cref="Gekkota.Net.Datagram" />.
        /// </param>
        /// <returns>
        /// An <see cref="System.Int32" /> that represents the index of the
        /// <see cref="Gekkota.Net.Field" /> described by
        /// <paramref name="metafield" /> if found in the
        /// <see cref="Gekkota.Net.Datagram" />; otherwise, -1.
        /// </returns>
        /// <example>
        /// The following example shows how to determine the index of a
        /// <see cref="Gekkota.Net.Field" /> described by a specified
        /// <see cref="Metafield" />.
        /// <code>
        /// <![CDATA[
        /// //
        /// // create and initialize a new Datagram
        /// //
        /// Datagram datagram = new Datagram();
        /// datagram.Add(new Field(1, "Red"));
        /// datagram.Add(new Field(2, "Green"));
        /// datagram.Add(new Field(3, "Blue"));
        ///
        /// foreach (Field field in datagram) {
        ///   Console.WriteLine("\"{0}\" is at index {1}.",
        ///     field.ValueAsString, datagram.IndexOf(field.GetMetafield());
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
        public virtual int IndexOf(Metafield metafield)
        {
            OnValidate(metafield);
            return base.IndexOf(new Field(null, metafield));
        }

        /// <summary>
        /// Returns a read-only wrapper for the
        /// <see cref="Gekkota.Net.Datagram" />.
        /// </summary>
        /// <param name="datagram">
        /// The <see cref="Gekkota.Net.Datagram" /> to wrap.
        /// </param>
        /// <returns>
        /// A read-only wrapper around the <see cref="Gekkota.Net.Datagram" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="datagram" /> is <see langword="null" />.
        /// </exception>
        /// <example>
        /// The following example shows how to create a read-only wrapper around
        /// a <see cref="Gekkota.Net.Datagram" />.
        /// <code>
        /// <![CDATA[
        /// //
        /// // create and initialize a new Datagram
        /// //
        /// Datagram datagram = new Datagram();
        /// datagram.Add(new Field(1, "Red"));
        /// datagram.Add(new Field(2, "Green"));
        /// datagram.Add(new Field(3, "Blue"));
        ///
        /// Datagram readOnlyDatagram = Datagram.ReadOnly(datagram);
        ///
        /// //
        /// // any attempt to modify a read-only Datagram throws an exception
        /// //
        /// try {
        ///   readOnlyDatagram.Sort();
        /// } catch (NotSupportedException e) {
        ///   Console.WriteLine("Error: {0}", e.Message);
        /// }
        /// ]]>
        /// </code>
        /// The code above produces the following output:
        /// <![CDATA[
        /// Error: Instance not modifiable.
        /// ]]>
        /// </example>
        public static Datagram ReadOnly(Datagram datagram)
        {
            if (datagram == null) {
                throw new ArgumentNullException("datagram");
            }

            if (datagram.IsReadOnly) {
                return datagram;
            }

            return new ReadOnlyDatagram(datagram);
        }

        /// <summary>
        /// Removes the first occurrence of the <see cref="Gekkota.Net.Field" />
        /// described by the specified <see cref="Gekkota.Net.Metafield" /> from
        /// the <see cref="Gekkota.Net.Datagram" />.
        /// </summary>
        /// <param name="metafield">
        /// A <see cref="Gekkota.Net.Metafield" /> that describes the
        /// <see cref="Gekkota.Net.Field" /> to remove from the
        /// <see cref="Gekkota.Net.Datagram" />.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="metafield" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// The <see cref="Gekkota.Net.Datagram" /> is read-only.
        /// <para>-or-</para>
        /// The <see cref="Gekkota.Net.Datagram" /> has a fixed size.
        /// </exception>
        /// <example>
        /// The following example shows how to remove fields from a
        /// <see cref="Gekkota.Net.Datagram" />.
        /// <code>
        /// <![CDATA[
        /// //
        /// // create and initialize a new Datagram
        /// //
        /// Datagram datagram = new Datagram();
        /// datagram.Add(new Field(1, "Red"));
        /// datagram.Add(new Field(2, "Green"));
        /// datagram.Add(new Field(3, "Blue"));
        /// datagram.Add(new Field(4, "White"));
        ///
        /// datagram.Remove(datagram[1].GetMetafield());
        /// datagram.Remove(datagram[3].GetMetafield());
        ///
        /// foreach (Field field in datagram) {
        ///   Console.WriteLine(field.ValueAsString);
        /// }
        /// ]]>
        /// </code>
        /// The code above produces the following output:
        /// <![CDATA[
        /// Red
        /// Blue
        /// ]]>
        /// </example>
        public virtual bool Remove(Metafield metafield)
        {
            OnValidate(metafield);
            return base.Remove(new Field(null, metafield));
        }

        /// <summary>
        /// Returns a synchronized wrapper for the
        /// <see cref="Gekkota.Net.Datagram" />.
        /// </summary>
        /// <param name="datagram">
        /// The <see cref="Gekkota.Net.Datagram" /> to wrap.
        /// </param>
        /// <returns>
        /// A synchronized wrapper around the <see cref="Gekkota.Net.Datagram" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="datagram" /> is <see langword="null" />.
        /// </exception>
        /// <example>
        /// The following example shows how to create a synchronized wrapper
        /// around a <see cref="Gekkota.Net.Datagram" />.
        /// <code>
        /// <![CDATA[
        /// Datagram datagram = new Datagram();
        /// Console.WriteLine("datagram is {0}.",
        ///   datagram.IsSynchronized ? "synchronized" : "not synchronized");
        ///
        /// Datagram syncDatagram = Datagram.Synchronized(datagram);
        /// Console.WriteLine("datagram is {0}.",
        ///   syncDatagram.IsSynchronized ? "synchronized" : "not synchronized");
        /// ]]>
        /// </code>
        /// The code above produces the following output:
        /// <![CDATA[
        /// datagram is not synchronized.
        /// datagram is synchronized.
        /// ]]>
        /// </example>
        public static Datagram Synchronized(Datagram datagram)
        {
            if (datagram == null) {
                throw new ArgumentNullException("datagram");
            }

            if (datagram.IsSynchronized) {
                return datagram;
            }

            return new SyncDatagram(datagram);
        }
        #endregion public methods

        #region protected methods
        /// <summary>
        /// Sets the <see cref="Gekkota.Net.Datagram.Size" /> property to 0.
        /// </summary>
        protected override void OnClearComplete()
        {
            size = 0;
        }

        /// <summary>
        /// Increments the <see cref="Gekkota.Net.Datagram" /> size by the size
        /// of the specified <see cref="Gekkota.Net.Field" />.
        /// </summary>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the zero-based index
        /// at which <paramref name="field" /> has been inserted.
        /// </param>
        /// <param name="field">
        /// The <see cref="Gekkota.Net.Field" /> inserted into the
        /// <see cref="Gekkota.Net.Datagram" />.
        /// </param>
        protected override void OnInsertComplete(int index, Field field)
        {
            size += field.Size;
        }

        /// <summary>
        /// Decrements the <see cref="Gekkota.Net.Datagram" /> size by the size
        /// of the specified <see cref="Gekkota.Net.Field" />.
        /// </summary>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the zero-based index
        /// of the removed <see cref="Gekkota.Net.Field" />.
        /// </param>
        /// <param name="field">
        /// The <see cref="Gekkota.Net.Field" /> removed from the
        /// <see cref="Gekkota.Net.Datagram" />.
        /// </param>
        protected override void OnRemoveComplete(int index, Field field)
        {
            size -= field.Size;
        }
     
        /// <summary>
        /// Adjusts the <see cref="Gekkota.Net.Datagram" /> size by subtracting
        /// the size of <paramref name="oldField" /> and then by adding the size
        /// of <paramref name="newField" />.
        /// </summary>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the zero-based index
        /// of the <see cref="Gekkota.Net.Field" /> to locate.
        /// </param>
        /// <param name="oldField">
        /// The old <see cref="Gekkota.Net.Field" /> at <paramref name="index" />.
        /// </param>
        /// <param name="newField">
        /// The new <see cref="Gekkota.Net.Field" /> at <paramref name="index" />.
        /// </param>
        protected override void OnSetComplete(int index, Field oldField, Field newField)
        {
            size -= oldField.Size;
            size += newField.Size;
        }

        /// <summary>
        /// Checks whether or not the specified element is <see langword="null" />.
        /// </summary>
        /// <param name="value">
        /// The element to validate.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="value" /> is <see langword="null" />.
        /// </exception>
        protected override void OnValidate(Field value)
        {
            if (value == null) {
                throw new ArgumentNullException("value");
            }
        }

        /// <summary>
        /// Checks whether or not the specified element descriptor is
        /// <see langword="null" />.
        /// </summary>
        /// <param name="value">
        /// The element descriptor to validate.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="value" /> is <see langword="null" />.
        /// </exception>
        protected virtual void OnValidate(Metafield value)
        {
            if (value == null) {
                throw new ArgumentNullException("value");
            }
        }
        #endregion protected methods

        #region internal methods
        /// <summary>
        /// Returns the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" />
        /// that contains the <see cref="Gekkota.Net.Field" /> described by the
        /// specified <see cref="Gekkota.Net.Metafield" />.
        /// </summary>
        /// <param name="metafield">
        /// The <see cref="Gekkota.Net.Metafield" /> that describes the
        /// <see cref="Gekkota.Net.Field" /> contained by the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" /> to locate
        /// in the <see cref="Gekkota.Net.Datagram" />.
        /// </param>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the zero-based index
        /// of the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" />
        /// that contains the <see cref="Gekkota.Net.Field" /> described by
        /// <paramref name="metafield" />, if found; otherwise, -1.
        /// </param>
        /// <returns>
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;.Node" /> that
        /// contains the <see cref="Gekkota.Net.Field" /> described by
        /// <paramref name="metafield" />, if found; otherwise,
        /// <see langword="null" />.
        /// </returns>
        internal virtual Node GetNode(Metafield metafield, out int index)
        {
            OnValidate(metafield);
            Node node = null;

            if (metafield.ProtocolFieldAttribute != null &&
                metafield.ProtocolFieldAttribute.Position < Count) {
                index = metafield.ProtocolFieldAttribute.Position;
                node = GetNodeAt(index);

                if (((Field) node.Value).Metafield == metafield) {
                    return node;
                }
            }

            return base.GetNode(new Field(null, metafield), out index);
        }
        #endregion internal methods

        #region inner classes
        /// <summary>
        /// Implements a wrapper for the <see cref="Gekkota.Net.Datagram" />.
        /// </summary>
        private class DatagramWrapper : Datagram
        {
            private Datagram datagram;

            public override Field this[int index]
            {
                get { return datagram[index]; }
                set { datagram[index] = value; }
            }

            public override Field this[Metafield metafield]
            {
                get { return datagram[metafield]; }
                set { datagram[metafield] = value; }
            }

            public override IComparer<Field> Comparer
            {
                get { return datagram.Comparer; }
                set { datagram.Comparer = value; }
            }

            public override int Count
            {
                get { return datagram.Count; }
            }

            public override bool IsFixedSize
            {
                get { return datagram.IsFixedSize; }
            }

            public override bool IsReadOnly
            {
                get { return datagram.IsReadOnly; }
            }

            public override bool IsSynchronized
            {
                get { return datagram.IsSynchronized; }
            }

            public override bool Sorted
            {
                get { return datagram.Sorted; }
                set { datagram.Sorted = value; }
            }

            public override object SyncRoot
            {
                get { return datagram.SyncRoot; }
            }

            internal override Node Head
            {
                get { return datagram.Head; }
            }

            internal override Node Tail
            {
                get { return datagram.Tail; }
            }

            public override int Size
            {
                get { return datagram.Size; }
            }

            public DatagramWrapper(Datagram datagram)
            {
                this.datagram = datagram;
            }

            public override void Add(Field field)
            {
                datagram.Add(field);
            }

            public override Datagram Clone()
            {
                return datagram.Clone();
            }

            public override bool Contains(Metafield metafield)
            {
                return datagram.Contains(metafield);
            }

            public override bool Equals(Datagram datagram)
            {
                return Datagram.Equals(this.datagram, datagram);
            }

            public override bool Equals(object datagram)
            {
                return Datagram.Equals(this.datagram, datagram as Datagram);
            }

            public override FieldEnumerator GetEnumerator()
            {
                return datagram.GetEnumerator();
            }

            public override int IndexOf(Metafield metafield)
            {
                return datagram.IndexOf(new Field(null, metafield));
            }

            public override void Insert(int index, Field field)
            {
                datagram.Insert(index, field);
            }

            public override bool Remove(Metafield metafield)
            {
                return datagram.Remove(metafield);
            }

            public override void RemoveAt(int index)
            {
                datagram.RemoveAt(index);
            }

            public override void Reverse()
            {
                datagram.Reverse();
            }

            public override void Sort()
            {
                datagram.Sort();
            }

            internal override Node GetNode(Metafield metafield, out int index)
            {
                return datagram.GetNode(metafield, out index);
            }

            internal override Node GetNodeAt(int index)
            {
                return datagram.GetNodeAt(index);
            }
        }

        /// <summary>
        /// Implements a fixed-size wrapper for the
        /// <see cref="Gekkota.Net.Datagram" />.
        /// </summary>
        private class FixedSizeDatagram : DatagramWrapper
        {
            public override bool IsFixedSize
            {
                get { return true; }
            }

            public FixedSizeDatagram(Datagram datagram) : base(datagram) {}

            public override void Add(Field field)
            {
                throw new NotSupportedException(
                    Resources.Error_CannotAddOrRemove);
            }

            public override Datagram Clone()
            {
                return new FixedSizeDatagram(base.Clone());
            }

            public override void Insert(int index, Field field)
            {
                throw new NotSupportedException(
                    Resources.Error_CannotAddOrRemove);
            }

            public override bool Remove(Metafield metafield)
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
        /// <see cref="Gekkota.Net.Datagram" />.
        /// </summary>
        private sealed class ReadOnlyDatagram : FixedSizeDatagram
        {
            public override Field this[int index]
            {
                get { return base[index]; }
                set {
                    throw new NotSupportedException(
                        Resources.Error_InstanceNotModifiable);
                }
            }

            public override Field this[Metafield metafield]
            {
                get { return base[metafield]; }
                set {
                    throw new NotSupportedException(
                        Resources.Error_InstanceNotModifiable);
                }
            }

            public override IComparer<Field> Comparer
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

            public ReadOnlyDatagram(Datagram datagram) : base(datagram) {}

            public override Datagram Clone()
            {
                return new ReadOnlyDatagram(base.Clone());
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
        /// <see cref="Gekkota.Net.Datagram" />.
        /// </summary>
        private sealed class SyncDatagram : DatagramWrapper
        {
            public override Field this[int index]
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

            public override Field this[Metafield metafield]
            {
                get {
                    lock (SyncRoot) {
                        return base[metafield];
                    }
                }

                set {
                    lock (SyncRoot) {
                        base[metafield] = value;
                    }
                }
            }

            public override IComparer<Field> Comparer
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

            public override int Size
            {
                get {
                    lock (SyncRoot) {
                        return base.Size;
                    }
                }
            }

            public SyncDatagram(Datagram datagram) : base(datagram) {}

            public override void Add(Field field)
            {
                lock (SyncRoot) {
                    base.Add(field);
                }
            }

            public override Datagram Clone()
            {
                lock (SyncRoot) {
                    return new SyncDatagram(base.Clone());
                }
            }

            public override bool Contains(Metafield metafield)
            {
                lock (SyncRoot) {
                    return base.Contains(metafield);
                }
            }

            public override bool Equals(Datagram datagram)
            {
                lock (SyncRoot) {
                    return base.Equals(datagram);
                }
            }

            public override bool Equals(object datagram)
            {
                lock (SyncRoot) {
                    return base.Equals(datagram as Datagram);
                }
            }

            public override FieldEnumerator GetEnumerator()
            {
                lock (SyncRoot) {
                    return base.GetEnumerator();
                }
            }

            public override int IndexOf(Metafield metafield)
            {
                lock (SyncRoot) {
                    return base.IndexOf(metafield);
                }
            }

            public override void Insert(int index, Field field)
            {
                lock (SyncRoot) {
                    base.Insert(index, field);
                }
            }

            public override bool Remove(Metafield metafield)
            {
                lock (SyncRoot) {
                    return base.Remove(metafield);
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

            internal override Node GetNode(Metafield metafield, out int index)
            {
                lock (SyncRoot) {
                    return base.GetNode(metafield, out index);
                }
            }

            internal override Node GetNodeAt(int index)
            {
                lock (SyncRoot) {
                    return base.GetNodeAt(index);
                }
            }
        }

        /// <summary>
        /// Implements the default comparer for the
        /// <see cref="Gekkota.Net.Datagram" />.
        /// </summary>
        private sealed class DefaultComparer : IComparer<Field>
        {
            int IComparer<Field>.Compare(Field x, Field y)
            {
                return x.Metafield != y.Metafield ? 1 : 0;
            }
        }
        #endregion inner classes
    }
}
