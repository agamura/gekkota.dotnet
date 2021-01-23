//------------------------------------------------------------------------------
// <sourcefile name="FieldSerializer.cs" language="C#" begin="06/05/2003">
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
using System.Net;
using Gekkota.Utilities;

namespace Gekkota.Net
{
    /// <summary>
    /// Serializes and deserializes <see cref="Gekkota.Net.Field" /> objects
    /// into and from byte arrays. This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// When serializing or deserializing primitive data types, the byte order
    /// is adjusted automatically so that they can be used by a program or
    /// sent over a network without further transformations.
    /// </remarks>
    public sealed class FieldSerializer
    {
        #region public methods
        /// <summary>
        /// Deserializes the specified byte array into a
        /// <see cref="Gekkota.Net.Field" />.
        /// </summary>
        /// <param name="data">
        /// A <see cref="System.Byte" /> array from which to deserialiize the
        /// <see cref="Gekkota.Net.Field" />.
        /// </param>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the starting position
        /// within <paramref name="data" />.
        /// </param>
        /// <returns>
        /// The <see cref="Gekkota.Net.Field" /> deserialized from
        /// <paramref name="data" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="data" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="index" /> is equal to or greater than the length of
        /// <paramref name="data" />.
        /// </exception>
        /// <example>
        /// The following example shows how to deserialize a byte array that
        /// contains metadata into a <c>Field</c>.
        /// <code>
        /// <![CDATA[
        /// //
        /// // create and initialize a new Field
        /// //
        /// Field field = new Field(1, Int32.MaxValue);
        /// byte[] serializedField = new byte[field.Size + Metafield.LayoutSize];
        ///
        /// //
        /// // serialize and deserialize the Field with metadata
        /// //
        /// FieldSerializer.Serialize(field, serializedField, 0, true);
        /// Field deserializedField = FieldSerializer.Deserialize(serializedField, 0);
        /// ]]>
        /// </code>
        /// </example>
        public static unsafe Field Deserialize(byte[] data, int index)
        {
            BoundsChecker.Check("data", data, index);
            Metafield metafield = null;

            unchecked {
                fixed (byte* pData = data) {
                    FieldHeader* pFieldHeader = (FieldHeader*) (pData + index);
                    metafield = new Metafield(
                    (ushort) IPAddress.NetworkToHostOrder((short) pFieldHeader->Id),
                    pFieldHeader->Type,
                    (ushort) IPAddress.NetworkToHostOrder((short) pFieldHeader->Size),
                    pFieldHeader->Category);
                }
            }

            if (metafield.Size > 0) {
                index += Metafield.LayoutSize;
            }

            return Deserialize(data, index, metafield);
        }

        /// <summary>
        /// Deserializes the specified byte array into a
        /// <see cref="Gekkota.Net.Field" />.
        /// </summary>
        /// <param name="data">
        /// A <see cref="System.Byte" /> array from which to deserialize the
        /// <see cref="Gekkota.Net.Field" />.
        /// </param>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the starting position
        /// within <paramref name="data" />.
        /// </param>
        /// <param name="metafield">
        /// A <see cref="Gekkota.Net.Metafield" /> that describes the
        /// <see cref="Gekkota.Net.Field" /> to deserialize.
        /// </param>
        /// <returns>
        /// The <see cref="Gekkota.Net.Field" /> deserialized from
        /// <paramref name="data" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="data" /> or <paramref name="metafield" /> is
        /// <see langword="null" />.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="startIndex" /> is less than 0.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="index" /> is equal to or greater than the length of
        /// <paramref name="data" />.
        /// </exception>
        /// <example>
        /// The following example shows how to deserialize a byte array that
        /// does not contain metadata into a <c>Field</c>.
        /// <code>
        /// <![CDATA[
        /// //
        /// // create and initialize a new Field
        /// //
        /// Field field = new Field(1, Int32.MaxValue);
        /// byte[] serializedField = new byte[field.Size];
        ///
        /// //
        /// // serialize and deserialize the Field without metadata
        /// //
        /// FieldSerializer.Serialize(field, serializedField, 0, false);
        /// Field deserializedField = FieldSerializer.Deserialize(
        ///     serializedField, 0, field.GetMetafield());
        /// ]]>
        /// </code>
        /// </example>
        public static unsafe Field Deserialize(byte[] data, int index, Metafield metafield)
        {
            BoundsChecker.Check("data", data, index);

            if (metafield == null) {
                throw new ArgumentNullException("metafield");
            }

            byte[] temp = null;

            if (metafield.Size > 0) {
                if (metafield.Size < UInt16.MaxValue) {
                    temp = new byte[metafield.Size];
                } else {
                    //
                    // the field extends to the end of the data
                    //
                    temp = new byte[data.Length - index];
                }

                System.Buffer.BlockCopy(
                    data, index,    // source
                    temp, 0,        // destination
                    temp.Length);   // count
            }

            return Map(temp, metafield);
        }

        /// <summary>
        /// Serializes the specified <see cref="Gekkota.Net.Field" /> into the
        /// given byte array.
        /// </summary>
        /// <param name="field">
        /// The <see cref="Gekkota.Net.Field" /> to serialize.
        /// </param>
        /// <param name="data">
        /// A <see cref="System.Byte" /> array into which <paramref name="field" />
        /// is to be serialized.
        /// </param>
        /// <param name="index">
        /// An <see cref="System.Int32" /> that represents the starting position
        /// within <paramref name="data" />.
        /// </param>
        /// <param name="embedMetadata">
        /// A <see cref="System.Boolean" /> value indicating whether or not
        /// <paramref name="field" /> has to be serialized with metadata.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="field" /> or <paramref name="data" /> is
        /// <see langword="null" />.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="index" /> is equal to or greater than the length of
        /// <paramref name="data" />.
        /// </exception>
        /// <example>
        /// The following example shows how to serialize a <c>Field</c>into a
        /// byte array.
        /// <code>
        /// <![CDATA[
        /// //
        /// // create and initialize a new Field
        /// //
        /// Field field = new Field(1, Int32.MaxValue);
        /// byte[] serializedField1 = new byte[field.Size + Metafield.LayoutSize];
        /// byte[] serializedField2 = new byte[field.Size];
        ///
        /// //
        /// // serialize the Field with metadata
        /// //
        /// FieldSerializer.Serialize(field, serializedField1, 0, true);
        ///
        /// //
        /// // serialize the Field without metadata
        /// //
        /// FieldSerializer.Serialize(field, serializedField2, 0, false);
        /// ]]>
        /// </code>
        /// </example>
        public static unsafe void Serialize(
            Field field, byte[] data, int index, bool embedMetadata)
        {
            if (field == null) {
                throw new ArgumentNullException("field");
            }

            BoundsChecker.Check("data", data, index);

            int position = 0;

            if (embedMetadata) {
                unchecked {
                    fixed (byte* pData = data) {
                        FieldHeader* pFieldHeader = (FieldHeader*) (pData + index);
                        pFieldHeader->Id =
                        (ushort) IPAddress.HostToNetworkOrder((short) field.Id);
                        pFieldHeader->Type = field.Type;
                        pFieldHeader->Size =
                        (ushort) IPAddress.HostToNetworkOrder((short) field.Size);
                        pFieldHeader->Category = field.Category;
                    }
                }

                position = Metafield.LayoutSize;
            }

            if (field.Size > 0) {
                System.Buffer.BlockCopy(
                    Unmap(field), 0,            // source
                    data, index + position,     // destination
                    field.Size);                // count
            }
        }
        #endregion public methods

        #region private methods
        /// <summary>
        /// Maps the specified array of bytes into a
        /// <see cref="Gekkota.Net.Field" />.
        /// </summary>
        /// <param name="data">
        /// The <see cref="System.Byte" /> array to map.
        /// </param>
        /// <param name="metafield">
        /// A <see cref="Gekkota.Net.Metafield" /> that describes the
        /// <see cref="Gekkota.Net.Field" /> to map.
        /// </param>
        /// <returns>
        /// The <see cref="Gekkota.Net.Field" /> mapped from <paramref name="data" />.
        /// </returns>
        private static unsafe Field Map(byte[] data, Metafield metafield)
        {
            Field field = null;

            if (metafield.Size == sizeof(byte) || !metafield.IsPrimitive) {
                field = new Field(data, metafield.InternalClone());
            } else {
                fixed (byte* pData = data) {
                    PrimitiveValue* pValue = (PrimitiveValue*) pData;

                    if (metafield.Size == sizeof(short)) {
                        //
                        // map to 16-bit integral
                        //
                        pValue->AsInt16 = IPAddress.NetworkToHostOrder(pValue->AsInt16);
                        field = new Field(metafield.Id, pValue->AsInt16);
                    } else if (metafield.Size == sizeof(long)) {
                        //
                        // map to 64-bit integral/floating point
                        //
                        pValue->AsInt64 = IPAddress.NetworkToHostOrder(pValue->AsInt64);
                        field = metafield.Type == FieldType.Integral
                            ? new Field(metafield.Id, pValue->AsInt64)
                            : new Field(metafield.Id, pValue->AsDouble);
                    } else {
                        //
                        // map to 32-bit integral/floating point
                        //
                        pValue->AsInt32 = IPAddress.NetworkToHostOrder(pValue->AsInt32);
                        field = metafield.Type == FieldType.Integral
                            ? new Field(metafield.Id, pValue->AsInt32)
                            : new Field(metafield.Id, pValue->AsSingle);
                    }
                }
            }

            return field;
        }

        /// <summary>
        /// Unmaps the specified <see cref="Gekkota.Net.Field" /> into an array
        /// of bytes.
        /// </summary>
        /// <param name="field">
        /// The <see cref="Gekkota.Net.Field" /> to unmap.
        /// </param>
        /// <returns>
        /// A <see cref="System.Byte" /> array that contains the unmapped
        /// <see cref="Gekkota.Net.Field" />.
        /// </returns>
        private static unsafe byte[] Unmap(Field field)
        {
            if (field.Size == sizeof(byte) || !field.IsPrimitive) {
                return field.ValueAsByteArray;
            }

            byte[] data = new byte[field.Size];

            fixed (byte* pData = data) {
                PrimitiveValue* pValue = (PrimitiveValue*) pData;

                if (field.Size == sizeof(short)) {
                    //
                    // unmap from 16-bit integral
                    //
                    pValue->AsInt16 = IPAddress.HostToNetworkOrder(field.ValueAsInt16);
                } else if (field.Size == sizeof(long)) {
                    //
                    // unmap from 64-bit integral/floating point
                    //
                    pValue->AsInt64 = IPAddress.HostToNetworkOrder(field.ValueAsInt64);
                } else {
                    //
                    // unmap from 32-bit integral/floating point
                    //
                    pValue->AsInt32 = IPAddress.HostToNetworkOrder(field.ValueAsInt32);
                }
            }
  
            return data;
        }
        #endregion private methods
    }
}
