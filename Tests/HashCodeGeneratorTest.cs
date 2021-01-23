//------------------------------------------------------------------------------
// <sourcefile name="HashCodeGeneratorTest.cs" language="C#" begin="10/31/2004">
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
using NUnit.Framework;

namespace Gekkota.Tests
{
    [TestFixture]
    public class HashCodeGeneratorTest
    {
        #region service methods
        [SetUp]
        public void Init() {}

        [TearDown]
        public void Clean() {}
        #endregion service methods

        #region unit test methods
        [Test]
        public void GenerateHashCode()
        {
            string s = "Gorilla";
            Assert.AreEqual(
                HashCodeGenerator.Generate(s),
                HashCodeGenerator.Generate(s));
        }

        [Test]
        public void GenerateHashCode_ToLower()
        {
            string s = "Gorilla";
            Assert.AreEqual(
                HashCodeGenerator.Generate(s, CasingRule.ToLower),
                HashCodeGenerator.Generate(s, CasingRule.ToLower));
        }

        [Test]
        public void GenerateHashCode_ToUpper()
        {
            string s = "Gorilla";
            Assert.AreEqual(
                HashCodeGenerator.Generate(s, CasingRule.ToUpper),
                HashCodeGenerator.Generate(s, CasingRule.ToUpper));
        }
        #endregion unit test methods
    }
}
#endif
