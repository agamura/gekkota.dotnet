//------------------------------------------------------------------------------
// <sourcefile name="CryptoService.cs" language="C#" begin="11/05/2005">
//
//     <author name="Giuseppe Greco" email="giuseppe.greco@agamura.com" />
//
//     <copyright company="Agamura" url="http://www.agamura.com">
//         Copyright (C) 2005 Agamura, Inc.  All rights reserved.
//     </copyright>
//
// </sourcefile>
//------------------------------------------------------------------------------

namespace Gekkota.Security.Cryptography
{
    #region internal enums
    /// <summary>
    /// Defines crypto service values for the
    /// <see cref="Gekkota.Security.Cryptography.CryptoServiceProvider.Transform" />
    /// method.
    /// </summary>
    /// <seealso cref="Gekkota.Security.Cryptography.CryptoServiceProvider" />
    internal enum CryptoService : byte
    {
        /// <summary>
        /// The
        /// <see cref="Gekkota.Security.Cryptography.CryptoServiceProvider.Transform" />
        /// method provides encryption services.
        /// </summary>
        Encrypt = 0x01,

        /// <summary>
        /// The
        /// <see cref="Gekkota.Security.Cryptography.CryptoServiceProvider.Transform" />
        /// method provides decryption services.
        /// </summary>
        Decrypt = 0x02,
    }
    #endregion internal enums
}
