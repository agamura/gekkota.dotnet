//------------------------------------------------------------------------------
// <sourcefile name="ExceptionEventArgs.cs" language="C#" begin="03/16/2004">
//
//     <author name="Giuseppe Greco" email="giuseppe.greco@agamura.com" />
//
//     <copyright company="Agamura" url="http://www.agamura.com">
//         Copyright (C) 2004 Agamura, Inc.  All rights reserved.
//     </copyright>
//
// </sourcefile>
//------------------------------------------------------------------------------

using System;

namespace Gekkota.Net.Sockets
{
    /// <summary>
    /// Provides data for the
    /// <see cref="Gekkota.Net.Sockets.IPClient.Exception" /> event.
    /// </summary>
    public class ExceptionEventArgs : EventArgs
    {
        #region private fields
        private Exception exception;
        private DateTime timeStamp;
        #endregion private fields

        #region public properties
        /// <summary>
        /// Gets the raised Exception.
        /// </summary>
        /// <value>
        /// The raised <see cref="System.Exception" />.
        /// </value>
        public Exception Exception
        {
            get { return exception; }
        }

        /// <summary>
        /// Gets the time the <see cref="System.Exception" /> was raised.
        /// </summary>
        /// <value>
        /// A <see cref="System.DateTime" /> that represents the time the
        /// <see cref="System.Exception" /> was raised.
        /// </value>
        public DateTime TimeStamp
        {
            get { return timeStamp; }
        }
        #endregion public properties

        #region public constructors
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Gekkota.Net.Sockets.ExceptionEventArgs" /> class with the
        /// specified <see cref="System.Exception" />.
        /// </summary>
        /// <param name="exception">
        /// The raised <see cref="System.Exception" />.
        /// </param>
        public ExceptionEventArgs(Exception exception)
        {
            timeStamp = DateTime.Now;
            this.exception = exception;
        }
        #endregion public constructors
    }
}
