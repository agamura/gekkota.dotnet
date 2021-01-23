//------------------------------------------------------------------------------
// <sourcefile name="MetafieldTest.cs" language="C#" begin="05/06/2004">
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
using System.Collections;
using Gekkota.Net;
using NUnit.Framework;

namespace Gekkota.Tests
{
    [TestFixture]
    public class MetafieldTest
    {
        #region service methods
        [SetUp]
        public void Init() {}

        [TearDown]
        public void Clean() {}
        #endregion service methods

        #region unit test methods
        [Test]
        public unsafe void Category()
        {
            Metafield metafield = new Metafield(1, FieldType.Integral, sizeof(int));
            metafield.Category = FieldCategory.Header;
            Assert.AreEqual(FieldCategory.Header, metafield.Category);
        }

        [Test]
        public unsafe void LayoutSize()
        {
            Assert.AreEqual(sizeof(FieldHeader), Metafield.LayoutSize);
        }

        [Test]
        public void Id()
        {
            Metafield metafield = new Metafield();
            metafield.Id = 1;
            Assert.AreEqual(1, metafield.Id);
        }

        [Test]
        public void IsReadOnly()
        {
            Metafield metafield = new Metafield();
            Assert.AreEqual(false, metafield.IsReadOnly);
        }

        [Test]
        public void IsPrimitive()
        {
            Metafield metafield = new Metafield();

            metafield.Type = FieldType.Integral;
            Assert.AreEqual(true, metafield.IsPrimitive);

            metafield.Type = FieldType.FloatingPoint;
            Assert.AreEqual(true, metafield.IsPrimitive);

            metafield.Type = FieldType.String;
            Assert.AreEqual(false, metafield.IsPrimitive);

            metafield.Type = FieldType.ByteArray;
            Assert.AreEqual(false, metafield.IsPrimitive);

            metafield.Type = FieldType.Undefined;
            Assert.AreEqual(false, metafield.IsPrimitive);
        }

        [Test]
        public unsafe void Size()
        {
            Metafield metafield = new Metafield();
            Assert.AreEqual(0, metafield.Size);

            metafield.Type = FieldType.Integral;
            metafield.Size = sizeof(byte);
            Assert.AreEqual(sizeof(byte), metafield.Size);

            metafield.Size = sizeof(short);
            Assert.AreEqual(sizeof(short), metafield.Size);

            metafield.Size = sizeof(int);
            Assert.AreEqual(sizeof(int), metafield.Size);

            metafield.Size = sizeof(long);
            Assert.AreEqual(sizeof(long), metafield.Size);

            metafield.Type = FieldType.FloatingPoint;
            metafield.Size = sizeof(float);
            Assert.AreEqual(sizeof(float), metafield.Size);

            metafield.Size = sizeof(double);
            Assert.AreEqual(sizeof(double), metafield.Size);

            metafield.Type = FieldType.String;
            Assert.AreEqual(UInt16.MaxValue, metafield.Size);
            metafield.Size = 0;
            Assert.AreEqual(0, metafield.Size);

            metafield.Type = FieldType.ByteArray;
            Assert.AreEqual(UInt16.MaxValue, metafield.Size);
            metafield.Size = 0;
            Assert.AreEqual(0, metafield.Size);
        }

        [Test]
        public void Type()
        {
            Metafield metafield = new Metafield();

            metafield.Type = FieldType.Integral;
            Assert.AreEqual(FieldType.Integral, metafield.Type);

            metafield.Type = FieldType.FloatingPoint;
            Assert.AreEqual(FieldType.FloatingPoint, metafield.Type);

            metafield.Type = FieldType.String;
            Assert.AreEqual(FieldType.String, metafield.Type);

            metafield.Type = FieldType.ByteArray;
            Assert.AreEqual(FieldType.ByteArray, metafield.Type);
        }

        [Test]
        public void Equals()
        {
            Metafield metafield1 = new Metafield(1, FieldType.Integral, 4);
            Metafield metafield2 = new Metafield(1, FieldType.Integral, 4);
            Metafield metafield3 = new Metafield(2, FieldType.Integral, 4);

            Assert.AreEqual(true, metafield1 == metafield2);
            Assert.AreEqual(false, metafield1 == metafield3);

            metafield1 = new Metafield(1, FieldType.ByteArray, 10);
            metafield2 = new Metafield(1, FieldType.ByteArray, 20);
            metafield3 = new Metafield(1, FieldType.ByteArray);

            Assert.AreEqual(false, metafield1 == metafield2);
            Assert.AreEqual(true, metafield1 == metafield3);
        }

        [Test]
        public void CreateInstance()
        {
            Metafield metafield = new Metafield();

            Assert.AreEqual(0, metafield.Id);
            Assert.AreEqual(FieldType.Undefined, metafield.Type);
            Assert.AreEqual(0, metafield.Size);
            Assert.AreEqual(false, metafield.IsPrimitive);
            Assert.AreEqual(FieldCategory.Undefined, metafield.Category);
        }

        [Test]
        public void CreateInstance_Id()
        {
            Metafield metafield = new Metafield(1);

            Assert.AreEqual(1, metafield.Id);
            Assert.AreEqual(FieldType.Undefined, metafield.Type);
            Assert.AreEqual(0, metafield.Size);
            Assert.AreEqual(false, metafield.IsPrimitive);
            Assert.AreEqual(FieldCategory.Undefined, metafield.Category);
        }

        [Test]
        public unsafe void CreateInstance_Id_Type()
        {
            Metafield metafield = new Metafield(1, FieldType.Undefined);
            Assert.AreEqual(1, metafield.Id);
            Assert.AreEqual(FieldType.Undefined, metafield.Type);
            Assert.AreEqual(0, metafield.Size);
            Assert.AreEqual(FieldCategory.Undefined, metafield.Category);
            Assert.AreEqual(false, metafield.IsPrimitive);

            metafield = new Metafield(1, FieldType.Integral);
            Assert.AreEqual(1, metafield.Id);
            Assert.AreEqual(FieldType.Integral, metafield.Type);
            Assert.AreEqual(sizeof(int), metafield.Size);
            Assert.AreEqual(FieldCategory.Undefined, metafield.Category);
            Assert.AreEqual(true, metafield.IsPrimitive);

            metafield = new Metafield(1, FieldType.String);
            Assert.AreEqual(1, metafield.Id);
            Assert.AreEqual(FieldType.String, metafield.Type);
            Assert.AreEqual(UInt16.MaxValue, metafield.Size);
            Assert.AreEqual(FieldCategory.Undefined, metafield.Category);
            Assert.AreEqual(false, metafield.IsPrimitive);

            metafield = new Metafield(1, FieldType.ByteArray);
            Assert.AreEqual(1, metafield.Id);
            Assert.AreEqual(FieldType.ByteArray, metafield.Type);
            Assert.AreEqual(UInt16.MaxValue, metafield.Size);
            Assert.AreEqual(FieldCategory.Undefined, metafield.Category);
            Assert.AreEqual(false, metafield.IsPrimitive);
        }

        [Test]
        public unsafe void CreateInstance_Id_Type_Size()
        {
            Metafield metafield = new Metafield(1, FieldType.Undefined, 0);
            Assert.AreEqual(1, metafield.Id);
            Assert.AreEqual(FieldType.Undefined, metafield.Type);
            Assert.AreEqual(0, metafield.Size);
            Assert.AreEqual(FieldCategory.Undefined, metafield.Category);
            Assert.AreEqual(false, metafield.IsPrimitive);

            metafield = new Metafield(1, FieldType.Integral, sizeof(int));
            Assert.AreEqual(1, metafield.Id);
            Assert.AreEqual(FieldType.Integral, metafield.Type);
            Assert.AreEqual(sizeof(int), metafield.Size);
            Assert.AreEqual(FieldCategory.Undefined, metafield.Category);
            Assert.AreEqual(true, metafield.IsPrimitive);

            metafield = new Metafield(1, FieldType.Integral, sizeof(byte));
            Assert.AreEqual(1, metafield.Id);
            Assert.AreEqual(FieldType.Integral, metafield.Type);
            Assert.AreEqual(sizeof(byte), metafield.Size);
            Assert.AreEqual(FieldCategory.Undefined, metafield.Category);
            Assert.AreEqual(true, metafield.IsPrimitive);

            metafield = new Metafield(1, FieldType.Integral, sizeof(short));
            Assert.AreEqual(1, metafield.Id);
            Assert.AreEqual(FieldType.Integral, metafield.Type);
            Assert.AreEqual(sizeof(short), metafield.Size);
            Assert.AreEqual(FieldCategory.Undefined, metafield.Category);
            Assert.AreEqual(true, metafield.IsPrimitive);

            metafield = new Metafield(1, FieldType.Integral, sizeof(long));
            Assert.AreEqual(1, metafield.Id);
            Assert.AreEqual(FieldType.Integral, metafield.Type);
            Assert.AreEqual(sizeof(long), metafield.Size);
            Assert.AreEqual(FieldCategory.Undefined, metafield.Category);
            Assert.AreEqual(true, metafield.IsPrimitive);

            metafield = new Metafield(1, FieldType.FloatingPoint, sizeof(float));
            Assert.AreEqual(1, metafield.Id);
            Assert.AreEqual(FieldType.FloatingPoint, metafield.Type);
            Assert.AreEqual(sizeof(float), metafield.Size);
            Assert.AreEqual(FieldCategory.Undefined, metafield.Category);
            Assert.AreEqual(true, metafield.IsPrimitive);

            metafield = new Metafield(1, FieldType.FloatingPoint, sizeof(double));
            Assert.AreEqual(1, metafield.Id);
            Assert.AreEqual(FieldType.FloatingPoint, metafield.Type);
            Assert.AreEqual(sizeof(double), metafield.Size);
            Assert.AreEqual(FieldCategory.Undefined, metafield.Category);
            Assert.AreEqual(true, metafield.IsPrimitive);

            metafield = new Metafield(1, FieldType.String, 0);
            Assert.AreEqual(1, metafield.Id);
            Assert.AreEqual(FieldType.String, metafield.Type);
            Assert.AreEqual(0, metafield.Size);
            Assert.AreEqual(FieldCategory.Undefined, metafield.Category);
            Assert.AreEqual(false, metafield.IsPrimitive);

            metafield = new Metafield(1, FieldType.String, 2 * sizeof(char));
            Assert.AreEqual(1, metafield.Id);
            Assert.AreEqual(FieldType.String, metafield.Type);
            Assert.AreEqual(2 * sizeof(char), metafield.Size);
            Assert.AreEqual(FieldCategory.Undefined, metafield.Category);
            Assert.AreEqual(false, metafield.IsPrimitive);

            metafield = new Metafield(1, FieldType.ByteArray, 0);
            Assert.AreEqual(1, metafield.Id);
            Assert.AreEqual(FieldType.ByteArray, metafield.Type);
            Assert.AreEqual(0, metafield.Size);
            Assert.AreEqual(FieldCategory.Undefined, metafield.Category);
            Assert.AreEqual(false, metafield.IsPrimitive);

            metafield = new Metafield(1, FieldType.ByteArray, 2 * sizeof(byte));
            Assert.AreEqual(1, metafield.Id);
            Assert.AreEqual(FieldType.ByteArray, metafield.Type);
            Assert.AreEqual(2 * sizeof(byte), metafield.Size);
            Assert.AreEqual(FieldCategory.Undefined, metafield.Category);
            Assert.AreEqual(false, metafield.IsPrimitive);
        }

        [Test]
        public unsafe void CreateInstance_Id_Type_Size_Category()
        {
            Metafield metafield = new Metafield(
                1, FieldType.Undefined, 0, FieldCategory.Undefined);
            Assert.AreEqual(1, metafield.Id);
            Assert.AreEqual(FieldType.Undefined, metafield.Type);
            Assert.AreEqual(0, metafield.Size);
            Assert.AreEqual(FieldCategory.Undefined, metafield.Category);
            Assert.AreEqual(false, metafield.IsPrimitive);

            metafield = new Metafield(
                1, FieldType.Integral, sizeof(int), FieldCategory.Header);
            Assert.AreEqual(1, metafield.Id);
            Assert.AreEqual(FieldType.Integral, metafield.Type);
            Assert.AreEqual(sizeof(int), metafield.Size);
            Assert.AreEqual(FieldCategory.Header, metafield.Category);
            Assert.AreEqual(true, metafield.IsPrimitive);

            metafield = new Metafield(
                1, FieldType.Integral, sizeof(byte), FieldCategory.Header);
            Assert.AreEqual(1, metafield.Id);
            Assert.AreEqual(FieldType.Integral, metafield.Type);
            Assert.AreEqual(sizeof(byte), metafield.Size);
            Assert.AreEqual(FieldCategory.Header, metafield.Category);
            Assert.AreEqual(true, metafield.IsPrimitive);

            metafield = new Metafield(
                1, FieldType.Integral, sizeof(short), FieldCategory.Header);
            Assert.AreEqual(1, metafield.Id);
            Assert.AreEqual(FieldType.Integral, metafield.Type);
            Assert.AreEqual(sizeof(short), metafield.Size);
            Assert.AreEqual(FieldCategory.Header, metafield.Category);
            Assert.AreEqual(true, metafield.IsPrimitive);

            metafield = new Metafield(
                1, FieldType.Integral, sizeof(long), FieldCategory.Header);
            Assert.AreEqual(1, metafield.Id);
            Assert.AreEqual(FieldType.Integral, metafield.Type);
            Assert.AreEqual(sizeof(long), metafield.Size);
            Assert.AreEqual(FieldCategory.Header, metafield.Category);
            Assert.AreEqual(true, metafield.IsPrimitive);

            metafield = new Metafield(
                1, FieldType.FloatingPoint, sizeof(float), FieldCategory.Header);
            Assert.AreEqual(1, metafield.Id);
            Assert.AreEqual(FieldType.FloatingPoint, metafield.Type);
            Assert.AreEqual(sizeof(float), metafield.Size);
            Assert.AreEqual(FieldCategory.Header, metafield.Category);
            Assert.AreEqual(true, metafield.IsPrimitive);

            metafield = new Metafield(
                1, FieldType.FloatingPoint, sizeof(double), FieldCategory.Header);
            Assert.AreEqual(1, metafield.Id);
            Assert.AreEqual(FieldType.FloatingPoint, metafield.Type);
            Assert.AreEqual(sizeof(double), metafield.Size);
            Assert.AreEqual(FieldCategory.Header, metafield.Category);
            Assert.AreEqual(true, metafield.IsPrimitive);

            metafield = new Metafield(
                1, FieldType.String, 0, FieldCategory.Undefined);
            Assert.AreEqual(1, metafield.Id);
            Assert.AreEqual(FieldType.String, metafield.Type);
            Assert.AreEqual(0, metafield.Size);
            Assert.AreEqual(FieldCategory.Undefined, metafield.Category);
            Assert.AreEqual(false, metafield.IsPrimitive);

            metafield = new Metafield(
                1, FieldType.String, 2 * sizeof(char), FieldCategory.Header);
            Assert.AreEqual(1, metafield.Id);
            Assert.AreEqual(FieldType.String, metafield.Type);
            Assert.AreEqual(2 * sizeof(char), metafield.Size);
            Assert.AreEqual(FieldCategory.Header, metafield.Category);
            Assert.AreEqual(false, metafield.IsPrimitive);

            metafield = new Metafield(
                1, FieldType.ByteArray, 0, FieldCategory.Undefined);
            Assert.AreEqual(1, metafield.Id);
            Assert.AreEqual(FieldType.ByteArray, metafield.Type);
            Assert.AreEqual(0, metafield.Size);
            Assert.AreEqual(FieldCategory.Undefined, metafield.Category);
            Assert.AreEqual(false, metafield.IsPrimitive);

            metafield = new Metafield(
                1, FieldType.ByteArray, 2 * sizeof(byte), FieldCategory.Header);
            Assert.AreEqual(1, metafield.Id);
            Assert.AreEqual(FieldType.ByteArray, metafield.Type);
            Assert.AreEqual(2 * sizeof(byte), metafield.Size);
            Assert.AreEqual(FieldCategory.Header, metafield.Category);
            Assert.AreEqual(false, metafield.IsPrimitive);
        }

        [Test]
        public unsafe void Clone()
        {
            Metafield metafield = new Metafield(
                1, FieldType.Integral, sizeof(int), FieldCategory.Header);
            Metafield clone = metafield.Clone();

            Assert.AreEqual(metafield.Id, clone.Id);
            Assert.AreEqual(metafield.Type, clone.Type);
            Assert.AreEqual(metafield.Size, clone.Size);
            Assert.AreEqual(metafield.Category, clone.Category);
            Assert.AreEqual(metafield.IsPrimitive, clone.IsPrimitive);

            //
            // clone a read-only metafield
            //
            metafield = Metafield.ReadOnly(metafield);
            clone = metafield.Clone();
            Assert.AreEqual(metafield.IsReadOnly, clone.IsReadOnly);
        }

        [Test]
        public unsafe void ReadOnly()
        {
            Metafield metafield = new Metafield(
                1, FieldType.Integral, sizeof(int), FieldCategory.Header);
            Metafield readOnlyMetafield = Metafield.ReadOnly(metafield);

            Assert.AreEqual(true, readOnlyMetafield.IsReadOnly);
            Assert.AreEqual(metafield.Id, readOnlyMetafield .Id);
            Assert.AreEqual(metafield.Type, readOnlyMetafield .Type);
            Assert.AreEqual(metafield.Size, readOnlyMetafield .Size);
            Assert.AreEqual(metafield.Category, readOnlyMetafield .Category);
            Assert.AreEqual(metafield.IsPrimitive, readOnlyMetafield .IsPrimitive);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void SetCategory_ValueNotValid()
        {
            Metafield metafield = null;

            try {
                metafield = new Metafield(1, FieldType.String, 0, FieldCategory.Header);
            } catch (ArgumentException) {
                metafield = new Metafield(1, FieldType.ByteArray, 0, FieldCategory.Header);
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetId_ValueOutOfRange()
        {
            int id = -1;
            Metafield metafield = new Metafield();

            try {
                metafield.Id = id;
            } catch (ArgumentOutOfRangeException) {
                id = Int32.MaxValue;
            }

            metafield.Id = id;
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void SetSize_ValueNotValid()
        {
            int size = -1;
            Metafield metafield = new Metafield(1, FieldType.Integral);

            try {
                metafield.Size = size;
            } catch (ArgumentException) {
                size = 0;
            }

            try {
                metafield.Size = size;
            } catch (ArgumentException) {
                size = 3;
            }

            try {
                metafield.Size = size;
            } catch (ArgumentException) {
                size = 5;
            }

            try {
                metafield.Size = size;
            } catch (ArgumentException) {
                size = 7;
            }

            try {
                metafield.Size = size;
            } catch (ArgumentException) {
                size = 9;
            }

            metafield.Size = size;
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetSize_ValueOutOfRange()
        {
            int size = -1;
            Metafield metafield = new Metafield(1, FieldType.ByteArray);

            try {
                metafield.Size = size;
            } catch (ArgumentException) {
                size = Int32.MaxValue;
            }

            metafield.Size = size;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadOnly_NullMetafield()
        {
            Metafield.ReadOnly(null);
        }
        #endregion unit test methods
    }
}
#endif
