//------------------------------------------------------------------------------
// <sourcefile name="XudpClient.cs" language="C#" begin="06/29/2006">
//
//     <author name="Giuseppe Greco" email="giuseppe.greco@agamura.com" />
//
//     <copyright company="Agamura" url="http://www.agamura.com">
//         Copyright (C) 2006 Agamura, Inc.  All rights reserved.
//     </copyright>
//
// </sourcefile>
//------------------------------------------------------------------------------

using System;

namespace Gekkota.Net.Sockets
{
    /// <summary>
    /// Provides Cross User Datagram Protocol (XUDP) network services.
    /// </summary>
    public class XudpClient : IDisposable
    {
        #region private fields
        private BandwidthManager outgoingBandwidthManager;
        private BandwidthManager incomingBandwidthManager;
        private IPClient client;
        private bool isDisposed = false;
        #endregion private fields

        #region public properties
        /// <summary>
        /// Gets or sets a value indicating whether or not the
        /// <see cref="Gekkota.Net.Sockets.XudpClient" /> has been disposed off.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the <see cref="Gekkota.Net.Sockets.XudpClient" />
        /// has been disposed off; otherwise, <see langword="false" />.
        /// </value>
        public bool IsDisposed
        {
            get { return isDisposed; }
        }

        /// <summary>
        /// Gets or sets the downstream bandwidth.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int64" /> that represents the number of bits
        /// that can be received per second. The default value is
        /// <see cref="Gekkota.Net.Sockets.Bandwidth.Full" />.
        /// </value>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The specified value is less than 0
        /// </exception>
        public long IncomingBandwidth
        {
            get { return incomingBandwidthManager.Bandwidth; }
            set { incomingBandwidthManager.Bandwidth = value; }
        }

        /// <summary>
        /// Gets or sets the upstream bandwidth.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int64" /> that represents the number of bits
        /// that can be sent per second. The default value is
        /// <see cref="Gekkota.Net.Sockets.Bandwidth.Full" />.
        /// </value>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The specified value is less than 0
        /// </exception>
        public long OutgoingBandwidth
        {
            get { return outgoingBandwidthManager.Bandwidth; }
            set { outgoingBandwidthManager.Bandwidth = value; }
        }
        #endregion public properties

        #region public constructors
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Gekkota.Net.Sockets.XudpClient" /> class with the
        /// specified base protocol.
        /// </summary>
        /// <param name="client">
        /// A <see cref="Gekkota.Net.Sockets.IPClient" /> that represents the
        /// protocol the <see cref="Gekkota.Net.Sockets.XudpClient" /> is based
        /// on.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="client" /> is <see langword="null" />.
        /// </exception>
        /// <remarks>
        /// <paramref name="client"/> can be any IP-based protocol
        /// implementation.
        /// </remarks>
        public XudpClient(IPClient client)
        {
            if (client == null) {
                throw new ArgumentNullException("client");
            }

            this.client = client;

            outgoingBandwidthManager = new BandwidthManager();
            incomingBandwidthManager = new BandwidthManager();
        }
        #endregion public constructors

        #region finalizer
        /// <summary>
        /// Frees the resources used by the
        /// <see cref="Gekkota.Net.Sockets.XudpClient" />.
        /// </summary>
        ~XudpClient()
        {
            Dispose(false);
        }
        #endregion finalizer

        #region public methods
        /// <summary>
        /// Terminates the <see cref="Gekkota.Net.Sockets.XudpClient" /> and
        /// releases all the associated resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion public methods

        #region protected methods
        /// <summary>
        /// Releases the unmanaged resources used by the
        /// <see cref="Gekkota.Net.Sockets.XudpClient" />, and optionally disposes
        /// off the managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true" /> to release both managed and unmanaged
        /// resources; <see langword="false" /> to release only unmanaged
        /// resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed) {
                isDisposed = true;

                if (disposing) {
                    //
                    // release managed resources
                    //
                }

                //
                // release unmanaged resources
                //
                if (client != null) {
                    client.Close();
                    client = null;
                }
            }
        }
        #endregion public methods
    }
}