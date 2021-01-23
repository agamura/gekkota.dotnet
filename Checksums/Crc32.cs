//------------------------------------------------------------------------------
// <sourcefile name="Crc32.cs" language="C#" begin="01/29/2004">
//
//     <author name="Giuseppe Greco" email="giuseppe.greco@agamura.com" />
//
//     <copyright company="Agamura" url="http://www.agamura.com">
//         Copyright (C) 2004 Agamura, Inc.  All rights reserved.
//     </copyright>
//
// </sourcefile>
//------------------------------------------------------------------------------

namespace Gekkota.Checksums
{
    /// <summary>
    /// Parameterizes the CRC algorithm implemented by the
    /// <see cref="Gekkota.Checksums.ChecksumGenerator" /> to make it behave as
    /// the CRC32 algorithm.
    /// </summary>
    public sealed class Crc32 : IChecksumModel
    {
        #region public properties
        /// <summary>
        /// Gets the algorithm name.
        /// </summary>
        /// <value>
        /// A <see cref="System.String" /> that contains the algorithm name.
        /// </value>
        public string Name
        {
            get { return "CRC32"; }
        }

        /// <summary>
        /// Gets the algorithm width.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int32" /> that represents the algorithm width,
        /// in bits.
        /// </value>
        public int Width
        {
            get { return 0x00000020; }
        }

        /// <summary>
        /// Gets the polynom.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int64" /> that represents the polynom.
        /// </value>
        /// <remarks>
        /// The <c>Polynom</c> property represents the unreflected polynom, and
        /// its bottom bit is always the LSB of the divisor regardless whether
        /// or not the algorithm is reflected.
        /// </remarks>
        public long Polynom
        {
            get { return 0x04C11DB7; }
        }

        /// <summary>
        /// Gets the initial CRC value when the algorithm starts.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int64" /> that represents the initial CRC value
        /// when the algorithm starts.
        /// </value>
        public long Initial
        {
            get { return 0xFFFFFFFF; }
        }

        /// <summary>
        /// Gets a value indicating whether or not input bytes are processed
        /// with bit 0 being treated as the most significant bit (MSB) and bit 7
        /// being treated as the least significant bit (LSB).
        /// </summary>
        /// <value>
        /// <see langword="true" /> if input bytes have to be reflected;
        /// otherwise, <see langword="false" />.
        /// </value>
        public bool InputReflected
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the calculated CRC should to
        /// be reflected before it is XORed.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the calculated CRC should be reflected
        /// before it is XORed; otherwise, <see langword="false" />.
        /// </value>
        public bool OutputReflected
        {
            get { return true; }
        }

        /// <summary>
        /// Gets value that should be XORed to the calculated CRC before it is
        /// returned.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int64" /> that represents the value that should
        /// be XORed to the calculated CRC before it is returned.
        /// </value>
        public long XorOutput
        {
            get { return 0xFFFFFFFF; }
        }
        #endregion public properties
    }
}
