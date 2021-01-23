//------------------------------------------------------------------------------
// <sourcefile name="ChecksumGeneratorTest.cs" language="C#" begin="05/12/2004">
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
using Gekkota.Checksums;
using NUnit.Framework;

namespace Gekkota.Tests
{
    [TestFixture]
    public class ChecksumGeneratorTest
    {
        #region private fields
        private byte[] data1;
        private byte[] data2;
        #endregion private fields

        #region service methods
        [SetUp]
        public void Init()
        {
            data1 = Encoding.UTF8.GetBytes("Gorilla");
            data2 = Encoding.UTF8.GetBytes("Bear");
        }

        [TearDown]
        public void Clean() {}
        #endregion service methods

        #region unit test methods
        [Test]
        public void Model()
        {
            ChecksumGenerator checksumGenerator = new ChecksumGenerator(new Crc16());
            IChecksumModel model = new Crc32();
            checksumGenerator.Model = model;
            Assert.AreEqual(model, checksumGenerator.Model);
        }

        [Test]
        public void CreateInstance()
        {
            IChecksumModel model = new Crc16();
            ChecksumGenerator checksumGenerator = new ChecksumGenerator(model);
            Assert.AreEqual(model, checksumGenerator.Model);
        }

        [Test]
        public void Generate_Crc16()
        {
            ChecksumGenerator checksumGenerator = new ChecksumGenerator(new Crc16());
            long checksum1 = checksumGenerator.Generate(data1, 0, data1.Length);
            long checksum2 = checksumGenerator.Generate(data2, 0, data2.Length);

            Assert.IsTrue((ulong) checksum1 > 0);
            Assert.IsTrue((ulong) checksum2 > 0);
            Assert.IsTrue(checksum1 != checksum2);
        }

        [Test]
        public void Generate_Crc32()
        {
            ChecksumGenerator checksumGenerator = new ChecksumGenerator(new Crc32());
            long checksum1 = checksumGenerator.Generate(data1, 0, data1.Length);
            long checksum2 = checksumGenerator.Generate(data2, 0, data2.Length);

            Assert.IsTrue((ulong) checksum1 > 0);
            Assert.IsTrue((ulong) checksum2 > 0);
            Assert.IsTrue(checksum1 != checksum2);
        }

        [Test]
        public void Generate_Ccitt()
        {
            ChecksumGenerator checksumGenerator = new ChecksumGenerator(new Ccitt());
            long checksum1 = checksumGenerator.Generate(data1, 0, data1.Length);
            long checksum2 = checksumGenerator.Generate(data2, 0, data2.Length);

            Assert.IsTrue((ulong) checksum1 > 0);
            Assert.IsTrue((ulong) checksum2 > 0);
            Assert.IsTrue(checksum1 != checksum2);
        }

        [Test]
        public void Generate_Xmodem()
        {
            ChecksumGenerator checksumGenerator = new ChecksumGenerator(new Xmodem());
            long checksum1 = checksumGenerator.Generate(data1, 0, data1.Length);
            long checksum2 = checksumGenerator.Generate(data2, 0, data2.Length);

            Assert.IsTrue((ulong) checksum1 > 0);
            Assert.IsTrue((ulong) checksum2 > 0);
            Assert.IsTrue(checksum1 != checksum2);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Model_NullValue()
        {
            ChecksumGenerator checksumGenerator = new ChecksumGenerator(new Crc16());
            checksumGenerator.Model = null;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateInstance_NullModel()
        {
            new ChecksumGenerator(null);
        }

        [Test]
        [ExpectedException (typeof(ArgumentNullException))]
        public void Generate_NullData()
        {
            ChecksumGenerator checksumGenerator = new ChecksumGenerator(new Crc16());
            checksumGenerator.Generate(null, 0, 0);
        }

        [Test]
        [ExpectedException (typeof(ArgumentOutOfRangeException))]
        public void Generate_NegativeIndex()
        {
            ChecksumGenerator checksumGenerator = new ChecksumGenerator(new Crc16());

            try {
                checksumGenerator.Generate(data1, -1, 0);
            } catch (ArgumentOutOfRangeException) {
                checksumGenerator.Generate(data1, 0, -1);
            }
        }

        [Test]
        [ExpectedException (typeof(ArgumentException))]
        public void Generate_IndexOutOfBounds()
        {
            ChecksumGenerator checksumGenerator = new ChecksumGenerator(new Crc16());

            try {
                checksumGenerator.Generate(data1, data1.Length + 1, 0);
            } catch (ArgumentOutOfRangeException) {
                checksumGenerator.Generate(data1, 0, data1.Length + 1);
            }
        }
        #endregion unit test methods
    }
}
#endif
