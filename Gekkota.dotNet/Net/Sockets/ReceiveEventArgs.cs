//------------------------------------------------------------------------------
// <sourcefile name="ReceiveEventArgs.cs" language="C#" begin="06/05/2003">
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
using System.Net;

namespace Gekkota.Net.Sockets
{
    /// <summary>
    /// Provides data for the <c>Receive</c> event.
    /// </summary>
    public class ReceiveEventArgs : EventArgs
    {
        #region private fields
        private Datagram datagram;
        private EndPoint remoteEndPoint;
        #endregion private fields

        #region public properties
        /// <summary>
        /// Gets the received <see cref="Gekkota.Net.Datagram" />.
        /// </summary>
        /// <value>
        /// The received <see cref="Gekkota.Net.Datagram" />.
        /// </value>
        public Datagram Datagram
        {
            get { return datagram; }
        }

        /// <summary>
        /// Gets the remote <see cref="System.Net.EndPoint" /> from which the
        /// <see cref="Gekkota.Net.Datagram" /> was sent.
        /// </summary>
        /// <value>
        /// An <see cref="System.Net.EndPoint" /> that represents the remote
        /// endpoint from which the <see cref="Gekkota.Net.Datagram" /> was
        /// sent.
        /// </value>
        public EndPoint RemoteEndPoint
        {
            get { return remoteEndPoint; }
        }
        #endregion public properties

        #region public constructors
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Gekkota.Net.Sockets.ReceiveEventArgs" /> class with the
        /// specified datagram and remote endpoint.
        /// </summary>
        /// <param name="datagram">
        /// The received <see cref="Gekkota.Net.Datagram" />.
        /// </param>
        /// <param name="remoteEndPoint">
        /// The remote <see cref="System.Net.EndPoint" /> from which
        /// <paramref name="datagram" /> was sent.
        /// </param>
        public ReceiveEventArgs(Datagram datagram, EndPoint remoteEndPoint)
        {
            this.datagram = datagram;
            this.remoteEndPoint = remoteEndPoint;
        }
        #endregion public constructors
    }
}
