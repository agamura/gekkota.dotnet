//------------------------------------------------------------------------------
// <sourcefile name="ChecksumGenerator.cs" language="C#" begin="01/28/2004">
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
using Gekkota.Utilities;

namespace Gekkota.Checksums
{
    /// <summary>
    /// Implements the Rocksoft(TM) Model CRC Algorithm.
    /// </summary>
    /// <remarks>
    /// The <c>ChecksumGenerator</c> class implements a generic, high
    /// performance, parameterizable CRC algorithm that can behave like any
    /// other native CRC algorithm.
    /// <note>
    /// More information about the Rocksoft(TM) Model CRC Algorithm is
    /// available at http://www.rocksoft.com/rocksoft/tutorial/.
    /// </note>
    /// </remarks>
    /// <example>
    /// The following example shows how to create and initialize a
    /// <c>ChecksumGenerator</c> and how to generate a 16-bit CRC.
    /// <code>
    /// <![CDATA[
    /// byte[] data = Encoding.UTF8.GetBytes("White");
    /// ChecksumGenerator checksumGenerator = new ChecksumGenerator(new Crc16());
    /// long crc = checksumGenerator.Generate(data, 0, data.Length);
    /// ]]>
    /// </code>
    /// </example>
    public class ChecksumGenerator
    {
        #region private fields
        private const int LookupTableLength = 256;

        private IChecksumModel model;
        private long initial;
        private long mask;
        private long highBit;
        private long[] lookupTable;
        #endregion private fields

        #region public properties
        /// <summary>
        /// Gets or sets the model that parameterizes the CRC algorithm.
        /// </summary>
        /// <value>
        /// An <see cref="Gekkota.Checksums.IChecksumModel" /> implementation
        /// that parameterizes the CRC algorithm.
        /// </value>
        /// <exception cref="System.ArgumentNullException">
        /// The specified value is <see langword="null" />.
        /// </exception>
        public IChecksumModel Model
        {
            get { return model; }
            set { Initialize(value); }
        }
        #endregion public properties

        #region protected properties
        /// <summary>
        /// Gets the lookup table.
        /// </summary>
        /// <value>
        /// An <see cref="System.UInt64" /> array that represents the lookup
        /// table.
        /// </value>
        /// <remarks>
        /// Lookup tables provide a very efficient way to process data, since
        /// most of the calculation is precomputed and assembled into them.
        /// </remarks>
        protected long[] LookupTable
        {
            get {
                if (lookupTable == null) {
                    CreateLookupTable();
                }
                return lookupTable;
            }
        }
        #endregion protected properties

        #region public constructors
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Gekkota.Checksums.ChecksumGenerator" /> class with the
        /// specified model.
        /// </summary>
        /// <param name="model">
        /// An <see cref="Gekkota.Checksums.IChecksumModel" /> implementation
        /// that parameterizes the CRC algorithm.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="model" /> is <see langword="null" />.
        /// </exception>
        public ChecksumGenerator(IChecksumModel model)
        {
            Initialize(model);
        }
        #endregion public constructors

        #region public methods
        /// <summary>
        /// Generates the CRC for the specified data, according to the current
        /// <see cref="Gekkota.Checksums.IChecksumModel" /> implementation.
        /// </summary>
        /// <param name="data">
        /// A <see cref="System.Byte" /> array that contains the data to process.
        /// </param>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the starting position
        /// within <paramref name="data" />.
        /// </param>
        /// <param name="count">
        /// An <see cref="System.Int32" /> that represents the number of bytes to
        /// process.
        /// </param>
        /// <returns>
        /// An <see cref="System.UInt64" /> that represents the generated CRC.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="data" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> or <paramref name="count" /> is less
        /// than 0.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="index" /> is equal to or greater than the length of
        /// <paramref name="data" />.
        /// <para>-or-</para>
        /// <paramref name="index" /> plus <paramref name="count" /> is greater
        /// than the length of <paramref name="data" />.
        /// </exception>
        public virtual long Generate(byte[] data, int index, int count)
        {
            BoundsChecker.Check("data", data, index, count);

            long crc = model.InputReflected
                     ? Reflect(initial, model.Width)
                     : initial;

            if (model.InputReflected) {
                for (int i = index, j = count; j > 0; i++, j--) {
                    crc = (crc << 8)
                        ^ LookupTable[((crc >> (model.Width - 8)) & 0xFF) ^ data[i]];
                }
            } else {
                for (int i = index, j = count; j > 0; i++, j--) {
                    crc = (crc >> 8) ^ LookupTable[(crc & 0xFF) ^ data[i]];
                }
            }

            if (model.OutputReflected ^ model.InputReflected) {
                crc = Reflect(crc, model.Width);
            }

            crc ^= model.XorOutput;
            crc &= mask;

            return crc;
        }
        #endregion public methods

        #region protected methods
        /// <summary>
        /// Reflects the lower <paramref name="bitCount" /> bits of
        /// <paramref name="bitStream" />.
        /// </summary>
        /// <param name="bitStream">
        /// An <see cref="System.UInt64" /> that contains the bits to reflect.
        /// </param>
        /// <param name="bitCount">
        /// An <see cref="System.Int32" /> that represents the number of bits to
        /// reflect.
        /// </param>
        /// <returns>
        /// An <see cref="System.UInt64" /> that represents the reflection.
        /// </returns>
        protected long Reflect(long bitStream, int bitCount)
        {
            long reflection = 0;

            for (long j = 1, i = 1L << (bitCount - 1); i != 0; i >>= 1) {
                if ((bitStream & i) != 0) {
                    reflection |= j;
                }
                j <<= 1;
            }

            return reflection;
        }
        #endregion protected methods

        #region private methods
        /// <summary>
        /// Creates the lookup table.
        /// </summary>
        private void CreateLookupTable()
        {
            lookupTable = new long[LookupTableLength];

            long bit;
            long crc;

            for (int j, i = 0; i < lookupTable.Length; i++) {
                crc = model.InputReflected ? Reflect((long) i, 8) : (long) i;
                crc <<= (model.Width - 8);

                for (j = 0; j < 8; j++) {
                    bit = crc & highBit;
                    crc <<= 1;

                    if (bit != 0) {
                        crc ^= model.Polynom;
                    }
                }

                if (model.InputReflected) {
                    crc = Reflect(crc, model.Width);
                }

                crc &= mask;
                lookupTable[i] = crc;
            }
        }

        /// <summary>
        /// Initializes the <see cref="Gekkota.Checksums.ChecksumGenerator" />
        /// with the specified model.
        /// </summary>
        /// <param name="model">
        /// An <see cref="Gekkota.Checksums.IChecksumModel" /> implementation
        /// that parameterizes the CRC algorithm.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="model" /> is <see langword="null" />.
        /// </exception>
        private void Initialize(IChecksumModel model)
        {
            if (model == null) {
                throw new ArgumentNullException("model");
            }

            this.model = model;

            initial = model.Initial;
            mask = (((1L << (model.Width - 1)) - 1) << 1) | 1;
            highBit = 1L << (model.Width - 1);

            long bit;

            for (int i = 0; i < model.Width; i++) {
                bit = initial & 1;

                if (bit != 0) {
                    initial ^= model.Polynom;
                }

                initial >>= 1;

                if (bit != 0) {
                    initial |= highBit;
                }
            }
        }
        #endregion private methods
    }
}
