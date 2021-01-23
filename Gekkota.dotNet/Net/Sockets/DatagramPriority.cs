//------------------------------------------------------------------------------
// <sourcefile name="DatagramPriority.cs" language="C#" begin="07/27/2005">
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
    #region public enums
    /// <summary>
    /// Defines <see cref="Gekkota.Net.Datagram" /> priority values.
    /// </summary>
    /// <seealso cref="Gekkota.Net.Datagram" />
    public enum DatagramPriority : byte
    {
        /// <summary>
        /// The <see cref="Gekkota.Net.Datagram" /> has highest priority.
        /// </summary>
        Highest = 0x01,

        /// <summary>
        /// The <see cref="Gekkota.Net.Datagram" /> has between highest and high
        /// priority.
        /// </summary>
        VeryHigh = 0x02,

        /// <summary>
        /// The <see cref="Gekkota.Net.Datagram" /> has high priority.
        /// </summary>
        High = 0x03,

        /// <summary>
        /// The <see cref="Gekkota.Net.Datagram" /> has between high and normal
        /// priority.
        /// </summary>
        AboveNormal = 0x04,

        /// <summary>
        /// The <see cref="Gekkota.Net.Datagram" /> has normal priority.
        /// </summary>
        Normal = 0x05,

        /// <summary>
        /// The <see cref="Gekkota.Net.Datagram" /> has low priority.
        /// </summary>
        Low = 0x06,

        /// <summary>
        /// The <see cref="Gekkota.Net.Datagram" /> has between low and lowest
        /// priority.
        /// </summary>
        VeryLow = 0x07,

        /// <summary>
        /// The <see cref="Gekkota.Net.Datagram" /> has lowest priority.
        /// </summary>
        Lowest = 0x08,
    }
    #endregion public enums
}
