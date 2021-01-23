//------------------------------------------------------------------------------
// <sourcefile name="BandwidthManager.cs" language="C#" begin="06/19/2003">
//
//     <author name="Giuseppe Greco" email="giuseppe.greco@agamura.com" />
//
//     <copyright company="Agamura" url="http://www.agamura.com">
//         Copyright (C) 2003 Agamura, Inc.  All rights reserved.
//     </copyright>
//
// </sourcefile>
//------------------------------------------------------------------------------

using System;
using System.Threading;
using Gekkota.Properties;

namespace Gekkota.Net.Sockets
{
    /// <summary>
    /// Provides functionaltiy for managing bandwidth usage. This class cannot
    /// be inherited.
    /// </summary>
    internal class BandwidthManager
    {
        #region private fields
        private DateTime lastTime;
        private long bandwidth;
        private long overTicks;
        private int lastBits;
        #endregion private fields

        #region public properties
        /// <summary>
        /// Gets or sets the bandwidth.
        /// </summary>
        /// <value>
        /// An <see cref="System.Int64" /> that represents the number of bits
        /// that can pass through per second.
        /// </value>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The specified value is less than 0
        /// </exception>
        public long Bandwidth
        {
            get { return bandwidth; }
            set {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException(
                        "value", value, Resources.Error_NonNegativeNumberRequired);
                }

                if (value != Gekkota.Net.Sockets.Bandwidth.Full) {
                    lastTime = DateTime.Now;
                }

                bandwidth = value;
                overTicks = 0;
                lastBits = 0;
            }
        }
        #endregion public properties

        #region public constructors
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Gekkota.Net.Sockets.BandwidthManager" /> class.
        /// </summary>
        /// <remarks>
        /// This constructor sets <see cref="Gekkota.Net.Sockets.BandwidthManager.Bandwidth" />
        /// to <see cref="Gekkota.Net.Sockets.Bandwidth.Full" />.
        /// </remarks>
        public BandwidthManager() : this(Gekkota.Net.Sockets.Bandwidth.Full)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gekkota.Net.Sockets.BandwidthManager" />
        /// class with the specified bandwidth.
        /// </summary>
        /// <param name="bandwidth">
        /// An <see cref="System.Int64" /> that represents the number of bits
        /// that can pass through per second.
        /// </param>
        public BandwidthManager(long bandwidth)
        {
            Bandwidth = bandwidth;
        }
        #endregion public constructors

        #region public methods
        /// <summary>
        /// Suspends the current thread for the time window required to respect
        /// the set bandwidth. The time window is calculated in function of the
        /// given buffer length and the set bandwidth.
        /// </summary>
        /// <param name="bufferLength">
        /// An <see cref="System.Int32" /> that represents the buffer length, in
        /// bytes, for which the set bandwidth should be respected.
        /// </param>
        /// <returns>
        /// An <see cref="System.Int64" /> that represents the number of ticks
        /// the current thread has been suspended. 1 tick corresponds to 10e-7
        /// seconds.
        /// </returns>
        public long Throttle(int bufferLength)
        {
            if (bandwidth == Gekkota.Net.Sockets.Bandwidth.Full) {
                return 0;
            }

            //
            // ticks since last pass through
            //
            DateTime currentTime = DateTime.Now;
            long elapsed = currentTime.Ticks - lastTime.Ticks;
            lastTime = currentTime;

            //
            // ticks required to perform last pass through
            //
            long delta = (long) (((double) lastBits / (double) bandwidth) * 10e6)
                       + overTicks;

            //
            // convert from bytes to bits
            //
            lastBits = bufferLength << 3;

            overTicks = 0;
            long ticks = 0;

            if (delta > elapsed) {
                ticks = delta - elapsed;

                //
                // 1 tick = 10e-5 ms
                //
                DateTime beginTime = DateTime.Now;
                Thread.Sleep((int) (ticks * 10e-5));

                //
                // detect eventual delay in resuming the current thread;
                // delays are recovered in the next pass through
                //
                overTicks = DateTime.Now.Ticks - beginTime.Ticks - ticks;
            }
       
            return ticks + overTicks;
        }
        #endregion public methods
    }
}
