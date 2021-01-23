//------------------------------------------------------------------------------
// <sourcefile name="FieldType.cs" language="C#" begin="05/31/2003">
//
//     <author name="Giuseppe Greco" email="giuseppe.greco@agamura.com" />
//
//     <copyright company="Agamura" url="http://www.agamura.com">
//         Copyright (C) 2003 Agamura, Inc.  All rights reserved.
//     </copyright>
//
// </sourcefile>
//------------------------------------------------------------------------------

namespace Gekkota.Net
{
    #region public enums
    /// <summary>
    /// Defines <see cref="Gekkota.Net.Field" /> type values.
    /// </summary>
    /// <seealso cref="Gekkota.Net.Field" />
    public enum FieldType : byte
    {
        /// <summary>
        /// The <see cref="Gekkota.Net.Field" /> contains undefined data.
        /// </summary>
        Undefined = 0x00,

        /// <summary>
        /// The <see cref="Gekkota.Net.Field" /> contains an integral number.
        /// </summary>
        /// <remarks>
        /// Integral numbers include int8, int16, int32, and int64.
        /// </remarks>
        Integral = 0x01,

        /// <summary>
        /// The <see cref="Gekkota.Net.Field" /> contains a floating point number.
        /// </summary>
        /// <remarks>
        /// Floating point numbers include float32 and float64.
        /// </remarks>
        FloatingPoint = 0x02,

        /// <summary>
        /// The <see cref="Gekkota.Net.Field" /> contains an utf-8 character
        /// string.
        /// </summary>
        String = 0x03,

        /// <summary>
        /// The <see cref="Gekkota.Net.Field" /> contains a raw byte array.
        /// </summary>
        ByteArray = 0x04,
    }
    #endregion public enums
}
