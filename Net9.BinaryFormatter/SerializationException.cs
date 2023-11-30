// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
//using System.Runtime.ExceptionServices;

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
        //#pragma warning disable SYSLIB0051 // Type or member is obsolete
        //        protected SerializationException(SerializationInfo info, StreamingContext context) : base(GetBaseInfo(info), GetBaseContext(context))
        //#pragma warning restore SYSLIB0051 // Type or member is obsolete
        //        {

        //        }

        //        private static System.Runtime.Serialization.StreamingContext GetBaseContext(StreamingContext context)
        //        {
        //            //System.Runtime.Serialization.StreamingContextStates.CrossAppDomain
        //#pragma warning disable SYSLIB0050 // Type or member is obsolete
        //            return new System.Runtime.Serialization.StreamingContext((System.Runtime.Serialization.StreamingContextStates)context.State, context.Context);
        //#pragma warning restore SYSLIB0050 // Type or member is obsolete
        //        }

        //        private static System.Runtime.Serialization.SerializationInfo GetBaseInfo(SerializationInfo info)
        //        {
        //            var res = new System.Runtime.Serialization.SerializationInfo(info.ObjectType, info..);
        //        }





    }

    internal class HResults
    {
        internal const int COR_E_SERIALIZATION = unchecked((int)0x8013150C);
    }
}
