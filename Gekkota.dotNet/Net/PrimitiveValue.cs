//------------------------------------------------------------------------------
// <sourcefile name="PrimitiveValue.cs" language="C#" begin="03/18/2004">
//
//     <author name="Giuseppe Greco" email="giuseppe.greco@agamura.com" />
//
//     <copyright company="Agamura" url="http://www.agamura.com">
//         Copyright (C) 2004 Agamura, Inc.  All rights reserved.
//     </copyright>
//
// </sourcefile>
//------------------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace Gekkota.Net
{
    /// <summary>
    /// Implements a struct for mapping/unmapping primitive values into/from
    /// raw memory.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct PrimitiveValue
    {
        #region public fields
        [FieldOffset(0)] public byte    AsByte;
        [FieldOffset(0)] public short   AsInt16;
        [FieldOffset(0)] public int     AsInt32;
        [FieldOffset(0)] public long    AsInt64;
        [FieldOffset(0)] public float   AsSingle;
        [FieldOffset(0)] public double  AsDouble;
        #endregion public fields
    }
}
