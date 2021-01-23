//------------------------------------------------------------------------------
// <sourcefile name="HashCodeGenerator.cs" language="C#" begin="10/31/2004">
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
using System.Text;

namespace Gekkota
{
    /// <summary>
    /// Provides functionality for generating 16-bit hash codes from character
    /// strings.
    /// </summary>
    /// <remarks>
    /// <see cref="Gekkota.HashCodeGenerator" /> implements the PJ Weinberger's
    /// rolling shift algorithm.
    /// </remarks>
    public class HashCodeGenerator
    {
        #region public methods
        /// <summary>
        /// Generates a 16-bit hash code from the specified <see cref="System.String" />.
        /// </summary>
        /// <param name="value">
        /// The <see cref="System.String" /> from which to generate the hash code.
        /// </param>
        /// <returns>
        /// An <see cref="System.Int16" /> that represents the generated hash
        /// code. If <paramref name="value" /> is <see langword="null" /> or its
        /// length is 0, then this method returns 0.
        /// </returns>
        /// <example>
        /// The following example demonstrates the <c>Generate</c> method using
        /// the input character string as it is.
        /// <code>
        /// <![CDATA[
        /// int hashCode = HashCodeGenerator("Red");
        /// ]]>
        /// </code>
        /// </example>
        public static short Generate(string value)
        {
            return Generate(value, CasingRule.None);
        }

        /// <summary>
        /// Generates a 16-bit hash code from the specified <see cref="System.String" />
        /// using the specified casing rule.
        /// </summary>
        /// <param name="value">
        /// The <see cref="System.String" /> from which to generate the hash code.
        /// </param>
        /// <param name="casingRule">
        /// One of the <see cref="Gekkota.CasingRule" /> values.
        /// </param>
        /// <returns>
        /// An <see cref="System.Int16" /> that represents the generated hash
        /// code. If <paramref name="source" /> is <see langword="null" /> or its
        /// length is 0, then this method returns 0.
        /// </returns>
        /// <example>
        /// The following example demonstrates the <c>Generate</c> method using
        /// the input character string in uppercase.
        /// <code>
        /// <![CDATA[
        /// int hashCode = HashCodeGenerator("Red", CasingRule.ToUpper);
        /// ]]>
        /// </code>
        /// </example>
        public static short Generate(string value, CasingRule casingRule)
        {
            if (value == null || value.Length == 0) {
                return 0;
            }

            switch (casingRule) {
                case CasingRule.ToLower:
                    value = value.ToLower();
                    break;

                case CasingRule.ToUpper:
                    value = value.ToUpper();
                    break;

                default:
                    break;
            }

            ushort shift = 0;
            ushort hashCode = 0;
            byte[] data = Encoding.ASCII.GetBytes(value);

            for (int i = 0; i < data.Length; i++) {
                hashCode = (ushort) ((hashCode << 4) + data[i]);
                shift = (ushort) (hashCode & 0xF000);

                if (shift != 0) {
                    hashCode ^= (ushort) (shift >> 8);
                    hashCode ^= shift;
                }
            }

            return unchecked((short) hashCode);
        }
        #endregion public methods
    }
}
