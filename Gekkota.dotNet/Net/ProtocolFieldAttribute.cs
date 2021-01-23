//------------------------------------------------------------------------------
// <sourcefile name="ProtocolFieldAttribute.cs" language="C#" begin="02/20/2004">
//
//     <author name="Giuseppe Greco" email="giuseppe.greco@agamura.com" />
//
//     <copyright company="Agamura" url="http://www.agamura.com">
//         Copyright (C) 2004 Agamura, Inc.  All rights reserved.
//     </copyright>
//
// </sourcefile>
//------------------------------------------------------------------------------

using System;
using Gekkota.Properties;

namespace Gekkota.Net
{
    /// <summary>
    /// Indicates that a <see cref="Gekkota.Net.Field" /> is a protocol field.
    /// </summary>
    /// <remarks>
    /// Before sending a <see cref="Gekkota.Net.Datagram" />, the
    /// <see cref="Gekkota.Net.Sockets.IPClient" /> validates it by comparing
    /// its fields to the field descriptors defined for the current protocol
    /// implementation, and since the caller do not care about protocol
    /// headers, this attribute should be applied to payload fields only.
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property,
        AllowMultiple = false,
        Inherited = false)]
    public class ProtocolFieldAttribute : Attribute
    {
        #region private fields
        private int position;
        #endregion private fields

        #region public constructors
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Gekkota.Net.ProtocolFieldAttribute" />class with the
        /// specified position.
        /// </summary>
        /// <param name="position">
        /// An <see cref="System.Int32" /> that represents the zero-based
        /// position of the <see cref="Gekkota.Net.Field" /> in the protocol.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="position" /> is less than 0.
        /// </exception>
        public ProtocolFieldAttribute(int position)
        {
            if (position < 0) {
                throw new ArgumentOutOfRangeException("position", position,
                    Resources.Error_NonNegativeNumberRequired);
            }

            this.position = position;
        }
        #endregion public constructors

        #region public properties
        /// <summary>
        /// Gets the position of the <see cref="Gekkota.Net.Field" /> in the
        /// protocol.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int32" /> that represents the zero-based
        /// position of the <see cref="Gekkota.Net.Field" /> in the protocol.
        /// </value>
        public int Position
        {
            get { return position; }
        }
        #endregion public properties
    }
}
