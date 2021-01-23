//------------------------------------------------------------------------------
// <sourcefile name="XudpMessageType.cs" language="C#" begin="07/07/2005">
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
    internal enum XudpMessageType : byte
    {
        Data        = 0x00, // data message
        Sync        = 0X01, // synchronization message
        Nack        = 0x02, // negative acknowledgment message
        NackRequest = 0x03, // negative acknowledgment request message
        Reset       = 0x04, // flow-control reset message
        Null        = 0xFF, // null message
    }
    #endregion internal enums
}
