//------------------------------------------------------------------------------
// <sourcefile name="Datagram.cs" language="C#" begin="06/02/2003">
//
//     <author name="Giuseppe Greco" email="giuseppe.greco@agamura.com" />
//
//     <copyright company="Agamura" url="http://www.agamura.com">
//         Copyright (C) 2003 Agamura, Inc.  All rights reserved.
//     </copyright>
//
// </sourcefile>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Gekkota.Collections;
using Gekkota.Properties;

namespace Gekkota.Net
{
    [Serializable]
    public class Datagram : Collections.LinkedList<Field>
    {
        #region private fields
        private int size;
        #endregion private fields

        #region public properties
        public new virtual Field this[int index]
        {
            get { return (Field) base[index]; }
            set { base[index] = value; }
        }

        public virtual Field this[Metafield metafield]
        {
            get {
                if (Count == 0) {
                    throw new InvalidOperationException(
                        Resources.Error_DatagramEmpty);
                }

                int index = 0;
                Node node = GetNode(metafield, out index);

                if (node == null) {
                    throw new ArgumentException(String.Format(
                        Resources.Error_FieldDoesNotExist, metafield.Id));
                }

                return (Field) node.Value;
            }

            set {
                OnValidate(value);

                if (Count == 0) {
                    throw new InvalidOperationException(
                        Resources.Error_CollectionEmpty);
                }

                int index = 0;
                Node node = GetNode(metafield, out index);

                if (node == null) {
                    throw new ArgumentException(String.Format(
                        Resources.Error_FieldDoesNotExist, metafield.Id));
                }

                node.Value = value;
            }
        }

        public virtual int Size
        {
            get { return size; }
        }
        #endregion public properties

        #region operators
        public static bool operator ==(Datagram datagram1, Datagram datagram2)
        {
            return Equals(datagram1, datagram2);
        }

        public static bool operator !=(Datagram datagram1, Datagram datagram2)
        {
            return !Equals(datagram1, datagram2);
        }
        #endregion operators

        #region public constructors
        public Datagram() : this(false) {}
        public Datagram(bool sorted) : base(sorted)
        {
            base.Comparer = new DefaultComparer();
        }
        #endregion public constructors

        #region public methods
        public new virtual Datagram Clone()
        {
            Datagram datagram = new Datagram();

            //
            // this method creates a shallow copy
            //
            for (Node node = Head; node != null; node = node.Next) {
                datagram.InternalInsert(datagram.Count, (Field) node.Value);
            }

            //
            // setting [Sorted] to true causes the LinkedList to perform a
            // QuickSort, which is faster than keeping the list sorted while
            // inserting new elements
            //
            datagram.Comparer = Comparer;
            datagram.Sorted = Sorted;

            return datagram;
        }

        public virtual bool Contains(Metafield metafield)
        {
            OnValidate(metafield);
            return base.Contains(new Field(null, metafield));
        }

        public static bool Equals(Datagram datagram1, Datagram datagram2)
        {
            if ((datagram1 as object) == (datagram2 as object)) {
                return true;
            }

            if ((datagram1 as object) == null || (datagram2 as object) == null) {
                return false;
            }

            if (datagram1.Size != datagram2.Size ||
                datagram1.Count != datagram2.Count) {
                return false;
            }

            for (int i = 0; i < datagram1.Count; i++) {
                if (!Field.Equals(datagram1[i], datagram2[i])) {
                    return false;
                }
            }

            return true;
        }

        public virtual bool Equals(Datagram datagram)
        {
            return Equals(this, datagram);
        }

        public override bool Equals(object datagram)
        {
            return Equals(this, datagram as Datagram);
        }

        public static Datagram FixedSize(Datagram datagram)
        {
            if (datagram == null) {
                throw new ArgumentNullException("datagram");
            }

            if (datagram.IsFixedSize) {
                return datagram;
            }

            return new FixedSizeDatagram(datagram);
        }

        public new virtual FieldEnumerator GetEnumerator()
        {
            return new FieldEnumerator(this);
        }

        public virtual int IndexOf(Metafield metafield)
        {
            OnValidate(metafield);
            return base.IndexOf(new Field(null, metafield));
        }

        public static Datagram ReadOnly(Datagram datagram)
        {
            if (datagram == null) {
                throw new ArgumentNullException("datagram");
            }

            if (datagram.IsReadOnly) {
                return datagram;
            }

            return new ReadOnlyDatagram(datagram);
        }

        public virtual bool Remove(Metafield metafield)
        {
            OnValidate(metafield);
            return base.Remove(new Field(null, metafield));
        }

        public static Datagram Synchronized(Datagram datagram)
        {
            if (datagram == null) {
                throw new ArgumentNullException("datagram");
            }

            if (datagram.IsSynchronized) {
                return datagram;
            }

            return new SyncDatagram(datagram);
        }
        #endregion public methods

        #region protected methods
        protected override void OnClearComplete()
        {
            size = 0;
        }

        protected override void OnInsertComplete(int index, Field field)
        {
            size += field.Size;
        }

        protected override void OnRemoveComplete(int index, Field field)
        {
            size -= field.Size;
        }
     
        protected override void OnSetComplete(int index, Field oldField, Field newField)
        {
            size -= oldField.Size;
            size += newField.Size;
        }

        protected override void OnValidate(Field value)
        {
            if (value == null) {
                throw new ArgumentNullException("value");
            }
        }

        protected virtual void OnValidate(Metafield value)
        {
            if (value == null) {
                throw new ArgumentNullException("value");
            }
        }
        #endregion protected methods

        #region internal methods
        internal virtual Node GetNode(Metafield metafield, out int index)
        {
            OnValidate(metafield);
            Node node = null;

            if (metafield.ProtocolFieldAttribute != null &&
                metafield.ProtocolFieldAttribute.Position < Count) {
                index = metafield.ProtocolFieldAttribute.Position;
                node = GetNodeAt(index);

                if (((Field) node.Value).Metafield == metafield) {
                    return node;
                }
            }

            return base.GetNode(new Field(null, metafield), out index);
        }
        #endregion internal methods

        #region inner classes
        private class DatagramWrapper : Datagram
        {
            private Datagram datagram;

            public override Field this[int index]
            {
                get { return datagram[index]; }
                set { datagram[index] = value; }
            }

            public override Field this[Metafield metafield]
            {
                get { return datagram[metafield]; }
                set { datagram[metafield] = value; }
            }

            public override IComparer<Field> Comparer
            {
                get { return datagram.Comparer; }
                set { datagram.Comparer = value; }
            }

            public override int Count
            {
                get { return datagram.Count; }
            }

            public override bool IsFixedSize
            {
                get { return datagram.IsFixedSize; }
            }

            public override bool IsReadOnly
            {
                get { return datagram.IsReadOnly; }
            }

            public override bool IsSynchronized
            {
                get { return datagram.IsSynchronized; }
            }

            public override bool Sorted
            {
                get { return datagram.Sorted; }
                set { datagram.Sorted = value; }
            }

            public override object SyncRoot
            {
                get { return datagram.SyncRoot; }
            }

            internal override Node Head
            {
                get { return datagram.Head; }
            }

            internal override Node Tail
            {
                get { return datagram.Tail; }
            }

            public override int Size
            {
                get { return datagram.Size; }
            }

            public DatagramWrapper(Datagram datagram)
            {
                this.datagram = datagram;
            }

            public override void Add(Field field)
            {
                datagram.Add(field);
            }

            public override Datagram Clone()
            {
                return datagram.Clone();
            }

            public override bool Contains(Metafield metafield)
            {
                return datagram.Contains(metafield);
            }

            public override bool Equals(Datagram datagram)
            {
                return Datagram.Equals(this.datagram, datagram);
            }

            public override bool Equals(object datagram)
            {
                return Datagram.Equals(this.datagram, datagram as Datagram);
            }

            public override FieldEnumerator GetEnumerator()
            {
                return datagram.GetEnumerator();
            }

            public override int IndexOf(Metafield metafield)
            {
                return datagram.IndexOf(new Field(null, metafield));
            }

            public override void Insert(int index, Field field)
            {
                datagram.Insert(index, field);
            }

            public override bool Remove(Metafield metafield)
            {
                return datagram.Remove(metafield);
            }

            public override void RemoveAt(int index)
            {
                datagram.RemoveAt(index);
            }

            public override void Reverse()
            {
                datagram.Reverse();
            }

            public override void Sort()
            {
                datagram.Sort();
            }

            internal override Node GetNode(Metafield metafield, out int index)
            {
                return datagram.GetNode(metafield, out index);
            }

            internal override Node GetNodeAt(int index)
            {
                return datagram.GetNodeAt(index);
            }
        }

        [Serializable]
        private class FixedSizeDatagram : DatagramWrapper
        {
            public override bool IsFixedSize
            {
                get { return true; }
            }

            public FixedSizeDatagram(Datagram datagram) : base(datagram) {}

            public override void Add(Field field)
            {
                throw new NotSupportedException(
                    Resources.Error_CannotAddOrRemove);
            }

            public override Datagram Clone()
            {
                return new FixedSizeDatagram(base.Clone());
            }

            public override void Insert(int index, Field field)
            {
                throw new NotSupportedException(
                    Resources.Error_CannotAddOrRemove);
            }

            public override bool Remove(Metafield metafield)
            {
                throw new NotSupportedException(
                    Resources.Error_CannotAddOrRemove);
            }

            public override void RemoveAt(int index)
            {
                throw new NotSupportedException(
                    Resources.Error_CannotAddOrRemove);
            }
        }

        [Serializable]
        private sealed class ReadOnlyDatagram : FixedSizeDatagram
        {
            public override Field this[int index]
            {
                get { return base[index]; }
                set {
                    throw new NotSupportedException(
                        Resources.Error_InstanceNotModifiable);
                }
            }

            public override Field this[Metafield metafield]
            {
                get { return base[metafield]; }
                set {
                    throw new NotSupportedException(
                        Resources.Error_InstanceNotModifiable);
                }
            }

            public override IComparer<Field> Comparer
            {
                get { return base.Comparer; }
                set {
                    throw new NotSupportedException(
                        Resources.Error_InstanceNotModifiable);
                }
            }

            public override bool IsReadOnly
            {
                get { return true; }
            }

            public override bool Sorted
            {
                get { return base.Sorted; }
                set {
                    throw new NotSupportedException(
                        Resources.Error_InstanceNotModifiable);
                }
            }

            public ReadOnlyDatagram(Datagram datagram) : base(datagram) {}

            public override Datagram Clone()
            {
                return new ReadOnlyDatagram(base.Clone());
            }

            public override void Reverse()
            {
                throw new NotSupportedException(
                    Resources.Error_InstanceNotModifiable);
            }

            public override void Sort()
            {
                throw new NotSupportedException(
                    Resources.Error_InstanceNotModifiable);
            }
        }

        [Serializable]
        private sealed class SyncDatagram : DatagramWrapper
        {
            public override Field this[int index]
            {
                get {
                    lock (SyncRoot) {
                        return base[index];
                    }
                }

                set {
                    lock (SyncRoot) {
                        base[index] = value;
                    }
                }
            }

            public override Field this[Metafield metafield]
            {
                get {
                    lock (SyncRoot) {
                        return base[metafield];
                    }
                }

                set {
                    lock (SyncRoot) {
                        base[metafield] = value;
                    }
                }
            }

            public override IComparer<Field> Comparer
            {
                get {
                    lock (SyncRoot) {
                        return base.Comparer;
                    }
                }

                set {
                    lock (SyncRoot) {
                        base.Comparer = value;
                    }
                }
            }

            public override int Count
            {
                get {
                    lock (SyncRoot) {
                        return base.Count;
                    }
                }
            }

            public override bool IsSynchronized
            {
                get { return true; }
            }

            public override bool Sorted
            {
                get {
                    lock (SyncRoot) {
                        return base.Sorted;
                    }
                }

                set {
                    lock (SyncRoot) {
                        base.Sorted = value;
                    }
                }
            }

            internal override Node Head
            {
                get {
                    lock (SyncRoot) {
                        return base.Head;
                    }
                }
            }

            internal override Node Tail
            {
                get {
                    lock (SyncRoot) {
                        return base.Tail;
                    }
                }
            }

            public override int Size
            {
                get {
                    lock (SyncRoot) {
                        return base.Size;
                    }
                }
            }

            public SyncDatagram(Datagram datagram) : base(datagram) {}

            public override void Add(Field field)
            {
                lock (SyncRoot) {
                    base.Add(field);
                }
            }

            public override Datagram Clone()
            {
                lock (SyncRoot) {
                    return new SyncDatagram(base.Clone());
                }
            }

            public override bool Contains(Metafield metafield)
            {
                lock (SyncRoot) {
                    return base.Contains(metafield);
                }
            }

            public override bool Equals(Datagram datagram)
            {
                lock (SyncRoot) {
                    return base.Equals(datagram);
                }
            }

            public override bool Equals(object datagram)
            {
                lock (SyncRoot) {
                    return base.Equals(datagram as Datagram);
                }
            }

            public override FieldEnumerator GetEnumerator()
            {
                lock (SyncRoot) {
                    return base.GetEnumerator();
                }
            }

            public override int IndexOf(Metafield metafield)
            {
                lock (SyncRoot) {
                    return base.IndexOf(metafield);
                }
            }

            public override void Insert(int index, Field field)
            {
                lock (SyncRoot) {
                    base.Insert(index, field);
                }
            }

            public override bool Remove(Metafield metafield)
            {
                lock (SyncRoot) {
                    return base.Remove(metafield);
                }
            }

            public override void RemoveAt(int index)
            {
                lock (SyncRoot) {
                    base.RemoveAt(index);
                }
            }

            public override void Reverse()
            {
                lock (SyncRoot) {
                    base.Reverse();
                }
            }

            public override void Sort()
            {
                lock (SyncRoot) {
                    base.Sort();
                }
            }

            internal override Node GetNode(Metafield metafield, out int index)
            {
                lock (SyncRoot) {
                    return base.GetNode(metafield, out index);
                }
            }

            internal override Node GetNodeAt(int index)
            {
                lock (SyncRoot) {
                    return base.GetNodeAt(index);
                }
            }
        }

        private sealed class DefaultComparer : IComparer<Field>
        {
            int IComparer<Field>.Compare(Field x, Field y)
            {
                return x.Metafield != y.Metafield ? 1 : 0;
            }
        }
        #endregion inner classes
    }
}
