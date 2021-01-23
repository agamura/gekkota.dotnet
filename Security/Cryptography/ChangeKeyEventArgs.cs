//------------------------------------------------------------------------------
// <sourcefile name="ChangeKeyEventArgs.cs" language="C#" begin="11/15/2005">
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

namespace Gekkota.Security.Cryptography
{
    /// <summary>
    /// Provides data for the <c>ChangeKey</c> event.
    /// </summary>
    public class ChangeKeyEventArgs : EventArgs
    {
        #region private fields
        private byte[] newKey;
        private byte[] newIV;
        #endregion private fields

        #region public properties
        /// <summary>
        /// Gets the new secret key.
        /// </summary>
        /// <value>
        /// A <see cref="System.Byte" /> array that contains the new secret key.
        /// </value>
        public byte[] NewKey
        {
            get { return newKey; }
        }

        /// <summary>
        /// Gets the new initialization vector.
        /// </summary>
        /// <value>
        /// A <see cref="System.Byte" /> array that contains the new
        /// initialization vector.
        /// </value>
        public byte[] NewIV
        {
            get { return newIV; }
        }
        #endregion public properties

        #region public constructors
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Gekkota.Security.Cryptography.ChangeKeyEventArgs" />
        /// class with the specified secret key and initialization vector.
        /// </summary>
        /// <param name="key">
        /// A <see cref="System.Byte" /> array that contains the new secret key.
        /// </param>
        /// <param name="IV">
        /// A <see cref="System.Byte" /> array that contains the new
        /// initialization vector.
        /// </param>
        public ChangeKeyEventArgs(byte[] newKey, byte[] newIV)
        {
            this.newKey = newKey;
            this.newIV = newIV;
        }
        #endregion public constructors
    }
}
