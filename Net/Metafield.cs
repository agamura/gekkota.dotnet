//------------------------------------------------------------------------------
// <sourcefile name="Metafield.cs" language="C#" begin="08/05/2003">
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
using Gekkota.Properties;

namespace Gekkota.Net
{
    /// <summary>
    /// Describes a <see cref="Gekkota.Net.Field" />.
    /// </summary>
    /// <example>
    /// The following example shows how to create and initialize a
    /// <c>Metafield</c>, and how to print out its properties.
    /// <code>
    /// <![CDATA[
    /// using System;
    /// using Gekkota.Net;
    ///
    /// public class MyClass
    /// {
    ///   public static void Main()
    ///   {
    ///     //
    ///     // create and initialize a new Metafield
    ///     //
    ///     Metafield metafield = new Metafield(1, FieldType.Integral);
    ///
    ///     //
    ///     // display the properties of the Metafield
    ///     //
    ///     Console.WriteLine("metafield");
    ///     Console.WriteLine("\tCategory:    {0}", metafield.Category);
    ///     Console.WriteLine("\tId:          {0}", metafield.Id);
    ///     Console.WriteLine("\tIsPrimitive: {0}", metafield.IsPrimitive);
    ///     Console.WriteLine("\tIsReadOnly:  {0}", metafield.IsReadOnly);
    ///     Console.WriteLine("\tSize:        {0}", metafield.Size);
    ///     Console.WriteLine("\tType:        {0}", metafield.Type);
    ///   }
    /// }
    /// ]]>
    /// </code>
    /// The code above produces the following output:
    /// <![CDATA[
    /// metafield
    ///     Category:     FieldCategory.Undefined
    ///     Id:           1
    ///     IsPrimitive:  true
    ///     IsReadOnly:   false
    ///     Size:         4
    ///     Type:         FieldType.Integral
    /// ]]>
    /// </example>
    /// <seealso cref="Gekkota.Net.Field" />
    /// <seealso cref="Gekkota.Net.FieldSerializer" />
    /// <seealso cref="Gekkota.Net.Datagram" />
    public class Metafield : ICloneable
    {
        #region private fields
        private ProtocolFieldAttribute protocolFieldAttribute;
        private FieldHeader fieldHeader;
        private static readonly int FieldHeaderSize;
        #endregion

        #region public properties
        /// <summary>
        /// Gets or sets the field category.
        /// </summary>
        /// <value>
        /// One of the <see cref="Gekkota.Net.FieldCategory" /> values.
        /// </value>
        /// <exception cref="System.ArgumentException">
        /// The specified value is <see cref="Gekkota.Net.FieldCategory.Header" />
        /// and the <see cref="Gekkota.Net.Metafield.Size" /> property is not
        /// greater than 0.
        /// </exception>
        /// <remarks>
        /// Header fields must always have a predetermined size greater than 0,
        /// even if they are not primitive.
        /// </remarks>
        public virtual FieldCategory Category
        {
            get { return fieldHeader.Category; }
            set {
                if (value == FieldCategory.Header && Size == UInt16.MinValue) {
                    throw new ArgumentException(Resources.Error_ValueNotValid);
                }

                fieldHeader.Category = value;
            }
        }

        /// <summary>
        /// Gets the size of the physical layout of the
        /// <see cref="Gekkota.Net.Metafield" />.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int32" /> that represents the size of the
        /// physical layout of the <see cref="Gekkota.Net.Metafield" />, in bytes.
        /// </value>
        /// <remarks>
        /// The physical layout contains the internal metadata that describes
        /// the field.
        /// </remarks>
        public static int LayoutSize
        {
            get { return FieldHeaderSize; }
        }

        /// <summary>
        /// Gets or sets the field id.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int32" /> that represents the field id.
        /// </value>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The specified value is not between <see cref="System.UInt16.MinValue" />
        /// and <see cref="System.UInt16.MaxValue" />.
        /// </exception>
        public virtual int Id
        {
            get { return fieldHeader.Id; }
            set {
                if (value < UInt16.MinValue || value > UInt16.MaxValue) {
                    string message = String.Format(
                        Resources.Error_ValueOutOfRange,
                        UInt16.MinValue, UInt16.MaxValue);

                    throw new ArgumentOutOfRangeException("value", value, message);
                }

                fieldHeader.Id = unchecked((ushort) value);
            }
        }

        /// <summary>
        /// Specifies whether the <see cref="Gekkota.Net.Metafield" /> is
        /// read-only.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the <see cref="Gekkota.Net.Metafield" />
        /// is read-only; otherwise, <see langword="false" />.
        /// </value>
        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the field is primitive.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the field is primitive; otherwise,
        /// <see langword="false" />.
        /// </value>
        /// <seealso cref="Gekkota.Net.FieldType" />
        public virtual bool IsPrimitive
        {
            get {
                return
                    fieldHeader.Type > FieldType.Undefined &&
                    fieldHeader.Type < FieldType.String;
            }
        }

        /// <summary>
        /// Gets or sets the field size.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int32" /> that represents the field size, in bytes.
        /// </value>
        /// <exception cref="System.ArgumentException">
        /// The <see cref="Gekkota.Net.Metafield.Type" /> property is
        /// <see cref="Gekkota.Net.FieldType.Integral" /> and the specified value
        /// is not 1 (<see cref="System.Byte" />), 2 (<see cref="System.Int16" />),
        /// 4 (<see cref="System.Int32" />), or 8 (<see cref="System.Int64" />).
        /// <para>-or</para>
        /// The <see cref="Gekkota.Net.Metafield.Type" /> property is
        /// <see cref="Gekkota.Net.FieldType.FloatingPoint" /> and the specified
        /// value is not 4 (<see cref="System.Single" />) or 8 (<see cref="System.Double" />).
        /// <para>-or-</para>
        /// The <see cref="Gekkota.Net.Metafield.Type" /> property identifies a
        /// vector type, the <see cref="Gekkota.Net.Metafield.Category" /> property
        /// is <see cref="Gekkota.Net.FieldCategory.Header" />, and the specified
        /// value is not greater than 0.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The <see cref="Gekkota.Net.Metafield.Type" /> property identifies a
        /// vector type and the specified value is not between
        /// <see cref="System.UInt16.MinValue" /> and <see cref="System.UInt16.MaxValue" />.
        /// </exception>
        public virtual unsafe int Size
        {
            get { return fieldHeader.Size; }
            set {
                if (IsPrimitive) {
                    int minSize = Type == FieldType.Integral ? sizeof(byte) : sizeof(int);
                    int pack = sizeof(long);

                    while (value > UInt16.MinValue) {
                        if (value >= pack) break;
                        if (pack == minSize) break;
                        pack >>= 1;
                    }

                    if (value != pack) {
                        throw new ArgumentException(Resources.Error_ValueNotValid);
                    }
                } else {
                    if (value < UInt16.MinValue || value > UInt16.MaxValue) {
                        throw new ArgumentOutOfRangeException("value", value,
                            String.Format(Resources.Error_ValueOutOfRange,
                            UInt16.MinValue, UInt16.MaxValue));
                    } else if (Category == FieldCategory.Header && value == UInt16.MinValue) {
                        throw new ArgumentException(Resources.Error_ValueNotValid);
                    }
                }

                fieldHeader.Size = unchecked((ushort) value);
            }
        }

        /// <summary>
        /// Gets or sets the field type.
        /// </summary>
        /// <value>
        /// One of the <see cref="Gekkota.Net.FieldType" /> values.
        /// </value>
        /// <remarks>
        /// If the specified value identifies a primitive type, the
        /// <see cref="Gekkota.Net.Metafield.Size" /> property is default to the
        /// size of an <see cref="System.Int32" />, otherwise the
        /// <see cref="Gekkota.Net.Metafield.Size" /> property is default to 0.
        /// </remarks>
        public virtual unsafe FieldType Type
        {
            get { return fieldHeader.Type; }
            set {
                if (fieldHeader.Type != value) {
                    fieldHeader.Type = value;

                    if (IsPrimitive) {
                        fieldHeader.Size = unchecked(sizeof(int));
                    } else if (value != FieldType.Undefined) {
                        fieldHeader.Size = UInt16.MaxValue;
                    } else {
                        fieldHeader.Size = 0;
                    }
                }
            }
        }
        #endregion public properties

        #region internal properties
        internal virtual ProtocolFieldAttribute ProtocolFieldAttribute
        {
            get { return protocolFieldAttribute; }
            set { protocolFieldAttribute = value; }
        }
        #endregion internal properties

        #region operators
        /// <summary>
        /// Determines whether or not the specified
        /// <see cref="Gekkota.Net.Metafield" /> objects are equal.
        /// </summary>
        /// <param name="metafield1">
        /// The <see cref="Gekkota.Net.Metafield" /> to compare with
        /// <paramref name="metafield2" />.
        /// </param>
        /// <param name="metafield2">
        /// The <see cref ="Gekkota.Net.Metafield" /> to compare with
        /// <paramref name="metafield1" />.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="metafield1" /> is equal to
        /// <paramref name="metafield2" />; otherwise, <see langword="false" />.
        /// </returns>
        public static bool operator ==(Metafield metafield1, Metafield metafield2)
        {
            return Equals(metafield1, metafield2);
        }

        /// <summary>
        /// Determines whether or not the specified
        /// <see cref="Gekkota.Net.Metafield" /> objects are not equal.
        /// </summary>
        /// <param name="metafield1">
        /// The <see cref="Gekkota.Net.Metafield" /> to compare with
        /// <paramref name="metafield2" />.
        /// </param>
        /// <param name="metafield2">
        /// The <see cref ="Gekkota.Net.Metafield" /> to compare with
        /// <paramref name="metafield1" />.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="metafield1" /> is not equal to
        /// <paramref name="metafield2" />; otherwise, <see langword="false" />.
        /// </returns>
        public static bool operator !=(Metafield metafield1, Metafield metafield2)
        {
            return !Equals(metafield1, metafield2);
        }
        #endregion operators

        #region public constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Gekkota.Net.Metafield" />
        /// class.
        /// </summary>
        public Metafield()
        {
            fieldHeader.Id = 0;
            fieldHeader.Type = FieldType.Undefined;
            fieldHeader.Size = 0;
            fieldHeader.Category = FieldCategory.Undefined;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gekkota.Net.Metafield" />
        /// class with the specified field id.
        /// </summary>
        /// <param name="id">
        /// An <see cref="System.Int32" /> that represents the field id.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="id" /> is not between <see cref="System.UInt16.MinValue" />
        /// and <see cref="System.UInt16.MaxValue" />.
        /// </exception>
        public Metafield(int id)
        {
            Id = id;
            fieldHeader.Type = FieldType.Undefined;
            fieldHeader.Size = 0;
            fieldHeader.Category = FieldCategory.Undefined;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gekkota.Net.Metafield" />
        /// class with the specified field id and field type.
        /// </summary>
        /// <param name="id">
        /// An <see cref="System.Int32" /> that represents the field id.
        /// </param>
        /// <param name="type">
        /// One of the <see cref="Gekkota.Net.FieldType" /> values.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="id" /> is not between <see cref="System.UInt16.MinValue" />
        /// and <see cref="System.UInt16.MaxValue" />.
        /// </exception>
        public Metafield(int id, FieldType type)
        {
            //
            // field size for primitive types is set automatically
            // by the type setter
            //
            Id = id;
            Type = type;
            fieldHeader.Category = FieldCategory.Undefined;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gekkota.Net.Metafield" />
        /// class with the specified field id, field type, and field size.
        /// </summary>
        /// <param name="id">
        /// An <see cref="System.Int32" /> that represents the field id.
        /// </param>
        /// <param name="type">
        /// One of the <see cref="Gekkota.Net.FieldType" /> values.
        /// </param>
        /// <param name="size">
        /// An <see cref="System.Int32" /> that represents the field size.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="id" /> is not between <see cref="System.UInt16.MinValue" />
        /// and <see cref="System.UInt16.MaxValue" />.
        /// <para>-or-</para>
        /// <paramref name="type" /> identifies a vector type and <paramref name="size" />
        /// is not between <see cref="System.UInt16.MinValue" /> and
        /// <see cref="System.UInt16.MaxValue" />.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="type" /> is <see cref="Gekkota.Net.FieldType.Integral" />
        /// and <paramref name="size" /> is not 1 (<see cref="System.Byte" />),
        /// 2 (<see cref="System.Int16" />), 4 (<see cref="System.Int32" />), or
        /// 8 (<see cref="System.Int64" />).
        /// <para>-or-</para>
        /// <paramref name="type" /> is <see cref="Gekkota.Net.FieldType.FloatingPoint" />
        /// and <paramref name="size" /> is not 4 (<see cref="System.Single" />) or
        /// 8 (<see cref="System.Double" />).
        /// </exception>
        public Metafield(int id, FieldType type, int size)
            : this(id, type, size, FieldCategory.Undefined)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gekkota.Net.Metafield" />
        /// class with the specified field id, field type, field size, and field
        /// category.
        /// </summary>
        /// <param name="id">
        /// An <see cref="System.Int32" /> that represents the field id.
        /// </param>
        /// <param name="type">
        /// One of the <see cref="Gekkota.Net.FieldType" /> values.
        /// </param>
        /// <param name="size">
        /// An <see cref="System.Int32" /> that represents the field size.
        /// </param>
        /// <param name="category">
        /// One of the <see cref="Gekkota.Net.FieldCategory" /> values.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="id" /> is not between <see cref="System.UInt16.MinValue" />
        /// and <see cref="System.UInt16.MaxValue" />.
        /// <para>-or-</para>
        /// <paramref name="type" /> identifies a vector type and <paramref name="size" />
        /// is not between <see cref="System.UInt16.MinValue" /> and
        /// <see cref="System.UInt16.MaxValue" />.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="type" /> is <see cref="Gekkota.Net.FieldType.Integral" />
        /// and <paramref name="size" /> is not 1 (<see cref="System.Byte" />),
        /// 2 (<see cref="System.Int16" />), 4 (<see cref="System.Int32" />), or
        /// 8 (<see cref="System.Int64" />).
        /// <para>-or-</para>
        /// <paramref name="type" /> is <see cref="Gekkota.Net.FieldType.FloatingPoint" />
        /// and <paramref name="size" /> is not 4 (<see cref="System.Single" />) or
        /// 8 (<see cref="System.Double" />).
        /// <para>-or-</para>
        /// <paramref name="category" /> is <see cref="Gekkota.Net.FieldCategory.Header" />
        /// and <paramref name="size" /> is not greater than 0.
        /// </exception>
        public Metafield(int id, FieldType type, int size, FieldCategory category)
        {
            Id = id;
            Type = type;

            if (type != FieldType.Undefined) {
                if (!IsPrimitive || size > 0) Size = size;
            }

            Category = category;
        }
        #endregion public constructors

        #region private constructors
        /// <summary>
        /// Initializes static members.
        /// </summary>
        static unsafe Metafield()
        {
            FieldHeaderSize = sizeof(FieldHeader);
        }
        #endregion private constructors

        #region public methods
        /// <summary>
        /// Creates a deep copy of the <see cref="Gekkota.Net.Metafield" />.
        /// </summary>
        /// <returns>
        /// A deep copy of the <see cref="Gekkota.Net.Metafield" />.
        /// </returns>
        public virtual Metafield Clone()
        {
            return InternalClone();
        }

        object ICloneable.Clone()
        {
            return InternalClone();
        }

        /// <summary>
        /// Determines whether or not the specified <see cref="Gekkota.Net.Metafield" />
        /// objects are equal.
        /// </summary>
        /// <param name="metafield1">
        /// The <see cref="Gekkota.Net.Metafield" /> to compare with
        /// <paramref name="metafield2" />.
        /// </param>
        /// <param name="metafield2">
        /// The <see cref="Gekkota.Net.Metafield" /> to compare with
        /// <paramref name="metafield1" />.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="metafield1" /> is equal to
        /// <paramref name="metafield2" />; otherwise, <see langword="false" />.
        /// </returns>
        public static bool Equals(Metafield metafield1, Metafield metafield2)
        {
            if ((metafield1 as object) == (metafield2 as object)) {
                return true;
            }

            if ((metafield1 as object) == null || (metafield2 as object) == null) {
                return false;
            }

            bool equals = metafield1.Id == metafield2.Id &&
                metafield1.Type == metafield2.Type &&
                metafield1.Category == metafield2.Category;

            if (equals && metafield1.Size < UInt16.MaxValue && metafield2.Size < UInt16.MaxValue) {
                equals = equals && metafield1.Size == metafield2.Size;
            }

            return equals;
        }

        /// <summary>
        /// Determines whether or not the specified <see cref="Gekkota.Net.Metafield" />
        /// is equal to the current <see cref="Gekkota.Net.Metafield" />.
        /// </summary>
        /// <param name="metafield">
        /// The <see cref="Gekkota.Net.Metafield" /> to compare with the current
        /// <see cref="Gekkota.Net.Metafield" />.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="metafield" /> is equal to
        /// the current <see cref="Gekkota.Net.Metafield" />; otherwise,
        /// <see langword="false" />.
        /// </returns>
        public virtual bool Equals(Metafield metafield)
        {
            return Equals(this, metafield);
        }

        /// <summary>
        /// Determines whether or not the specified <see cref="System.Object" />
        /// is equal to the current <see cref="Gekkota.Net.Metafield" />.
        /// </summary>
        /// <param name="metafield">
        /// The <see cref="System.Object" /> to compare with the current
        /// <see cref="Gekkota.Net.Metafield" />.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="metafield" /> is equal to
        /// the current <see cref="Gekkota.Net.Metafield" />; otherwise,
        /// <see langword="false" />.
        /// </returns>
        public override bool Equals(object metafield)
        {
            return Equals(this, metafield as Metafield);
        }

        /// <summary>
        /// Returns a read-only wrapper for the <see cref="Gekkota.Net.Metafield" />.
        /// </summary>
        /// <param name="metafield">
        /// The <see cref="Gekkota.Net.Metafield" /> to wrap.
        /// </param>
        /// <returns>
        /// A read-only wrapper around the <see cref="Gekkota.Net.Metafield" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="metafield" /> is <see langword="null" />.
        /// </exception>
        /// <example>
        /// The following example shows how to create a read-only wrapper around a
        /// <see cref="Gekkota.Net.Metafield" />.
        /// <code>
        /// <![CDATA[
        /// //
        /// // create and initialize a new Metafield
        /// //
        /// Metafield metafield = new Metafield(1, FieldType.Integral);
        /// Metafield readOnlyMetafield = Metafield.ReadOnly(metafield);
        ///
        /// //
        /// // any attempt to modify a read-only Metafield throws an exception
        /// //
        /// try {
        ///   readOnlyMetafield.Id = 2;
        /// } catch (NotSupportedException e) {
        ///   Console.WriteLine("Error: {0}", e.Message);
        /// }
        /// ]]>
        /// </code>
        /// The code above produces the following output:
        /// <![CDATA[
        /// Error: Instance not modifiable.
        /// ]]>
        /// </example>
        public static Metafield ReadOnly(Metafield metafield)
        {
            if (metafield == null) {
                throw new ArgumentNullException("metafield");
            }

            if (metafield.IsReadOnly) {
                return metafield;
            }

            return new ReadOnlyMetafield(metafield);
        }
        #endregion public methods

        #region internal methods
        /// <summary>
        /// Creates a deep copy of the <see cref="Gekkota.Net.Metafield" />.
        /// </summary>
        /// <returns>
        /// A deep copy of the <see cref="Gekkota.Net.Metafield" />.
        /// </returns>
        internal virtual Metafield InternalClone()
        {
            Metafield metafield = new Metafield();
            metafield.fieldHeader.Id = fieldHeader.Id;
            metafield.fieldHeader.Type = fieldHeader.Type;
            metafield.fieldHeader.Size = fieldHeader.Size;
            metafield.fieldHeader.Category = fieldHeader.Category;

            if (ProtocolFieldAttribute != null) {
                metafield.ProtocolFieldAttribute = new ProtocolFieldAttribute(
                    ProtocolFieldAttribute.Position);
            }

            return metafield;
        }
        #endregion internal methods

        #region inner classes
        /// <summary>
        /// Implements a read-only wrapper for the
        /// <see cref="Gekkota.Net.Metafield" />.
        /// </summary>
        private sealed class ReadOnlyMetafield : Metafield
        {
            private Metafield metafield;

            public override int Id
            {
                get { return metafield.Id; }
                set {
                    throw new NotSupportedException(
                        Resources.Error_InstanceNotModifiable);
                }
            }

            public override FieldCategory Category
            {
                get { return metafield.Category; }
                set {
                    throw new NotSupportedException(
                        Resources.Error_InstanceNotModifiable);
                }
            }

            public override bool IsPrimitive
            {
                get { return metafield.IsPrimitive; }
            }

            public override bool IsReadOnly
            {
                get { return true; }
            }

            public override int Size
            {
                get { return metafield.Size; }
                set {
                    throw new NotSupportedException(
                        Resources.Error_InstanceNotModifiable);
                }
            }

            public override FieldType Type
            {
                get { return metafield.Type; }
                set {
                    throw new NotSupportedException(
                        Resources.Error_InstanceNotModifiable);
                }
            }

            internal override ProtocolFieldAttribute ProtocolFieldAttribute
            {
                get { return metafield.ProtocolFieldAttribute; }
                set { metafield.ProtocolFieldAttribute = value; }
            }

            public ReadOnlyMetafield(Metafield metafield)
            {
                this.metafield = metafield;
            }

            public override Metafield Clone()
            {
                return new ReadOnlyMetafield(metafield.InternalClone());
            }

            public override bool Equals(Metafield metafield)
            {
                return this.metafield.Equals(metafield);
            }

            internal override Metafield InternalClone()
            {
                return metafield.InternalClone();
            }
        }
        #endregion inner classes
    }
}
