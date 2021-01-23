//------------------------------------------------------------------------------
// <sourcefile name="ChangeKeyEventHandler.cs" language="C#" begin="11/15/2005">
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
    /// <summary>
    /// Represents the method that handles the <c>ChangeKey</c> event.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="args">
    /// A <see cref="Gekkota.Security.Cryptography.ChangeKeyEventArgs" /> that
    /// contains the event data.
    /// </param>
    public delegate void ChangeKeyEventHandler(
        object sender, ChangeKeyEventArgs args);
}
