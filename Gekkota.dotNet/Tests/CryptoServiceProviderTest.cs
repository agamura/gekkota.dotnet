//------------------------------------------------------------------------------
// <sourcefile name="CryptoServiceProviderTest.cs" language="C#" begin="11/05/2005">
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
using System.Security.Cryptography;
using NUnit.Framework;
using Gekkota.Utilities;
using Gekkota.Security.Cryptography;

namespace Gekkota.Tests
{
    [TestFixture]
    public class RijndaelTest
      {
#region private fields
        private byte[] data;
        #endregion private fields

#region service methods
        [SetUp]
        public void Init()
        {
            //
            // create a random sized byte array
            //
            Random random = new Random();
            data = new byte[random.Next(Int16.MaxValue, UInt16.MaxValue)];

            for (int i = 0; i < data.Length; i++) {
                data[i] = (byte) (i % Byte.MaxValue);
            }
        }

        [TearDown]
        public void Clean() {}
        #endregion service methods

#region unit test methods
        [Test]
        public void Algorithm()
        {
            Rijndael algorithm = Rijndael.Create();
            CryptoServiceProvider cryptor = new CryptoServiceProvider(algorithm);

            Assert.AreEqual(algorithm, cryptor.Algorithm);
        }

        [Test]
        public void CreateInstance()
        {
            Rijndael algorithm = Rijndael.Create();
            CryptoServiceProvider cryptor = new CryptoServiceProvider(algorithm);

            Assert.IsTrue(cryptor.Algorithm != null);
        }

        [Test]
        public void Encrypt()
        {
        }

        [Test]
        public void Decrypt()
        {
        }

        [Test]
        [ExpectedException (typeof(ArgumentNullException))]
        public void CreateInstance_NullAlgorithm()
        {
            Rijndael algorithm = Rijndael.Create();
            CryptoServiceProvider cryptor = new CryptoServiceProvider(algorithm);
            cryptor.Algorithm = null;
        }

        [Test]
        [ExpectedException (typeof(ArgumentNullException))]
        public void Algorithm_NullValue()
        {
            CryptoServiceProvider cryptor = new CryptoServiceProvider(null);
        }

        [Test]
        [ExpectedException (typeof(ArgumentNullException))]
        public void Encrypt_NullData()
        {
            Rijndael algorithm = Rijndael.Create();
            CryptoServiceProvider cryptor = new CryptoServiceProvider(algorithm);
            cryptor.Encrypt(null, 0, 0);
        }

        [Test]
        [ExpectedException (typeof(ArgumentNullException))]
        public void Decrypt_NullData()
        {
            Rijndael algorithm = Rijndael.Create();
            CryptoServiceProvider cryptor = new CryptoServiceProvider(algorithm);
            cryptor.Decrypt(null, 0, 0);
        }

        [Test]
        [ExpectedException (typeof(ArgumentOutOfRangeException))]
        public void Encrypt_NegativeIndex()
        {
            Rijndael algorithm = Rijndael.Create();
            CryptoServiceProvider cryptor = new CryptoServiceProvider(algorithm);

            try {
                cryptor.Encrypt(data, -1, 0);
            } catch (ArgumentOutOfRangeException) {
                cryptor.Encrypt(data, 0, -1);
            }
        }

        [Test]
        [ExpectedException (typeof(ArgumentOutOfRangeException))]
        public void Decrypt_NegativeIndex()
        {
            Rijndael algorithm = Rijndael.Create();
            CryptoServiceProvider cryptor = new CryptoServiceProvider(algorithm);

            try {
                cryptor.Decrypt(data, -1, 0);
            } catch (ArgumentOutOfRangeException) {
                cryptor.Decrypt(data, 0, -1);
            }
        }

        [Test]
        [ExpectedException (typeof(ArgumentException))]
        public void Encrypt_IndexOutOfBounds()
        {
            Rijndael algorithm = Rijndael.Create();
            CryptoServiceProvider cryptor = new CryptoServiceProvider(algorithm);

            try {
                cryptor.Encrypt(data, data.Length + 1, 0);
            } catch (ArgumentOutOfRangeException) {
                cryptor.Encrypt(data, 0, data.Length + 1);
            }
        }

        [Test]
        [ExpectedException (typeof(ArgumentException))]
        public void Decrypt_IndexOutOfBounds()
        {
            Rijndael algorithm = Rijndael.Create();
            CryptoServiceProvider cryptor = new CryptoServiceProvider(algorithm);

            try {
                cryptor.Decrypt(data, data.Length + 1, 0);
            } catch (ArgumentOutOfRangeException) {
                cryptor.Decrypt(data, 0, data.Length + 1);
            }
        }
        #endregion unit test methods

#region private methods
        private void Transform()
        {
            Rijndael algorithm = Rijndael.Create();
            CryptoServiceProvider cryptor = new CryptoServiceProvider(algorithm);

            byte[] encrypted = cryptor.Encrypt(data, 0, data.Length);
            Assert.IsFalse(Utilities.Buffer.Equals(data, encrypted));
            byte[] decrypted = cryptor.Decrypt(encrypted, 0, encrypted.Length);
            Assert.IsTrue(Utilities.Buffer.Equals(data, decrypted));

            //
            // encrypt/decrypt twice in order to test trasform reuse
            //
            encrypted = cryptor.Encrypt(data, 0, data.Length);
            Assert.IsFalse(Utilities.Buffer.Equals(data, encrypted));
            decrypted = cryptor.Decrypt(encrypted, 0, encrypted.Length);
            Assert.IsTrue(Utilities.Buffer.Equals(data, decrypted));
        }
#endregion private methods
    }
}
#endif
