//------------------------------------------------------------------------------
// <sourcefile name="NodeEnumerator.cs" language="C#" begin="05/06/2003">
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

namespace Gekkota.Collections
{
    /// <summary>
    /// Provides a simple iteration mechanism over a
    /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
    /// </summary>
    /// <example>
    /// The following example shows how the <c>NodeEnumerator</c> class can be
    /// used to iterate over a <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
    /// <code>
    /// <![CDATA[
    /// LinkedList list = new LinkedList();
    /// list.Add("Red");
    /// list.Add("Green");
    /// list.Add("Blue");
    ///
    /// IEnumerator enumerator = list.GetEnumerator();
    ///
    /// while (enumerator.MoveNext()) {
    ///     Console.WriteLine(enumerator.Current);
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
    public class NodeEnumerator<T> : IEnumerator<T>
    {
        #region private readonly fields
        private readonly int MaxCount;
        #endregion private readonly fields

        #region private fields
        private LinkedList<T> list;
        private LinkedList<T>.Node node;
        private int count;
        #endregion private fields

        #region public properties
        /// <summary>
        /// Gets the value of the current node in the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <value>
        /// The value of the current node in the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </value>
        /// <exception cref="System.InvalidOperationException">
        /// The enumerator is positioned before the first node of the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> or after the
        /// last node.
        /// </exception>
        public T Current
        {
            get {
                if (node == null || node.Value == null) {
                    throw new InvalidOperationException(
                        Resources.Error_EnumeratorNotValid);
                }

                return node.Value;
            }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }
        #endregion public properties

        #region internal constructors
        /// <summary>
        /// Initialized a new instance of the
        /// <see cref="Gekkota.Collections.NodeEnumerator&lt;T&gt;" /> class.
        /// </summary>
        /// <param name="list">
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> to
        /// enumerate.
        /// </param>
        internal NodeEnumerator(LinkedList<T> list)
        {
            this.list = list;
            MaxCount = list.Count;
        }
        #endregion internal constructors

        #region public methods
        /// <summary>
        /// Releases all resources used by the
        /// <see cref="Gekkota.Collections.NodeEnumerator&lt;T&gt;" />.
        /// </summary>
        /// <remarks>
        /// The default implementation of the <c>Dispose</c> method does
        /// nothing.
        /// </remarks>
        void IDisposable.Dispose()
        {
        }

        /// <summary>
        /// Advances the enumerator to the next node of the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <returns>
        /// <see langword="true" /> if the enumerator was successfully advanced
        /// to the next node; <see langword="false" /> if the enumerator has
        /// passed the end of the
        /// <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> was
        /// modified after the enumerator was created.
        /// </exception>
        public bool MoveNext()
        {
            if (MaxCount != list.Count) {
                throw new InvalidOperationException(
                    Resources.Error_CollectionNotConsistent);
            }

            if (count < MaxCount) {
                if (node == null) {
                    node = list.Head;
                } else {
                    node = node.Next;
                }
            }

            return !(++count > MaxCount);
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the
        /// first node in the <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" />.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// The <see cref="Gekkota.Collections.LinkedList&lt;T&gt;" /> was
        /// modified after the enumerator was created.
        /// </exception>
        public void Reset()
        {
            if (MaxCount != list.Count) {
                throw new InvalidOperationException(
                    Resources.Error_CollectionNotConsistent);
            }

            count = 0;
            node = null;
        }
        #endregion public methods
    } 
}
