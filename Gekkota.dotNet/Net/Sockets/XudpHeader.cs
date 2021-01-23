//------------------------------------------------------------------------------
// <sourcefile name="XudpHeader.cs" language="C#" begin="07/07/2005">
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
    internal struct XudpHeader
    {
        #region public fields
        public ushort     ProtocolId;     // XUDP hash code
        public byte       Version;        // major (MSB), minor (LSB)
        public byte       MessageType;    // one of the XudpMessageType values
        public byte       Priority;       // one of the DatagramPriority values
        public byte       Wild;           // unused
        public ushort     Flags;          // any combination of the XudpFlag values
        public ushort     SequenceNumber; // message sequence number
        public ushort     Checksum;       // checksum (see XudpFlag)
        #endregion public fields
    }
}
