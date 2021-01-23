//------------------------------------------------------------------------------
// <sourcefile name="XudpClientTest.cs" language="C#" begin="10/20/2005">
//
//     <author name="Giuseppe Greco" email="giuseppe.greco@agamura.com" />
//
//     <copyright company="Agamura" url="http://www.agamura.com">
//         Copyright (C) 2005 Agamura, Inc.  All rights reserved.
//     </copyright>
//
// </sourcefile>
//------------------------------------------------------------------------------

#if DEBUG
using System;
using System.Net;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Gekkota.Net;
using Gekkota.Net.Sockets;
using Gekkota.Security.Cryptography;

namespace Gekkota.Tests
{
    [TestFixture]
    public class XudpClientTest
    {
        #region private fields
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
        public new void GetHashCode()
        {
            XudpClient client = new XudpClient();
            ushort hashCode = (ushort) HashCodeGenerator.Generate("XUDP");
            Assert.AreEqual(hashCode, client.GetHashCode());
        }
        #endregion unit test methods
    }
}
#endif
