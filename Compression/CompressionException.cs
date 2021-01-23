//------------------------------------------------------------------------------
// <sourcefile name="CompressionException.cs" language="C#" begin="11/06/2005">
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
using System.Runtime.Serialization;
using Gekkota.Properties;

namespace Gekkota.Compression
{
    /// <summary>
    /// The exception thrown when an error occurs during compression or
    /// decompression.
    /// </summary>
    [Serializable]
    public class CompressionException : SystemException
    {
        #region public constructors
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Gekkota.Compression.CompressionException" /> class.
        /// </summary>
        public CompressionException()
            : base(Resources.Error_CompressionError)
        {
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Gekkota.Compression.CompressionException" /> class
        /// with the specified message.
        /// </summary>
        /// <param name="message">
        /// A <see cref="System.String" /> that contains the reason why the
        /// exception occurred.
        /// </param>
        public CompressionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Gekkota.Compression.CompressionException" /> class
        /// with the specified message and a reference to the inner exception
        /// that is the cause of this exception.
        /// </summary>
        /// <param name="message">
        /// A <see cref="System.String" /> that contains the reason why the
        /// exception occurred.
        /// </param>
        /// <param name="inner">
        /// The <see cref="System.Exception" /> that is the cause of the current
        /// exception. If <paramref name="inner" /> is not <see langword="null" />,
        /// the current exception is raised in a catch block that handles the inner
        /// exception. 
        /// </param>
        public CompressionException(string message, Exception inner)
            : base(message, inner)
		{
        }
        #endregion public constructors

        #region protected constructors
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Gekkota.Compression.CompressionException" /> class from
        /// serialized data.
        /// </summary>
        /// <param name="info">
        /// A <see cref="System.Runtime.Serialization.SerializationInfo" /> that
        /// represents the serialization information object holding the
        /// serialized object data in the name-value form.
        /// </param>
        /// <param name="context">
        /// A <see cref="System.Runtime.Serialization.StreamingContext" /> that
        /// represents the contextual information about the source or
        /// destination of the exception.
        /// </param>
        protected CompressionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion protected constructors
    }
}
