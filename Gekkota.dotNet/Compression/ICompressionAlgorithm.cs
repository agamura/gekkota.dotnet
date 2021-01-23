//------------------------------------------------------------------------------
// <sourcefile name="ICompressionAlgorithm.cs" language="C#" begin="11/06/2005">
//
//     <author name="Giuseppe Greco" email="giuseppe.greco@agamura.com" />
//
//     <copyright company="Agamura" url="http://www.agamura.com">
//         Copyright (C) 2005 Agamura, Inc.  All rights reserved.
//     </copyright>
//
// </sourcefile>
//------------------------------------------------------------------------------

namespace Gekkota.Compression
{
    /// <summary>
    /// Defines the basic operations of compression algorithms.
    /// </summary>
    public interface ICompressionAlgorithm
    {
        #region methods
        /// <summary>
        /// Compresses the specified region of the specified
        /// <see cref="System.Byte" /> array.
        /// </summary>
        /// <param name="data">
        /// A <see cref="System.Byte" /> array that contains the data to
        /// compress.
        /// </param>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the starting position
        /// within <paramref name="data" />.
        /// </param>
        /// <param name="count">
        /// An <see cref="System.Int32" /> that represents the number of bytes to
        /// compress.
        /// </param>
        /// <returns>
        /// A <see cref="System.Byte" /> array that contains the compressed data.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="data" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> or <paramref name="count" /> is less
        /// than 0.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="index" /> is out of bounds.
        /// <para>-or-</para>
        /// <paramref name="index" /> plus <paramref name="count" /> is
        /// greater than the length of <paramref name="data" />.
        /// </exception>
        byte[] Deflate(byte[] data, int index, int count);

        /// <summary>
        /// Decompresses the specified region of the specified
        /// <see cref="System.Byte" /> array.
        /// </summary>
        /// <param name="data">
        /// A <see cref="System.Byte" /> array that contains the data to
        /// decompress.
        /// </param>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the starting position
        /// within <paramref name="data" />.
        /// </param>
        /// <param name="count">
        /// An <see cref="System.Int32" /> that represents the number of bytes
        /// to decompress.
        /// </param>
        /// <returns>
        /// A <see cref="System.Byte" /> array that contains the decompressed
        /// data.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="data" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> or <paramref name="count" /> is less
        /// than 0.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="index" /> is out of bounds.
        /// <para>-or-</para>
        /// <paramref name="index" /> plus <paramref name="count" /> is
        /// greater than the length of <paramref name="data" />.
        /// </exception>
        byte[] Inflate(byte[] data, int index, int count);
        #endregion methods
    }
}
