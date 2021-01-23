//------------------------------------------------------------------------------
// <sourcefile name="CompressionLevel.cs" language="C#" begin="11/15/2005">
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
    #region public enums
    /// <summary>
    /// Defines compression level values.
    /// </summary>
    /// <seealso cref="Gekkota.Net.Field" />
    public enum CompressionLevel : byte
    {
        /// <summary>
        /// Fastest processing speed and worst compression ratio.
        /// </summary>
        Fastest = 0x0D,

        /// <summary>
        /// Very fast processing speed and near worst compression ratio.
        /// </summary>
        VeryFast = 0x0E,

        /// <summary>
        /// Fast processing speed and fairly good compression ratio.
        /// </summary>
        Fast = 0x0F,

        /// <summary>
        /// Good compression ratio and fairly fast processing speed.
        /// </summary>
        Good = 0x10,

        /// <summary>
        /// Very good compression ratio and near slowest processing speed.
        /// </summary>
        VeryGood = 0x11,

        /// <summary>
        /// Best compression ratio and slowest processing speed.
        /// </summary>
        Best = 0x12,
    }
    #endregion public enums
}