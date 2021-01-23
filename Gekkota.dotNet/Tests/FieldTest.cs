//------------------------------------------------------------------------------
// <sourcefile name="FieldTest.cs" language="C#" begin="05/06/2004">
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
using System.Collections;
using Gekkota.Net;
using Gekkota.Utilities;
using NUnit.Framework;

namespace Gekkota.Tests
{
    [TestFixture]
    public class FieldTest
    {
        #region service methods
        [SetUp]
        public void Init() {}

        [TearDown]
        public void Clean() {}
        #endregion service methods

        #region unit test methods
        [Test]
        public void Category()
        {
            Field field = new Field(1, Int16.MaxValue);
            field.Category = FieldCategory.Header;
            Assert.AreEqual(FieldCategory.Header, field.Category);
        }

        [Test]
        public void Id()
        {
            Field field = new Field();
            field.Id = 1;
            Assert.AreEqual(1, field.Id);
        }

        [Test]
        public void IsReadOnly()
        {
            Field field = new Field();
            Assert.AreEqual(false, field.IsReadOnly);
        }

        [Test]
        public unsafe void Size()
        {
            Field field = new Field();
            field.ValueAsByte = Byte.MaxValue;
            Assert.AreEqual(sizeof(byte), field.Size);

            field.ValueAsInt16 = Int16.MaxValue;
            Assert.AreEqual(sizeof(short), field.Size);

            field.ValueAsInt32 = Int32.MaxValue;
            Assert.AreEqual(sizeof(int), field.Size);

            field.ValueAsInt64 = Int64.MaxValue;
            Assert.AreEqual(sizeof(long), field.Size);

            field.ValueAsSingle = Single.MaxValue;
            Assert.AreEqual(sizeof(float), field.Size);

            field.ValueAsDouble = Double.MaxValue;
            Assert.AreEqual(sizeof(double), field.Size);

            string text = "Bear";
            byte[] data = Encoding.UTF8.GetBytes(text);
            field.ValueAsString = text;
            Assert.AreEqual(data.Length, field.Size);

            field.ValueAsByteArray = data;
            Assert.AreEqual(data.Length, field.Size);
        }

        [Test]
        public void Type()
        {
            Field field = new Field();
            Assert.AreEqual(FieldType.Undefined, field.Type);

            field.ValueAsInt16 = Int16.MaxValue;
            Assert.AreEqual(FieldType.Integral, field.Type);

            field.ValueAsInt32 = Int32.MaxValue;
            Assert.AreEqual(FieldType.Integral, field.Type);

            field.ValueAsInt64 = Int64.MaxValue;
            Assert.AreEqual(FieldType.Integral, field.Type);

            field.ValueAsSingle = Single.MaxValue;
            Assert.AreEqual(FieldType.FloatingPoint, field.Type);

            field.ValueAsDouble = Double.MaxValue;
            Assert.AreEqual(FieldType.FloatingPoint, field.Type);

            string text = "Bear";
            field.ValueAsString = text;
            Assert.AreEqual(FieldType.String, field.Type);

            field.ValueAsByteArray = Encoding.UTF8.GetBytes(text);
            Assert.AreEqual(FieldType.ByteArray, field.Type);
        }

        [Test]
        public void ValueAsByte()
        {
            Field field = new Field(1);
            field.ValueAsByte = Byte.MaxValue;
            Assert.AreEqual(Byte.MaxValue, field.ValueAsByte);
        }

        [Test]
        public void ValueAsInt16()
        {
            Field field = new Field(1);
            field.ValueAsInt16 = Int16.MaxValue;
            Assert.AreEqual(Byte.MaxValue, field.ValueAsByte);
            Assert.AreEqual(Int16.MaxValue, field.ValueAsInt16);
        }

        [Test]
        public void ValueAsInt32()
        {
            Field field = new Field(1);
            field.ValueAsInt32 = Int32.MaxValue;
            Assert.AreEqual(Byte.MaxValue, field.ValueAsByte);
            Assert.AreEqual(unchecked((short) UInt16.MaxValue), field.ValueAsInt16);
            Assert.AreEqual(Int32.MaxValue, field.ValueAsInt32);
        }

        [Test]
        public void ValueAsInt64()
        {
            Field field = new Field(1);
            field.ValueAsInt64 = Int64.MaxValue;
            Assert.AreEqual(Byte.MaxValue, field.ValueAsByte);
            Assert.AreEqual(unchecked((short) UInt16.MaxValue), field.ValueAsInt16);
            Assert.AreEqual(unchecked((int) UInt32.MaxValue), field.ValueAsInt32);
            Assert.AreEqual(Int64.MaxValue, field.ValueAsInt64);
        }

        [Test]
        public void ValueAsSingle()
        {
            Field field = new Field(1);
            field.ValueAsSingle = Single.MaxValue;
            Assert.AreEqual(Byte.MaxValue, field.ValueAsByte);
            Assert.AreEqual(unchecked((short) UInt16.MaxValue), field.ValueAsInt16);
            Assert.AreEqual(Single.MaxValue, field.ValueAsSingle);
            Assert.IsTrue(field.ValueAsInt32 > 0);
        }

        [Test]
        public void ValueAsDouble()
        {
            Field field = new Field(1);
            field.ValueAsDouble = Double.MaxValue;
            Assert.AreEqual(Byte.MaxValue, field.ValueAsByte);
            Assert.AreEqual(unchecked((short) UInt16.MaxValue), field.ValueAsInt16);
            Assert.AreEqual(unchecked((int) UInt32.MaxValue), field.ValueAsInt32);
            Assert.AreEqual(Single.NaN, field.ValueAsSingle);
            Assert.AreEqual(Double.MaxValue, field.ValueAsDouble);
            Assert.IsTrue(field.ValueAsInt64 > 0);
        }

        [Test]
        public void ValueAsString()
        {
            string text = "Bear";
            Field field = new Field(1);
            field.ValueAsString = text;
            Assert.AreEqual(text, field.ValueAsString);
            Assert.IsTrue(Utilities.Buffer.Equals(Encoding.UTF8.GetBytes(text), field.ValueAsByteArray));
        }

        [Test]
        public void ValueAsByteArray()
        {
            byte[] data = Encoding.UTF8.GetBytes("Bear");
            Field field = new Field(1);
            field.ValueAsByteArray = data;
            Assert.IsTrue(Utilities.Buffer.Equals(data, field.ValueAsByteArray));
        }

        [Test]
        public void Equals()
        {
            Field field1 = new Field(1, Int32.MaxValue);
            Field field2 = new Field(1, Int32.MaxValue);
            Field field3 = new Field(2, Int32.MinValue);

            Assert.AreEqual(true, field1 == field2);
            Assert.AreEqual(false, field1 == field3);
        }

        [Test]
        public void CreateInstance()
        {
            Field field = new Field();

            Assert.AreEqual(0, field.Id);
            Assert.AreEqual(FieldType.Undefined, field.Type);
            Assert.AreEqual(0, field.Size);
            Assert.AreEqual(FieldCategory.Undefined, field.Category);
            Assert.AreEqual(false, field.IsPrimitive);
            Assert.AreEqual(0, field.ValueAsByte);
            Assert.AreEqual(0, field.ValueAsInt16);
            Assert.AreEqual(0, field.ValueAsInt32);
            Assert.AreEqual(0, field.ValueAsInt64);
            Assert.AreEqual(0.0F, field.ValueAsSingle);
            Assert.AreEqual(0.0D, field.ValueAsDouble);
            Assert.AreEqual(null, field.ValueAsString);
            Assert.AreEqual(null, field.ValueAsByteArray);
        }

        [Test]
        public void CreateInstance_Id()
        {
            Field field = new Field(1);

            Assert.AreEqual(1, field.Id);
            Assert.AreEqual(FieldType.Undefined, field.Type);
            Assert.AreEqual(0, field.Size);
            Assert.AreEqual(FieldCategory.Undefined, field.Category);
            Assert.AreEqual(false, field.IsPrimitive);
            Assert.AreEqual(0, field.ValueAsByte);
            Assert.AreEqual(0, field.ValueAsInt16);
            Assert.AreEqual(0, field.ValueAsInt32);
            Assert.AreEqual(0, field.ValueAsInt64);
            Assert.AreEqual(0.0F, field.ValueAsSingle);
            Assert.AreEqual(0.0D, field.ValueAsDouble);
            Assert.AreEqual(null, field.ValueAsString);
            Assert.AreEqual(null, field.ValueAsByteArray);
        }

        [Test]
        public void CreateInstance_Id_Category()
        {
            Field field = new Field(1, FieldCategory.Manifest);

            Assert.AreEqual(1, field.Id);
            Assert.AreEqual(FieldType.Undefined, field.Type);
            Assert.AreEqual(0, field.Size);
            Assert.AreEqual(FieldCategory.Manifest, field.Category);
            Assert.AreEqual(false, field.IsPrimitive);
            Assert.AreEqual(0, field.ValueAsByte);
            Assert.AreEqual(0, field.ValueAsInt16);
            Assert.AreEqual(0, field.ValueAsInt32);
            Assert.AreEqual(0, field.ValueAsInt64);
            Assert.AreEqual(0.0F, field.ValueAsSingle);
            Assert.AreEqual(0.0D, field.ValueAsDouble);
            Assert.AreEqual(null, field.ValueAsString);
            Assert.AreEqual(null, field.ValueAsByteArray);
        }

        [Test]
        public unsafe void CreateInstance_Id_Value()
        {
            Field field = new Field(1, Byte.MaxValue);
            Assert.AreEqual(1, field.Id);
            Assert.AreEqual(FieldType.Integral, field.Type);
            Assert.AreEqual(sizeof(byte), field.Size);
            Assert.AreEqual(FieldCategory.Undefined, field.Category);
            Assert.AreEqual(true, field.IsPrimitive);
            Assert.AreEqual(Byte.MaxValue, field.ValueAsByte);

            field = new Field(1, Int16.MaxValue);
            Assert.AreEqual(1, field.Id);
            Assert.AreEqual(FieldType.Integral, field.Type);
            Assert.AreEqual(sizeof(short), field.Size);
            Assert.AreEqual(FieldCategory.Undefined, field.Category);
            Assert.AreEqual(true, field.IsPrimitive);
            Assert.AreEqual(Int16.MaxValue, field.ValueAsInt16);

            field = new Field(1, Int32.MaxValue);
            Assert.AreEqual(1, field.Id);
            Assert.AreEqual(FieldType.Integral, field.Type);
            Assert.AreEqual(sizeof(int), field.Size);
            Assert.AreEqual(FieldCategory.Undefined, field.Category);
            Assert.AreEqual(true, field.IsPrimitive);
            Assert.AreEqual(Int32.MaxValue, field.ValueAsInt32);

            field = new Field(1, Int64.MaxValue);
            Assert.AreEqual(1, field.Id);
            Assert.AreEqual(FieldType.Integral, field.Type);
            Assert.AreEqual(sizeof(long), field.Size);
            Assert.AreEqual(FieldCategory.Undefined, field.Category);
            Assert.AreEqual(true, field.IsPrimitive);
            Assert.AreEqual(Int64.MaxValue, field.ValueAsInt64);

            field = new Field(1, Single.MaxValue);
            Assert.AreEqual(1, field.Id);
            Assert.AreEqual(FieldType.FloatingPoint, field.Type);
            Assert.AreEqual(sizeof(float), field.Size);
            Assert.AreEqual(FieldCategory.Undefined, field.Category);
            Assert.AreEqual(true, field.IsPrimitive);
            Assert.AreEqual(Single.MaxValue, field.ValueAsSingle);

            field = new Field(1, Double.MaxValue);
            Assert.AreEqual(1, field.Id);
            Assert.AreEqual(FieldType.FloatingPoint, field.Type);
            Assert.AreEqual(sizeof(double), field.Size);
            Assert.AreEqual(FieldCategory.Undefined, field.Category);
            Assert.AreEqual(true, field.IsPrimitive);
            Assert.AreEqual(Double.MaxValue, field.ValueAsDouble);

            string text = "Bear";
            byte[] data = Encoding.UTF8.GetBytes(text);

            field = new Field(1, text);
            Assert.AreEqual(1, field.Id);
            Assert.AreEqual(FieldType.String, field.Type);
            Assert.AreEqual(data.Length, field.Size);
            Assert.AreEqual(FieldCategory.Undefined, field.Category);
            Assert.AreEqual(false, field.IsPrimitive);
            Assert.AreEqual(text, field.ValueAsString);

            field = new Field(1, data);
            Assert.AreEqual(1, field.Id);
            Assert.AreEqual(FieldType.ByteArray, field.Type);
            Assert.AreEqual(data.Length, field.Size);
            Assert.AreEqual(FieldCategory.Undefined, field.Category);
            Assert.AreEqual(false, field.IsPrimitive);
            Assert.IsTrue(Utilities.Buffer.Equals(data, field.ValueAsByteArray));
        }

        [Test]
        public unsafe void CreateInstance_Id_Value_Category()
        {
            Field field = new Field(1, Byte.MaxValue, FieldCategory.Header);
            Assert.AreEqual(1, field.Id);
            Assert.AreEqual(FieldType.Integral, field.Type);
            Assert.AreEqual(sizeof(byte), field.Size);
            Assert.AreEqual(FieldCategory.Header, field.Category);
            Assert.AreEqual(true, field.IsPrimitive);
            Assert.AreEqual(Byte.MaxValue, field.ValueAsByte);

            field = new Field(1, Int16.MaxValue, FieldCategory.Header);
            Assert.AreEqual(1, field.Id);
            Assert.AreEqual(FieldType.Integral, field.Type);
            Assert.AreEqual(sizeof(short), field.Size);
            Assert.AreEqual(FieldCategory.Header, field.Category);
            Assert.AreEqual(true, field.IsPrimitive);
            Assert.AreEqual(Int16.MaxValue, field.ValueAsInt16);

            field = new Field(1, Int32.MaxValue, FieldCategory.Header);
            Assert.AreEqual(1, field.Id);
            Assert.AreEqual(FieldType.Integral, field.Type);
            Assert.AreEqual(sizeof(int), field.Size);
            Assert.AreEqual(FieldCategory.Header, field.Category);
            Assert.AreEqual(true, field.IsPrimitive);
            Assert.AreEqual(Int32.MaxValue, field.ValueAsInt32);

            field = new Field(1, Int64.MaxValue, FieldCategory.Header);
            Assert.AreEqual(1, field.Id);
            Assert.AreEqual(FieldType.Integral, field.Type);
            Assert.AreEqual(sizeof(long), field.Size);
            Assert.AreEqual(FieldCategory.Header, field.Category);
            Assert.AreEqual(true, field.IsPrimitive);

            field = new Field(1, Single.MaxValue, FieldCategory.Header);
            Assert.AreEqual(1, field.Id);
            Assert.AreEqual(FieldType.FloatingPoint, field.Type);
            Assert.AreEqual(sizeof(float), field.Size);
            Assert.AreEqual(FieldCategory.Header, field.Category);
            Assert.AreEqual(true, field.IsPrimitive);
            Assert.AreEqual(Single.MaxValue, field.ValueAsSingle);

            field = new Field(1, Double.MaxValue, FieldCategory.Header);
            Assert.AreEqual(1, field.Id);
            Assert.AreEqual(FieldType.FloatingPoint, field.Type);
            Assert.AreEqual(sizeof(double), field.Size);
            Assert.AreEqual(FieldCategory.Header, field.Category);
            Assert.AreEqual(true, field.IsPrimitive);
            Assert.AreEqual(Double.MaxValue, field.ValueAsDouble);

            string text = "Bear";
            byte[] data = Encoding.UTF8.GetBytes(text);

            field = new Field(1, text, FieldCategory.Undefined);
            Assert.AreEqual(1, field.Id);
            Assert.AreEqual(FieldType.String, field.Type);
            Assert.AreEqual(data.Length, field.Size);
            Assert.AreEqual(FieldCategory.Undefined, field.Category);
            Assert.AreEqual(false, field.IsPrimitive);
            Assert.AreEqual(text, field.ValueAsString);

            field = new Field(1, data, FieldCategory.Undefined);
            Assert.AreEqual(1, field.Id);
            Assert.AreEqual(FieldType.ByteArray, field.Type);
            Assert.AreEqual(data.Length, field.Size);
            Assert.AreEqual(FieldCategory.Undefined, field.Category);
            Assert.AreEqual(false, field.IsPrimitive);
            Assert.IsTrue(Utilities.Buffer.Equals(data, field.ValueAsByteArray));
        }

        [Test]
        public void Clone()
        {
            Field field = new Field(1, Int32.MaxValue, FieldCategory.Header);
            Field clone = field.Clone();

            Assert.AreEqual(field.Id, clone.Id);
            Assert.AreEqual(field.Type, clone.Type);
            Assert.AreEqual(field.Size, clone.Size);
            Assert.AreEqual(field.Category, clone.Category);
            Assert.AreEqual(field.IsPrimitive, clone.IsPrimitive);

            //
            // clone a read-only field
            //
            field = Field.ReadOnly(field);
            clone = field.Clone();
            Assert.AreEqual(field.IsReadOnly, clone.IsReadOnly);
        }

        [Test]
        public unsafe void GetMetafield()
        {
            Metafield metafield = new Metafield(
                1, FieldType.Integral, sizeof(int), FieldCategory.Header);
            Field field = new Field(1, Int32.MaxValue, FieldCategory.Header);

            Assert.AreEqual(metafield, field.GetMetafield());
        }

        [Test]
        public unsafe void Match()
        {
            Metafield metafield = new Metafield(
                1, FieldType.Integral, sizeof(int), FieldCategory.Header);
            Field field = new Field(1, Int32.MaxValue, FieldCategory.Header);

            Assert.AreEqual(true, field.Match(metafield));
        }

        [Test]
        public void ReadOnly()
        {
            Field field = new Field(1, Int32.MaxValue, FieldCategory.Header);
            Field readOnlyField = Field.ReadOnly(field);

            Assert.AreEqual(true, readOnlyField .IsReadOnly);
            Assert.AreEqual(field.Id, readOnlyField .Id);
            Assert.AreEqual(field.Type, readOnlyField .Type);
            Assert.AreEqual(field.Size, readOnlyField .Size);
            Assert.AreEqual(field.Category, readOnlyField .Category);
            Assert.AreEqual(field.IsPrimitive, readOnlyField .IsPrimitive);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void SetCategory_ValueNotValid()
        {
            Field field = new Field();
            field.Category = FieldCategory.Header;
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetId_ValueOutOfRange()
        {
            int id = -1;
            Field field = new Field();

            try {
                field.Id = id;
            } catch (ArgumentOutOfRangeException) {
                id = Int32.MaxValue;
            }

            field.Id = id;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadOnly_NullMetafield()
        {
            Field.ReadOnly(null);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ValueAsByte_CannotConvert()
        {
            Field field = new Field(1, "Gorilla");
            byte value = field.ValueAsByte;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ValueAsInt16_CannotConvert()
        {
            Field field = new Field(1, "Gorilla");
            short value = field.ValueAsInt16;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ValueAsInt32_CannotConvert()
        {
            Field field = new Field(1, "Gorilla");
            int value = field.ValueAsInt32;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ValueAsInt64_CannotConvert()
        {
            Field field = new Field(1, "Gorilla");
            long value = field.ValueAsInt64;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ValueAsSingle_CannotConvert()
        {
            Field field = new Field(1, "Gorilla");
            float value = field.ValueAsSingle;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ValueAsDouble_CannotConvert()
        {
            Field field = new Field(1, "Gorilla");
            double value = field.ValueAsSingle;
        }
        #endregion unit test methods
    }
}
#endif
