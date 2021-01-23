//------------------------------------------------------------------------------
// <sourcefile name="CasingRule.cs" language="C#" begin="10/31/2004">
//
//     <author name="Giuseppe Greco" email="giuseppe.greco@agamura.com" />
//
//     <copyright company="Agamura" url="http://www.agamura.com">
//         Copyright (C) 2004 Agamura, Inc.  All rights reserved.
//     </copyright>
//
// </sourcefile>
//------------------------------------------------------------------------------

namespace Gekkota
{
    #region public enums
    /// <summary>
    /// Defines casing rules for the
    /// <see cref="Gekkota.HashCodeGenerator.Generate(System.String, Gekkota.CasingRule)" />
    /// method.
    /// </summary>
    /// <seealso cref="Gekkota.HashCodeGenerator" />
    public enum CasingRule : byte
    {
        /// <summary>
        /// The <see cref="System.String" /> is hashed as it is.
        /// </summary>
        None    = 0x00,

        /// <summary>
        /// The <see cref="System.String" /> is hashed in uppercase.
        /// </summary>
        ToUpper = 0x01,

        /// <summary>
        /// The <see cref="System.String" /> is hashed in lowercase.
        /// </summary>
        ToLower = 0x02,
    }
    #endregion public enums
}
