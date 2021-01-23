//------------------------------------------------------------------------------
// <sourcefile name="FieldSerializerTest.cs" language="C#" begin="05/10/2004">
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
using System.Collections;
using System.Runtime.InteropServices;
using Gekkota.Net;
using NUnit.Framework;

namespace Gekkota.Tests
{
    [TestFixture]
    public class FieldSerializerTest
    {
        #region service methods
        [SetUp]
        public void Init() {}

        [TearDown]
        public void Clean() {}
        #endregion service methods

        #region unit test methods
        [Test]
        public unsafe void Serialize_Byte()
        {
            Field field = new Field(1, Byte.MaxValue);
            byte[] buffer = new byte[Metafield.LayoutSize + field.Size];

            //
            // serialize with metadata
            //
            FieldSerializer.Serialize(field, buffer, 0, true);

            fixed (byte* pBuffer = buffer) {
                FieldHeader* pHeader = (FieldHeader*) pBuffer;

                Assert.AreEqual(pHeader->Id,
                    (ushort) IPAddress.HostToNetworkOrder((short) field.Id));
                Assert.AreEqual(pHeader->Size,
                    (ushort) IPAddress.HostToNetworkOrder((short) field.Size));
                Assert.AreEqual(pHeader->Type, field.Type);
                Assert.AreEqual(pHeader->Category, field.Category);
                Assert.AreEqual(*((byte*) (pBuffer + Metafield.LayoutSize)), field.ValueAsByte);
            }

            //
            // serialize wathout metadata
            //
            FieldSerializer.Serialize(field, buffer, 0, false);

            fixed (byte* pValue = buffer) {
                Assert.AreEqual(*((byte*) pValue), field.ValueAsByte);
            }
        }

        [Test]
        public unsafe void Serialize_Int16()
        {
            Field field = new Field(1, Int16.MaxValue);
            byte[] buffer = new byte[Metafield.LayoutSize + field.Size];

            //
            // serialize with metadata
            //
            FieldSerializer.Serialize(field, buffer, 0, true);

            fixed (byte* pBuffer = buffer) {
                FieldHeader* pHeader = (FieldHeader*) pBuffer;

                Assert.AreEqual(pHeader->Id,
                    (ushort) IPAddress.HostToNetworkOrder((short) field.Id));
                Assert.AreEqual(pHeader->Size,
                    (ushort) IPAddress.HostToNetworkOrder((short) field.Size));
                Assert.AreEqual(pHeader->Type, field.Type);
                Assert.AreEqual(pHeader->Category, field.Category);
                Assert.AreEqual(*((short*) (pBuffer + Metafield.LayoutSize)),
                    IPAddress.HostToNetworkOrder(field.ValueAsInt16));
            }

            //
            // serialize wathout metadata
            //
            FieldSerializer.Serialize(field, buffer, 0, false);

            fixed (byte* pValue = buffer) {
                Assert.AreEqual(*((short*) pValue),
                    IPAddress.HostToNetworkOrder(field.ValueAsInt16));
            }
        }

        [Test]
        public unsafe void Serialize_Int32()
        {
            Field field = new Field(1, Int32.MaxValue);
            byte[] buffer = new byte[Metafield.LayoutSize + field.Size];

            //
            // serialize with metadata
            //
            FieldSerializer.Serialize(field, buffer, 0, true);

            fixed (byte* pBuffer = buffer) {
                FieldHeader* pHeader = (FieldHeader*) pBuffer;

                Assert.AreEqual(pHeader->Id,
                    (ushort) IPAddress.HostToNetworkOrder((short) field.Id));
                Assert.AreEqual(pHeader->Size,
                    (ushort) IPAddress.HostToNetworkOrder((short) field.Size));
                Assert.AreEqual(pHeader->Type, field.Type);
                Assert.AreEqual(pHeader->Category, field.Category);
                Assert.AreEqual(*((int*) (pBuffer + Metafield.LayoutSize)),
                    IPAddress.HostToNetworkOrder(field.ValueAsInt32));
            }

            //
            // serialize wathout metadata
            //
            FieldSerializer.Serialize(field, buffer, 0, false);

            fixed (byte* pValue = buffer) {
                Assert.AreEqual(*((int*) pValue),
                    IPAddress.HostToNetworkOrder(field.ValueAsInt32));
            }
        }

        [Test]
        public unsafe void Serialize_Int64()
        {
            Field field = new Field(1, Int64.MaxValue);
            byte[] buffer = new byte[Metafield.LayoutSize + field.Size];

            //
            // serialize with metadata
            //
            FieldSerializer.Serialize(field, buffer, 0, true);

            fixed (byte* pBuffer = buffer) {
                FieldHeader* pHeader = (FieldHeader*) pBuffer;

                Assert.AreEqual(pHeader->Id,
                    (ushort) IPAddress.HostToNetworkOrder((short) field.Id));
                Assert.AreEqual(pHeader->Size,
                    (ushort) IPAddress.HostToNetworkOrder((short) field.Size));
                Assert.AreEqual(pHeader->Type, field.Type);
                Assert.AreEqual(pHeader->Category, field.Category);
                Assert.AreEqual(*((long*) (pBuffer + Metafield.LayoutSize)),
                    IPAddress.HostToNetworkOrder(field.ValueAsInt64));
            }

            //
            // serialize wathout metadata
            //
            FieldSerializer.Serialize(field, buffer, 0, false);

            fixed (byte* pValue = buffer) {
                Assert.AreEqual(*((long*) pValue),
                    IPAddress.HostToNetworkOrder(field.ValueAsInt64));
            }
        }

        [Test]
        public unsafe void Serialize_Single()
        {
            Field field = new Field(1, Single.MaxValue);
            byte[] buffer = new byte[Metafield.LayoutSize + field.Size];

            //
            // serialize with metadata
            //
            FieldSerializer.Serialize(field, buffer, 0, true);

            fixed (byte* pBuffer = buffer) {
                FieldHeader* pHeader = (FieldHeader*) pBuffer;

                Assert.AreEqual(pHeader->Id,
                    (ushort) IPAddress.HostToNetworkOrder((short) field.Id));
                Assert.AreEqual(pHeader->Size,
                    (ushort) IPAddress.HostToNetworkOrder((short) field.Size));
                Assert.AreEqual(pHeader->Type, field.Type);
                Assert.AreEqual(pHeader->Category, field.Category);
                Assert.AreEqual(*((int*) (pBuffer + Metafield.LayoutSize)),
                    IPAddress.HostToNetworkOrder(field.ValueAsInt32));
            }

            //
            // serialize wathout metadata
            //
            FieldSerializer.Serialize(field, buffer, 0, false);

            fixed (byte* pValue = buffer) {
                Assert.AreEqual(*((int*) pValue),
                    IPAddress.HostToNetworkOrder(field.ValueAsInt32));
            }
        }

        [Test]
        public unsafe void Serialize_Double()
        {
            Field field = new Field(1, Double.MaxValue);
            byte[] buffer = new byte[Metafield.LayoutSize + field.Size];

            //
            // serialize with metadata
            //
            FieldSerializer.Serialize(field, buffer, 0, true);

            fixed (byte* pBuffer = buffer) {
                FieldHeader* pHeader = (FieldHeader*) pBuffer;

                Assert.AreEqual(pHeader->Id,
                    (ushort) IPAddress.HostToNetworkOrder((short) field.Id));
                Assert.AreEqual(pHeader->Size,
                    (ushort) IPAddress.HostToNetworkOrder((short) field.Size));
                Assert.AreEqual(pHeader->Type, field.Type);
                Assert.AreEqual(pHeader->Category, field.Category);
                Assert.AreEqual(*((long*) (pBuffer + Metafield.LayoutSize)),
                    IPAddress.HostToNetworkOrder(field.ValueAsInt64));
            }

            //
            // serialize wathout metadata
            //
            FieldSerializer.Serialize(field, buffer, 0, false);

            fixed (byte* pValue = buffer) {
                Assert.AreEqual(*((long*) pValue),
                    IPAddress.HostToNetworkOrder(field.ValueAsInt64));
            }
        }

        [Test]
        public unsafe void Serialize_String()
        {
            Field field = new Field(1, "Gorilla");
            byte[] buffer = new byte[Metafield.LayoutSize + field.Size];

            //
            // serialize with metadata
            //
            FieldSerializer.Serialize(field, buffer, 0, true);

            fixed (byte* pBuffer = buffer) {
                FieldHeader* pHeader = (FieldHeader*) pBuffer;
                Assert.AreEqual(pHeader->Id,
                    (ushort) IPAddress.HostToNetworkOrder((short) field.Id));
                Assert.AreEqual(pHeader->Size,
                    (ushort) IPAddress.HostToNetworkOrder((short) field.Size));
                Assert.AreEqual(pHeader->Type, field.Type);
                Assert.AreEqual(pHeader->Category, field.Category);
            }

            Assert.AreEqual(
                Encoding.UTF8.GetString(
                    buffer,
                    Metafield.LayoutSize,
                    buffer.Length - Metafield.LayoutSize), field.ValueAsString);

            //
            // serialize wathout metadata
            //
            FieldSerializer.Serialize(field, buffer, 0, false);
            Assert.AreEqual(
                Encoding.UTF8.GetString(buffer, 0, buffer.Length - Metafield.LayoutSize),
                field.ValueAsString);
        }

        [Test]
        public unsafe void Serialize_ByteArray()
        {
            Field field = new Field(1, Encoding.UTF8.GetBytes("Gorilla"));
            byte[] buffer = new byte[Metafield.LayoutSize + field.Size];

            //
            // serialize with metadata
            //
            FieldSerializer.Serialize(field, buffer, 0, true);

            fixed (byte* pBuffer = buffer) {
                FieldHeader* pHeader = (FieldHeader*) pBuffer;
                Assert.AreEqual(pHeader->Id,
                    (ushort) IPAddress.HostToNetworkOrder((short) field.Id));
                Assert.AreEqual(pHeader->Size,
                    (ushort) IPAddress.HostToNetworkOrder((short) field.Size));
                Assert.AreEqual(pHeader->Type, field.Type);
                Assert.AreEqual(pHeader->Category, field.Category);
            }

            Assert.AreEqual(
                Encoding.UTF8.GetString(
                    buffer,
                    Metafield.LayoutSize,
                    buffer.Length - Metafield.LayoutSize), field.ValueAsString);

            //
            // serialize wathout metadata
            //
            FieldSerializer.Serialize(field, buffer, 0, false);
            Assert.AreEqual(
                Encoding.UTF8.GetString(buffer, 0, buffer.Length - Metafield.LayoutSize),
                field.ValueAsString);
        }

        [Test]
        public unsafe void Serialize_Undefined()
        {
            Field field = new Field(1);
            field.Category = FieldCategory.Manifest;
            byte[] buffer = new byte[Metafield.LayoutSize];

            //
            // field type is undefined: only its metadata is serialized
            //
            FieldSerializer.Serialize(field, buffer, 0, true);

            fixed (byte* pBuffer = buffer) {
                FieldHeader* pHeader = (FieldHeader*) pBuffer;
                Assert.AreEqual(pHeader->Id,
                    (ushort) IPAddress.HostToNetworkOrder((short) field.Id));
                Assert.AreEqual(pHeader->Size,
                    (ushort) IPAddress.HostToNetworkOrder((short) field.Size));
                Assert.AreEqual(pHeader->Type, field.Type);
                Assert.AreEqual(pHeader->Category, field.Category);
            }

            //
            // field type is undefined: no serialization takes place
            //
            FieldSerializer.Serialize(field, buffer, 0, false);

            for (int i = 0; i < buffer.Length; i++) { buffer[i] = 0x00; }
            fixed (byte* pBuffer = buffer) {
                FieldHeader* pHeader = (FieldHeader*) pBuffer;
                Assert.AreEqual((ushort) 0, pHeader->Id);
                Assert.AreEqual((ushort) 0, pHeader->Size);
                Assert.AreEqual((byte) 0, (byte) pHeader->Type);
                Assert.AreEqual((byte) 0, (byte) pHeader->Category);
            }
        }

        [Test]
        public unsafe void Deserialize_Byte()
        {
            Field field = new Field(1, Byte.MaxValue);
            byte[] buffer = new byte[Metafield.LayoutSize + field.Size];

            fixed (byte* pBuffer = buffer) {
                FieldHeader* pHeader = (FieldHeader*) pBuffer;

                pHeader->Id = (ushort) IPAddress.HostToNetworkOrder((short) field.Id);
                pHeader->Size = (ushort) IPAddress.HostToNetworkOrder((short) field.Size);
                pHeader->Type = field.Type;
                pHeader->Category = field.Category;
                *((byte*) (pBuffer + Metafield.LayoutSize)) = field.ValueAsByte;
            }

            //
            // deserialize from a byte array that contains metadata
            //
            Field deserialized = FieldSerializer.Deserialize(buffer, 0);
            Assert.AreEqual(field, deserialized);

            //
            // deserialize from a byte array that does not contain metadata
            //
            deserialized = FieldSerializer.Deserialize(
                buffer, Metafield.LayoutSize, field.GetMetafield());
            Assert.AreEqual(field, deserialized);
        }

        [Test]
        public unsafe void Deserialize_Int16()
        {
            Field field = new Field(1, Int16.MaxValue);
            byte[] buffer = new byte[Metafield.LayoutSize + field.Size];

            fixed (byte* pBuffer = buffer) {
                FieldHeader* pHeader = (FieldHeader*) pBuffer;

                pHeader->Id = (ushort) IPAddress.HostToNetworkOrder((short) field.Id);
                pHeader->Size = (ushort) IPAddress.HostToNetworkOrder((short) field.Size);
                pHeader->Type = field.Type;
                pHeader->Category = field.Category;
                *((short*) (pBuffer + Metafield.LayoutSize)) =
                    IPAddress.HostToNetworkOrder(field.ValueAsInt16);
            }

            //
            // deserialize from a byte array that contains metadata
            //
            Field deserialized = FieldSerializer.Deserialize(buffer, 0);
            Assert.AreEqual(field, deserialized);

            //
            // deserialize from a byte array that does not contain metadata
            //
            deserialized = FieldSerializer.Deserialize(
                buffer, Metafield.LayoutSize, field.GetMetafield());
            Assert.AreEqual(field, deserialized);
        }

        [Test]
        public unsafe void Deserialize_Int32()
        {
            Field field = new Field(1, Int32.MaxValue);
            byte[] buffer = new byte[Metafield.LayoutSize + field.Size];

            fixed (byte* pBuffer = buffer) {
                FieldHeader* pHeader = (FieldHeader*) pBuffer;

                pHeader->Id = (ushort) IPAddress.HostToNetworkOrder((short) field.Id);
                pHeader->Size = (ushort) IPAddress.HostToNetworkOrder((short) field.Size);
                pHeader->Type = field.Type;
                pHeader->Category = field.Category;
                *((int*) (pBuffer + Metafield.LayoutSize)) =
                    IPAddress.HostToNetworkOrder(field.ValueAsInt32);
            }

            //
            // deserialize from a byte array that contains metadata
            //
            Field deserialized = FieldSerializer.Deserialize(buffer, 0);
            Assert.AreEqual(field, deserialized);

            //
            // deserialize from a byte array that does not contain metadata
            //
            deserialized = FieldSerializer.Deserialize(
                buffer, Metafield.LayoutSize, field.GetMetafield());
            Assert.AreEqual(field, deserialized);
        }

        [Test]
        public unsafe void Deserialize_Int64()
        {
            Field field = new Field(1, Int64.MaxValue);
            byte[] buffer = new byte[Metafield.LayoutSize + field.Size];

            fixed (byte* pBuffer = buffer) {
                FieldHeader* pHeader = (FieldHeader*) pBuffer;

                pHeader->Id = (ushort) IPAddress.HostToNetworkOrder((short) field.Id);
                pHeader->Size = (ushort) IPAddress.HostToNetworkOrder((short) field.Size);
                pHeader->Type = field.Type;
                pHeader->Category = field.Category;
                *((long*) (pBuffer + Metafield.LayoutSize)) =
                    IPAddress.HostToNetworkOrder(field.ValueAsInt64);
            }

            //
            // deserialize from a byte array that contains metadata
            //
            Field deserialized = FieldSerializer.Deserialize(buffer, 0);
            Assert.AreEqual(field, deserialized);

            //
            // deserialize from a byte array that does not contain metadata
            //
            deserialized = FieldSerializer.Deserialize(
                buffer, Metafield.LayoutSize, field.GetMetafield());
            Assert.AreEqual(field, deserialized);
        }

        [Test]
        public unsafe void Deserialize_Single()
        {
            Field field = new Field(1, Single.MaxValue);
            byte[] buffer = new byte[Metafield.LayoutSize + field.Size];

            fixed (byte* pBuffer = buffer) {
                FieldHeader* pHeader = (FieldHeader*) pBuffer;

                pHeader->Id = (ushort) IPAddress.HostToNetworkOrder((short) field.Id);
                pHeader->Size = (ushort) IPAddress.HostToNetworkOrder((short) field.Size);
                pHeader->Type = field.Type;
                pHeader->Category = field.Category;
                *((int*) (pBuffer + Metafield.LayoutSize)) =
                    IPAddress.HostToNetworkOrder(field.ValueAsInt32);
            }

            //
            // deserialize from a byte array that contains metadata
            //
            Field deserialized = FieldSerializer.Deserialize(buffer, 0);
            Assert.AreEqual(field, deserialized);

            //
            // deserialize from a byte array that does not contain metadata
            //
            deserialized = FieldSerializer.Deserialize(
                buffer, Metafield.LayoutSize, field.GetMetafield());
            Assert.AreEqual(field, deserialized);
        }

        [Test]
        public unsafe void Deserialize_Double()
        {
            Field field = new Field(1, Double.MaxValue);
            byte[] buffer = new byte[Metafield.LayoutSize + field.Size];

            fixed (byte* pBuffer = buffer) {
                FieldHeader* pHeader = (FieldHeader*) pBuffer;

                pHeader->Id = (ushort) IPAddress.HostToNetworkOrder((short) field.Id);
                pHeader->Size = (ushort) IPAddress.HostToNetworkOrder((short) field.Size);
                pHeader->Type = field.Type;
                pHeader->Category = field.Category;
                *((long*) (pBuffer + Metafield.LayoutSize)) =
                    IPAddress.HostToNetworkOrder(field.ValueAsInt64);
            }

            //
            // deserialize from a byte array that contains metadata
            //
            Field deserialized = FieldSerializer.Deserialize(buffer, 0);
            Assert.AreEqual(field, deserialized);

            //
            // deserialize from a byte array that does not contain metadata
            //
            deserialized = FieldSerializer.Deserialize(
                buffer, Metafield.LayoutSize, field.GetMetafield());
            Assert.AreEqual(field, deserialized);
        }

        [Test]
        public unsafe void Deserialize_String()
        {
            Field field = new Field(1, "Gorilla");
            byte[] buffer = new byte[Metafield.LayoutSize + field.Size];

            fixed (byte* pBuffer = buffer) {
                FieldHeader* pHeader = (FieldHeader*) pBuffer;

                pHeader->Id = (ushort) IPAddress.HostToNetworkOrder((short) field.Id);
                pHeader->Size = (ushort) IPAddress.HostToNetworkOrder((short) field.Size);
                pHeader->Type = field.Type;
                pHeader->Category = field.Category;
                Buffer.BlockCopy(
                    field.ValueAsByteArray, 0,              // source
                    buffer, Metafield.LayoutSize,           // destination
                    buffer.Length - Metafield.LayoutSize);  // count
            }

            //
            // deserialize from a byte array that contains metadata
            //
            Field deserialized = FieldSerializer.Deserialize(buffer, 0);
            Assert.AreEqual(field, deserialized);

            //
            // deserialize from a byte array that does not contain metadata
            //
            deserialized = FieldSerializer.Deserialize(
                buffer, Metafield.LayoutSize, field.GetMetafield());
            Assert.AreEqual(field, deserialized);
        }

        public unsafe void Deserialize_ByteArray()
        {
            Field field = new Field(1, Encoding.UTF8.GetBytes("Gorilla"));
            byte[] buffer = new byte[Metafield.LayoutSize + field.Size];

            fixed (byte* pBuffer = buffer) {
                FieldHeader* pHeader = (FieldHeader*) pBuffer;
                pHeader->Id = (ushort) IPAddress.HostToNetworkOrder((short) field.Id);
                pHeader->Size = (ushort) IPAddress.HostToNetworkOrder((short) field.Size);
                pHeader->Type = field.Type;
                pHeader->Category = field.Category;
                Buffer.BlockCopy(
                    field.ValueAsByteArray, 0,              // source
                    buffer, Metafield.LayoutSize,           // destination
                    buffer.Length - Metafield.LayoutSize);  // count
            }

            //
            // deserialize from a byte array that contains metadata
            //
            Field deserialized = FieldSerializer.Deserialize(buffer, 0);
            Assert.AreEqual(field, deserialized);

            //
            // deserialize from a byte array that does not contain metadata
            //
            deserialized = FieldSerializer.Deserialize(
                buffer, Metafield.LayoutSize, field.GetMetafield());
            Assert.AreEqual(field, deserialized);
        }

        [Test]
        public unsafe void Deserialize_Undefined()
        {
            Field field = new Field(1);
            field.Category = FieldCategory.Manifest;
            byte[] buffer = new byte[Metafield.LayoutSize];

            fixed (byte* pBuffer = buffer) {
                FieldHeader* pHeader = (FieldHeader*) pBuffer;
                pHeader->Id = (ushort) IPAddress.HostToNetworkOrder((short) field.Id);
                pHeader->Size = (ushort) IPAddress.HostToNetworkOrder((short) field.Size);
                pHeader->Type = field.Type;
                pHeader->Category = field.Category;
            }

            //
            // deserialize an undefined field
            //
            Field deserialized = FieldSerializer.Deserialize(buffer, 0);
            Assert.AreEqual(field, deserialized);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Serialize_NullField()
        {
            byte[] buffer = new byte[1];
            FieldSerializer.Serialize(null, buffer, 0, false);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Serialize_NullBuffer()
        {
            Field field = new Field(1, Int32.MaxValue);
            FieldSerializer.Serialize(field, null, 0, false);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Serialize_NegativeIndex()
        {
            Field field = new Field(1, Int32.MaxValue);
            byte[] buffer = new byte[field.Size];
            FieldSerializer.Serialize(field, buffer, -1, false);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Serialize_IndexOutOfBounds()
        {
            Field field = new Field(1, Int32.MaxValue);
            byte[] buffer = new byte[field.Size];
            FieldSerializer.Serialize(field, buffer, field.Size, false);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Deserialize_NullBuffer()
        {
            FieldSerializer.Deserialize(null, 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Deserialize_NullMetafield()
        {
            byte[] buffer = new byte[1];
            FieldSerializer.Deserialize(buffer, 0, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Deserialize_NegativeIndex()
        {
            byte[] buffer = new byte[1];
            FieldSerializer.Deserialize(buffer, -1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Deserialize_IndexOutOfBounds()
        {
            byte[] buffer = new byte[1];
            FieldSerializer.Deserialize(buffer, buffer.Length);
        }
        #endregion unit test methods

        #region inner structs
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        internal struct FieldHeader
        {
            public ushort         Id;
            public FieldType      Type;
            public ushort         Size;
            public FieldCategory  Category;
        }
        #endregion inner structs
    }
}
#endif
