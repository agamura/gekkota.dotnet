//------------------------------------------------------------------------------
// <sourcefile name="IChecksumModel.cs" language="C#" begin="01/28/2004">
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
    /// <see cref="Gekkota.Checksums.ChecksumGenerator" />.
    /// </summary>
    /// <remarks>
    /// <see cref="Gekkota.Checksums.ChecksumGenerator" /> implements the
    /// Rocksoft(TM) Model CRC Algorithm.
    /// </remarks>
    public interface IChecksumModel
    {
        #region public properties
        /// <summary>
        /// Gets the algorithm name.
        /// </summary>
        /// <value>
        /// A <see cref="System.String" /> that contains the algorithm name.
        /// </value>
        string Name
        {
            get;
        }

        /// <summary>
        /// Gets the algorithm width.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int32" /> that represents the algorithm width,
        /// in bits.
        /// </value>
        int Width
        {
            get;
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
        long Polynom
        {
            get;
        }

        /// <summary>
        /// Gets the initial CRC value when the algorithm starts.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int64" /> that represents the initial CRC value
        /// when the algorithm starts.
        /// </value>
        long Initial
        {
            get;
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
        bool InputReflected
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether or not the calculated CRC should to
        /// be reflected before it is XORed.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the calculated CRC should be reflected
        /// before it is XORed; otherwise, <see langword="false" />.
        /// </value>
        bool OutputReflected
        {
            get;
        }

        /// <summary>
        /// Gets value that should be XORed to the calculated CRC before it is
        /// returned.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int64" /> that represents the value that should
        /// be XORed to the calculated CRC before it is returned.
        /// </value>
        long XorOutput
        {
            get;
        }
        #endregion public properties
    }
}
