//------------------------------------------------------------------------------
// <sourcefile name="XudpSyncMessage.cs" language="C#" begin="07/27/2005">
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
    internal struct XudpSyncMessage
    {
        #region public fields
        public XudpHeader Header; // XUDP header
        public uint SyncNumber;   // random sync number
        #endregion public fields
    }
}
