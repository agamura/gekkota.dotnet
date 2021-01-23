//------------------------------------------------------------------------------
// <sourcefile name="IPClientTest.cs" language="C#" begin="05/10/2004">
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
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization;
using NUnit.Framework;
using Gekkota.Net;
using Gekkota.Net.Sockets;

namespace Gekkota.Tests
{
    [TestFixture]
    public class IPClientTest
    {
        #region private fields
        private static readonly IPAddress MulticastAddress;
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

        #region public constructors
        static IPClientTest()
        {
            MulticastAddress = IPAddress.Parse("224.0.1.10");
        }
        #endregion public constructors

        #region unit test methods
        [Test]
        public void Active()
        {
            TestClient client = null;

            try {
                client = new TestClient(IPAddress.Loopback.ToString(), 0);
                Assert.AreEqual(true, client.Active);
                client.Active = false;
                Assert.AreEqual(false, client.Active);
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        public void IncomingBandwidth()
        {
            IPClient client = null;

            try {
                client = new IPClient();
                client.IncomingBandwidth = Int32.MaxValue;
                Assert.AreEqual(Int32.MaxValue, client.IncomingBandwidth);
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        public void OutgoingBandwidth()
        {
            IPClient client = null;

            try {
                client = new IPClient();
                client.OutgoingBandwidth = Int32.MaxValue;
                Assert.AreEqual(Int32.MaxValue, client.OutgoingBandwidth);
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        public void BroadcastEnabled()
        {
            #if win32
            IPClient client = null;

            try {
                client = new IPClient();
                client.BroadcastEnabled = true;
                Assert.AreEqual(true, client.BroadcastEnabled);
            } finally {
                if (client != null) client.Close();
            }
            #endif
        }

        [Test]
        public void EmbedMetadata()
        {
            IPClient client = null;

            try {
                client = new IPClient();
                client.EmbedMetadata = true;
                Assert.AreEqual(true, client.EmbedMetadata);
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        public void IsDisposed()
        {
            IPClient client = null;

            try {
                client = new IPClient();
            } finally {
                if (client != null) {
                    client.Close();
                    Assert.AreEqual(true, client.IsDisposed);
                }
            }
        }

        [Test]
        public void LocalEndPoint()
        {
            IPClient client = null;

            try {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 0);
                client = new IPClient(new IPEndPoint(IPAddress.Loopback, 0));
                Assert.AreEqual(endPoint.Address, client.LocalEndPoint.Address);
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        public void PollTimeout()
        {
            IPClient client = null;

            try {
                client = new IPClient();
                client.PollTimeout = 1000000;
                Assert.AreEqual(1000000, client.PollTimeout);
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        public void ReceiveBufferSize()
        {
            #if win32
            IPClient client = null;

            try {
                client = new IPClient();
                client.ReceiveBufferSize = 1024;
                Assert.AreEqual(1024, client.ReceiveBufferSize);
            } finally {
                if (client != null) client.Close();
            }
            #endif
        }

        [Test]
        public void ReceiveTimeout()
        {
            #if win32
            IPClient client = null;

            try {
                client = new IPClient();
                client.ReceiveTimeout = 1000;
                Assert.AreEqual(1000, client.ReceiveTimeout);
            } finally {
                if (client != null) client.Close();
            }
            #endif
        }

        [Test]
        public void RemoteEndPoint()
        {
            IPClient client = null;

            try {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, Int16.MaxValue);
                client = new IPClient(IPAddress.Loopback.ToString(), Int16.MaxValue);
                Assert.AreEqual(endPoint, client.RemoteEndPoint);
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        public void SendBufferSize()
        {
            #if win32
            IPClient client = null;

            try {
                client = new IPClient();
                client.SendBufferSize = 1024;
                Assert.AreEqual(1024, client.SendBufferSize);
            } finally {
                if (client != null) client.Close();
            }
            #endif
        }

        [Test]
        public void SendTimeout()
        {
            #if win32
            IPClient client = null;

            try {
                client = new IPClient();
                client.SendTimeout = 1000;
                Assert.AreEqual(1000, client.SendTimeout);
            } finally {
                if (client != null) client.Close();
            }
            #endif
        }

        [Test]
        public void TimeToLive()
        {
            #if win32
            IPClient client = null;

            try {
                client = new IPClient();
                client.TimeToLive = 10;
                Assert.AreEqual(10, client.TimeToLive);
            } finally {
                if (client != null) client.Close();
            }
            #endif
        }

        [Test]
        public void TypeOfService()
        {
            #if win32
            IPClient client = null;

            try {
                client = new IPClient();
                client.TypeOfService = 1;
                Assert.AreEqual(1, client.TypeOfService);
            } finally {
                if (client != null) client.Close();
            }
            #endif
        }

        [Test]
        public void UseLoopback()
        {
            #if win32
            IPClient client = null;

            try {
                client = new IPClient();
                client.UseLoopback = false;
                Assert.AreEqual(false, client.UseLoopback);
            } finally {
                if (client != null) client.Close();
            }
            #endif
        }

        [Test]
        public void CreateInstance()
        {
            #if win32
            TestClient client = null;

            try {
                client = new TestClient();

                Assert.IsTrue(client.Client != null);
                Assert.AreEqual(false, client.Active);
                Assert.AreEqual(Gekkota.Net.Sockets.Bandwidth.Full, client.IncomingBandwidth);
                Assert.AreEqual(Gekkota.Net.Sockets.Bandwidth.Full, client.OutgoingBandwidth);
                Assert.AreEqual(false, client.BroadcastEnabled);
                Assert.AreEqual(false, client.IsDisposed);
                Assert.AreEqual(new IPEndPoint(IPAddress.Any, 0), client.LocalEndPoint);
                Assert.AreEqual(0, client.Options.Length);
                Assert.AreEqual(250000, client.PollTimeout);
                Assert.AreEqual(32768, client.ReceiveBufferSize);
                Assert.AreEqual(0, client.ReceiveTimeout);
                Assert.AreEqual(8192, client.SendBufferSize);
                Assert.AreEqual(0, client.SendTimeout);
                Assert.AreEqual(60, client.TimeToLive);
                Assert.AreEqual(0, client.TypeOfService);
                Assert.AreEqual(true, client.UseLoopback);
            } finally {
                if (client != null) client.Close();
            }
            #endif
        }

        [Test]
        public void CreateInstance_LocalEndPoint()
        {
            #if win32
            TestClient client = null;

            try {
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Loopback, 0);
                client = new TestClient(localEndPoint);

                Assert.IsTrue(client.Client != null);
                Assert.AreEqual(false, client.Active);
                Assert.AreEqual(Gekkota.Net.Sockets.Bandwidth.Full, client.IncomingBandwidth);
                Assert.AreEqual(Gekkota.Net.Sockets.Bandwidth.Full, client.OutgoingBandwidth);
                Assert.AreEqual(false, client.BroadcastEnabled);
                Assert.AreEqual(false, client.IsDisposed);
                Assert.AreEqual(localEndPoint, client.LocalEndPoint);
                Assert.AreEqual(0, client.Options.Length);
                Assert.AreEqual(250000, client.PollTimeout);
                Assert.AreEqual(32768, client.ReceiveBufferSize);
                Assert.AreEqual(0, client.ReceiveTimeout);
                Assert.AreEqual(8192, client.SendBufferSize);
                Assert.AreEqual(0, client.SendTimeout);
                Assert.AreEqual(60, client.TimeToLive);
                Assert.AreEqual(0, client.TypeOfService);
                Assert.AreEqual(true, client.UseLoopback);
            } finally {
                if (client != null) client.Close();
            }
            #endif
        }

        [Test]
        public void CreateInstance_Hostname_Port()
        {
            #if win32
            TestClient client = null;

            try {
                string hostname = "localhost";
                int port = 0;
                IPEndPoint remoteEndPoint = new IPEndPoint(
                    Dns.Resolve(hostname).AddressList[0], port);
                client = new TestClient(hostname, port);

                Assert.IsTrue(client.Client != null);
                Assert.AreEqual(true, client.Active);
                Assert.AreEqual(Gekkota.Net.Sockets.Bandwidth.Full, client.IncomingBandwidth);
                Assert.AreEqual(Gekkota.Net.Sockets.Bandwidth.Full, client.OutgoingBandwidth);
                Assert.AreEqual(false, client.BroadcastEnabled);
                Assert.AreEqual(false, client.IsDisposed);
                Assert.AreEqual(IPAddress.Any, client.LocalEndPoint.Address);
                Assert.AreEqual(0, client.Options.Length);
                Assert.AreEqual(250000, client.PollTimeout);
                Assert.AreEqual(32768, client.ReceiveBufferSize);
                Assert.AreEqual(0, client.ReceiveTimeout);
                Assert.AreEqual(remoteEndPoint, client.RemoteEndPoint);
                Assert.AreEqual(8192, client.SendBufferSize);
                Assert.AreEqual(0, client.SendTimeout);
                Assert.AreEqual(60, client.TimeToLive);
                Assert.AreEqual(0, client.TypeOfService);
                Assert.AreEqual(true, client.UseLoopback);
            } finally {
                if (client != null) client.Close();
            }
            #endif
        }

        [Test]
        public void Close()
        {
            IPClient client = null;

            try {
                client = new IPClient();
            } finally {
                if (client != null) client.Close();
            }

            Assert.AreEqual(true, client.IsDisposed);
        }

        [Test]
        public void Connect_EndPoint()
        {
            TestClient client = null;

            try {
                client = new TestClient();
                Assert.AreEqual(false, client.Active);
                client.Connect(new IPEndPoint(IPAddress.Loopback, 0));
                Assert.AreEqual(true, client.Active);
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        public void Connect_Hostname_Port()
        {
            TestClient client = null;

            try {
                client = new TestClient();
                Assert.AreEqual(false, client.Active);
                client.Connect(IPAddress.Loopback.ToString(), 0);
                Assert.AreEqual(true, client.Active);
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        public void Connect_IPAddress_Port()
        {
            TestClient client = null;

            try {
                client = new TestClient();
                Assert.AreEqual(false, client.Active);
                client.Connect(IPAddress.Loopback, 0);
                Assert.AreEqual(true, client.Active);
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        public void LimitIncomingBandwidth()
        {
            //
            // TODO
            //
        }

        [Test]
        public void LimitOutgoingBandwidth()
        {
            IPClient client = null;

            try {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 0);
                client = new IPClient(endPoint);

                //
                // set transmission speed to 800 bit/s
                //
                client.OutgoingBandwidth = 800;

                long begin = 0;
                long delta = 0;

                for (int i = 0; i < 2; i++) {
                    //
                    // create a datagram of 200 bytes
                    // Ethernet header.............14
                    // IP header...................20
                    // Payload....................166
                    //
                    byte[] data = new byte[166];
                    for (int j = 0; j < data.Length; j++) {
                        data[j] = (byte) j;
                    }

                    Datagram datagram = new Datagram();
                    datagram.Add(new Field(0, data));
                    begin = DateTime.Now.Ticks;
                    client.Send(datagram, endPoint);
                    delta = DateTime.Now.Ticks - begin;
                }

                //
                // to transmitt 200 bytes at 800 bit/s, IPClient.Send() should at
                // least wait 10e6 ticks (1 second)
                //
                Assert.IsTrue(delta > 10e6);
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        public void JoinDropMulticastGroup()
        {
            IPClient client = null;

            try {
                client = new IPClient();
                client.JoinMulticastGroup(MulticastAddress);
                client.DropMulticastGroup(MulticastAddress);
                client.JoinMulticastGroup(MulticastAddress, IPClient.MaxTimeToLive);
                client.DropMulticastGroup(MulticastAddress);
            } finally {
                if (client != null) client.Close();
            }
        }
        [Test]

        public new void GetHashCode()
        {
            IPClient client = new IPClient();
            ushort hashCode = (ushort) HashCodeGenerator.Generate("IP");
            Assert.AreEqual(hashCode, client.GetHashCode());
        }

        [Test]
        public void SendReceive()
        {
            IPClient sender = null;
            IPClient receiver = null;

            try {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 0);
                sender = new IPClient(endPoint.Address.ToString(), endPoint.Port);
                receiver = new IPClient(endPoint);
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
        public void SendReceive_RemoteEndPoint()
        {
            IPClient client = null;

            try {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 0);
                client = new IPClient(endPoint);
                client.Receive += new ReceiveEventHandler(OnReceive);

                sentDatagram = new Datagram();
                sentDatagram.Add(new Field(
                    IPClient.DefaultMetafield.Id, Encoding.UTF8.GetBytes("Gorilla")));

                client.Send(sentDatagram, endPoint);
                receiveEvent.WaitOne();
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        public void SendReceive_IPAddress_Port()
        {
            IPClient client = null;

            try {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 0);
                client = new IPClient(endPoint);
                client.Receive += new ReceiveEventHandler(OnReceive);

                sentDatagram = new Datagram();
                sentDatagram.Add(new Field(
                    IPClient.DefaultMetafield.Id, Encoding.UTF8.GetBytes("Gorilla")));

                client.Send(sentDatagram, endPoint.Address.ToString(), 0);
                receiveEvent.WaitOne();
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        public void SendReceive_WithMetadata()
        {
            IPClient sender = null;
            IPClient receiver = null;

            try {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 0);
                sender = new TestClient(endPoint.Address.ToString(), endPoint.Port);
                sender.EmbedMetadata = true;
                receiver = new TestClient(endPoint);
                receiver.Receive += new ReceiveEventHandler(OnReceive);

                sentDatagram = new Datagram();
                sentDatagram.Add(new Field(TestClient.IdMetafield.Id, (int) 1));
                sentDatagram.Add(new Field(TestClient.NameMetafield.Id, "Gorilla"));

                sender.Send(sentDatagram);
                receiveEvent.WaitOne();
            } finally {
                if (sender != null) sender.Close();
                if (receiver != null) receiver.Close();
            }
        }

        [Test]
        public void ErrorOnReceive()
        {
            DummyClient client = null;

            try {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 0);
                client = new DummyClient(endPoint);
                client.Receive += new ReceiveEventHandler(OnReceive);
                client.Exception += new ExceptionEventHandler(OnError);

                sentDatagram = new Datagram();
                sentDatagram.Add(new Field(
                    IPClient.DefaultMetafield.Id, Encoding.UTF8.GetBytes("Gorilla")));

                client.Send(sentDatagram, endPoint.Address.ToString(), endPoint.Port);
                errorEvent.WaitOne();
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void BroadcastEnabled_ClientDisposed()
        {
            IPClient client = null;

            try {
                client = new IPClient();
            } finally {
                if (client != null) client.Close();
            }

            bool broadcastEnabled = true;

            try {
                broadcastEnabled = client.BroadcastEnabled;
            } catch (ObjectDisposedException) {
                client.BroadcastEnabled = broadcastEnabled;
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void LocalEndPoint_ClientDisposed()
        {
            IPClient client = null;

            try {
                client = new IPClient();
            } finally {
                if (client != null) client.Close();
            }

            IPEndPoint endPoint = client.LocalEndPoint;
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Options_ClientDisposed()
        {
            IPClient client = null;

            try {
                client = new IPClient();
            } finally {
                if (client != null) client.Close();
            }

            byte[] options = new byte[1];

            try {
                options = client.Options;
            } catch (ObjectDisposedException) {
                client.Options = options;
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void PollTimeout_NegativeValue()
        {
            IPClient client = null;

            try {
                client = new IPClient();
                client.PollTimeout = -1;
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void ReceiveBufferSize_ClientDisposed()
        {
            IPClient client = null;

            try {
                client = new IPClient();
            } finally {
                if (client != null) client.Close();
            }

            int bufferSize = 0;

            try {
                bufferSize = client.ReceiveBufferSize;
            } catch (ObjectDisposedException) {
                client.ReceiveBufferSize = bufferSize;
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void ReceiveTimeout_ClientDisposed()
        {
            IPClient client = null;

            try {
                client = new IPClient();
            } finally {
                if (client != null) client.Close();
            }

            int timeout = 0;

            try {
                timeout = client.ReceiveTimeout;
            } catch (ObjectDisposedException) {
                client.ReceiveTimeout = timeout;
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void RemoteEndPoint_ClientDisposed()
        {
            IPClient client = null;

            try {
                client = new IPClient();
            } finally {
                if (client != null) client.Close();
            }

            IPEndPoint endPoint = client.RemoteEndPoint;
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void SendBufferSize_ClientDisposed()
        {
            IPClient client = null;

            try {
                client = new IPClient();
            } finally {
                if (client != null) client.Close();
            }

            int bufferSize = 0;

            try {
                bufferSize = client.SendBufferSize;
            } catch (ObjectDisposedException) {
                client.SendBufferSize = bufferSize;
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void SendTimeout_ClientDisposed()
        {
            IPClient client = null;

            try {
                client = new IPClient();
            } finally {
                if (client != null) client.Close();
            }

            int timeout = 0;

            try {
                timeout = client.SendTimeout;
            } catch (ObjectDisposedException) {
                client.SendTimeout = timeout;
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TimeToLive_ClientDisposed()
        {
            IPClient client = null;

            try {
                client = new IPClient();
            } finally {
                if (client != null) client.Close();
            }

            int timeToLive = 0;

            try {
                timeToLive = client.TimeToLive;
            } catch (ObjectDisposedException) {
                client.TimeToLive = timeToLive;
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TimeToLive_ValueOutOfRange()
        {
            IPClient client = null;

            try {
                client = new IPClient();
                client.TimeToLive = Int32.MaxValue;
            } catch (ArgumentOutOfRangeException) {
                client.TimeToLive = Int32.MinValue;
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TypeOfService_ClientDisposed()
        {
            IPClient client = null;

            try {
                client = new IPClient();
            } finally {
                if (client != null) client.Close();
            }

            int typeOfService = 0;

            try {
                typeOfService = client.TypeOfService;
            } catch (ObjectDisposedException) {
                client.TypeOfService = typeOfService;
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateInstance_NullHostname()
        {
            IPClient client = new IPClient(null, 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CreateInstance_PortOutOfRange()
        {
            IPClient client = new IPClient(IPAddress.Loopback.ToString(), -1);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Connect_ClientDisposed()
        {
            IPClient client = null;

            try {
                client = new IPClient();
            } finally {
                if (client != null) client.Close();
            }

            client.Connect(new IPEndPoint(IPAddress.Loopback, 0));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Connect_NullEndPoint()
        {
            IPClient client = null;

            try {
                client = new IPClient();
                client.Connect(null);
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Connect_NullHostname()
        {
            IPClient client = null;

            try {
                client = new IPClient();
                client.Connect((string) null, 0);
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Connect_PortOutOfRange()
        {
            IPClient client = null;

            try {
                client = new IPClient();
                client.Connect(IPAddress.Loopback.ToString(), -1);
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Connect_NullIPAddress()
        {
            IPClient client = null;

            try {
                client = new IPClient();
                client.Connect((IPAddress) null, 0);
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void DropMulticastGroup_ClientDisposed()
        {
            IPClient client = null;

            try {
                client = new IPClient();
            } finally {
                if (client != null) client.Close();
            }

            client.DropMulticastGroup(MulticastAddress);
          }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DropMulticastGroup_NullAddress()
        {
            IPClient client = null;

            try {
                client = new IPClient();
                client.DropMulticastGroup(null);
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void JoinMulticastGroup_ClientDisposed()
        {
            IPClient client = null;

            try {
                client = new IPClient();
            } finally {
                if (client != null) client.Close();
            }

            client.JoinMulticastGroup(MulticastAddress);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void JoinMulticastGroup_NullAddress()
        {
            IPClient client = null;

            try {
                client = new IPClient();
                client.JoinMulticastGroup(null);
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void JoinMulticastGroup_TimeToLiveOutOfRange()
        {
            IPClient client = null;

            try {
                client = new IPClient();
                client.JoinMulticastGroup(MulticastAddress, -1);
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Send_ClientDisposed()
        {
            IPClient client = null;

            try {
                client = new IPClient(IPAddress.Loopback.ToString(), 0);
            } finally {
                if (client != null) client.Close();
            }

            client.Send(new Datagram());
        }

        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void Send_NullDatagram()
        {
            IPClient client = null;

            try {
                client = new IPClient(IPAddress.Loopback.ToString(), 0);
                client.Send(null);
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Send_RemoteHostNotEstablished()
        {
            IPClient client = null;

            try {
                client = new IPClient();
                client.Send(new Datagram());
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Send_RemoteEndPoint_ClientDisposed()
        {
            IPClient client = null;

            try {
                client = new IPClient();
            } finally {
                if (client != null) client.Close();
            }
           
            Datagram datagram = new Datagram();
            datagram.Add(new Field(
                IPClient.DefaultMetafield.Id, Encoding.UTF8.GetBytes("Gorilla")));

            client.Send(datagram, new IPEndPoint(IPAddress.Loopback, 0));
        }

        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void Send_RemoteEndPoint_NullDatagram()
        {
            IPClient client = null;

            try {
                client = new IPClient();
                client.Send((Datagram) null, new IPEndPoint(IPAddress.Loopback, 0));
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Send_RemoteEndPoint_NullEndPoint()
        {
            IPClient client = null;

            try {
                client = new IPClient();

                Datagram datagram = new Datagram();
                datagram.Add(new Field(
                    IPClient.DefaultMetafield.Id, Encoding.UTF8.GetBytes("Gorilla")));

                client.Send(datagram, null);
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Send_RemoteEndPoint_RemoteHostAlreadyEstablished()
        {
            IPClient client = null;

            try {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 0);
                client = new IPClient(endPoint.Address.ToString(), endPoint.Port);

                Datagram datagram = new Datagram();
                datagram.Add(new Field(
                    IPClient.DefaultMetafield.Id, Encoding.UTF8.GetBytes("Gorilla")));

                client.Send(datagram, endPoint);
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Send_Hostname_Port_ClientDisposed()
        {
            IPClient client = null;

            try {
                client = new IPClient();
            } finally {
                if (client != null) client.Close();
            }

            Datagram datagram = new Datagram();
            datagram.Add(new Field(
                IPClient.DefaultMetafield.Id, Encoding.UTF8.GetBytes("Gorilla")));

            client.Send(datagram, IPAddress.Loopback.ToString(), 0);
        }

        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void Send_Hostname_Port_NullDatagram()
        {
            IPClient client = null;

            try {
                client = new IPClient();
                client.Send(null, IPAddress.Loopback.ToString(), 0);
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Send_Hostname_Port_NullHostname()
        {
            IPClient client = null;

            try {
                client = new IPClient();

                Datagram datagram = new Datagram();
                datagram.Add(new Field(
                    IPClient.DefaultMetafield.Id, Encoding.UTF8.GetBytes("Gorilla")));

                client.Send(datagram, null, 0);
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Send_Hostname_Port_PortOutOfRange()
        {
            IPClient client = null;

            try {
                client = new IPClient();

                Datagram datagram = new Datagram();
                datagram.Add(new Field(
                    IPClient.DefaultMetafield.Id, Encoding.UTF8.GetBytes("Gorilla")));

                client.Send(datagram, IPAddress.Loopback.ToString(), -1);
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Send_Hostname_Port_RemoteHostAlreadyEstablished()
        {
            IPClient client = null;

            try {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 0);
                client = new IPClient(endPoint.Address.ToString(), endPoint.Port);

                Datagram datagram = new Datagram();
                datagram.Add(new Field(
                    IPClient.DefaultMetafield.Id, Encoding.UTF8.GetBytes("Gorilla")));

                client.Send(datagram, endPoint.Address.ToString(), endPoint.Port);
            } finally {
                if (client != null) client.Close();
            }
        }

        [Test]
        [ExpectedException(typeof(SerializationException))]
        public void SendWithMetadata_InvalidDatagram()
        {
            TestClient client = null;
            Datagram datagram = new Datagram();

            try {
                client = new TestClient(IPAddress.Loopback.ToString(), 0);
                client.EmbedMetadata = true;
                datagram.Add(new Field(1, (long) 5));
                client.Send(datagram);
            } catch (SerializationException) {
                try {
                    datagram.Add(new Field(2, (double) 5.5));
                    client.Send(datagram);
                } catch (SerializationException) {
                    datagram.Add(new Field(3, "Gorilla"));
                    client.Send(datagram);
                }
            } finally {
                if (client != null) client.Close();
            }
        }
        #endregion unit test methods

        #region private methods
        private void OnError(Object sender, ExceptionEventArgs args)
        {
            Assert.AreEqual(true, args.Exception.InnerException is TestException);
            errorEvent.Set();
            Thread.Sleep(0);
        }

        private void OnReceive(Object sender, ReceiveEventArgs args)
        {
            Assert.AreEqual(sentDatagram, args.Datagram);
            receiveEvent.Set();
            Thread.Sleep(0);
        }
        #endregion private methods

        #region inner classes
        private sealed class TestClient : IPClient
        {
            private static Metafield idMetafield;
            private static Metafield nameMetafield;

            [ProtocolField(0)]
            public static Metafield IdMetafield
            {
                get {
                    if (idMetafield == null) {
                        lock (typeof(TestClient)) {
                            if (idMetafield == null) {
                                idMetafield = new Metafield(1, FieldType.Integral, 4);
                                idMetafield = Metafield.ReadOnly(idMetafield);
                            }
                        }
                    } return idMetafield;
                }
            }

            [ProtocolField(1)]
            public static Metafield NameMetafield
            {
                get {
                    if (nameMetafield == null) {
                        lock (typeof(TestClient)) {
                            if (nameMetafield == null) {
                                nameMetafield = new Metafield(2, FieldType.String);
                                nameMetafield = Metafield.ReadOnly(nameMetafield);
                            }
                        }
                    } return nameMetafield;
                }
            }

            public new bool Active
            {
                get { return base.Active; }
                set { base.Active = value; }
            }

            public new Socket Client
            {
                get { return base.Client; }
            }

            public TestClient() {}
            public TestClient(IPEndPoint localEndPoint)
                : base(localEndPoint) {}
            public TestClient(string hostname, int port)
                : base(hostname, port) {}
        }

        private sealed class DummyClient : IPClient
        {
            public DummyClient() {}
            public DummyClient(IPEndPoint localEndPoint)
                : base(localEndPoint) {}

            protected override int OnDeserialize(
                byte[] data, int startIndex, out Field manifest)
            {
                 throw new TestException();
            }
        }

        private sealed class TestException : Exception
        {
        }
        #endregion inner classes
    }
}
#endif
