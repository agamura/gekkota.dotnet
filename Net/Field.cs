//------------------------------------------------------------------------------
// <sourcefile name="Field.cs" language="C#" begin="05/31/2003">
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
using System.Text;
using Gekkota.Properties;
using Gekkota.Utilities;

namespace Gekkota.Net
{
    /// <summary>
    /// Represents a field in a datagram.
    /// </summary>
    /// <example>
    /// The following example shows how to create and initialize a <c>Field</c>,
    /// and how to print out its value and properties.
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
    ///     // create and initialize a new Field
    ///     //
    ///     Field field = new Field(1, "Red");
    ///
    ///     //
    ///     // display value and properties of the Field
    ///     //
    ///     Console.WriteLine("field");
    ///     Console.WriteLine("\tCategory:    {0}", field.Category);
    ///     Console.WriteLine("\tId:          {0}", field.Id);
    ///     Console.WriteLine("\tIsPrimitive: {0}", field.IsPrimitive);
    ///     Console.WriteLine("\tIsReadOnly:  {0}", field.IsReadOnly);
    ///     Console.WriteLine("\tSize:        {0}", field.Size);
    ///     Console.WriteLine("\tType:        {0}", field.Type);
    ///     Console.WriteLine("\tValue:       {0}", field.ValueAsString);
    ///   }
    /// }
    /// ]]>
    /// </code>
    /// The code above produces the following output:
    /// <![CDATA[
    /// field
    ///     Category:     FieldCategory.Undefined
    ///     Id:           1
    ///     IsPrimitive:  false
    ///     IsReadOnly:   false
    ///     Size:         7
    ///     Type:         FieldType.String
    ///     Value:        Red
    /// ]]>
    /// </example>
    /// <seealso cref="Gekkota.Net.Metafield" />
    /// <seealso cref="Gekkota.Net.FieldSerializer" />
    /// <seealso cref="Gekkota.Net.Datagram" />
    public class Field : ICloneable
    {
        #region internal fields
        internal Metafield Metafield;
        #endregion internal fields

        #region private fields
        private byte[] value;
        #endregion private fields

        #region public properties
        /// <summary>
        /// Gets or sets the <see cref="Gekkota.Net.Field" /> category.
        /// </summary>
        /// <value>
        /// One of the <see cref="Gekkota.Net.FieldCategory" /> values.
        /// </value>
        public virtual FieldCategory Category
        {
            get { return Metafield.Category; }
            set { Metafield.Category = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="Gekkota.Net.Field" /> id.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int32" /> that identifies the
        /// <see cref="Gekkota.Net.Field" />.
        /// </value>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The specified value is not between
        /// <see cref="System.UInt16.MinValue" /> and
        /// <see cref="System.UInt16.MaxValue" />.
        /// </exception>
        public virtual int Id
        {
            get { return Metafield.Id; }
            set { Metafield.Id = value; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the
        /// <see cref="Gekkota.Net.Field" /> is primitive.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the <see cref="Gekkota.Net.Field" /> is
        /// primitive; otherwise, <see langword="false" />.
        /// </value>
        /// <seealso cref="Gekkota.Net.FieldType" />
        public virtual bool IsPrimitive
        {
            get { return Metafield.IsPrimitive; }
        }

        /// <summary>
        /// Specifies whether the <see cref="Gekkota.Net.Field" /> is read-only.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the <see cref="Gekkota.Net.Field" /> is
        /// read-only; otherwise, <see langword="false" />.
        /// </value>
        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the <see cref="Gekkota.Net.Field" /> size.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int32" /> that represents the
        /// <see cref="Gekkota.Net.Field" /> size, in bytes.
        /// </value>
        public virtual int Size
        {
            get { return Metafield.Size; }
        }

        /// <summary>
        /// Gets the <see cref="Gekkota.Net.Field" /> type.
        /// </summary>
        /// <value>
        /// One of the <see cref="Gekkota.Net.FieldType" /> values.
        /// </value>
        public virtual FieldType Type 
        {
            get { return Metafield.Type; }
        }

        /// <summary>
        /// Gets or sets the <see cref="Gekkota.Net.Field" /> value as a
        /// <see cref="System.Byte" />.
        /// </summary>
        /// <value>
        /// A <see cref="System.Byte" /> that represents the
        /// <see cref="Gekkota.Net.Field" /> value.
        /// </value>
        /// <exception cref="System.InvalidOperationException">
        /// The <see cref="Gekkota.Net.Field" /> value cannot be converted to a
        /// <see cref="System.Byte" />.
        /// </exception>
        public virtual unsafe byte ValueAsByte
        {
            get {
                if (value == null) {
                    return 0;
                }

                if (!IsPrimitive) {
                    throw new InvalidOperationException(String.Format(
                        Resources.Error_CannotConvertFieldValue, "Byte"));
                }

                return value[0];
            }

            set {
                if (this.value == null || this.value.Length != sizeof(byte)) {
                    this.value = new byte[sizeof(byte)];
                }
                this.value[0] = value;

                Metafield.Type = FieldType.Integral;
                Metafield.Size = this.value.Length;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Gekkota.Net.Field" /> value as an
        /// <see cref="System.Int16" />.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int16" /> that represents the
        /// <see cref="Gekkota.Net.Field" /> value.
        /// </value>
        /// <exception cref="System.InvalidOperationException">
        /// The <see cref="Gekkota.Net.Field" /> value cannot be converted to an
        /// <see cref="System.Int16" />.
        /// </exception>
        public virtual unsafe short ValueAsInt16
        {
            get {
                if (value == null) {
                    return 0;
                }

                if (!IsPrimitive) {
                    throw new InvalidOperationException(String.Format(
                        Resources.Error_CannotConvertFieldValue, "Int16"));
                }

                fixed (byte* pValue = value) {
                    PrimitiveValue* pPrimitiveValue = (PrimitiveValue*) pValue;
                    return pPrimitiveValue->AsInt16;
                }
            }

            set {
                if (this.value == null || this.value.Length != sizeof(short)) {
                    this.value = new byte[sizeof(short)];
                }

                fixed (byte* pValue = this.value) {
                    PrimitiveValue* pPrimitiveValue = (PrimitiveValue*) pValue;
                    pPrimitiveValue->AsInt16 = value;
                }

                Metafield.Type = FieldType.Integral;
                Metafield.Size = this.value.Length;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Gekkota.Net.Field" /> value as an
        /// <see cref="System.Int32" />.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int32" /> that represents the
        /// <see cref="Gekkota.Net.Field" /> value.
        /// </value>
        /// <exception cref="System.InvalidOperationException">
        /// The <see cref="Gekkota.Net.Field" /> value cannot be converted to an
        /// <see cref="System.Int32" />.
        /// </exception>
        public virtual unsafe int ValueAsInt32
        {
            get {
                if (value == null) {
                    return 0;
                }

                if (!IsPrimitive) {
                    throw new InvalidOperationException(String.Format(
                        Resources.Error_CannotConvertFieldValue, "Int32"));
                }

                fixed (byte* pValue = value) {
                    PrimitiveValue* pPrimitiveValue = (PrimitiveValue*) pValue;
                    return pPrimitiveValue->AsInt32;
                }
            }

            set {
                if (this.value == null || this.value.Length != sizeof(int)) {
                    this.value = new byte[sizeof(int)];
                }

                fixed (byte* pValue = this.value) {
                    PrimitiveValue* pPrimitiveValue = (PrimitiveValue*) pValue;
                    pPrimitiveValue->AsInt32 = value;
                }

                Metafield.Type = FieldType.Integral;
                Metafield.Size = this.value.Length;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Gekkota.Net.Field" /> value as an
        /// <see cref="System.Int64" />.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int64" /> that represents the
        /// <see cref="Gekkota.Net.Field" /> value.
        /// </value>
        /// <exception cref="System.InvalidOperationException">
        /// The <see cref="Gekkota.Net.Field" /> value cannot be converted to an
        /// <see cref="System.Int64" />.
        /// </exception>
        public virtual unsafe long ValueAsInt64
        {
            get {
                if (value == null) {
                    return 0;
                }

                if (!IsPrimitive) {
                    throw new InvalidOperationException(String.Format(
                        Resources.Error_CannotConvertFieldValue, "Int64"));
                }

                fixed (byte* pValue = value) {
                    PrimitiveValue* pPrimitiveValue = (PrimitiveValue*) pValue;
                    return pPrimitiveValue->AsInt64;
                }
            }

            set {
                if (this.value == null || this.value.Length != sizeof(long)) {
                    this.value = new byte[sizeof(long)];
                }

                fixed (byte* pValue = this.value) {
                    PrimitiveValue* pPrimitiveValue = (PrimitiveValue*) pValue;
                    pPrimitiveValue->AsInt64 = value;
                }

                Metafield.Type = FieldType.Integral;
                Metafield.Size = this.value.Length;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Gekkota.Net.Field" /> value as a
        /// <see cref="System.Single" />.
        /// </summary>
        /// <value>
        /// An <see cref="System.Single" /> that represents the
        /// <see cref="Gekkota.Net.Field" /> value.
        /// </value>
        /// <exception cref="System.InvalidOperationException">
        /// The <see cref="Gekkota.Net.Field" /> value cannot be converted to an
        /// <see cref="System.Single" />.
        /// </exception>
        public virtual unsafe float ValueAsSingle
        {
            get {
                if (value == null) {
                    return 0.0F;
                }

                if (!IsPrimitive) {
                    throw new InvalidOperationException(String.Format(
                        Resources.Error_CannotConvertFieldValue, "Single"));
                }

                fixed (byte* pValue = value) {
                    PrimitiveValue* pPrimitiveValue = (PrimitiveValue*) pValue;
                    return Metafield.Type != FieldType.FloatingPoint
                        ? (float) pPrimitiveValue->AsInt64
                        : pPrimitiveValue->AsSingle;
                }
            }

            set {
                if (this.value == null || this.value.Length != sizeof(float)) {
                    this.value = new byte[sizeof(float)];
                }

                fixed (byte* pValue = this.value) {
                    PrimitiveValue* pPrimitiveValue = (PrimitiveValue*) pValue;
                    pPrimitiveValue->AsSingle = value;
                }

                Metafield.Type = FieldType.FloatingPoint;
                Metafield.Size = this.value.Length;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Gekkota.Net.Field" /> value as a
        /// <see cref="System.Double" />.
        /// </summary>
        /// <value>
        /// An <see cref="System.Double" /> that represents the
        /// <see cref="Gekkota.Net.Field" /> value.
        /// </value>
        /// <exception cref="System.InvalidOperationException">
        /// The <see cref="Gekkota.Net.Field" /> value cannot be converted to an
        /// <see cref="System.Double" />.
        /// </exception>
        public virtual unsafe double ValueAsDouble
        {
            get {
                if (value == null) {
                    return 0.0D;
                }

                if (!IsPrimitive) {
                    throw new InvalidOperationException(String.Format(
                        Resources.Error_CannotConvertFieldValue, "Double"));
                }

                fixed (byte* pValue = value) {
                    PrimitiveValue* pPrimitiveValue = (PrimitiveValue*) pValue;
                    return Metafield.Type != FieldType.FloatingPoint
                        ? (double) pPrimitiveValue->AsInt64
                        : pPrimitiveValue->AsDouble;
                }
            }

            set {
                if (this.value == null || this.value.Length != sizeof(double)) {
                    this.value = new byte[sizeof(double)];
                }

                fixed (byte* pValue = this.value) {
                    PrimitiveValue* pPrimitiveValue = (PrimitiveValue*) pValue;
                    pPrimitiveValue->AsDouble = value;
                }

                Metafield.Type = FieldType.FloatingPoint;
                Metafield.Size = this.value.Length;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Gekkota.Net.Field" /> value as a
        /// <see cref="System.String" />.
        /// </summary>
        /// <value>
        /// A <see cref="System.String" /> that represents the
        /// <see cref="Gekkota.Net.Field" /> value.
        /// </value>
        public virtual string ValueAsString
        {
            get {
                return value != null
                    ? Encoding.UTF8.GetString(value, 0, value.Length)
                    : null;
            }

            set {
                this.value = value != null && value != ""
                    ? Encoding.UTF8.GetBytes(value)
                    : null;

                Metafield.Type = FieldType.String;
                Metafield.Size = this.value != null ? this.value.Length : 0;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Gekkota.Net.Field" /> value as a
        /// <see cref="System.Byte" /> array.
        /// </summary>
        /// <value>
        /// A <see cref="System.Byte" /> array that represents the
        /// <see cref="Gekkota.Net.Field" /> value.
        /// </value>
        public virtual byte[] ValueAsByteArray
        {
            get { return value; }
            set {
                this.value = value != null && value.Length > 0
                  ? value
                  : null;

                Metafield.Type = FieldType.ByteArray;
                Metafield.Size = this.value != null ? this.value.Length : 0;
            }
        }
        #endregion public properties

        #region operators
        /// <summary>
        /// Determines whether or not the specified
        /// <see cref="Gekkota.Net.Field" /> objects are equal.
        /// </summary>
        /// <param name="field1">
        /// The <see cref="Gekkota.Net.Field" /> to compare with
        /// <paramref name="field2" />.
        /// </param>
        /// <param name="field2">
        /// The <see cref="Gekkota.Net.Field" /> to compare with
        /// <paramref name="field1" />.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="field1" /> is equal to
        /// <paramref name="field2" />; otherwise, <see langword="false" />.
        /// </returns>
        public static bool operator ==(Field field1, Field field2)
        {
            return Equals(field1, field2);
        }

        /// <summary>
        /// Determines whether or not the specified
        /// <see cref="Gekkota.Net.Field" /> objects are equal.
        /// </summary>
        /// <param name="field1">
        /// The <see cref="Gekkota.Net.Field" /> to compare with
        /// <paramref name="field2" />.
        /// </param>
        /// <param name="field2">
        /// The <see cref="Gekkota.Net.Field" /> to compare with
        /// <paramref name="field1" />.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="field1" /> is equal to
        /// <paramref name="field2" />; otherwise, <see langword="false" />.
        /// </returns>
        public static bool operator !=(Field field1, Field field2)
        {
            return !Equals(field1, field2);
        }
        #endregion operators

        #region public constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Gekkota.Net.Field" />
        /// class.
        /// </summary>
        public Field() : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gekkota.Net.Field" />
        /// class with the specified id.
        /// </summary>
        /// <param name="id">
        /// An <see cref="System.Int32" /> that represents the
        /// <see cref="Gekkota.Net.Field" /> identifier.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="id" /> is not between <see cref="System.UInt16.MinValue" />
        /// and <see cref="System.UInt16.MaxValue" />.
        /// </exception>
        public Field(int id) : this(id, FieldCategory.Undefined)
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="Gekkota.Net.Field" />
        /// class with the specified id and category.
        /// </summary>
        /// <param name="id">
        /// An <see cref="System.Int32" /> that represents the
        /// <see cref="Gekkota.Net.Field" /> identifier.
        /// </param>
        /// <param name="category">
        /// One of the  <see cref="Gekkota.Net.FieldCategory" /> values.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="id" /> is not between <see cref="System.UInt16.MinValue" />
        /// and <see cref="System.UInt16.MaxValue" />.
        /// </exception>
        public Field(int id, FieldCategory category)
        {
            Metafield = new Metafield(id, FieldType.Undefined, 0, category);
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="Gekkota.Net.Field" />
        /// class with the specified id and value.
        /// </summary>
        /// <param name="id">
        /// An <see cref="System.Int32" /> that represents the
        /// <see cref="Gekkota.Net.Field" /> identifier.
        /// </param>
        /// <param name="value">
        /// A <see cref="System.Byte" /> that represents the field value.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="id" /> is not between <see cref="System.UInt16.MinValue" />
        /// and <see cref="System.UInt16.MaxValue" />.
        /// </exception>
        public Field(int id, byte value) : this(id)
        {
            ValueAsByte = value;
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="Gekkota.Net.Field" />
        /// class with the specified id and value.
        /// </summary>
        /// <param name="id">
        /// An <see cref="System.Int32" /> that represents the
        /// <see cref="Gekkota.Net.Field" /> identifier.
        /// </param>
        /// <param name="value">
        /// An <see cref="System.Int16" /> that represents the field value.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="id" /> is not between <see cref="System.UInt16.MinValue" />
        /// and <see cref="System.UInt16.MaxValue" />.
        /// </exception>
        public Field(int id, short value) : this(id)
        {
            ValueAsInt16 = value;
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="Gekkota.Net.Field" />
        /// class with the specified id and value.
        /// </summary>
        /// <param name="id">
        /// An <see cref="System.Int32" /> that represents the
        /// <see cref="Gekkota.Net.Field" /> identifier.
        /// </param>
        /// <param name="value">
        /// An <see cref="System.Int32" /> that represents the field value.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="id" /> is not between <see cref="System.UInt16.MinValue" />
        /// and <see cref="System.UInt16.MaxValue" />.
        /// </exception>
        public Field(int id, int value) : this(id)
        {
            ValueAsInt32 = value;
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="Gekkota.Net.Field" />
        /// class with the specified id and value.
        /// </summary>
        /// <param name="id">
        /// An <see cref="System.Int32" /> that represents the
        /// <see cref="Gekkota.Net.Field" /> identifier.
        /// </param>
        /// <param name="value">
        /// An <see cref="System.Int64" /> that represents the field value.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="id" /> is not between <see cref="System.UInt16.MinValue" />
        /// and <see cref="System.UInt16.MaxValue" />.
        /// </exception>
        public Field(int id, long value) : this(id)
        {
            ValueAsInt64 = value;
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="Gekkota.Net.Field" />
        /// class with the specified id and value.
        /// </summary>
        /// <param name="id">
        /// An <see cref="System.Int32" /> that represents the
        /// <see cref="Gekkota.Net.Field" /> identifier.
        /// </param>
        /// <param name="value">
        /// A <see cref="System.Single" /> that represents the field value.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="id" /> is not between <see cref="System.UInt16.MinValue" />
        /// and <see cref="System.UInt16.MaxValue" />.
        /// </exception>
        public Field(int id, float value) : this(id)
        {
            ValueAsSingle = value;
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="Gekkota.Net.Field" />
        /// class with the specified id and value.
        /// </summary>
        /// <param name="id">
        /// An <see cref="System.Int32" /> that represents the
        /// <see cref="Gekkota.Net.Field" /> identifier.
        /// </param>
        /// <param name="value">
        /// A <see cref="System.Double" /> that represents the field value.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="id" /> is not between <see cref="System.UInt16.MinValue" />
        /// and <see cref="System.UInt16.MaxValue" />.
        /// </exception>
        public Field(int id, double value) : this(id)
        {
            ValueAsDouble = value;
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="Gekkota.Net.Field" />
        /// class with the specified id and value.
        /// </summary>
        /// <param name="id">
        /// An <see cref="System.Int32" /> that represents the
        /// <see cref="Gekkota.Net.Field" /> identifier.
        /// </param>
        /// <param name="value">
        /// A <see cref="System.String" /> that represents the field value.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="id" /> is not between <see cref="System.UInt16.MinValue" />
        /// and <see cref="System.UInt16.MaxValue" />.
        /// </exception>
        public Field(int id, string value) : this(id)
        {
            ValueAsString = value;
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="Gekkota.Net.Field" />
        /// class with the specified id and value.
        /// </summary>
        /// <param name="id">
        /// An <see cref="System.Int32" /> that represents the
        /// <see cref="Gekkota.Net.Field" /> identifier.
        /// </param>
        /// <param name="value">
        /// A <see cref="System.Byte" /> array that represents the field value.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="id" /> is not between <see cref="System.UInt16.MinValue" />
        /// and <see cref="System.UInt16.MaxValue" />.
        /// </exception>
        public Field(int id, byte[] value) : this(id)
        {
            ValueAsByteArray = value;
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="Gekkota.Net.Field" />
        /// class with the specified id, value, and category.
        /// </summary>
        /// <param name="id">
        /// An <see cref="System.Int32" /> that represents the
        /// <see cref="Gekkota.Net.Field" /> identifier.
        /// </param>
        /// <param name="value">
        /// A <see cref="System.Byte" /> that represents the field value.
        /// </param>
        /// <param name="category">
        /// One of the  <see cref="Gekkota.Net.FieldCategory" /> values.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="id" /> is not between <see cref="System.UInt16.MinValue" />
        /// and <see cref="System.UInt16.MaxValue" />.
        /// </exception>
        public Field(int id, byte value, FieldCategory category) : this(id, value)
        {
            Category = category;
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="Gekkota.Net.Field" />
        /// class with the specified id, value, and category.
        /// </summary>
        /// <param name="id">
        /// An <see cref="System.Int32" /> that represents the
        /// <see cref="Gekkota.Net.Field" /> identifier.
        /// </param>
        /// <param name="value">
        /// An <see cref="System.Int16" /> that represents the field value.
        /// </param>
        /// <param name="category">
        /// One of the  <see cref="Gekkota.Net.FieldCategory" /> values.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="id" /> is not between <see cref="System.UInt16.MinValue" />
        /// and <see cref="System.UInt16.MaxValue" />.
        /// </exception>
        public Field(int id, short value, FieldCategory category) : this(id, value)
        {
            Category = category;
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="Gekkota.Net.Field" />
        /// class with the specified id, value, and category.
        /// </summary>
        /// <param name="id">
        /// An <see cref="System.Int32" /> that represents the
        /// <see cref="Gekkota.Net.Field" /> identifier.
        /// </param>
        /// <param name="value">
        /// An <see cref="System.Int32" /> that represents the field value.
        /// </param>
        /// <param name="category">
        /// One of the  <see cref="Gekkota.Net.FieldCategory" /> values.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="id" /> is not between <see cref="System.UInt16.MinValue" />
        /// and <see cref="System.UInt16.MaxValue" />.
        /// </exception>
        public Field(int id, int value, FieldCategory category) : this(id, value)
        {
            Category = category;
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="Gekkota.Net.Field" />
        /// class with the specified id, value, and category.
        /// </summary>
        /// <param name="id">
        /// An <see cref="System.Int32" /> that represents the
        /// <see cref="Gekkota.Net.Field" /> identifier.
        /// </param>
        /// <param name="value">
        /// An <see cref="System.Int64" /> that represents the field value.
        /// </param>
        /// <param name="category">
        /// One of the  <see cref="Gekkota.Net.FieldCategory" /> values.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="id" /> is not between <see cref="System.UInt16.MinValue" />
        /// and <see cref="System.UInt16.MaxValue" />.
        /// </exception>
        public Field(int id, long value, FieldCategory category) : this(id, value)
        {
            Category = category;
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="Gekkota.Net.Field" />
        /// class with the specified id, value, and category.
        /// </summary>
        /// <param name="id">
        /// An <see cref="System.Int32" /> that represents the
        /// <see cref="Gekkota.Net.Field" /> identifier.
        /// </param>
        /// <param name="value">
        /// A <see cref="System.Single" /> that represents the field value.
        /// </param>
        /// <param name="category">
        /// One of the  <see cref="Gekkota.Net.FieldCategory" /> values.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="id" /> is not between <see cref="System.UInt16.MinValue" />
        /// and <see cref="System.UInt16.MaxValue" />.
        /// </exception>
        public Field(int id, float value, FieldCategory category) : this(id, value)
        {
            Category = category;
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="Gekkota.Net.Field" />
        /// class with the specified id, value, and category.
        /// </summary>
        /// <param name="id">
        /// An <see cref="System.Int32" /> that represents the
        /// <see cref="Gekkota.Net.Field" /> identifier.
        /// </param>
        /// <param name="value">
        /// A <see cref="System.Double" /> that represents the field value.
        /// </param>
        /// <param name="category">
        /// One of the  <see cref="Gekkota.Net.FieldCategory" /> values.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="id" /> is not between <see cref="System.UInt16.MinValue" />
        /// and <see cref="System.UInt16.MaxValue" />.
        /// </exception>
        public Field(int id, double value, FieldCategory category) : this(id, value)
        {
            Category = category;
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="Gekkota.Net.Field" />
        /// class with the specified id, value, and category.
        /// </summary>
        /// <param name="id">
        /// An <see cref="System.Int32" /> that represents the
        /// <see cref="Gekkota.Net.Field" /> identifier.
        /// </param>
        /// <param name="value">
        /// A <see cref="System.String" /> that represents the field value.
        /// </param>
        /// <param name="category">
        /// One of the  <see cref="Gekkota.Net.FieldCategory" /> values.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="id" /> is not between <see cref="System.UInt16.MinValue" />
        /// and <see cref="System.UInt16.MaxValue" />.
        /// </exception>
        public Field(int id, string value, FieldCategory category) : this(id, value)
        {
            Category = category;
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="Gekkota.Net.Field" />
        /// class with the specified id, value, and category.
        /// </summary>
        /// <param name="id">
        /// An <see cref="System.Int32" /> that represents the
        /// <see cref="Gekkota.Net.Field" /> identifier.
        /// </param>
        /// <param name="value">
        /// A <see cref="System.Byte" /> array that represents the field value.
        /// </param>
        /// <param name="category">
        /// One of the  <see cref="Gekkota.Net.FieldCategory" /> values.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="id" /> is not between <see cref="System.UInt16.MinValue" />
        /// and <see cref="System.UInt16.MaxValue" />.
        /// </exception>
        public Field(int id, byte[] value, FieldCategory category) : this(id, value)
        {
            Category = category;
        }
        #endregion public constructors

        #region internal constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Gekkota.Net.Field" />
        /// class with the specified value and Metafield.
        /// </summary>
        /// <param name="value">
        /// The field value.
        /// </param>
        /// <param name="metafield">
        /// A <see cref="Gekkota.Net.Metafield" /> that describes the
        /// <see cref="Gekkota.Net.Field" />.
        /// </param>
        /// <remarks>
        /// This constructor by-passes any integrity checks, and it is used
        /// internally to improve performance.
        /// </remarks>
        internal Field(byte[] value, Metafield metafield)
        {
            this.value = value;
            this.Metafield = metafield;

            if (value != null) {
                this.Metafield.Size = value.Length;
            }
        }
        #endregion internal constructors

        #region public methods
        /// <summary>
        /// Creates a deep copy of the <see cref="Gekkota.Net.Field" />.
        /// </summary>
        /// <returns>
        /// A deep copy of the <see cref="Gekkota.Net.Field" />.
        /// </returns>
        public virtual Field Clone()
        {
            byte[] value = null;

            if (this.value != null) {
                value = new byte[this.value.Length];
                System.Buffer.BlockCopy(this.value, 0, value, 0, this.value.Length);
            }

            return new Field(value, Metafield.Clone());
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Determines whether or not the specified
        /// <see cref="Gekkota.Net.Field" /> objects are equal.
        /// </summary>
        /// <param name="field1">
        /// The <see cref="Gekkota.Net.Field" /> to compare with
        /// <paramref name="field2" />.
        /// </param>
        /// <param name="field2">
        /// The <see cref="Gekkota.Net.Field" /> to compare with
        /// <paramref name="field1" />.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="field1" /> is equal to
        /// <paramref name="field2" />; otherwise, <see langword="false" />.
        /// </returns>
        public static bool Equals(Field field1, Field field2)
        {
            if ((field1 as object) == (field2 as object)) {
                return true;
            }

            if ((field1 as object) == null || (field2 as object) == null) {
                return false;
            }

            if (field1.Metafield != field2.Metafield) {
                return false;
            }

            return Utilities.Buffer.Equals(
                field1.ValueAsByteArray,
                field2.ValueAsByteArray);
        }

        /// <summary>
        /// Determines whether or not the specified
        /// <see cref="Gekkota.Net.Field" /> is equal to the current
        /// <see cref="Gekkota.Net.Field" />.
        /// </summary>
        /// <param name="field">
        /// The <see cref="Gekkota.Net.Field" /> to compare with the current
        /// <see cref="Gekkota.Net.Field" />.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="field" /> is equal to the
        /// current <see cref="Gekkota.Net.Field" />; otherwise,
        ///<see langword="false" />.
        /// </returns>
        public virtual bool Equals(Field field)
        {
            return Equals(this, field);
        }

        /// <summary>
        /// Determines whether or not the specified
        /// <see cref="System.Object" /> is equal to the current
        /// <see cref="Gekkota.Net.Field" />.
        /// </summary>
        /// <param name="field">
        /// The <see cref="System.Object" /> to compare with the current
        /// <see cref="Gekkota.Net.Field" />.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="field" /> is equal to the
        /// current <see cref="Gekkota.Net.Field" />; otherwise,
        ///<see langword="false" />.
        /// </returns>
        public override bool Equals(object field)
        {
            return Equals(this, field as Field);
        }

        /// <summary>
        /// Returns a <see cref="Gekkota.Net.Metafield" /> that describes the
        /// <see cref="Gekkota.Net.Field" />.
        /// </summary>
        /// <returns>
        /// A <see cref="Gekkota.Net.Metafield" /> that describes the
        /// <see cref="Gekkota.Net.Field" />.
        /// </returns>
        /// <example>
        /// The following example demonstrates the <c>GetMetafield</c> method.
        /// <code>
        /// <![CDATA[
        /// //
        /// // create and initialize a new Field
        /// //
        /// Field field = new Field(1, Int32.MaxValue);
        /// Metafield metafield = field.GetMetafield();
        ///
        /// //
        /// // display the properties of the returned metafield
        /// //
        /// Console.WriteLine("metafield");
        /// Console.WriteLine("\tCategory:    {0}", metafield.Category);
        /// Console.WriteLine("\tId:          {0}", metafield.Id);
        /// Console.WriteLine("\tIsPrimitive: {0}", metafield.IsPrimitive);
        /// Console.WriteLine("\tIsReadOnly:  {0}", metafield.IsReadOnly);
        /// Console.WriteLine("\tSize:        {0}", metafield.Size);
        /// Console.WriteLine("\tType:        {0}", metafield.Type);
        /// ]]>
        /// </code>
        /// The code above produces the following output:
        /// <![CDATA[
        /// metafield
        ///     Category:     FieldCategory.Undefined
        ///     Id:           1
        ///     IsPrimitive:  false
        ///     IsReadOnly:   false
        ///     Size:         7
        ///     Type:         FieldType.String
        /// ]]>
        /// </example>
        public virtual Metafield GetMetafield()
        {
            return Metafield.Clone();
        }

        /// <summary>
        /// Returns a value indicating whether or not the specified
        /// <see cref="Gekkota.Net.Metafield" /> describes the current
        /// <see cref="Gekkota.Net.Field" />.
        /// </summary>
        /// <param name="metafield">
        /// The <see cref="Gekkota.Net.Metafield" /> to compare with the
        /// underlaying <see cref="Gekkota.Net.Metafield" /> that describes the
        /// current <see cref="Gekkota.Net.Field" />.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="metafield" /> describes
        /// the current <see cref="Gekkota.Net.Field" />; otherwise,
        ///<see langword="false" />.
        /// </returns>
        /// <example>
        /// The following example demonstrates the <c>Match</c> method.
        /// <code>
        /// <![CDATA[
        /// //
        /// // create and initialize a new Field
        /// //
        /// Field field = new Field(1, Int32.MaxValue);
        ///
        /// //
        /// // create and initialize two different Metafield objects
        /// //
        /// Metafield metafield1 = new Metafield(1, FieldType.Integral, sizeof(int));
        /// Metafield metafield2 = new Metafield(2, FieldType.Integral, sizeof(short));
        ///
        /// Console.WriteLine("'field' matches 'metafield1': {0}", field.Match(metafield1));
        /// Console.WriteLine("'field' matches 'metafield2': {0}", field.Match(metafield2));
        /// ]]>
        /// </code>
        /// The code above produces the following output:
        /// <![CDATA[
        /// 'field' matches 'metafield1': true
        /// 'field' matches 'metafield2': false
        /// ]]>
        /// </example>
        public virtual bool Match(Metafield metafield)
        {
            return Metafield == metafield;
        }

        /// <summary>
        /// Returns a read-only wrapper for the <see cref="Gekkota.Net.Field" />.
        /// </summary>
        /// <param name="field">
        /// The <see cref="Gekkota.Net.Field" /> to wrap.
        /// </param>
        /// <returns>
        /// A read-only wrapper around the <see cref="Gekkota.Net.Field" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="field" /> is <see langword="null" />
        /// </exception>
        /// <example>
        /// The following example shows how to create a read-only wrapper around
        /// a <see cref="Gekkota.Net.Field" />.
        /// <code>
        /// <![CDATA[
        /// //
        /// // create and initialize a new Field
        /// //
        /// Field field = new Field(1, Int32.MaxValue);
        /// Field readOnlyField = Field.ReadOnly(field);
        ///
        /// //
        /// // any attempt to modify a read-only Field throws an exception
        /// //
        /// try {
        ///   readOnlyField.Id = 2;
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
        public static Field ReadOnly(Field field)
        {
            if (field == null) {
                throw new ArgumentNullException("field");
            }

            if (field.IsReadOnly) {
                return field;
            }

            return new ReadOnlyField(field);
        }
        #endregion public methods

        #region inner classes
        /// <summary>
        /// Implements a read-only wrapper for the
        /// <see cref="Gekkota.Net.Field" />.
        /// </summary>
        [Serializable]
        private sealed class ReadOnlyField : Field
        {
            private Field field;

            public override int Id
            {
                get { return field.Id; }
                set {
                    throw new NotSupportedException(
                        Resources.Error_InstanceNotModifiable);
                }
            }

            public override FieldCategory Category
            {
                get { return field.Category; }
                set {
                    throw new NotSupportedException(
                        Resources.Error_InstanceNotModifiable);
                }
            }

            public override bool IsPrimitive
            {
                get { return field.IsPrimitive; }
            }

            public override bool IsReadOnly
            {
                get { return true; }
            }

            public override FieldType Type
            {
                get { return field.Type; }
            }

            public override int Size
            {
                get { return field.Size; }
            }

            public override byte ValueAsByte
            {
                get { return field.ValueAsByte; }
                set {
                    throw new NotSupportedException(
                        Resources.Error_InstanceNotModifiable);
                }
            }

            public override short ValueAsInt16
            {
                get { return field.ValueAsInt16; }
                set {
                    throw new NotSupportedException(
                        Resources.Error_InstanceNotModifiable);
                }
            }

            public override int ValueAsInt32
            {
                get { return field.ValueAsInt32; }
                set {
                    throw new NotSupportedException(
                        Resources.Error_InstanceNotModifiable);
                }
            }

            public override long ValueAsInt64
            {
                get { return field.ValueAsInt64; }
                set {
                    throw new NotSupportedException(
                        Resources.Error_InstanceNotModifiable);
                }
            }

            public override float ValueAsSingle
            {
                get { return field.ValueAsSingle; }
                set {
                    throw new NotSupportedException(
                        Resources.Error_InstanceNotModifiable);
                }
            }

            public override double ValueAsDouble
            {
                get { return field.ValueAsDouble; }
                set {
                    throw new NotSupportedException(
                        Resources.Error_InstanceNotModifiable);
                }
            }

            public override string ValueAsString
            {
                get { return field.ValueAsString; }
                set {
                    throw new NotSupportedException(
                        Resources.Error_InstanceNotModifiable);
                }
            }

            public override byte[] ValueAsByteArray
            {
                get { return field.ValueAsByteArray; }
                set {
                    throw new NotSupportedException(
                        Resources.Error_InstanceNotModifiable);
                }
            }

            public ReadOnlyField(Field field)
            {
                this.field = field;
            }

            public override Field Clone()
            {
                return new ReadOnlyField(field.Clone());
            }

            public override bool Equals(Field field)
            {
                return this.field.Equals(field);
            }

            public override Metafield GetMetafield()
            {
                return field.GetMetafield();
            }

            public override bool Match(Metafield metafield)
            {
                return field.Match(metafield);
            }
        }
        #endregion inner classes
    }
}
