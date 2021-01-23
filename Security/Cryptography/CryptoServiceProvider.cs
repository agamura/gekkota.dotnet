//------------------------------------------------------------------------------
// <sourcefile name="CryptoServiceProvider.cs" language="C#" begin="11/05/2005">
//
//     <author name="Giuseppe Greco" email="giuseppe.greco@agamura.com" />
//
//     <copyright company="Agamura" url="http://www.agamura.com">
//         Copyright (C) 2005 Agamura, Inc.  All rights reserved.
//     </copyright>
//
// </sourcefile>
//------------------------------------------------------------------------------

using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Gekkota.Properties;
using Gekkota.Utilities;

namespace Gekkota.Security.Cryptography
{
    public class CryptoServiceProvider
    {
        #region private fields
        private const string DefaultAssemblyString = "mscorlib";
        private const string DefaultAlgorithmName = "System.Security.Cryptography.RijndaelManaged";
        private const long DefaultKeyDuration = 0x25B7F3D4000; // 72 hours

        private Type algorithmType;
        private SymmetricAlgorithm algorithm;
        private TimeSpan keyDuration;
        private DateTime lastEncryptionTime;
        private ICryptoTransform decryptor;
        private ICryptoTransform encryptor;
        #endregion private fields

        #region public events
        public event ChangeKeyEventHandler ChangeKey;
        #endregion public events

        #region public properties
        public SymmetricAlgorithm Algorithm
        {
            get {
                if (algorithm == null) {
                    algorithm = CreatePeerAlgorithm();
                }

                return algorithm;
            }
        }

        public TimeSpan KeyDuration
        {
            get { return keyDuration; }
            set { KeyDuration = value; }
        }
        #endregion public properties

        #region public constructors
        public CryptoServiceProvider()
            : this(DefaultAssemblyString, DefaultAlgorithmName)
        {
        }

        public CryptoServiceProvider(string algorithmName)
            : this(null, algorithmName)
        {
        }

        public CryptoServiceProvider(string assemblyString, string algorithmName)
        {
            LoadAlgorithm(assemblyString, algorithmName);
            keyDuration = new TimeSpan(DefaultKeyDuration);
            lastEncryptionTime = new DateTime(0);
        }
        #endregion public constructors

        #region public methods
        public SymmetricAlgorithm CreatePeerAlgorithm()
        {
            return (SymmetricAlgorithm) Activator.CreateInstance(algorithmType);
        }

        public byte[] Encrypt(byte[] data, int index, int count)
        {
            if (encryptor == null) {
                encryptor = Algorithm.CreateEncryptor();
            }

            byte[] encrypted = Transform(
                data, index, count,
                encryptor, CryptoService.Encrypt);

            if (!encryptor.CanReuseTransform) {
                encryptor.Dispose();
                encryptor = null;
            }

            return encrypted;
        }

        public byte[] Decrypt(byte[] data, int index, int count)
        {
            if (decryptor == null) {
                decryptor = Algorithm.CreateDecryptor();
            }

            byte[] decrypted = Transform(
                data, index, count,
                decryptor, CryptoService.Decrypt);

            if (!decryptor.CanReuseTransform) {
                decryptor.Dispose();
                decryptor = null;
            }

            return decrypted;
        }

        public void LoadAlgorithm(string assemblyString, string algorithmName)
        {
            if (algorithmName == null) {
                throw new ArgumentNullException("algorithmName");
            }

            Assembly assembly = assemblyString != null
                ? Assembly.Load(assemblyString)
                : Assembly.GetCallingAssembly();

            Type type = assembly.GetType(algorithmName);

            if (type == null) {
                throw new ArgumentException(string.Format(
                    Resources.Error_TypeNotFound, algorithmName, assembly.CodeBase),
                    "algorithmName");
            }

            if (!type.IsSubclassOf(typeof(SymmetricAlgorithm))) {
                throw new ArgumentException(string.Format(
                    Resources.Error_NotSubclassOf, algorithmName, "SymmetricAlgorithm"),
                    "algorithmName");
            }

            algorithmType = type;
            algorithm = null;
            decryptor = encryptor = null;
        }
        #endregion public methods

        #region private methods
        private byte[] Transform(byte[] data, int index, int count,
            ICryptoTransform cryptoTransform, CryptoService cryptoService)
        {
            BoundsChecker.Check("data", data, index, count);

            //
            // the memory stream granularity must match the block size
            // of the current cryptographic operation
            //
            int blockSize = cryptoService == CryptoService.Encrypt
                ? cryptoTransform.OutputBlockSize
                : cryptoTransform.InputBlockSize;

            int capacity = count;
            int mod = count % blockSize;
            if (mod > 0) capacity += (blockSize - mod);

            MemoryStream memoryStream = new MemoryStream(capacity);
            CryptoStream cryptoStream = new CryptoStream(
                memoryStream,
                cryptoTransform,
                CryptoStreamMode.Write);

            cryptoStream.Write(data, index, count);
            cryptoStream.FlushFinalBlock();

            cryptoStream.Close();
            cryptoStream = null;

            return cryptoService == CryptoService.Encrypt
                ? memoryStream.GetBuffer()
                : memoryStream.ToArray();
        }
        #endregion private methods
    }
}