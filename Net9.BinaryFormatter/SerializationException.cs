// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Net9.BinaryFormatter
{
    [Serializable]
//    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")] // HMMM??
    public class SerializationException : SystemException
    {
        /// <summary>
        /// Creates a new SerializationException with its message
        /// string set to a default message.
        /// </summary>
        public SerializationException()
            : base(SR.SerializationException)
        {
            HResult = HResults.COR_E_SERIALIZATION;
        }

        public SerializationException(string? message)
            : base(message)
        {
            HResult = HResults.COR_E_SERIALIZATION;
        }

        public SerializationException(string? message, Exception? innerException)
            : base(message, innerException)
        {
            HResult = HResults.COR_E_SERIALIZATION;
        }

//        [EditorBrowsable(EditorBrowsableState.Never)]
//        protected SerializationException(SerializationInfo info, StreamingContext context)
//#pragma warning disable SYSLIB0051 // Type or member is obsolete
//            : base(GetBaseInfo(info), GetBaseContext(context))
//#pragma warning restore SYSLIB0051 // Type or member is obsolete
//        {
//        }
    }

    internal class HResults
    {
        internal const int COR_E_SERIALIZATION = unchecked((int)0x8013150C);
    }
}
