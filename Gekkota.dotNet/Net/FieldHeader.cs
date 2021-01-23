//------------------------------------------------------------------------------
// <sourcefile name="FieldHeader.cs" language="C#" begin="03/18/2004">
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
    /// Represents a <see cref="Gekkota.Net.Field" /> header.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal struct FieldHeader
    {
        #region public fields
        public ushort         Id;
        public FieldType      Type;
        public ushort         Size;
        public FieldCategory  Category;
        #endregion public fields
    }
}
