//------------------------------------------------------------------------------
// <sourcefile name="FieldEnumerator.cs" language="C#" begin="06/05/2003">
//
//     <author name="Giuseppe Greco" email="giuseppe.greco@agamura.com" />
//
//     <copyright company="Agamura" url="http://www.agamura.com">
//         Copyright (C) 2003 Agamura, Inc.  All rights reserved.
//     </copyright>
//
// </sourcefile>
//------------------------------------------------------------------------------

using System;
using Gekkota.Collections;

namespace Gekkota.Net
{
    /// <summary>
    /// Provides a simple iteration mechanism over a
    /// <see cref="Gekkota.Net.Datagram" />.
    /// </summary>
    public class FieldEnumerator : NodeEnumerator<Field>
    {
        #region internal constructors
        /// <summary>
        /// Gets the current <see cref="Gekkota.Net.Field" /> in the
        /// <see cref="Gekkota.Net.Datagram" />.
        /// </summary>
        /// <value>
        /// The current <see cref="Gekkota.Net.Field" /> in the
        /// <see cref="Gekkota.Net.Datagram" />.
        /// </value>
        /// <exception cref="System.InvalidOperationException">
        /// The enumerator is positioned before the first
        /// <see cref="Gekkota.Net.Field" /> of the
        /// <see cref="Gekkota.Net.Datagram" /> or after the last
        /// <see cref="Gekkota.Net.Field" />.
        /// </exception>
        internal FieldEnumerator(Datagram datagram) : base(datagram)
        {
        }
        #endregion internal constructors
    } 
}
