//------------------------------------------------------------------------------
// <sourcefile name="DatagramTest.cs" language="C#" begin="05/07/2004">
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
using NUnit.Framework;

namespace Gekkota.Tests
{
    [TestFixture]
    public class DatagramTest
    {
        #region service methods
        [SetUp]
        public void Init() {}

        [TearDown]
        public void Clean() {}
        #endregion service methods

        #region unit test methods
        [Test]
        public void Item()
        {
            Field[] fields = {
                new Field(1, Byte.MaxValue),
                new Field(2, Int16.MaxValue),
                new Field(3, Int32.MaxValue),
                new Field(4, Int64.MaxValue),
                new Field(5, Single.MaxValue),
                new Field(6, Double.MaxValue)
            };

            Datagram datagram = new Datagram();

            for (int i = 0; i < fields.Length; i++) {
                datagram.Add(new Field());
                datagram[i] = fields[i];
                Assert.AreEqual(fields[i], datagram[i]);
            }
        }

        [Test]
        public void Size()
        {
            string text = "Bear";
            byte[] data = Encoding.UTF8.GetBytes(text);

            Field[] fields = {
                new Field(),
                new Field(1, Byte.MaxValue),
                new Field(2, Int16.MaxValue),
                new Field(3, Int32.MaxValue),
                new Field(4, Int64.MaxValue),
                new Field(5, Single.MaxValue),
                new Field(6, Double.MaxValue),
                new Field(7, text),
                new Field(8, data)
            };

            int size = 0;
            Datagram datagram = new Datagram();

            foreach (Field field in fields) {
                size += field.Size;
                datagram.Add(field);
            }

            Assert.AreEqual(size, datagram.Size);

            size -= datagram[1].Size;
            datagram[1] = new Field(9, Int64.MaxValue);
            size += datagram[1].Size;
            Assert.AreEqual(size, datagram.Size);
        }

        [Test]
        public void Add()
        {
            Field[] fields = {
                new Field(1, Byte.MaxValue),
                new Field(2, Int16.MaxValue),
                new Field(3, Int32.MaxValue),
                new Field(4, Int64.MaxValue),
                new Field(5, Single.MaxValue),
                new Field(6, Double.MaxValue)
            };

            Datagram datagram = new Datagram();

            foreach (Field field in fields) {
                datagram.Add(field);
            }

            Assert.AreEqual(fields.Length, datagram.Count);
            for (int i = 0; i < fields.Length; i++) {
                Assert.AreEqual(fields[i], datagram[i]);
            }
        }

        [Test]
        public void Clone()
        {
            //
            // clone an empty datagram
            //
            Datagram datagram = new Datagram();
            Datagram clone = datagram.Clone();
            
            Assert.AreEqual(0, clone.Size);

            //
            // clone a non-empty datagram
            //
            Field[] fields = {
                new Field(1, Byte.MaxValue),
                new Field(2, Int16.MaxValue),
                new Field(3, Int32.MaxValue),
                new Field(4, Int64.MaxValue),
                new Field(5, Single.MaxValue),
                new Field(6, Double.MaxValue)
            };

            foreach (Field field in fields) {
                datagram.Add(field);
            }

            clone = datagram.Clone();

            Assert.AreEqual(datagram.Size, clone.Size);
            for (int i = 0; i < datagram.Count; i++) {
                Assert.AreEqual(datagram[i], clone[i]);
            }

            //
            // clone a fixed-size, read-only, synchronized datagram
            //
            datagram = Datagram.ReadOnly(datagram);
            datagram = Datagram.Synchronized(datagram);
            clone = datagram.Clone();
            Assert.AreEqual(datagram.IsReadOnly, clone.IsReadOnly);
            Assert.AreEqual(datagram.IsFixedSize, clone.IsFixedSize);
            Assert.AreEqual(datagram.IsSynchronized, clone.IsSynchronized);
        }

        [Test]
        public void Contains()
        {
            Field[] fields = {
                new Field(1, Byte.MaxValue),
                new Field(2, Int16.MaxValue),
                new Field(3, Int32.MaxValue),
                new Field(4, Int64.MaxValue),
                new Field(5, Single.MaxValue),
                new Field(6, Double.MaxValue)
            };

            Datagram datagram = new Datagram();

            foreach (Field field in fields) {
                datagram.Add(field);
            }

            foreach (Field field in fields) {
                Assert.AreEqual(true, datagram.Contains(field.GetMetafield()));
            }   Assert.AreEqual(false, datagram.Contains(new Field().GetMetafield()));
        }

        [Test]
        public void Equals()
        {
            Field[] fields1 = {
                new Field(1, Byte.MaxValue),
                new Field(2, Int16.MaxValue),
                new Field(3, Int32.MaxValue),
                new Field(4, Int64.MaxValue),
                new Field(5, Single.MaxValue),
                new Field(6, Double.MaxValue),
                new Field(7, "Gorilla"),
                new Field(8, Encoding.UTF8.GetBytes("Bear"))
            };

            Field[] fields2 = {
                new Field(1, Byte.MaxValue),
                new Field(2, Int16.MaxValue),
                new Field(3, Int32.MaxValue),
                new Field(4, Int64.MaxValue),
                new Field(5, Single.MaxValue),
                new Field(6, Double.MaxValue),
                new Field(7, "Gorilla"),
                new Field(8, Encoding.UTF8.GetBytes("Bear"))
            };

            Field[] fields3 = {
                new Field(1, Byte.MaxValue),
                new Field(2, Int16.MaxValue),
                new Field(3, Int32.MaxValue),
                new Field(4, Int64.MaxValue),
                new Field(5, Single.MaxValue),
                new Field(6, Double.MaxValue),
            };

            Datagram datagram1 = new Datagram();

            foreach (Field field in fields1) {
                datagram1.Add(field);
            }

            Datagram datagram2 = new Datagram();

            foreach (Field field in fields2) {
                datagram2.Add(field);
            }

            Datagram datagram3 = new Datagram();

            foreach (Field field in fields3) {
                datagram3.Add(field);
            }

            Assert.AreEqual(true, datagram1 == datagram2);
            Assert.AreEqual(false, datagram1 == datagram3);
            Assert.AreEqual(false, datagram1 == new Datagram());
        }

        [Test]
        public void FixedSize()
        {
            Field[] fields = {
                new Field(1, Byte.MaxValue),
                new Field(2, Int16.MaxValue),
                new Field(3, Int32.MaxValue),
                new Field(4, Int64.MaxValue),
                new Field(5, Single.MaxValue),
                new Field(6, Double.MaxValue)
            };

            Datagram datagram = new Datagram();

            foreach (Field field in fields) {
                datagram.Add(field);
            }

            datagram = Datagram.Synchronized(datagram);
            Datagram fixedSizeDatagram = Datagram.FixedSize(datagram);

            Assert.AreEqual(true, fixedSizeDatagram.IsFixedSize);
            Assert.AreEqual(datagram.IsReadOnly, fixedSizeDatagram.IsReadOnly);
            Assert.AreEqual(datagram.IsSynchronized, fixedSizeDatagram.IsSynchronized);
            Assert.AreEqual(datagram.Count, fixedSizeDatagram.Count);
            Assert.AreEqual(datagram.SyncRoot, fixedSizeDatagram.SyncRoot);

            Field newField = new Field(7, "Gorilla");

            for (int i = 0; i < datagram.Count; i++) {
                Assert.AreEqual(datagram[i], fixedSizeDatagram[i]);
                fixedSizeDatagram[i] = newField;
            }
        }

        [Test]
        public void GetEnumerator()
        {
            Field[] fields = {
                new Field(1, Byte.MaxValue),
                new Field(2, Int16.MaxValue),
                new Field(3, Int32.MaxValue),
                new Field(4, Int64.MaxValue),
                new Field(5, Single.MaxValue),
                new Field(6, Double.MaxValue)
            };

            Datagram datagram = new Datagram();

            foreach (Field field in fields) {
                datagram.Add(field);
            }

            IEnumerator enumerator = datagram.GetEnumerator();

            int i = 0;
            while (enumerator.MoveNext()) {
                Assert.AreEqual(fields[i++], enumerator.Current);
            }
        }

        [Test]
        public void IndexOf()
        {
            Field[] fields = {
                new Field(1, Byte.MaxValue),
                new Field(2, Int16.MaxValue),
                new Field(3, Int32.MaxValue),
                new Field(4, Int64.MaxValue),
                new Field(5, Single.MaxValue),
                new Field(6, Double.MaxValue)
            };

            Datagram datagram = new Datagram();

            foreach (Field field in fields) {
                datagram.Add(field);
            }

            for (int i = 0; i < fields.Length; i++) {
                Assert.AreEqual(i, datagram.IndexOf(fields[i].GetMetafield()));
            }
        }

        [Test]
        public void Insert()
        {
            Field[] fields = {
                new Field(1, Byte.MaxValue),
                new Field(2, Int16.MaxValue),
                new Field(3, Int32.MaxValue),
                new Field(4, Int64.MaxValue),
                new Field(5, Single.MaxValue),
                new Field(6, Double.MaxValue)
            };

            Datagram datagram = new Datagram();

            datagram.Insert(0, fields[0]); // insert field "1" at position 0
            datagram.Insert(1, fields[2]); // insert field "3" at position 1
            datagram.Insert(1, fields[1]); // insert field "2" at position 1
            datagram.Insert(3, fields[3]); // insert field "4" at position 3
            datagram.Insert(4, fields[4]); // insert field "5" at position 4
            datagram.Insert(5, fields[5]); // insert field "6" at position 5
            
            for (int i = 0; i < fields.Length; i++) {
                Assert.AreEqual(fields[i], datagram[i]);
            }
        }

        [Test]
        public void ReadOnly()
        {
            Field[] fields = {
                new Field(1, Byte.MaxValue),
                new Field(2, Int16.MaxValue),
                new Field(3, Int32.MaxValue),
                new Field(4, Int64.MaxValue),
                new Field(5, Single.MaxValue),
                new Field(6, Double.MaxValue)
            };

            Datagram datagram = new Datagram();

            foreach (Field field in fields) {
                datagram.Add(field);
            }

            datagram = Datagram.Synchronized(datagram);
            Datagram readOnlyDatagram = Datagram.ReadOnly(datagram);

            Assert.AreEqual(true, readOnlyDatagram.IsReadOnly);
            Assert.AreEqual(true, readOnlyDatagram.IsFixedSize);
            Assert.AreEqual(datagram.IsSynchronized, readOnlyDatagram.IsSynchronized);
            Assert.AreEqual(datagram.Count, readOnlyDatagram.Count);
            Assert.AreEqual(datagram.SyncRoot, readOnlyDatagram.SyncRoot);

            for (int i = 0; i < datagram.Count; i++) {
                Assert.AreEqual(datagram[i], readOnlyDatagram[i]);
            }
        }

        [Test]
        public void Remove()
        {
            Field[] fields = {
                new Field(1, Byte.MaxValue),
                new Field(2, Int16.MaxValue),
                new Field(3, Int32.MaxValue),
                new Field(4, Int64.MaxValue),
                new Field(5, Single.MaxValue),
                new Field(6, Double.MaxValue)
            };

            int count = fields.Length;
            Datagram datagram = new Datagram();

            foreach (Field field in fields) {
                datagram.Add(field);
            }

            //
            // remove field "3"
            //
            datagram.Remove(fields[2].GetMetafield());
            Assert.AreEqual(--count, datagram.Count);
            Assert.AreEqual(false, datagram.Contains(fields[2].GetMetafield()));

            //
            // remove field "1" (head)
            //
            datagram.Remove(fields[0].GetMetafield());
            Assert.AreEqual(--count, datagram.Count);
            Assert.AreEqual(false, datagram.Contains(fields[0].GetMetafield()));

            //
            // remove field "6" (tail)
            //
            datagram.Remove(fields[5].GetMetafield());
            Assert.AreEqual(--count, datagram.Count);
            Assert.AreEqual(false, datagram.Contains(fields[5].GetMetafield()));

            //
            // fields "2", "4", and "5" should still be there
            //
            Assert.AreEqual(true, datagram.Contains(fields[1].GetMetafield()));
            Assert.AreEqual(true, datagram.Contains(fields[3].GetMetafield()));
            Assert.AreEqual(true, datagram.Contains(fields[4].GetMetafield()));
        }

        [Test]
        public void Synchronized()
        {
            Field[] fields = {
                new Field(1, Byte.MaxValue),
                new Field(2, Int16.MaxValue),
                new Field(3, Int32.MaxValue),
                new Field(4, Int64.MaxValue),
                new Field(5, Single.MaxValue),
                new Field(6, Double.MaxValue)
            };

            Datagram datagram = new Datagram();

            foreach (Field field in fields) {
                datagram.Add(field);
            }

            datagram = Datagram.FixedSize(datagram);
            Datagram syncDatagram = Datagram.Synchronized(datagram);

            Assert.AreEqual(true, syncDatagram.IsSynchronized);
            Assert.AreEqual(datagram.IsFixedSize, syncDatagram.IsFixedSize);
            Assert.AreEqual(datagram.IsReadOnly, syncDatagram.IsReadOnly);
            Assert.AreEqual(datagram.Count, syncDatagram.Count);
            Assert.AreEqual(datagram.SyncRoot, syncDatagram.SyncRoot);

            Field newField = new Field(7, "Gorilla");

            for (int i = 0; i < datagram.Count; i++) {
                Assert.AreEqual(datagram[i], syncDatagram[i]);
                syncDatagram[i] = newField;
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetItem_EmptyDatagram()
        {
            Datagram datagram = new Datagram();
            Field field = datagram[new Metafield(1, FieldType.Integral)];
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetItem_EmptyDatagram()
        {
            Datagram datagram = new Datagram();
            datagram[new Metafield(1, FieldType.Integral)] = new Field();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetItem_NullMetafield()
        {
            Datagram datagram = new Datagram();
            Field field = datagram[(Metafield) null];
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetItem_NullMetafield()
        {
            Datagram datagram = new Datagram();
            datagram[(Metafield) null] = new Field();
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetItem_InvalidMetafield()
        {
            Datagram datagram = new Datagram();
            datagram.Add(new Field(1, Int32.MaxValue));
            Field field = datagram[new Metafield(2, FieldType.Integral)];
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void SetItem_InvalidMetafield()
        {
            Datagram datagram = new Datagram();
            datagram.Add(new Field(1, Int32.MaxValue));
            datagram[new Metafield(2, FieldType.Integral)] = new Field(2, Byte.MaxValue);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetItem_NullField()
        {
            Datagram datagram = new Datagram();
            Field field = new Field(1, Int32.MaxValue);
            datagram.Add(field);
            datagram[field.GetMetafield()] = null;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetItemByIndex_NullField()
        {
            Datagram datagram = new Datagram();
            datagram.Add(new Field());
            datagram[0] = null;
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void SetItem_ReadOnlyDatagram()
        {
            Datagram datagram = new Datagram();
            datagram.Add(new Field());

            datagram = Datagram.ReadOnly(datagram);
            datagram[0] = new Field();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FixedSize_NullDatagram()
        {
            Datagram.FixedSize(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Insert_NullField()
        {
            Datagram datagram = new Datagram();
            datagram.Insert(0, null);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Insert_FixedSizeDatagram()
        {
            Datagram datagram = new Datagram();
            datagram.Add(new Field());

            datagram = Datagram.FixedSize(datagram);
            datagram.Insert(0, new Field());
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Insert_ReadOnlyDatagram()
        {
            Datagram datagram = new Datagram();
            datagram.Add(new Field());

            datagram = Datagram.ReadOnly(datagram);
            datagram.Insert(0, new Field());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadOnly_NullDatagram()
        {
            Datagram.ReadOnly(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Remove_NullMetafield()
        {
            Datagram datagram = new Datagram();
            datagram.Add(new Field());
            datagram.Remove((Metafield) null);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Remove_FixedSizeDatagram()
        {
            Datagram datagram = new Datagram();

            Field field = new Field();
            datagram.Add(field);

            datagram = Datagram.FixedSize(datagram);
            datagram.Remove(field.GetMetafield());
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Remove_ReadOnlyDatagram()
        {
            Datagram datagram = new Datagram();

            Field field = new Field();
            datagram.Add(field);

            datagram = Datagram.ReadOnly(datagram);
            datagram.Remove(field.GetMetafield());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Synchronized_NullDatagram()
        {
            Datagram.Synchronized(null);
        }
        #endregion unit test methods
    }
}
#endif
