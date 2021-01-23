//------------------------------------------------------------------------------
// <sourcefile name="UdpClientTest.cs" language="C#" begin="05/30/2004">
//
//     <author name="Giuseppe Greco" email="giuseppe.greco@agamura.com" />
//
//     <copyright company="Agamura" url="http://www.agamura.com">
//         Copyright (C) 2004 Agamura, Inc.  All rights reserved.
//     </copyright>
//
// </sourcefile>
//------------------------------------------------------------------------------

#if DEBUG
using System;
using System.Net;
using System.Text;
using System.Threading;
using Gekkota.Net;
using Gekkota.Net.Sockets;
using NUnit.Framework;

namespace Gekkota.Tests
{
    [TestFixture]
    public class UdpClientTest
    {
        #region private fields
        private Datagram sentDatagram;
        private AutoResetEvent receiveEvent;
        private AutoResetEvent errorEvent;
        #endregion private fields

        #region service methods
        [SetUp]
        public void Init()
        {
            receiveEvent = new AutoResetEvent(false);
            errorEvent = new AutoResetEvent(false);
        }

        [TearDown]
        public void Clean() {}
        #endregion service methods

        #region unit test methods
        [Test]
        public void NoChecksum()
        {
            #if win32
            UdpClient client = null;

            try {
                client = new UdpClient();
                client.NoChecksum = true;
                Assert.AreEqual(true, client.NoChecksum);
            } finally {
                if (client != null) client.Close();
            }
            #endif
        }

        [Test]
        public void CreateInstance()
        {
            #if win32
            UdpClient client = null;

            try {
                client = new UdpClient();
                Assert.AreEqual(false, client.NoChecksum);
            } finally {
                if (client != null) client.Close();
            }
            #endif
        }

        [Test]
        public void CreateInstance_LocalEndPoint()
        {
            #if win32
            UdpClient client = null;

            try {
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Loopback, 9000);
                client = new UdpClient(localEndPoint);
                Assert.AreEqual(false, client.NoChecksum);
            } finally {
                if (client != null) client.Close();
            }
            #endif
        }

        [Test]
        public void CreateInstance_Port()
        {
            #if win32
            UdpClient client = null;

            try {
                client = new UdpClient(9000);
                Assert.AreEqual(false, client.NoChecksum);
            } finally {
                if (client != null) client.Close();
            }
            #endif
        }

        [Test]
        public void CreateInstance_Hostname_Port()
        {
            #if win32
            UdpClient client = null;

            try {
                string hostname = "localhost";
                int port = 9000;
                IPEndPoint remoteEndPoint = new IPEndPoint(
                    Dns.Resolve(hostname).AddressList[0], port);
                client = new UdpClient(hostname, port);
                Assert.AreEqual(false, client.NoChecksum);
            } finally {
                if (client != null) client.Close();
            }
            #endif
        }

        [Test]
        public new void GetHashCode()
        {
            UdpClient client = new UdpClient();
            ushort hashCode = (ushort) HashCodeGenerator.Generate("UDP");
            Assert.AreEqual(hashCode, client.GetHashCode());
        }

        [Test]
        public void SendReceive()
        {
            UdpClient sender = null;
            UdpClient receiver = null;

            try {
                sender = new UdpClient("localhost", 9000);
                receiver = new UdpClient(9000);
                receiver.Receive += new ReceiveEventHandler(OnReceive);

                sentDatagram = new Datagram();
                sentDatagram.Add(new Field(
                    IPClient.DefaultMetafield.Id, Encoding.UTF8.GetBytes("Gorilla")));

                sender.Send(sentDatagram);
                receiveEvent.WaitOne();
            } finally {
                if (sender != null) sender.Close();
                if (receiver != null) receiver.Close();
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateInstance_NullHostname()
        {
            UdpClient client = new UdpClient(null, 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CreateInstance_PortOutOfRange()
        {
            UdpClient client = new UdpClient(IPAddress.Loopback.ToString(), -1);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void NoChecksum_ClientDisposed()
        {
            UdpClient client = null;

            try {
                client = new UdpClient();
            } finally {
                if (client != null) client.Close();
            }

            bool noChecksum = false;

            try {
                noChecksum = client.NoChecksum;
            } catch (ObjectDisposedException) {
                client.NoChecksum = noChecksum;
            }
        }
        #endregion unit test methods

        #region private methods
        private void OnReceive(Object sender, ReceiveEventArgs args)
        {
            Assert.AreEqual(sentDatagram, args.Datagram);
            receiveEvent.Set();
            Thread.Sleep(0);
        }
        #endregion private methods
    }
}
#endif
