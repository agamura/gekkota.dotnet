//------------------------------------------------------------------------------
// <sourcefile name="XudpClient.cs" language="C#" begin="06/03/2004">
//
//     <author name="Giuseppe Greco" email="giuseppe.greco@agamura.com" />
//
//     <copyright company="Agamura" url="http://www.agamura.com">
//         Copyright (C) 2004 Agamura, Inc.  All rights reserved.
//     </copyright>
//
// </sourcefile>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using Gekkota.Checksums;
using Gekkota.Collections;
using Gekkota.Compression;
using Gekkota.Properties;
using Gekkota.Security.Cryptography;
using Gekkota.Utilities;

namespace Gekkota.Net.Sockets
{
    public class XudpClient_Bak : UdpClient, IDisposable
    {
        #region private fields
        //
        // version 1.0:
        //     MSB 0001 (0x10) major
        //     LSB 0000 (0x00) minor
        //
        private const byte Version = 0x10;

        private Thread sender;
        private Gekkota.Collections.Queue<NetPackage> sendQueue;
        private ICompressionAlgorithm compressionAlgorithm;
        private CryptoServiceProvider cryptoServiceProvider;
        private DatagramPriority datagramPriority;
        private BitVector32 flags;
        private bool send;
        #endregion private fields

        #region public properties
        public bool Compressed
        {
            get {
                CheckIfDisposed();
                return flags[(int) XudpFlag.Compressed];
            }

            set {
                CheckIfDisposed();
                flags[(int) XudpFlag.Compressed] = value;
            }
        }

        public DatagramPriority DatagramPriority
        {
            get {
                CheckIfDisposed();
                return datagramPriority;
            }

            set {
                CheckIfDisposed();
                datagramPriority = value;
            }
        }

        public bool Encrypted
        {
            get {
                CheckIfDisposed();
                return flags[(int) XudpFlag.Encrypted];
            }

            set {
                CheckIfDisposed();
                flags[(int) XudpFlag.Encrypted] = value;
            }
        }

        public bool FullChecksum
        {
            get {
                CheckIfDisposed();
                return flags[(int) XudpFlag.Checksum];
            }

            set {
                CheckIfDisposed();
                flags[(int) XudpFlag.Checksum] = value;
            }
        }

        public new bool NoChecksum
        {
            get {
                CheckIfDisposed();
                return false;
            }

            set {
                throw new NotSupportedException(
                    Resources.Error_PropertyNotSettable);
            }
        }

        public override string ProtocolName
        {
            get { return "XUDP"; }
        }

        public bool Reliable
        {
            get {
                CheckIfDisposed();
                return flags[(int) XudpFlag.Reliable];
            }

            set {
                CheckIfDisposed();
                flags[(int) XudpFlag.Reliable] = value;
            }
        }

        public bool Sequenced
        {
            get {
                CheckIfDisposed();
                return flags[(int) XudpFlag.Sequenced];
            }

            set {
                CheckIfDisposed();
                flags[(int) XudpFlag.Sequenced] = value;
            }
        }
        #endregion public properties

        #region protected properties
        protected CryptoServiceProvider CryptoServiceProvider
        {
            get {
                CheckIfDisposed();
                if (cryptoServiceProvider == null) {
                    cryptoServiceProvider = new CryptoServiceProvider();
                }

                return cryptoServiceProvider;
            }
        }

        protected ICompressionAlgorithm CompressionAlgorithm
        {
            get {
                CheckIfDisposed();
                if (compressionAlgorithm == null)
                {
                    compressionAlgorithm = new LZF();
                }

                return compressionAlgorithm;
            }

            set {
                CheckIfDisposed();
                compressionAlgorithm = value;
            }
        }
        #endregion protected properties

        #region public constructors
        public XudpClient_Bak()
        {
            Initialize();
        }

        public XudpClient_Bak(AddressFamily addressFamily)
            : base(addressFamily)
        {
            Initialize();
        }

        public XudpClient_Bak(IPEndPoint localEndPoint)
            : base(localEndPoint)
        {
            Initialize();
        }

        public XudpClient_Bak(int port)
            : base(port)
        {
            Initialize();
        }

        public XudpClient_Bak(int port, AddressFamily addressFamily)
            : base(port, addressFamily)
        {
            Initialize();
        }

        public XudpClient_Bak(string hostname, int port)
        {
            Initialize();
            base.Connect(hostname, port);
        }
        #endregion public constructors

        #region protected constructors
        protected XudpClient_Bak(string protocolId)
        {
            Initialize();
        }

        protected XudpClient_Bak(string protocolId, AddressFamily addressFamily)
            : base(addressFamily)
        {
            Initialize();
        }

        protected XudpClient_Bak(string protocolId, IPEndPoint localEndPoint)
            : base(localEndPoint)
        {
            Initialize();
        }

        protected XudpClient_Bak(int port, string protocolId)
            : base(port)
        {
            Initialize();
        }

        protected XudpClient_Bak(string protocolId, int port, AddressFamily addressFamily)
            : base(port, addressFamily)
        {
            Initialize();
        }

        protected XudpClient_Bak(string protocolId, string hostname, int port)
        {
            Initialize();
            base.Connect(hostname, port);
        }
        #endregion protected constructors

        #region public methods
        public override void Send(Datagram datagram, IPEndPoint remoteEndPoint)
        {
            //
            // this implementation of Send() is asynchronous in order to let
            // the sender thread handle priorities
            //

            NetPackage netPackage = new NetPackage(
                datagram,
                remoteEndPoint,
                DatagramPriority);

            sendQueue.Enqueue(netPackage);
        }
        #endregion pubilc methods

        #region protected methods
        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed) {
                base.Dispose(disposing);

                if (disposing) {
                    //
                    // release managed resources
                    //
                }

                //
                // release unmanaged resources
                //
                StopSender();
            }
        }

        protected unsafe override int OnDeserialize(
            byte[] data, int index, out Field manifest)
        {
            XudpHeader header = GetHeader(data, index);
            if (header.ProtocolId != GetHashCode()) {
                //
                // not a XUDP datagram... discard
                //
                goto Discard;
            }

            ushort checksum = header.Checksum;
            BitVector32 flags = new BitVector32(header.Flags);

            if (flags[(int) XudpFlag.Checksum]) {
                header.Checksum = 0;
            } else {
                index += sizeof(XudpHeader);
            }

            if (checksum != (ushort) ChecksumGenerator.Generate(
                data, index, data.Length - index)) {
                //
                // datagram corrupted... discard
                // 
                goto Discard;
            }

            //
            // by-pass the XUDP header...
            //
            index += sizeof(XudpHeader);

            if (flags[(int) XudpFlag.Encrypted]) {
                data = CryptoServiceProvider.Decrypt(
                    data, index,
                    data.Length - index);
            }

            if (flags[(int) XudpFlag.Compressed]) {
                data = CompressionAlgorithm.Inflate(
                   data, index,
                   data.Length - index);
            }

            return base.OnDeserialize(data, index, out manifest);

        Discard:
            manifest = null;
            return -1;    
        }

        protected override void OnListen(byte[] data, EndPoint remoteEndPoint)
        {
            base.OnListen(data, remoteEndPoint);
        }

        protected override byte[] OnSerializeComplete(
            Datagram datagram, byte[] data)
        {
            if (flags[(int) XudpFlag.Compressed]) {
                data = CompressionAlgorithm.Deflate(data, 0, data.Length);
            }

            if (flags[(int) XudpFlag.Encrypted]) {
                data = CryptoServiceProvider.Encrypt(data, 0, data.Length);
            }

            return data;
        }
        #endregion protected methods

        #region private methods
        private unsafe byte[] CreateDataMessage(Datagram datagram)
        {
            byte[] data = Serialize(datagram);
            byte[] message = new byte[sizeof(XudpHeader) + data.Length];

            fixed (byte* pMessage = message) {
                //
                // fill protocol header
                //
                XudpHeader* pHeader = (XudpHeader*) pMessage;
                pHeader->ProtocolId = (ushort) GetHashCode();
                pHeader->Version = Version;
                pHeader->MessageType = (byte) XudpMessageType.Data;
                pHeader->Priority = (byte) DatagramPriority;
                pHeader->Flags = unchecked((ushort) flags.Data);
                pHeader->SequenceNumber = 0;
                pHeader->Checksum = 0;

                //
                // copy data buffer
                //
                System.Buffer.BlockCopy(
                    data, 0,                        // source buffer            
                    message, sizeof(XudpHeader),    // destination buffer
                    data.Length);                   // number of bytes to copy

                //
                // calculate CRC
                //
                pHeader->Checksum = FullChecksum
                    ? (ushort) ChecksumGenerator.Generate(message, 0, message.Length)
                    : (ushort) ChecksumGenerator.Generate(message, 0, message.Length - data.Length);
            }

            return message;
        }

        private byte[] CreateNackMessage()
        {
            return null;
        }

        private byte[] CreateNackRequestMessage()
        {
            return null;
        }

        private unsafe byte[] CreateSyncMessage()
        {
            byte[] message = new byte[sizeof(XudpSyncMessage)];

            fixed (byte* pMessage = message) {
                //
                // fill protocol header
                //
                XudpSyncMessage* pDataMessage = (XudpSyncMessage*) pMessage;
                pDataMessage->Header.ProtocolId = (ushort) GetHashCode();
                pDataMessage->Header.Version = Version;
                pDataMessage->Header.MessageType = (byte) XudpMessageType.Sync;
                pDataMessage->Header.Priority = (byte) DatagramPriority.High;
                pDataMessage->Header.Flags = unchecked((ushort) flags.Data);
                pDataMessage->Header.SequenceNumber = 0;
                pDataMessage->Header.Checksum = 0;
                pDataMessage->SyncNumber = 0;
            }
            
            return message;
        }

        private unsafe XudpHeader GetHeader(byte[] data, int index)
        {
            XudpHeader header = new XudpHeader();

            if ((data.Length - index) >= sizeof(XudpHeader)) {
                fixed (byte* pData = data) {
                    XudpHeader* pHeader = (XudpHeader*) (pData + index);
                    header.ProtocolId = pHeader->ProtocolId;
                    header.Version = pHeader->Version;
                    header.MessageType = pHeader->MessageType;
                    header.Priority = pHeader->Priority;
                    header.Wild = pHeader->Wild;
                    header.Flags = pHeader->Flags;
                    header.SequenceNumber = pHeader->SequenceNumber;
                    header.Checksum = pHeader->Checksum;
                }
            }

            return header;
        }
        private void Initialize()
        {
            base.NoChecksum = true;
            datagramPriority = DatagramPriority.Normal;
            flags = new BitVector32((int) (XudpFlag.Reliable | XudpFlag.Sequenced));

            StartSender();
            Receive += new ReceiveEventHandler(OnReceive);
        }

        private void OnReceive(Object sender, ReceiveEventArgs args)
        {
        }

        private void Send()
        {
            byte[] data = null;
            NetPackage netPackage = null;

            while (send) {
                if (sendQueue.Count > 0) {
                    netPackage = (NetPackage) sendQueue.Dequeue();
                    data = CreateDataMessage(netPackage.Datagram);

                    Send(data, netPackage.EndPoint);
                }

                //
                // let other waiting threads execute...
                //
                Thread.Sleep(0);
            }
        }

        private void StartSender()
        {
            if (sender == null) {
                sendQueue = new Gekkota.Collections.Queue<NetPackage>(true);
                sendQueue.Comparer = new SendComparer();
                sendQueue = Gekkota.Collections.Queue<NetPackage>.Synchronized(sendQueue);
                send = true;
                sender = new Thread(new ThreadStart(Send));
                sender.IsBackground = true;
                sender.Start();
            }
        }

        private void StopSender()
        {
            if (sender != null) {
                send = false;
                sender.Join();
                sender = null;
                sendQueue = null;
            }
        }

        private byte[] Unzip(byte[] data, int index, int count)
        {
            return data;
        }

        private byte[] Zip(byte[] data, int index, int count)
        {
            return data;
        }
        #endregion private methods

        #region inner classes
        private sealed class SendComparer : IComparer<NetPackage>
        {
            int IComparer<NetPackage>.Compare(NetPackage x, NetPackage y)
            {
                return x.Priority - y.Priority;
            }
        }
        #endregion inner classes
    }
}
