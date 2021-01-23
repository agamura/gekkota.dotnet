//------------------------------------------------------------------------------
// <sourcefile name="BoundsChecker.cs" language="C#" begin="03/13/2004">
//
//     <author name="Giuseppe Greco" email="giuseppe.greco@agamura.com" />
//
//     <copyright company="Agamura" url="http://www.agamura.com">
//         Copyright (C) 2004 Agamura, Inc.  All rights reserved.
//     </copyright>
//
// </sourcefile>
//------------------------------------------------------------------------------

using System;
using Gekkota.Properties;

namespace Gekkota.Utilities
{
    /// <summary>
    /// Provides methods for determining whether or not parameters like start
    /// index and count are inside the bounds of a given array.
    /// </summary>
    internal static class BoundsChecker
    {
        #region public methods
        /// <summary>
        /// Checks whether or not the specified index is inside the bounds of the
        /// specified array.
        /// </summary>
        /// <param name="arrayName">
        /// A <see cref="System.String" /> that contains the name of
        /// <paramref name="array" />.
        /// </param>
        /// <param name="array">
        /// The array to check.
        /// </param>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the starting position
        /// within <paramref name="array" />.
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
        /// </exception>
        public static void Check(string arrayName, Array array, int index)
        {
            if (array == null) {
                throw new ArgumentNullException(arrayName != null ? arrayName : "array");
            }

            if (index < 0) {
                throw new ArgumentOutOfRangeException("index", index,
                    Resources.Error_NonNegativeNumberRequired);
            }

            if (index >= array.Length) {
                throw new ArgumentException(
                    Resources.Error_IndexOutOfBounds);
            }
        }

        /// <summary>
        /// Checks whether or not the specified index plus the specified count is
        /// inside the bounds of the specified array.
        /// </summary>
        /// <param name="arrayName">
        /// A <see cref="System.String" /> that contains the name of
        /// <paramref name="array" />.
        /// </param>
        /// <param name="array">
        /// The array to check.
        /// </param>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the starting position
        /// within <paramref name="array" />.
        /// </param>
        /// <param name="count">
        /// An <see cref="System.Int32" /> that represents the number of bytes to
        /// count.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="array" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> or <paramref name="count" /> is less than 0.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="index" /> is equal to or greater than the length of
        /// <paramref name="array" />.
        /// <paramref name="index" /> plus <paramref name="count" /> is
        /// greater than the length of <paramref name="array" />.
        /// </exception>
        public static void Check(string arrayName, Array array, int index, int count)
        {
            Check(arrayName, array, index);

            if (count < 0) {
                throw new ArgumentOutOfRangeException("count", count,
                    Resources.Error_NonNegativeNumberRequired);
            }

            if (index + count > array.Length) {
                throw new ArgumentException(String.Format(
                    Resources.Error_NotEnoughSpaceAvailable, "index", "count"));
            }
        }
        #endregion public methods
    }
}
