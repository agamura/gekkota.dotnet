//------------------------------------------------------------------------------
// <sourcefile name="LZF.cs" language="C#" begin="10/30/2005">
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
using System.Net;
using Gekkota.Utilities;

namespace Gekkota.Compression
{
    /// <summary>
    /// Implements the LZF compression algorithm. This class cannot be inherited.
    /// </summary>
    /// <example>
    /// The following example shows how to create and initialize a <c>LZF</c>
    /// compression algorithm that deflates and inflates a byte array.
    /// <code>
    /// <![CDATA[
    /// using System;
    /// using System.Text;
    /// using Gekkota.Compression;
    ///
    /// public class Deflater
    /// {
    ///     public static void Main(string[] args)
    ///     {
    ///         if (args.Length == 0) {
    ///             Console.WriteLine("Usage: Deflater <string>");
    ///             Environment.Exit(0);
    ///         }
    ///
    ///         //
    ///         // create and initialize a new LZF compression algorithm
    ///         // with fastest processing speed
    ///         //
    ///         LZF lzf = new LZF(CompressionLevel.Fastest);
    ///         byte[] data = Encoding.UTF8.GetBytes(args[0]);
    ///         byte[] deflated = lzf.Deflate(data, 0, data.Length);
    ///         byte[] inflated = lzf.Inflate(deflated, 0, deflated.Length);
    /// 
    ///         Console.WriteLine("Compressing \"{0}\" saves {1} bytes.",
    ///             args[0], inflated.Length - deflated.Lenght);
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// The code above produces the following output:
    /// <![CDATA[
    /// Compressing "To be, or not to be: that is the question." saves 4 bytes.
    /// ]]>
    /// </example>
    public sealed class LZF : ICompressionAlgorithm
    {
        #region private fields
        private const int MaxLiteral    = 1 << 5;
        private const int MaxOffset     = 1 << 13;
        private const int MaxReference  = (1 << 8) + (1 << 3);

        private int compressionLevel;
        private int hashtableSize;
        #endregion private fields

        #region public properties
        /// <summary>
        /// Gets or sets the compression level.
        /// </summary>
        /// <value>
        /// One of the <see cref="Gekkota.Compression.CompressionLevel" />
        /// values.
        /// </value>
        public CompressionLevel CompressionLevel
        {
            get { return unchecked((CompressionLevel) compressionLevel); }
            set {
                compressionLevel = (int) value;
                hashtableSize = 1 << (int) value;
            }
        }
        #endregion public properties

        #region public constructors
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Gekkota.Compression.LZF" /> class.
        /// </summary>
        public LZF() : this(CompressionLevel.Fast)
        {
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Gekkota.Compression.LZF" /> class with the specified
        /// compression level.
        /// </summary>
        /// <param name="compressionLevel">
        /// One of the <see cref="Gekkota.Compression.CompressionLevel" />
        /// values.
        /// </param>
        public LZF(CompressionLevel compressionLevel)
        {
            CompressionLevel = compressionLevel;
        }
        #endregion public constructors

        #region public methods
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
        public unsafe byte[] Deflate(byte[] data, int index, int count)
        {
            BoundsChecker.Check("data", data, index, count);

            byte[] temp = new byte[data.Length + (1 << 3)];
            int[] hashtable = new int[hashtableSize];
            int literal = 0, offset, reference;
            int i = index, j = 0, k;
            int value = GetFirst(data, i);

            while (true) {
                if (i < count - 2) {
                    value = GetNext(value, data, i);
                    k = GetIndex(value);
                    reference = hashtable[k];
                    hashtable[k] = i;

                    if ((offset = i - reference - 1) < MaxOffset
                        && i + 4 < count && reference > 0
                        && data[reference] == data[i]
                        && data[reference + 1] == data[i + 1]
                        && data[reference + 2] == data[i + 2]) {
                        int length = 2;
                        int maxLength = count - i - length;
                        if (maxLength > MaxReference) maxLength = MaxReference;

                        do {
                            length++;
                        } while (length < maxLength && data[reference + length] == data[i + length]);

                        if (literal != 0) {
                            temp[j++] = (byte) (literal - 1);
                            literal = -literal;
                            do {
                                temp[j++] = data[i + literal];
                            } while (++literal != 0);
                        }

                        length -= 2;
                        i++;

                        if (length < 7) {
                            temp[j++] = (byte) ((offset >> 8) + (length << 5));
                        } else {
                            temp[j++] = (byte) ((offset >> 8) + (7 << 5));
                            temp[j++] = (byte) (length - 7);
                        }

                        temp[j++] = (byte) offset;

                        i += length - 1;
                        value = GetFirst(data, i);

                        value = GetNext(value, data, i);
                        hashtable[GetIndex(value)] = i;
                        i++;

                        value = GetNext(value, data, i);
                        hashtable[GetIndex(value)] = i;
                        i++;

                        continue;
                    }
                } else if (i == count) { break; }

                literal++;
                i++;

                if (literal == MaxLiteral) {
                    temp[j++] = (byte) (MaxLiteral - 1);
                    literal = -literal;
                    do {
                        temp[j++] = data[i + literal];
                    } while (++literal != 0);
                }
            }

            if (literal != 0) {
                temp[j++] = (byte) (literal - 1);
                literal = -literal;
                do {
                    temp[j++] = data[i + literal];
                } while (++literal != 0);
            }

            //
            // embed original data size
            //
            byte [] deflated = new byte[j + sizeof(int)];
            System.Buffer.BlockCopy(temp, 0, deflated, 0, j);
            fixed (byte* pDeflated = deflated) {
                byte* pLength = pDeflated + j;
                *((int*) pLength) = IPAddress.HostToNetworkOrder(count);
            }

            return deflated;
        }

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
        public unsafe byte[] Inflate(byte[] data, int index, int count)
        {
            BoundsChecker.Check("data", data, index, count);

            //
            // extract original data size
            //
            byte[] inflated = null;
            fixed (byte* pData = data) {
                byte* pLength = pData + index + (count - sizeof(int));
                inflated = new byte[IPAddress.NetworkToHostOrder(*((int*) pLength))];
            }

            int i = 0, j = 0;

            do {
                int value = data[i++];
                if (value < (1 << 5)) {
                    value++;

                    do {
                        inflated[j++] = data[i++];
                    } while (--value != 0);
                } else {
                    int length = value >> 5;
                    int reference = (int) (j - ((value & 0x1F) << 8) - 1);

                    if (length == 7)
                        length += data[i++];

                    reference -= data[i++];

                    inflated[j++] = inflated[reference++];
                    inflated[j++] = inflated[reference++];

                    do {
                        inflated[j++] = inflated[reference++];
                    } while (--length != 0);
                }
            } while (j < inflated.Length && i < count);

            return inflated;
        }
        #endregion public methods

        #region private methods
        private int GetFirst(byte[] data, int index)
        {
            return (data[index] << 8) | data[index + 1];
        }

        private int GetNext(int value, byte[] data, int index)
        {
            return (value << 8) | data[index + 2];
        }

        private int GetIndex(int value)
        {
            return ((value ^ (value << 5))
                >> (((3 * 8 - compressionLevel)) - value * 5)
                & (hashtableSize - 1));
        }
        #endregion private methods
    }
}
