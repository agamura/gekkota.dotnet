//------------------------------------------------------------------------------
// <sourcefile name="ReceiveEventHandler.cs" language="C#" begin="06/05/2003">
//
//     <author name="Giuseppe Greco" email="giuseppe.greco@agamura.com" />
//
//     <copyright company="Agamura" url="http://www.agamura.com">
//         Copyright (C) 2003 Agamura, Inc.  All rights reserved.
//     </copyright>
//
// </sourcefile>
//------------------------------------------------------------------------------

namespace Gekkota.Net.Sockets
{
    /// <summary>
    /// Represents the method that handles the <c>Receive</c> event.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="args">
    /// A <see cref="Gekkota.Net.Sockets.ReceiveEventArgs" /> that contains the
    /// event data.
    /// </param>
    public delegate void ReceiveEventHandler(
        object sender, ReceiveEventArgs args);
}
