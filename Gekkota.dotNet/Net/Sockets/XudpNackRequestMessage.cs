//------------------------------------------------------------------------------
// <sourcefile name="XudpNackRequestMessage.cs" language="C#" begin="07/27/2005">
//
//     <author name="Giuseppe Greco" email="giuseppe.greco@agamura.com" />
//
//     <copyright company="Agamura" url="http://www.agamura.com">
//         Copyright (C) 2005 Agamura, Inc.  All rights reserved.
//     </copyright>
//
// </sourcefile>
//------------------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace Gekkota.Net.Sockets
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal struct XudpNackRequestMessage
    {
        #region public fields
        public XudpHeader Header;       // XUDP header

        public ushort LowerNackNumber;  // the sequence number that identifies
                                        // the first message for which the
                                        // receiver is allowed to send back
                                        // negative acknowledgment

        public ushort UpperNackNumber;  // the sequence number that identifies
                                        // the last message for which the
                                        // receiver is allowed to send back
                                        // negative acknowledgment
        #endregion public fields
    }
}
