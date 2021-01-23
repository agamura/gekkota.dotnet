//------------------------------------------------------------------------------
// <sourcefile name="ExceptionEventHandler.cs" language="C#" begin="03/16/2004">
//
//     <author name="Giuseppe Greco" email="giuseppe.greco@agamura.com" />
//
//     <copyright company="Agamura" url="http://www.agamura.com">
//         Copyright (C) 2004 Agamura, Inc.  All rights reserved.
//     </copyright>
//
// </sourcefile>
//------------------------------------------------------------------------------

namespace Gekkota.Net.Sockets
{
    /// <summary>
    /// Represents the method that handles the
    /// <see cref="Gekkota.Net.Sockets.IPClient.Exception" /> event.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="args">
    /// An <see cref="Gekkota.Net.Sockets.ExceptionEventArgs" /> that contains
    /// the event data.
    /// </param>
    public delegate void ExceptionEventHandler(
        object sender, ExceptionEventArgs args);
}
