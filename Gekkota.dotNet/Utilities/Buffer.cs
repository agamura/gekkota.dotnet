//------------------------------------------------------------------------------
// <sourcefile name="Buffer.cs" language="C#" begin="10/23/2005">
//
//     <author name="Giuseppe Greco" email="giuseppe.greco@agamura.com" />
//
//     <copyright company="Agamura" url="http://www.agamura.com">
//         Copyright (C) 2005 Agamura, Inc.  All rights reserved.
//     </copyright>
//
// </sourcefile>
//------------------------------------------------------------------------------

using System;

namespace Gekkota.Utilities
{
    /// <summary>
    /// Provides methods for manipulating arrays of bytes.
    /// </summary>
    internal static class Buffer
    {
        /// <summary>
        /// Determines whether or not the specified
        /// <see cref="System.Byte" /> arrays are equal.
        /// </summary>
        /// <param name="array1">
        /// The <see cref="System.Byte" /> array to compare with
        /// <paramref name="array2" />.
        /// </param>
        /// <param name="array2">
        /// The <see cref="System.Byte" /> array to compare with
        /// <paramref name="array1" />.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="array1" /> is equal to
        /// <paramref name="array2" />; otherwise, <see langword="false" />.
        /// </returns>
        public static unsafe bool Equals(byte[] array1, byte[] array2)
        {
            if ((array1 as object) == (array2 as object)) {
                return true;
            }

            if ((array1 as object) == null || (array2 as object) == null) {
                return false;
            }

            if (array1.Length != array2.Length) {
                return false;        
            }

            fixed (byte* pArray1 = array1, pArray2 = array2) {
                byte* p1 = pArray1;
                byte* p2 = pArray2;

                //
                // loop over the count in blocks of 8 bytes, checking a
                // long integer (8 bytes) at a time
                //
                int count = array1.Length / sizeof(long);
                for (int i = 0 ; i < count; i++) {
                    if (*((long*) p1) != *((long*) p2)) {
                        return false;
                    }

                    p1 += sizeof(long);
                    p2 += sizeof(long);
                }

                //
                // complete the check by comparing any bytes that weren't
                // compared in blocks of 8
                //
                count = array1.Length % sizeof(long);
                for (int i = 0; i < count; i++) {
                    if (*p1 != *p2) {
                        return false;
                    }

                    p1++;
                    p2++;
                }
            }

            return true;
        }
    }
}
