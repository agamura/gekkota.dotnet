//------------------------------------------------------------------------------
// <sourcefile name="ProtocolFieldAttributeTest.cs" language="C#" begin="05/12/2004">
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
using Gekkota.Net;
using NUnit.Framework;

namespace Gekkota.Tests
{
    [TestFixture]
    public class ProtocolFieldAttributeTest
    {
        #region service methods
        [SetUp]
        public void Init() {}

        [TearDown]
        public void Clean() {}
        #endregion service methods

        #region unit test methods
        [Test]
        public void Position()
        {
            ProtocolFieldAttribute attribute = new ProtocolFieldAttribute(1);
            Assert.AreEqual(1, attribute.Position);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Position_NegativeValue()
        {
            ProtocolFieldAttribute attribute = new ProtocolFieldAttribute(-1);
        }
        #endregion unit test methdos
    }
}
#endif
