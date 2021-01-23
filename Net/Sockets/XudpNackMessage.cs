// XudpNackMessage.cs
//
// Begin:  July 27, 2005
// Author: Giuseppe Greco <giuseppe.greco@agamura.com>
//
// Copyright (C) 2005 Agamura, Inc. <http://www.agamura.com>
// All rights reserved.

using System.Runtime.InteropServices;

namespace Gekkota.Net.Sockets
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct XudpNackMessage
    {
        #region public fields
        public XudpHeader Header; // XUDP header

        public byte[] NackBitmap; // bitmap where bits set to 1 represent
                                  // received messages and bits set to 0
                                  // represent lost messages
        #endregion public fields
    }
}
