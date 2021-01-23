//------------------------------------------------------------------------------
// <sourcefile name="XudpFlag.cs" language="C#" begin="07/07/2005">
//
//     <author name="Giuseppe Greco" email="giuseppe.greco@agamura.com" />
//
//     <copyright company="Agamura" url="http://www.agamura.com">
//         Copyright (C) 2005 Agamura, Inc.  All rights reserved.
//     </copyright>
//
// </sourcefile>
//------------------------------------------------------------------------------

namespace Gekkota.Net.Sockets
{
    #region internal enums
    internal enum XudpFlag : ushort
    {
        Reliable    = 0x0001, // transmission is reliable
        Sequenced   = 0x0002, // transmission is sequenced
        Compressed  = 0x0004, // transmission is compressed
        Encrypted   = 0x0008, // transmission is encrypted

        Checksum    = 0x0010, // indicates whether the checksum is calculated on
                              // just the header or the header and the body
    }
    #endregion internal enums
}
