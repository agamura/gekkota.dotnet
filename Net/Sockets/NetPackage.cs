//------------------------------------------------------------------------------
// <sourcefile name="NetPackage.cs" language="C#" begin="08/11/2005">
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
using System.Net;

namespace Gekkota.Net.Sockets
{
    internal class NetPackage
    {
        #region private fields
        public Datagram Datagram;
        public IPEndPoint EndPoint;
        public DatagramPriority Priority;
        #endregion private fields

        #region public constructors
        public NetPackage(Datagram datagram)
            : this(datagram, null, DatagramPriority.Normal) {}
        public NetPackage(Datagram datagram, IPEndPoint endPoint)
            : this(datagram, endPoint, DatagramPriority.Normal) {}
        public NetPackage(Datagram datagram, IPEndPoint endPoint, DatagramPriority priority)
        {
            Datagram = datagram;
            EndPoint = endPoint;
            Priority = priority;
        }
        #endregion public constructors
    }
}
