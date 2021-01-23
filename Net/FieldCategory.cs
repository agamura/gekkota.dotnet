//------------------------------------------------------------------------------
// <sourcefile name="FieldCategory.cs" language="C#" begin="06/05/2004">
//
//     <author name="Giuseppe Greco" email="giuseppe.greco@agamura.com" />
//
//     <copyright company="Agamura" url="http://www.agamura.com">
//         Copyright (C) 2004 Agamura, Inc.  All rights reserved.
//     </copyright>
//
// </sourcefile>
//------------------------------------------------------------------------------

namespace Gekkota.Net
{
    #region public enums
    /// <summary>
    /// Defines <see cref="Gekkota.Net.Field" /> category values.
    /// </summary>
    /// <seealso cref="Gekkota.Net.Field" />
    public enum FieldCategory : byte
    {
        /// <summary>
        /// The <see cref="Gekkota.Net.Field" /> is of a undefined category.
        /// </summary>
        Undefined = 0x00,

        /// <summary>
        /// The <see cref="Gekkota.Net.Field" /> is of a manifest field.
        /// </summary>
        Manifest  = 0x01,

        /// <summary>
        /// The <see cref="Gekkota.Net.Field" /> is a header field.
        /// </summary>
        Header    = 0x02,

        /// <summary>
        /// The <see cref="Gekkota.Net.Field" /> is a payload field.
        /// </summary>
        Payload   = 0x03,
    }
    #endregion public enums
}
