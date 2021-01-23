//------------------------------------------------------------------------------
// <sourcefile name="IPClient.cs" language="C#" begin="12/14/2003">
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
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using Gekkota.Checksums;
using Gekkota.Collections;
using Gekkota.Properties;

namespace Gekkota.Net.Sockets
{
    /// <summary>
    /// Provides Internet Protocol (IP) network services.
    /// </summary>
    /// <remarks>
    /// <c>IPClient</c> is the base class for any other IP-based network service
    /// providers.
    /// </remarks>
    /// <example>
    /// The following example shows how to create and initialize an
    /// <c>IPClient</c> that sends datagrams.
    /// <code>
    /// <![CDATA[
    /// using System;
    /// using System.Net;
    /// using Gekkota.Net;
    /// using Gekkota.Net.Sockets;
    ///
    /// public class Sender
    /// {
    ///     public static void Main(string[] args)
    ///     {
    ///         if (args.Length == 0) {
    ///             Console.WrileLine("Usage: Sender <hostname>");
    ///             Environment.Exit(0);
    ///         }
    /// 
    ///         //
    ///         // create and initialize a new Datagram
    ///         //
    ///         Datagram datagram = new Datagram();
    ///         datagram.Add(new Field(1, "Red"));
    ///         datagram.Add(new Field(2, "Green"));
    ///         datagram.Add(new Field(3, "Blue"));
    /// 
    ///         IPClient client = null;
    /// 
    ///         try {
    ///             //
    ///             // create and initialize a new IPClient
    ///             //
    ///             client = new IPClient(args[0], 0);
    /// 
    ///             //
    ///             // send the datagram
    ///             //
    ///             client.Send(datagram);
    ///         } finally {
    ///             if (client != null) client.Close();
    ///         }
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// The following example shows how to create and initialize an
    /// <c>IPClient</c> that listens for incoming datagrams.
    /// <code>
    /// <![CDATA[
    /// using System;
    /// using System.Net;
    /// using System.Threading;
    /// using Gekkota.Net;
    /// using Gekkota.Net.Sockets;
    ///
    /// public class Receiver
    /// {
    ///     public static void Main()
    ///     {
    ///         IPClient client = null;
    /// 
    ///         try {
    ///             //
    ///             // create and initialize a new IPClient
    ///             //
    ///             client = new IPClient();
    ///             client.Receive += new ReceiveEventHandler(OnReceive);
    /// 
    ///             //
    ///             // create an infinite loop to keep the application alive
    ///             //
    ///             while (true) {
    ///                 Thread.Sleep(0);
    ///             }
    ///         } finally {
    ///             if (client != null) client.Close();
    ///         }
    ///     }
    /// 
    ///     private static void OnReceive(Object sender, ReceiveEventArgs args)
    ///     {
    ///         Console.WriteLine("Receiving datagram from {0}...",
    ///             args.RemoteEndPoint.Address);
    /// 
    ///         foreach (Field field in args.Datagram) {
    ///             Console.WriteLine("Field {0}: {1}",
    ///                 field.Id, field.ValueAsString);
    ///         }
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// The code above produces the following output:
    /// <![CDATA[
    /// Receiving datagram from 85.0.155.150...
    ///     Field 1: Red
    ///     Field 2: Green
    ///     Field 3: Blue
    /// ]]>
    /// </example> 
    public class IPClient : IDisposable
    {
        #region public fields
        /// <summary>
        /// An <see cref="System.Int32" /> that represents the minimum time to
        /// live.
        /// </summary>
        public const int MinTimeToLive = 0;

        /// <summary>
        /// An <see cref="System.Int32" /> that represents the maximum time to
        /// live.
        /// </summary>
        public const int MaxTimeToLive = 255;
        #endregion public fields

        #region private fields
        private static Metafield defaultMetafield;

        private const int DefaultOptionsLength = 40;
        private const int DefaultPollTimeout = 250000;      // 250 ms
        private const int DefaultReceiveBufferSize = 32768; // 32 KB
        private const int DefaultSendBufferSize = 8192;     // 8 KB
        private const int DefaultTimeToLive = 60;           // hops count

        private const int IPMaxDatagramSize = 65535;
        private const int IPv6MaxDatagramSize = 65575;
        private const int IPHeaderSize = 20;
        private const int IPv6HeaderSize = 40;

        private ChecksumGenerator checksumGenerator;
        private Socket client;
        private Thread listener;
        private Collections.LinkedList<Metafield> payloadDescriptor;
        private ReceiveEventHandler receive;

        private int hashCode;
        private int baseOverhead;
        private int maxDatagramSize;
        private int pollTimeout = -1;
        private bool active = false;
        private bool isDisposed = false;
        private bool embedMetadata = false;
        #endregion private fields

        #region public events
        /// <summary>
        /// Occurs when an exception is thrown.
        /// </summary>
        public event ExceptionEventHandler Exception;

        /// <summary>
        /// Occurs when a <see cref="Gekkota.Net.Datagram" /> is received.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">
        /// There was an attempt to register or unregister an event handler
        /// even if the <see cref="Gekkota.Net.Sockets.IPClient" /> has already
        /// been terminated.
        /// </exception>
        public event ReceiveEventHandler Receive
        {
            add {
                CheckIfDisposed();

                lock (this) {
                    receive += value;
                }
                JoinListener();
            }

            remove {
                CheckIfDisposed();

                lock (this) {
                    receive -= value;
                }
                LeaveListener();
            }
        }
        #endregion public events

        #region public properties
        /// <summary>
        /// Gets or sets a value indicating whether or not the
        /// <see cref="Gekkota.Net.Sockets.IPClient" /> is allowed to Broadcast
        /// datagrams.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the <see cref="Gekkota.Net.Sockets.IPClient" />
        /// is allowed to broadcast datagrams; otherwise, <see langword="false" />.
        /// The default value is <see langword="false" />.
        /// </value>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="Gekkota.Net.Sockets.IPClient" /> has been terminated.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        public bool BroadcastEnabled
        {
            get {
                CheckIfDisposed();

                return 0 != (int) client.GetSocketOption(
                    SocketOptionLevel.Socket,
                    SocketOptionName.Broadcast);
            }

            set {
                CheckIfDisposed();

                client.SetSocketOption(
                    SocketOptionLevel.Socket,
                    SocketOptionName.Broadcast, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the
        /// <see cref="Gekkota.Net.Sockets.IPClient" /> should fragment outgoing
        /// datagrams.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the <see cref="Gekkota.Net.Sockets.IPClient" />
        /// should not fragment outgoing datagrams; otherwise,
        /// <see langword="false" />. The default value is <see langword="false" />.
        /// </value>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="Gekkota.Net.Sockets.IPClient" /> has been terminated.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        public bool DontFragment
        {
            get {
                CheckIfDisposed();

                return 0 != (int) client.GetSocketOption(
                    client.AddressFamily == AddressFamily.InterNetwork
                        ? SocketOptionLevel.IP
                        : SocketOptionLevel.IPv6,
                    SocketOptionName.DontFragment);
            }

            set {
                CheckIfDisposed();

                client.SetSocketOption(
                    client.AddressFamily == AddressFamily.InterNetwork
                        ? SocketOptionLevel.IP
                        : SocketOptionLevel.IPv6,
                    SocketOptionName.DontFragment, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the
        /// <see cref="Gekkota.Net.Sockets.IPClient" /> should embed metadata
        /// in outgoing datagrams.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the <see cref="Gekkota.Net.Sockets.IPClient" />
        /// should embed metadata in outgoing datagrams; otherwise,
        /// <see langword="false" />. The default value is <see langword="false" />.        /// </value>
        public virtual bool EmbedMetadata
        {
            get { return embedMetadata; }
            set { embedMetadata = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the
        /// <see cref="Gekkota.Net.Sockets.IPClient" /> has been disposed off.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the <see cref="Gekkota.Net.Sockets.IPClient" />
        /// has been disposed off; otherwise, <see langword="false" />.
        /// </value>
        public bool IsDisposed
        {
            get { return isDisposed; }
        }

        /// <summary>
        /// Gets the local endpoint.
        /// </summary>
        /// <value>
        /// The <see cref="System.Net.IPEndPoint" /> that the underlying
        /// <see cref="System.Net.Sockets.Socket" /> is using for communicating.
        /// </value>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="Gekkota.Net.Sockets.IPClient" /> has been terminated.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        public IPEndPoint LocalEndPoint
        {
            get {
                CheckIfDisposed();
                return client.LocalEndPoint as IPEndPoint;
            }
        }

        /// <summary>
        /// Gets or sets IP options to be inserted into outgoing datagrams.
        /// </summary>
        /// <value>
        /// A <see cref="System.Byte" /> array that specifies IP options to be
        /// inserted into outgoing datagrams.
        /// </value>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="Gekkota.Net.Sockets.IPClient" /> has been terminated.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The specified value is <see langword="null" />.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        public byte[] IPOptions
        {
            get {
                CheckIfDisposed();

                int length = 0;
                byte[] ipOptions = null;

                while (true) {
                    try {
                        ipOptions = (byte[]) client.GetSocketOption(
                            client.AddressFamily == AddressFamily.InterNetwork
                                ? SocketOptionLevel.IP
                                : SocketOptionLevel.IPv6,
                            SocketOptionName.IPOptions, length);
                        break;
                    } catch (SocketException) {
                        if (length < MaxDatagramSize - baseOverhead) {
                            length = length > 0 ? length << 1 : DefaultOptionsLength;
                        } else { throw; }
                    }
                }

                return ipOptions;
            }

            set {
                CheckIfDisposed();

                if (value == null) {
                    throw new ArgumentNullException("value");
                }

                client.SetSocketOption(
                    client.AddressFamily == AddressFamily.InterNetwork
                        ? SocketOptionLevel.IP
                        : SocketOptionLevel.IPv6,
                    SocketOptionName.IPOptions, value);
            }
        }

        /// <summary>
        /// Gets or sets the time to wait for a response when polling for
        /// incoming datagrams.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int32" /> that represents the time, in
        /// milliseconds, to wait for a response when polling for incoming
        /// datagrams.
        /// </value>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The specified value is less than 0.
        /// </exception>
        public int PollTimeout
        {
            get { return pollTimeout; }
            set {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException("value", value,
                        Resources.Error_NonNegativeNumberRequired);
                }

                pollTimeout = value;
            }
        }

        /// <summary>
        /// Gets the protocol name.
        /// </summary>
        /// <value>
        /// A <see cref="System.String" /> that contains the protocol name.
        /// </value>
        public virtual string ProtocolName
        {
            get { return "IP"; }
        }

        /// <summary>
        /// Gets or sets the total buffer space reserved for receives.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int32" /> that represents the total buffer
        /// space reserved for receives. The default value is 32768 bytes.
        /// </value>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="Gekkota.Net.Sockets.IPClient" /> has been terminated.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        /// <remarks>
        /// Incoming datagrams that do not fit in the
        /// <see cref="System.Net.Sockets.Socket" /> receive buffer are discarded.
        /// </remarks>
        public int ReceiveBufferSize
        {
            get {
                CheckIfDisposed();

                return (int) client.GetSocketOption(
                    SocketOptionLevel.Socket,
                    SocketOptionName.ReceiveBuffer);
            }

            set {
                CheckIfDisposed();

                lock (client) {
                    client.SetSocketOption(
                        SocketOptionLevel.Socket,
                        SocketOptionName.ReceiveBuffer, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum time the listener will block when
        /// attempting to receive data.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int32" /> that represents the maximum time, in
        /// milliseconds, the listener will block when attempting to receive
        /// data. The default is 0, which means the timeout is disabled.
        /// </value>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="Gekkota.Net.Sockets.IPClient" /> has been terminated.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        /// <remarks>
        /// If the <c>ReceiveTimeout</c> property is set to a value greater than
        /// 0 and data is not received within this time, a
        /// <see cref="System.Net.Sockets.SocketException" /> exception is thrown.
        /// <para>
        /// The <c>ReceiveTimeout</c> property has effect only if there are
        /// registered delegates for the Receive event.
        /// </para>
        /// </remarks>
        public int ReceiveTimeout
        {
            get {
                CheckIfDisposed();

                return (int) client.GetSocketOption(
                    SocketOptionLevel.Socket,
                    SocketOptionName.ReceiveTimeout);
            }

            set {
                CheckIfDisposed();

                lock (client) {
                    client.SetSocketOption(
                        SocketOptionLevel.Socket,
                        SocketOptionName.ReceiveTimeout, value);
                }
            }
        }

        /// <summary>
        /// Gets the remote endpoint.
        /// </summary>
        /// <value>
        /// The <see cref="System.Net.IPEndPoint" /> with which the underlying
        /// <see cref="System.Net.Sockets.Socket" /> is communicating.
        /// </value>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="Gekkota.Net.Sockets.IPClient" /> has been terminated.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        public IPEndPoint RemoteEndPoint
        {
            get {
                CheckIfDisposed();
                return client.RemoteEndPoint as IPEndPoint;
            }
        }

        /// <summary>
        /// Gets or sets the total buffer space reserved for sends.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int32" /> that represents the total buffer
        /// space reserved for sends. The default value is 8192 bytes.
        /// </value>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="Gekkota.Net.Sockets.IPClient" /> has been terminated.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        /// <remarks>
        /// IP sockets do not have a real send buffer; this property just
        /// represents the upper limit on the maximum-sized IP Datagram that can
        /// be written on the <see cref="System.Net.Sockets.Socket" />.
        /// </remarks>
        public int SendBufferSize
        {
            get {
                CheckIfDisposed();

                return (int) client.GetSocketOption(
                    SocketOptionLevel.Socket,
                    SocketOptionName.SendBuffer);
            }

            set {
                CheckIfDisposed();

                client.SetSocketOption(
                    SocketOptionLevel.Socket,
                    SocketOptionName.SendBuffer, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum time the
        /// <see cref="Gekkota.Net.Sockets.IPClient.Send(Gekkota.Net.Datagram)" />
        /// method will block when attempting to send data.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int32" /> that represents the maximum time, in
        /// milliseconds, the <see cref="Gekkota.Net.Sockets.IPClient.Send(Datagram)" />
        /// method will block when attempting to send data. The default is 0,
        /// which means the timeout is disabled.
        /// </value>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="Gekkota.Net.Sockets.IPClient" /> has been terminated.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        /// <remarks>
        /// If the <c>SendTimeout</c> property is set to a value greater than 0
        /// and data is not sent within this time, a
        /// <see cref="System.Net.Sockets.SocketException" /> exception is thrown.
        /// </remarks>
        public int SendTimeout
        {
            get {
                CheckIfDisposed();

                return (int) client.GetSocketOption(
                    SocketOptionLevel.Socket,
                    SocketOptionName.SendTimeout);
            }

            set {
                CheckIfDisposed();

                client.SetSocketOption(
                    SocketOptionLevel.Socket,
                    SocketOptionName.SendTimeout, value);
            }
        }

        /// <summary>
        /// Gets or sets the time to live.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int32" /> between
        /// <see cref="Gekkota.Net.Sockets.IPClient.MinTimeToLive" /> and
        /// <see cref="Gekkota.Net.Sockets.IPClient.MaxTimeToLive" /> that
        /// represents the number of router hops a
        /// <see cref="Gekkota.Net.Datagram" /> is allowed to traverse before it
        /// is terminated. The default value is 60.
        /// </value>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="Gekkota.Net.Sockets.IPClient" /> has been terminated.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The specified value is not between
        /// <see cref="Gekkota.Net.Sockets.IPClient.MinTimeToLive" /> and
        /// <see cref="Gekkota.Net.Sockets.IPClient.MaxTimeToLive" />.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        public int TimeToLive
        {
            get {
                CheckIfDisposed();

                return (int) client.GetSocketOption(
                    client.AddressFamily == AddressFamily.InterNetwork
                        ? SocketOptionLevel.IP
                        : SocketOptionLevel.IPv6,
                    SocketOptionName.IpTimeToLive);
            }

            set {
                CheckIfDisposed();

                if (value < MinTimeToLive || value > MaxTimeToLive) {
                    throw new ArgumentOutOfRangeException("value", value, 
                        String.Format(Resources.Error_ValueOutOfRange,
                        MinTimeToLive, MaxTimeToLive));
                }

                client.SetSocketOption(
                    client.AddressFamily == AddressFamily.InterNetwork
                        ? SocketOptionLevel.IP
                        : SocketOptionLevel.IPv6,
                    SocketOptionName.IpTimeToLive, value);
            }
        }

        /// <summary>
        /// Gets or sets the type of service.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int32" /> that represents the type of service.
        /// </value>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="Gekkota.Net.Sockets.IPClient" /> has been terminated.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        public int TypeOfService
        {
            get {
                CheckIfDisposed();

                return (int) client.GetSocketOption(
                    client.AddressFamily == AddressFamily.InterNetwork
                        ? SocketOptionLevel.IP
                        : SocketOptionLevel.IPv6,
                    SocketOptionName.TypeOfService);
            }

            set {
                CheckIfDisposed();

                client.SetSocketOption(
                    client.AddressFamily == AddressFamily.InterNetwork
                        ? SocketOptionLevel.IP
                        : SocketOptionLevel.IPv6,
                    SocketOptionName.TypeOfService, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the
        /// <see cref="Gekkota.Net.Sockets.IPClient" /> should bypass the
        /// hardware when possible.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the <see cref="Gekkota.Net.Sockets.IPClient" />
        /// should bypass the hardware when possible; otherwise,
        /// <see langword="false" />. The default value is <see langword="true" />.
        /// </value>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="Gekkota.Net.Sockets.IPClient" /> has been terminated.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        public bool UseLoopback
        {
            get {
                CheckIfDisposed();

                return 0 != (int) client.GetSocketOption(
                    client.AddressFamily == AddressFamily.InterNetwork
                        ? SocketOptionLevel.IP
                        : SocketOptionLevel.IPv6,
                    SocketOptionName.ReuseAddress);
            }

            set {
                CheckIfDisposed();

                client.SetSocketOption(
                    client.AddressFamily == AddressFamily.InterNetwork
                        ? SocketOptionLevel.IP
                        : SocketOptionLevel.IPv6,
                    SocketOptionName.ReuseAddress, value ? 1 : 0);
            }
        }
        #endregion public properties

        #region public field descriptors
        /// <summary>
        /// Gets the default <see cref="Gekkota.Net.Field" /> descriptor.
        /// </summary>
        /// <value>
        /// A <see cref="Gekkota.Net.Metafield" /> that describes the protocol
        /// payload as a unique <see cref="System.Byte" /> array.
        /// </value>
        /// <remarks>
        /// The default <see cref="Gekkota.Net.Field" /> descriptor is used to
        /// interpret datagrams when the
        /// <see cref="Gekkota.Net.Sockets.IPClient.EmbedMetadata" /> property
        /// is set to <see langword="false" /> and the
        /// <see cref="Gekkota.Net.ProtocolFieldAttribute" /> is not applied to
        /// any custom <see cref="Gekkota.Net.Field" /> descriptors.
        /// </remarks>
        public static Metafield DefaultMetafield
        {
            get {
                if (defaultMetafield == null) {
                    lock (typeof(IPClient)) {
                        if (defaultMetafield == null) {
                            defaultMetafield = new Metafield(0, FieldType.ByteArray);
                            defaultMetafield = Metafield.ReadOnly(defaultMetafield);
                        }
                    }
                }

                return defaultMetafield;
            }
        }
        #endregion public field descriptors

        #region protected properties
        /// <summary>
        /// Gets or sets a value indicating whether or not a default remote host
        /// has been establilshed.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if a connection is active; otherwise,
        /// <see langword="false" />.
        /// </value>
        protected bool Active
        {
            get { return active; }
            set { active = value; }
        }

        /// <summary>
        /// Gets the underlying network <see cref="System.Net.Sockets.Socket" />.
        /// </summary>
        /// <value>
        /// The underlying network <see cref="System.Net.Sockets.Socket" />.
        /// </value>
        /// <remarks>
        /// Derived classes can provide their own network
        /// <see cref="System.Net.Sockets.Socket" /> by overriding the
        /// <see cref="Gekkota.Net.Sockets.IPClient.CreateClient" /> method.
        /// </remarks>
        protected Socket Client
        {
            get { return client; }
        }
        #endregion protected properties

        #region internal properties
        /// <summary>
        /// Gets the overhead generated by TCP/IP headers.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int32" /> that represents the overhead
        /// generated by TCP/IP headers, in bytes.
        /// </value>
        /// <remarks>
        /// The <c>BaseOverhead</c> property does not include overhead produced
        /// by metadata.
        /// </remarks>
        internal virtual int BaseOverhead
        {
            get { return baseOverhead + IPOptions.Length; }
        }

        /// <summary>
        /// Gets the underlying <see cref="Gekkota.Checksums.ChecksumGenerator" />.
        /// </summary>
        /// <value>
        /// The underlying <see cref="Gekkota.Checksums.ChecksumGenerator" />.
        /// </value>
        /// <remarks>
        /// <see cref="Gekkota.Net.Sockets.IPClient.ChecksumGenerator" />
        /// generates 16-bit CRCs.
        /// </remarks>
        internal ChecksumGenerator ChecksumGenerator
        {
            get {
                if (checksumGenerator == null) {
                    checksumGenerator = new ChecksumGenerator(new Crc16());
                }

                return checksumGenerator;
            }
        }

        /// <summary>
        /// Gets the maximum <see cref="Gekkota.Net.Datagram" /> size.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int32" /> that represents the maximum
        /// <see cref="Gekkota.Net.Datagram" /> size, in bytes.
        /// </value>
        internal int MaxDatagramSize
        {
            get { return maxDatagramSize; }
        }
        #endregion internal properties

        #region private properties
        /// <summary>
        /// Gets the protocol payload descriptor.
        /// </summary>
        /// <value>
        /// The protocol payload descriptor.
        /// </value>
        private Collections.LinkedList<Metafield> PayloadDescriptor
        {
            get {
                if (payloadDescriptor == null) {
                    CreatePayloadDescriptor();
                }

                return payloadDescriptor;
            }
        }
        #endregion private properties

        #region public constructors
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Gekkota.Net.Sockets.IPClient" /> class.
        /// </summary>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        public IPClient()
        {
            Initialize(new IPEndPoint(IPAddress.Any, 0));
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Gekkota.Net.Sockets.IPClient" /> class with the specified
        /// addressing scheme.
        /// </summary>
        /// <param name="addressFamily">
        /// One of the <see cref="System.Net.Sockets.AddressFamily" /> values.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="addressFamily" /> is neither
        /// <see cref="System.Net.Sockets.AddressFamily.InterNetwork" /> nor
        /// <see cref="System.Net.Sockets.AddressFamily.InterNetworkV6" />.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        public IPClient(AddressFamily addressFamily)
        {
            IPAddress ipAddress = null;

            if (addressFamily == AddressFamily.InterNetwork) {
                ipAddress = IPAddress.Any;
            } else if (addressFamily == AddressFamily.InterNetworkV6) {
                ipAddress = IPAddress.IPv6Any;
            } else {
                throw new ArgumentException(
                    Resources.Error_AddressFamilyNotValid);
            }

            Initialize(new IPEndPoint(ipAddress, 0));
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Gekkota.Net.Sockets.IPClient" /> class and binds it to
        /// the specified local endpoint.
        /// </summary>
        /// <param name="localEndPoint">
        /// An <see cref="System.Net.IPEndPoint" /> that represents the local
        /// endpoint through which data is sent or received.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="localEndPoint" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        public IPClient(IPEndPoint localEndPoint)
        {
            if (localEndPoint == null) {
                throw new ArgumentNullException("localEndPoint");
            }

            Initialize(localEndPoint);
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Gekkota.Net.Sockets.IPClient" /> class and establishes a
        /// default remote host using the specified host name and port number.
        /// </summary>
        /// <param name="hostname">
        /// A <see cref="System.String" /> that contains the DNS name of the
        /// remote host to which data should be sent.
        /// </param>
        /// <param name="port">
        /// An <see cref="System.Int32" /> that represents the port number on
        /// the remote host to which data should be sent. If the current
        /// protocol does not support ports, this parameter is ignored.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="hostname" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="port" /> is not between
        /// <see cref="System.Net.IPEndPoint.MinPort" /> and
        /// <see cref="System.Net.IPEndPoint.MaxPort" />.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        /// <remarks>
        /// Do not use this constructor if you intend to receive multicasted
        /// datagrams.
        /// </remarks>
        public IPClient(string hostname, int port)
        {
            IPAddress[] addresses = Dns.GetHostEntry(hostname).AddressList;
            AddressFamily addressFamily = AddressFamily.Unspecified;

            for (int i = 0; i < addresses.Length; i++) {
                try {
                    if (addresses[i].AddressFamily != addressFamily) {
                        addressFamily = addresses[i].AddressFamily;

                        Initialize(
                            new IPEndPoint(addressFamily == AddressFamily.InterNetwork
                                ? IPAddress.Any
                                : IPAddress.IPv6Any, 0));
                    }
                    Connect(new IPEndPoint(addresses[i], port));
                    break;
                } catch (Exception) {
                    if (i == addresses.Length - 1) {
                        if (client != null) {
                            client.Close();
                            client = null;
                        }
                        throw;
                    }
                }
            }
        }
        #endregion public constructors

        #region internal constructors
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Gekkota.Net.Sockets.IPClient" /> class and binds it to
        /// the specified local port number.
        /// </summary>
        /// <param name="port">
        /// An <see cref="System.Int32" /> that represents the local port number
        /// through which data is sent or received. If the current protocol does
        /// not support ports, this parameter is ignored.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="port" /> is not between <see cref="System.Net.IPEndPoint.MinPort" />
        /// and <see cref="System.Net.IPEndPoint.MaxPort" />.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        internal IPClient(int port)
        {
            Initialize(new IPEndPoint(IPAddress.Any, port));
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Gekkota.Net.Sockets.IPClient" /> class with the
        /// specified addressing scheme and binds it to the specified local port
        /// number.
        /// </summary>
        /// <param name="addressFamily">
        /// One of the <see cref="System.Net.Sockets.AddressFamily" /> values.
        /// </param>
        /// <param name="port">
        /// An <see cref="System.Int32" /> that represents the local port number
        /// through which data is sent or received. If the current protocol does
        /// not support ports, this parameter is ignored.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="port" /> is not between <see cref="System.Net.IPEndPoint.MinPort" />
        /// and <see cref="System.Net.IPEndPoint.MaxPort" />.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="addressFamily" /> is neither
        /// <see cref="System.Net.Sockets.AddressFamily.InterNetwork" /> nor
        /// <see cref="System.Net.Sockets.AddressFamily.InterNetworkV6" />.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        internal IPClient(int port, AddressFamily addressFamily)
        {
            IPAddress ipAddress = null;

            if (addressFamily == AddressFamily.InterNetwork) {
                ipAddress = IPAddress.Any;
            } else if (addressFamily == AddressFamily.InterNetworkV6) {
                ipAddress = IPAddress.IPv6Any;
            } else {
                throw new ArgumentException(
                    Resources.Error_AddressFamilyNotValid);
            }

            Initialize(new IPEndPoint(ipAddress, port));
        }
        #endregion internal constructors

        #region finalizer
        /// <summary>
        /// Frees the resources used by the
        /// <see cref="Gekkota.Net.Sockets.IPClient" />.
        /// </summary>
        ~IPClient()
        {
            Dispose(false);
        }
        #endregion finalizer

        #region public methods
        /// <summary>
        /// Terminates the <see cref="Gekkota.Net.Sockets.IPClient" /> and
        /// releases all the associated resources.
        /// </summary>
        public void Close()
        {
          ((IDisposable) this).Dispose();
        }

        /// <summary>
        /// Establishes a default remote host using the specified endpoint.
        /// </summary>
        /// <param name="remoteEndPoint">
        /// An <see cref="System.Net.IPEndPoint" /> that represents the remote
        /// endpoint to which data should be sent.
        /// </param>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="Gekkota.Net.Sockets.IPClient" /> has been terminated.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="remoteEndPoint" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        /// <remarks>
        /// Do not call this method if you intend to receive multicasted
        /// datagrams.
        /// </remarks>
        public virtual void Connect(IPEndPoint remoteEndPoint)
        {
            CheckIfDisposed();

            //
            // after calling the Socket.Connect() method, Socket.LocalEndPoint
            // is wrongly set to the same value as Socket.RemoteEndPoint; the
            // following dummy assignment is to let the Socket class keep the
            // correct value
            //
            EndPoint endPoint = client.LocalEndPoint;

            client.Connect(remoteEndPoint);
            active = true;
        }

        /// <summary>
        /// Establishes a default remote host using the specified host name and
        /// port number.
        /// </summary>
        /// <param name="hostname">
        /// A <see cref="System.String" /> that contains the DNS name of the
        /// remote host to which data should be sent.
        /// </param>
        /// <param name="port">
        /// An <see cref="System.Int32" /> that represents the port number on
        /// the remote host to which data should be sent. If the current
        /// protocol does not support ports, this parameter is ignored.
        /// </param>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="Gekkota.Net.Sockets.IPClient" /> has been terminated.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="remoteEndPoint" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="port" /> is not between
        /// <see cref="System.Net.IPEndPoint.MinPort" /> and
        /// <see cref="System.Net.IPEndPoint.MaxPort" />.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        /// <remarks>
        /// Do not call this method if you intend to receive multicasted
        /// datagrams.
        /// </remarks>
        public virtual void Connect(String hostname, int port)
        {
            IPAddress[] addresses = Dns.GetHostEntry(hostname).AddressList;

            for (int i = 0; i < addresses.Length; i++) {
                try {
                    Connect(new IPEndPoint(addresses[i], port));
                    break;
                } catch (Exception) {
                    if (i == addresses.Length - 1) { throw; }
                }
            }
        }

        /// <summary>
        /// Establishes a default remote host using the specified IP address and
        /// port number.
        /// </summary>
        /// <param name="address">
        /// An <see cref="System.Net.IPAddress" /> that represents the IP
        /// address of the remote host to which data should be sent.
        /// </param>
        /// <param name="port">
        /// An <see cref="System.Int32" /> that represents the port number on
        /// the remote host to which data should be sent. If the current
        /// protocol does not support ports, this parameter is ignored.
        /// </param>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="Gekkota.Net.Sockets.IPClient" /> has been terminated.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="address" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="port" /> is not between
        /// <see cref="System.Net.IPEndPoint.MinPort" /> and
        /// <see cref="System.Net.IPEndPoint.MaxPort" />.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        /// <remarks>
        /// Do not call this method if you intend to receive multicasted
        /// datagrams.
        /// </remarks>
        public virtual void Connect(IPAddress address, int port)
        {
            Connect(new IPEndPoint(address, port));
        }

        /// <summary>
        /// Leaves the specified multicast group.
        /// </summary>
        /// <param name="multicastAddress">
        /// An <see cref="System.Net.IPAddress" /> that represents the multicast
        /// group to leave.
        /// </param>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="Gekkota.Net.Sockets.IPClient" /> has been terminated.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="multicastAddress" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="multicastAddress" /> is not compatible with the
        /// addressing scheme of the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        public virtual void DropMulticastGroup(IPAddress multicastAddress)
        {
            CheckIfDisposed();

            if (client.AddressFamily == AddressFamily.InterNetwork) {
                client.SetSocketOption(
                    SocketOptionLevel.IP,
                    SocketOptionName.DropMembership,
                    new MulticastOption(multicastAddress));
            } else {
                client.SetSocketOption(
                    SocketOptionLevel.IPv6,
                    SocketOptionName.DropMembership,
                    new IPv6MulticastOption(multicastAddress));
            }
        }

        /// <summary>
        /// Leaves the specified multicast group for the local interface at the
        /// specified index.
        /// </summary>
        /// <param name="multicastAddress">
        /// An <see cref="System.Net.IPAddress" /> that represents the multicast
        /// group to leave.
        /// </param>
        /// <param name="ifindex">
        /// An <see cref="System.Int32" /> that represents the index of the
        /// local interface for which to leave the multicast group.
        /// </param>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="Gekkota.Net.Sockets.IPClient" /> has been terminated.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="multicastAddress" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="ifindex" /> is less than 0.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="multicastAddress" /> is not compatible with the
        /// addressing scheme of the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        public virtual void DropMulticastGroup(IPAddress multicastAddress, int ifindex)
        {
            CheckIfDisposed();

            client.SetSocketOption(
                SocketOptionLevel.IPv6,
                SocketOptionName.DropMembership,
                new IPv6MulticastOption(multicastAddress, ifindex));
        }

        /// <summary>
        /// Joins the specified multicast group.
        /// </summary>
        /// <param name="multicastAddress">
        /// An <see cref="System.Net.IPAddress" /> that represents the multicast
        /// group to join.
        /// </param>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="Gekkota.Net.Sockets.IPClient" /> has been terminated.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="multicastAddress" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="multicastAddress" /> is not compatible with the
        /// addressing scheme of the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        public virtual void JoinMulticastGroup(IPAddress multicastAddress)
        {
            CheckIfDisposed();

            if (client.AddressFamily == AddressFamily.InterNetwork) {
                client.SetSocketOption(
                    SocketOptionLevel.IP,
                    SocketOptionName.AddMembership,
                    new MulticastOption(multicastAddress));
            } else {
                client.SetSocketOption(
                    SocketOptionLevel.IPv6,
                    SocketOptionName.AddMembership,
                    new IPv6MulticastOption(multicastAddress));
            }
        }

        /// <summary>
        /// Joins the specified multicast group with the specified time to live.
        /// </summary>
        /// <param name="multicastAddress">
        /// An <see cref="System.Net.IPAddress" /> that represents the multicast
        /// group to join.
        /// </param>
        /// <param name="timeToLive">
        /// An <see cref="System.Int32" /> that represents the number of router
        /// hops a datagram is allowed to traverse before it is terminated.
        /// </param>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="Gekkota.Net.Sockets.IPClient" /> has been terminated.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="multicastAddress" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="multicastAddress" /> is not compatible with the
        /// addressing scheme of the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="timeToLive" /> is not between
        /// <see cref="Gekkota.Net.Sockets.IPClient.MinTimeToLive" /> and
        /// <see cref="Gekkota.Net.Sockets.IPClient.MaxTimeToLive" />.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        public virtual void JoinMulticastGroup(IPAddress multicastAddress, int timeToLive)
        {
            JoinMulticastGroup(multicastAddress);

            if (timeToLive < MinTimeToLive || timeToLive > MaxTimeToLive) {

                throw new ArgumentOutOfRangeException("timeToLive", timeToLive,
                    String.Format(Resources.Error_ValueOutOfRange,
                    MinTimeToLive, MaxTimeToLive));
            }

            client.SetSocketOption(
                client.AddressFamily == AddressFamily.InterNetwork
                    ? SocketOptionLevel.IP
                    : SocketOptionLevel.IPv6,
                SocketOptionName.MulticastTimeToLive, timeToLive);
        }

        /// <summary>
        /// Joins the specified multicast group for the local interface at the
        /// specified index.
        /// </summary>
        /// <param name="ifindex">
        /// An <see cref="System.Int32" /> that represents the index of the
        /// local interface for which to join the multicast group.
        /// </param>
        /// <param name="multicastAddress">
        /// An <see cref="System.Net.IPAddress" /> that represents the multicast
        /// group to join.
        /// </param>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="Gekkota.Net.Sockets.IPClient" /> has been terminated.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="multicastAddress" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="ifindex" /> is less than 0.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="multicastAddress" /> is not compatible with the
        /// addressing scheme of the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        public virtual void JoinMulticastGroup(int ifindex, IPAddress multicastAddress)
        {
            CheckIfDisposed();

            client.SetSocketOption(
                SocketOptionLevel.IPv6,
                SocketOptionName.AddMembership,
                new IPv6MulticastOption(multicastAddress, ifindex));
        }

        /// <summary>
        /// Returns the hash code for this instance of the
        /// <see cref="Gekkota.Net.Sockets.IPClient" /> class.
        /// </summary>
        /// <returns>
        /// An <see cref="System.Int32" /> that represents the hash code
        /// generated from 
        /// </returns>
        /// <remarks>
        /// The default implementation of the <c>GetHashCode</c> method
        /// returns a hash code generated from
        /// <see cref="Gekkota.Net.Sockets.IPClient.ProtocolName" />.
        /// </remarks>
        public override int GetHashCode()
        {
            if (ProtocolName == null || ProtocolName.Length == 0) {
                throw new MemberAccessException(String.Format(
                    Resources.Error_MemberNotValid, "ProtocolName"));
            }

            if (hashCode == 0) {
                hashCode = HashCodeGenerator.Generate(ProtocolName);
            }

            return hashCode;
        }

        /// <summary>
        /// Sends the specified <see cref="Gekkota.Net.Datagram" /> to the
        /// default remote host.
        /// </summary>
        /// <param name="datagram">
        /// The <see cref="Gekkota.Net.Datagram" /> to send.
        /// </param>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="Gekkota.Net.Sockets.IPClient" /> has been terminated.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="datagram" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// A default remote host has not been established.
        /// </exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// An error has occurred during serialization of
        /// <paramref name="datagram" />. Refer to the inner exception for more
        /// details.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// <see cref="Gekkota.Net.Sockets.IPClient.SendTimeout" /> has elapsed.
        /// <para>-or-</para>
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        public virtual void Send(Datagram datagram)
        {
            CheckIfDisposed();

            if (!active) {
                throw new InvalidOperationException(
                    Resources.Error_RemoteHostNotEstablished);
            }

            Send(datagram, null);
        }

        /// <summary>
        /// Sends the specified <see cref="Gekkota.Net.Datagram" /> to the host
        /// at the specified remote endpoint.
        /// </summary>
        /// <param name="datagram">
        /// The <see cref="Gekkota.Net.Datagram" /> to send.
        /// </param>
        /// <param name="remoteEndPoint">
        /// An <see cref="System.Net.IPEndPoint" /> that represents the remote
        /// host and port number to which to send the Datagram.
        /// </param>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="Gekkota.Net.Sockets.IPClient" /> has been terminated.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="datagram" /> is <see langword="null" />.
        /// <para>-or-</para>
        /// <paramref name="remoteEndPoint" /> is <see langword="null" /> and a
        /// default remote host has not been established.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// <paramref name="remoteEndPoint" /> is not <see langword="null" /> and
        /// a default host has been already established.
        /// </exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// An error has occurred during serialization of
        /// <paramref name="datagram" />. Refer to the inner exception for more
        /// details.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// <see cref="Gekkota.Net.Sockets.IPClient.SendTimeout" /> has elapsed.
        /// <para>-or-</para>
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        public virtual void Send(Datagram datagram, IPEndPoint remoteEndPoint)
        {
            CheckIfDisposed();
            byte[] data = Serialize(datagram);
            Send(data, remoteEndPoint);
        }

        /// <summary>
        /// Sends the specified <see cref="Gekkota.Net.Datagram" /> to the
        /// specified port on the specified remote host.
        /// </summary>
        /// <param name="datagram">
        /// The <see cref="Gekkota.Net.Datagram" /> to send.
        /// </param>
        /// <param name="hostname">
        /// A <see cref="System.String" /> that contains the name of the remote
        /// host to which <paramref name="datagram" /> should be sent.
        /// </param>
        /// <param name="port">
        /// The remote port number with which to communicate.
        /// </param>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="Gekkota.Net.Sockets.IPClient" /> has been terminated.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="datagram" /> or <paramref name="hostname" /> is
        /// <see langword="null" />.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="port" /> is not between
        /// <see cref="System.Net.IPEndPoint.MinPort" /> and
        /// <see cref="System.Net.IPEndPoint.MaxPort" />.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// A default remote host has been already established.
        /// </exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// An error has occurred during serialization of
        /// <paramref name="datagram" />. Refer to the inner exception for more
        /// details.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// <see cref="Gekkota.Net.Sockets.IPClient.SendTimeout" /> has elapsed.
        /// <para>-or-</para>
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        public virtual void Send(Datagram datagram, string hostname, int port)
        {
            IPAddress[] addresses = Dns.GetHostEntry(hostname).AddressList;

            for (int i = 0; i < addresses.Length; i++) {
                try {
                    Send(datagram, new IPEndPoint(addresses[i], port));
                    break;
                } catch (Exception) {
                    if (i == addresses.Length - 1) { throw; }
                }
            }
        }

        /// <summary>
        /// Terminates the <see cref="Gekkota.Net.Sockets.IPClient" /> and
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
        /// Returns a network <see cref="System.Net.Sockets.Socket" /> that uses
        /// IP as the communication protocol.
        /// </summary>
        /// <param name="addressFamily">
        /// One of the <see cref="System.Net.Sockets.AddressFamily" /> values.
        /// </param>
        /// <returns>
        /// A <see cref="System.Net.Sockets.Socket" /> that uses IP as the
        /// communication protocol.
        /// </returns>
        protected virtual Socket CreateClient(AddressFamily addressFamily)
        {
            return new Socket(addressFamily, SocketType.Raw, ProtocolType.IP);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the
        /// <see cref="Gekkota.Net.Sockets.IPClient" />, and optionally disposes
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
                lock (this) {
                    receive = null;
                }

                LeaveListener();

                if (client != null) {
                    client.Close();
                    client = null;
                }
            }
        }

        /// <summary>
        /// Performs additional custom processes before deserializing the
        /// specified array of bytes.
        /// </summary>
        /// <param name="data">
        /// A <see cref="System.Byte" /> array that contains the
        /// <see cref="Gekkota.Net.Datagram" /> to deserialize.
        /// </param>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the starting position
        /// within <paramref name="data" />.
        /// </param>
        /// <param name="manifest">
        /// A <see cref="Gekkota.Net.Field" /> that represents the manifest
        /// generated for <paramref name="data"/>.
        /// </param>
        /// <returns>
        /// An <see cref="System.Int32" /> that represents the starting position
        /// of the payload within <paramref name="data" />, or -1 if
        /// <paramref name="data" /> cannot be interpreted by the current protocol.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="data" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.
        /// </exception>
        /// <remarks>
        /// The default implementation of the <c>OnDeserialize</c> method checks
        /// whether or not the specified byte array contains an IP datagram, and
        /// if it does, returns the starting position of the payload. Derived
        /// classes can override the <c>OnDeserialize</c> method to perform
        /// additional filtering operations.
        /// </remarks>
        protected virtual int OnDeserialize(byte[] data, int index, out Field manifest)
        {
            if (client.ProtocolType == ProtocolType.IP) {
                if (client.AddressFamily == AddressFamily.InterNetwork) {
                    index = (data[index] & 0x0F) << 2;
                }
            }

            //
            // IPv6 does not return header data; relay filtering to the next
            // protocol
            //

            manifest = GetManifest(data, index);

            if (manifest != null) {
                index += (manifest.Size + Metafield.LayoutSize);
            }

            return index;
        }

        /// <summary>
        /// Performs additional custom processes after deserializing the
        /// specified array of bytes.
        /// </summary>
        /// <param name="data">
        /// A <see cref="System.Byte" /> array that contains the
        /// <see cref="Gekkota.Net.Datagram" /> to deserialize.
        /// </param>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the starting position
        /// within <paramref name="data" />.
        /// </param>
        /// <param name="datagram">
        /// The deserialized <see cref="Gekkota.Net.Datagram" />.
        /// </param>
        /// <returns>
        /// A <see cref="Gekkota.Net.Datagram" /> array that contains the
        /// deserialized <see cref="Gekkota.Net.Datagram" />.
        /// </returns>
        /// <remarks>
        /// The default implementation of the <c>OnDeserializeComplete</c> method
        /// does nothing. Derived classes can override it to perform some action
        /// after the specified <see cref="Gekkota.Net.Datagram" /> is
        /// deserialized.
        /// </remarks>
        protected virtual Datagram[] OnDeserializeComplete(
            byte[] data, int index, Datagram datagram)
        {
            return new Datagram[] { datagram };
        }

        /// <summary>
        /// Performs additional custom processes before the Listener is
        /// terminated.
        /// </summary>
        /// <remarks>
        /// The default implementation of the <c>OnJoinListener</c> method does
        /// nothing. Derived classes can override it to perform some action
        /// before the Listener is terminated.
        /// </remarks>
        protected virtual void OnJoinListener()
        {
        }

        /// <summary>
        /// Provides default processing for incoming datagrams.
        /// </summary>
        /// <param name="data">
        /// A <see cref="System.Byte" /> array that contains the data to
        /// process.
        /// </param>
        /// <param name="remoteEndPoint">
        /// An <see cref="System.Net.EndPoint" /> that represents the remote
        /// endpoint from which <paramref name="data" /> was sent.
        /// </param>
        /// <remarks>
        /// The Listener invokes the <c>OnListen</c> method whenever new
        /// datagrams arrive.
        /// </remarks>
        protected virtual void OnListen(byte[] data, EndPoint remoteEndPoint)
        {
            if (data != null) {
                if (receive != null) {
                    Datagram[] datagrams = null;

                    try {
                        datagrams = Deserialize(data, 0);
                    } catch (Exception e) {
                        if (this.Exception != null) {
                            this.Exception(this, new ExceptionEventArgs(e));
                        }
                        return;
                    }

                    if (datagrams != null) {
                        foreach (Datagram datagram in datagrams) {
                            lock (this) {
                                if (receive != null) {
                                    receive(this, new ReceiveEventArgs(
                                        datagram, remoteEndPoint));
                                } else { break; }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Performs additional custom processes before serializing the
        /// specified <see cref="Gekkota.Net.Datagram" />.
        /// </summary>
        /// <param name="datagram">
        /// The <see cref="Gekkota.Net.Datagram" /> to serialize.
        /// </param>
        /// <returns>
        /// The <see cref="Gekkota.Net.Datagram" /> to serialize.
        /// </returns>
        /// <remarks>
        /// The default implementation of the <c>OnSerialize</c> method does
        /// nothing. Derived classes can override it to perform some action
        /// before the specified <see cref="Gekkota.Net.Datagram" /> is
        /// serialized.
        /// </remarks>
        protected virtual Datagram OnSerialize(Datagram datagram)
        {
            //
            // create a shallow copy of the given Datagram in order
            // to allow manipulations without affecting the caller
            //
            datagram = datagram.Clone();

            if (EmbedMetadata) {
                //
                // insert the manifest at the beginning of the Datagram
                //
                datagram.Insert(0, CreateManifest(datagram));
            }

            return datagram;
        }

        /// <summary>
        /// Performs additional custom processes after serializing the specified
        /// <see cref="Gekkota.Net.Datagram" />.
        /// </summary>
        /// <param name="datagram">
        /// The <see cref="Gekkota.Net.Datagram" /> to serialize.
        /// </param>
        /// <param name="data">
        /// A <see cref="System.Byte" /> array that contains the serialized
        /// <see cref="Gekkota.Net.Datagram" />.
        /// </param>
        /// <returns>
        /// A <see cref="System.Byte" /> array that contains the serialized
        /// <see cref="Gekkota.Net.Datagram" />.
        /// </returns>
        /// <remarks>
        /// The default implementation of the <c>OnSerializeComplete</c> method
        /// does nothing. Derived classes can override it to perform some action
        /// after the specified <see cref="Gekkota.Net.Datagram" /> is
        /// serialized.
        /// </remarks>
        protected virtual byte[] OnSerializeComplete(Datagram datagram, byte[] data)
        {
            return data;
        }

        /// <summary>
        /// Performs additional custom processes when validating the specified
        /// <see cref="Gekkota.Net.Datagram" />.
        /// </summary>
        /// <param name="datagram">
        /// The <see cref="Gekkota.Net.Datagram" /> to validate.
        /// </param>
        /// <remarks>
        /// The default implementation of the <c>OnValidate</c> method does
        /// nothing. Derived classes can override it to perform some action when
        /// the specified <see cref="Gekkota.Net.Datagram" /> is validated.
        /// </remarks>
        protected virtual void OnValidate(Datagram datagram)
        {
            if (PayloadDescriptor.Count > 0) {
                int count = 0;

                if (PayloadDescriptor.Count == datagram.Count) {
                    Collections.LinkedList<Field>.Node fNode = datagram.Head;
                    for (Collections.LinkedList<Metafield>.Node mNode = PayloadDescriptor.Head;
                        mNode != null;
                        mNode = mNode.Next) {
                        if (mNode.Value != fNode.Value.Metafield) {
                            if (mNode.Value.ProtocolFieldAttribute.Position > count) {
                                break;
                            } else { continue; }
                        }

                        fNode = fNode.Next;
                        count++;
                    }
                }

                if (datagram.Count != count) {
                    throw new InvalidOperationException(
                        Resources.Error_DatagramNotValid);
                }
            }
        }
        #endregion protected methods

        #region internal methods
        /// <summary>
        /// Checks whether or not the <see cref="Gekkota.Net.Sockets.IPClient" />
        /// has been terminated.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="Gekkota.Net.Sockets.IPClient" /> has been terminated.
        /// </exception>
        internal void CheckIfDisposed()
        {
            if (isDisposed) {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        /// <summary>
        /// Deserializes the specified byte array into a
        /// <see cref="Gekkota.Net.Datagram" />.
        /// </summary>
        /// <param name="data">
        /// The <see cref="System.Byte" /> array from which to deserialize the
        /// <see cref="Gekkota.Net.Datagram" />.
        /// </param>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the starting position
        /// within <paramref name="data" />.
        /// </param>
        /// <returns>
        /// A <see cref="Gekkota.Net.Datagram" /> array that contains the
        /// deserialized <see cref="Gekkota.Net.Datagram" />, if
        /// <paramref name="data" /> is interpretable by this protocol; otherwise,
        /// <see langword="null" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="data" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.
        /// </exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// An error has occurred during deserialization of
        /// <paramref name="data" />. Refer to the inner exception for more
        /// details.
        /// </exception>
        internal Datagram[] Deserialize(byte[] data, int index)
        {
            Datagram[] datagrams = null;

            try {
                Field manifest = null;
                index = OnDeserialize(data, index, out manifest);

                if (index < 0) {
                    return null;
                }

                Field field = null;
                Datagram datagram = new Datagram();

                if (manifest != null) {
                    //
                    // [data] contains metadata
                    //
                    while (index < data.Length) {
                        field = FieldSerializer.Deserialize(data, index);
                        datagram.Add(field);
                        index += (field.Size + Metafield.LayoutSize);
                    }
                } else {
                    //
                    // [data] is raw
                    //
                    if (PayloadDescriptor.Count > 0) {
                        foreach (Metafield metafield in PayloadDescriptor) {
                            if (index < data.Length) {
                                if (metafield.Category != FieldCategory.Header) {
                                    field = FieldSerializer.Deserialize(
                                        data, index, metafield);
                                    datagram.Add(field);
                                    index += field.Size;
                                }
                            } else { break; }
                        }
                    } else {
                        field = FieldSerializer.Deserialize(data, index, DefaultMetafield);
                        datagram.Add(field);
                    }
                }

                datagrams = OnDeserializeComplete(data, index, datagram);
            } catch (Exception e) {
                throw new SerializationException(
                    Resources.Error_DeserializationError, e);
            }

            return datagrams;
        }

        /// <summary>
        /// Serializes the specified <see cref="Gekkota.Net.Datagram" />.
        /// </summary>
        /// <param name="datagram">
        /// The <see cref="Gekkota.Net.Datagram" /> to serialize.
        /// </param>
        /// <returns>
        /// A <see cref="System.Byte" /> array that contains the serialized
        /// <see cref="Gekkota.Net.Datagram" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="datagram" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// An error has occurred during serialization of
        /// <paramref name="datagram" />. Refer to the inner exception for more
        /// details.
        /// </exception>
        internal byte[] Serialize(Datagram datagram)
        {
            byte[] buffer = null;

            try {
                Validate(datagram);
                datagram = OnSerialize(datagram);

                //
                // if [EmbedMetadata] is true, then the first Field of the
                // Datagram returned by OnSerialize() corresponds to the
                // manifest
                //

                int overhead = 0;
                int datagramSize = datagram.Size;

                if (EmbedMetadata) {
                    overhead = Metafield.LayoutSize;
                    //
                    // calculate the overhead for metafields and manifest
                    //
                    datagramSize += (datagram.Count * overhead);
                }

                int position = 0;
                buffer = new byte[datagramSize];

                foreach (Field field in datagram) {
                    FieldSerializer.Serialize(field, buffer, position, EmbedMetadata);
                    position += (field.Size + overhead);
                }

                buffer = OnSerializeComplete(datagram, buffer);
            } catch (Exception e) {
                throw new SerializationException(
                    Resources.Error_SerializationError, e);
            }

            return buffer;
        }

        /// <summary>
        /// Sends the specified <see cref="System.Byte" /> array to the host
        /// at the specified remote endpoint.
        /// </summary>
        /// <param name="data">
        /// The <see cref="System.Byte" /> array to send.
        /// </param>
        /// <param name="remoteEndPoint">
        /// An <see cref="System.Net.IPEndPoint" /> that represents the remote
        /// host and port number to which to send the data.
        /// </param>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="Gekkota.Net.Sockets.IPClient" /> has been terminated.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="data" /> is <see langword="null" />.
        /// <para>-or-</para>
        /// <paramref name="remoteEndPoint" /> is <see langword="null" /> and a
        /// default remote host has not been established.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// <paramref name="remoteEndPoint" /> is not <see langword="null" /> and
        /// a default host has been already established.
        /// </exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// An error has occurred during serialization of
        /// <paramref name="data" />. Refer to the inner exception for more
        /// details.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// <see cref="Gekkota.Net.Sockets.IPClient.SendTimeout" /> has elapsed.
        /// <para>-or-</para>
        /// An error occurred when accessing the underlying
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </exception>
        internal void Send(byte[] data, IPEndPoint remoteEndPoint)
        {
            if (active) {
                if (remoteEndPoint != null) {
                    throw new InvalidOperationException(
                        Resources.Error_RemoteHostAlreadyEstablished);
                } else {
                    client.Send(data, 0, data.Length, SocketFlags.None);
                }
            } else {
                client.SendTo(data, 0, data.Length, SocketFlags.None, remoteEndPoint);
            }
        }
        #endregion internal methods

        #region private methods
        /// <summary>
        /// Creates a manifest for the specified
        /// <see cref="Gekkota.Net.Datagram" />.
        /// </summary>
        /// <param name="datagram">
        /// The <see cref="Gekkota.Net.Datagram" /> for which the manifest
        /// should be created.
        /// </param>
        /// <returns>
        /// A <see cref="Gekkota.Net.Field" /> that represents the manifest.
        /// </returns>
        private unsafe Field CreateManifest(Datagram datagram)
        {
            //
            // the manifest consists of an index organized as an array of
            // id/offset pairs, and its id corresponds to the index CRC
            //
            uint[] items = new uint[datagram.Count];
            int layoutSize = EmbedMetadata ? Metafield.LayoutSize : 0;
            int byteLength = Buffer.ByteLength(items);
            int offset = layoutSize + byteLength;
            int i = 0;

            FieldEnumerator enumerator = datagram.GetEnumerator();

            while (enumerator.MoveNext()) {
                items[i] = (uint) ((Field) enumerator.Current).Id;
                items[i] <<= 16;
                items[i++] |= (uint) (offset & 0x00FF);
                offset += (layoutSize + ((Field) enumerator.Current).Size);
            }

            if (!datagram.IsSorted) {
                Array.Sort(items);
            }

            byte[] value = new byte[byteLength];
            Buffer.BlockCopy(items, 0, value, 0, value.Length);

            fixed (byte* pValue = value) {
                byte* pIndex = pValue;

                while (pIndex < (pValue + byteLength)) {
                    *((short*) pIndex) = IPAddress.HostToNetworkOrder(*((short*) pIndex));
                    pIndex += sizeof(short);
                }
            }

            return new Field(
                unchecked((int) ChecksumGenerator.Generate(value, 0, value.Length)),
                value,
                FieldCategory.Manifest);
        }

        /// <summary>
        /// Creates the payload descriptor.
        /// </summary>
        /// <remarks>
        /// The <c>CreatePayloadDescriptor</c> method creates a payload
        /// descriptor for the current protocol by reflecting its metafields.
        /// </remarks>
        private void CreatePayloadDescriptor()
        {
            Type type = GetType();

            MemberInfo[] members = type.GetMembers(
                BindingFlags.Static | BindingFlags.Instance |
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.DeclaredOnly);

            Metafield metafield = null;
            ProtocolFieldAttribute[] attributes = null;
            payloadDescriptor = new Collections.LinkedList<Metafield>();
            payloadDescriptor.Comparer = new PositionComparer();

            foreach (MemberInfo member in members) {
                attributes =
                    (ProtocolFieldAttribute[]) member.GetCustomAttributes(
                        typeof(ProtocolFieldAttribute), true);

                if (attributes.Length > 0) {
                    metafield = type.InvokeMember(
                        member.Name,
                        BindingFlags.GetField | BindingFlags.GetProperty,
                        null,
                        this,
                        null) as Metafield;

                    if (metafield != null) {
                        metafield.ProtocolFieldAttribute = attributes[0];
                        payloadDescriptor.Add(metafield);
                    }
                }
            }

            payloadDescriptor.Sort();
        }

        /// <summary>
        /// Extracts and returns the manifest embeded in the specified
        /// <see cref="System.Byte" /> array.
        /// </summary>
        /// <param name="data">
        /// A <see cref="System.Byte" /> array that contains the manifest to
        /// extract.
        /// </param>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the starting position
        /// within <paramref name="data" />.
        /// </param>
        /// <returns>
        /// A <see cref="Gekkota.Net.Field" /> that represents the manifest
        /// extracted from <paramref name="data" />.
        /// </returns>
        private unsafe Field GetManifest(byte[] data, int index)
        {
            Field manifest = null;

            try {
                Field field = FieldSerializer.Deserialize(data, index);
                if (field.Type == FieldType.ByteArray &&
                    field.Category == FieldCategory.Manifest) {
                    long checksum = ChecksumGenerator.Generate(
                        field.ValueAsByteArray, 0, field.Size);

                    unchecked {
                        if (field.Id == (int) checksum) {
                            //
                            // manifest found; [data] contains metadata
                            //
                            fixed (byte* pValue = field.ValueAsByteArray) {
                                byte* pIndex = pValue;
                                int count = field.ValueAsByteArray.Length / sizeof(ushort);

                                for (int i = 0 ; i < count; i++) {
                                    *((ushort*) pIndex) =
                                    (ushort) IPAddress.NetworkToHostOrder(*((short*) pIndex));
                                    pIndex += sizeof(ushort);
                                }
                            }
                        }

                        manifest = field;
                    }
                }
            } catch (Exception) {}

            return manifest;
        }

        /// <summary>
        /// Initializes the underlying network
        /// <see cref="System.Net.Sockets.Socket" />.
        /// </summary>
        /// <param name="localEndPoint">
        /// An <see cref="System.Net.IPEndPoint" /> that represents the local
        /// endpoint through which data is sent or received.
        /// </param>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred during <see cref="System.Net.Sockets.Socket" />
        /// initialization.
        /// </exception>
        private void Initialize(IPEndPoint localEndPoint)
        {
            if (client != null) {
                client.Close();
                client = null;
            }

            try {
                client = CreateClient(localEndPoint.AddressFamily);
                client.Bind(localEndPoint);
                ReceiveBufferSize = DefaultReceiveBufferSize;
                SendBufferSize = DefaultSendBufferSize;
                TimeToLive = DefaultTimeToLive;
            } catch (Exception) {
                if (client != null) {
                    client.Close();
                    client = null;
                } throw;
            }

            if (client.AddressFamily == AddressFamily.InterNetwork) {
                maxDatagramSize = IPMaxDatagramSize;
                baseOverhead = IPHeaderSize;
            } else {
                maxDatagramSize = IPv6MaxDatagramSize;
                baseOverhead = IPv6HeaderSize;
            }

            //
            // if this is not the first try to initialize this IPClient instance,
            // skip the next statements
            //
            if (pollTimeout > -1) {
                return;
            }

            pollTimeout = DefaultPollTimeout;
        }

        /// <summary>
        /// Starts the Listener if not already started and if at least one
        /// delegate is registered for the Receive event.
        /// </summary>
        private void JoinListener()
        {
            if (listener == null && receive != null) {
                listener = new Thread(new ThreadStart(Listen));
                listener.IsBackground = true;
                listener.Start();
            }
        }

        /// <summary>
        /// Stops the Listener if not already stopped and if no delegates are
        /// registered for the Receive event.
        /// </summary>
        private void LeaveListener()
        {
            if (listener != null && receive == null) {
                listener.Join();
                listener = null;
            }
        }

        /// <summary>
        /// Listens for incoming datagrams.
        /// </summary>
        private void Listen()
        {
            byte[] data = null;
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            while (receive != null) {
                lock (client) {
                    if (client.Poll(pollTimeout, SelectMode.SelectRead)) {
                        data = new byte[client.Available];
                        client.ReceiveFrom(
                            data,
                            0,
                            data.Length,
                            SocketFlags.None,
                            ref remoteEndPoint);
                    }
                }

                OnListen(data, data != null ? remoteEndPoint : null);
                data = null;

                //
                // let other waiting threads execute...
                //
                Thread.Sleep(0);
            }

            OnJoinListener();
        }

        /// <summary>
        /// Validates the specified <see cref="Gekkota.Net.Datagram" />.
        /// </summary>
        /// <param name="datagram">
        /// The <see cref="Gekkota.Net.Datagram" /> to validate.
        /// </param>
        /// <exception cref="System.InvalidOperationException">
        /// <paramref name="datagram" /> is empty.
        /// <para>-or-</para>
        /// <paramref name="datagram" /> is not valid.
        /// </exception>
        private void Validate(Datagram datagram)
        {
            if (datagram == null) {
                throw new ArgumentNullException("datagram");
            }

            if (datagram.Count == 0) {
                throw new InvalidOperationException(
                    Resources.Error_DatagramEmpty);
            }

            OnValidate(datagram);
        }
        #endregion private methods

        #region inner classes
        /// <summary>
        /// Compares two metafields for position equivalence.
        /// </summary>
        private sealed class PositionComparer : IComparer<Metafield>
        {
            int IComparer<Metafield>.Compare(Metafield x, Metafield y)
            {
                return
                    x.ProtocolFieldAttribute.Position -
                    y.ProtocolFieldAttribute.Position;
            }
        }

        /// <summary>
        /// Compares two fields for id equivalence.
        /// </summary>
        private sealed class IdComparer : IComparer<Field>
        {
            int IComparer<Field>.Compare(Field x, Field y)
            {
                return x.Id - y.Id;
            }
        }
        #endregion inner classes
    }
}
