// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

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


        [EditorBrowsable(EditorBrowsableState.Never)]
        protected SerializationException(SerializationInfo info, StreamingContext context) : base(
            info.GetString("Message"),
            (Exception?)(info.GetValue("InnerException", typeof(Exception))))
        {
            //ArgumentNullException.ThrowIfNull(info);

            //_message = info.GetString("Message"); // Do not rename (binary serialization)
            //_data = (IDictionary?)(info.GetValueNoThrow("Data", typeof(IDictionary))); // Do not rename (binary serialization)
            //_innerException = (Exception?)(info.GetValue("InnerException", typeof(Exception))); // Do not rename (binary serialization)
            //_helpURL = info.GetString("HelpURL"); // Do not rename (binary serialization)
            //_stackTraceString = info.GetString("StackTraceString"); // Do not rename (binary serialization)
            //_remoteStackTraceString = info.GetString("RemoteStackTraceString"); // Do not rename (binary serialization)
            //_HResult = info.GetInt32("HResult"); // Do not rename (binary serialization)
            //_source = info.GetString("Source"); // Do not rename (binary serialization)

            //RestoreRemoteStackTrace(info, context);

            ArgumentNullException.ThrowIfNull(info);

            //            Message = info.GetString("Message"); // Do not rename (binary serialization)
            //            Data = (IDictionary?)(info.GetValueNoThrow("Data", typeof(IDictionary))); // Do not rename (binary serialization)
            //            InnerException = (Exception?)(info.GetValue("InnerException", typeof(Exception))); // Do not rename (binary serialization)
            HelpLink = info.GetString("HelpURL"); // Do not rename (binary serialization)
                                                  //            _stackTraceString = info.GetString("StackTraceString"); // Do not rename (binary serialization)
                                                  //            _remoteStackTraceString = info.GetString("RemoteStackTraceString"); // Do not rename (binary serialization)
            HResult = info.GetInt32("HResult"); // Do not rename (binary serialization)
            Source = info.GetString("Source"); // Do not rename (binary serialization)

            RestoreRemoteStackTrace(info, context);
        }


        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ArgumentNullException.ThrowIfNull(info);

            //_source ??= Source; // Set the Source information correctly before serialization

            //info.AddValue("ClassName", GetClassName(), typeof(string)); // Do not rename (binary serialization)
            //info.AddValue("Message", _message, typeof(string)); // Do not rename (binary serialization)
            //info.AddValue("Data", _data, typeof(IDictionary)); // Do not rename (binary serialization)
            //info.AddValue("InnerException", _innerException, typeof(Exception)); // Do not rename (binary serialization)
            //info.AddValue("HelpURL", _helpURL, typeof(string)); // Do not rename (binary serialization)
            //info.AddValue("StackTraceString", SerializationStackTraceString, typeof(string)); // Do not rename (binary serialization)
            //info.AddValue("RemoteStackTraceString", _remoteStackTraceString, typeof(string)); // Do not rename (binary serialization)
            //info.AddValue("RemoteStackIndex", 0, typeof(int)); // Do not rename (binary serialization)
            //info.AddValue("ExceptionMethod", null, typeof(string)); // Do not rename (binary serialization)
            //info.AddValue("HResult", _HResult); // Do not rename (binary serialization)
            //info.AddValue("Source", _source, typeof(string)); // Do not rename (binary serialization)
            //info.AddValue("WatsonBuckets", SerializationWatsonBuckets, typeof(byte[])); // Do not rename (binary serialization)

            //_source ??= Source; // Set the Source information correctly before serialization

            //            info.AddValue("ClassName", GetClassName(), typeof(string)); // Do not rename (binary serialization)
            info.AddValue("Message", Message, typeof(string)); // Do not rename (binary serialization)
            info.AddValue("Data", Data, typeof(IDictionary)); // Do not rename (binary serialization)
            info.AddValue("InnerException", InnerException, typeof(Exception)); // Do not rename (binary serialization)
            info.AddValue("HelpURL", HelpLink, typeof(string)); // Do not rename (binary serialization)
                                                                //          info.AddValue("StackTraceString", SerializationStackTraceString, typeof(string)); // Do not rename (binary serialization)
                                                                //            info.AddValue("RemoteStackTraceString", _remoteStackTraceString, typeof(string)); // Do not rename (binary serialization)
            info.AddValue("RemoteStackIndex", 0, typeof(int)); // Do not rename (binary serialization)
            info.AddValue("ExceptionMethod", null, typeof(string)); // Do not rename (binary serialization)
            info.AddValue("HResult", HResult); // Do not rename (binary serialization)
            info.AddValue("Source", Source, typeof(string)); // Do not rename (binary serialization)
                                                             //            info.AddValue("WatsonBuckets", SerializationWatsonBuckets, typeof(byte[])); // Do not rename (binary serialization)
        }

        void RestoreRemoteStackTrace(SerializationInfo info, StreamingContext context)
        {
            // Get the WatsonBuckets that were serialized - this is particularly
            // done to support exceptions going across AD transitions.
            //
            // We use the no throw version since we could be deserializing a pre-V4
            // exception object that may not have this entry. In such a case, we would
            // get null.
            //            _watsonBuckets = (byte[]?)info.GetValueNoThrow("WatsonBuckets", typeof(byte[])); // Do not rename (binary serialization)

            // If we are constructing a new exception after a cross-appdomain call...
            if (context.State == StreamingContextStates.CrossAppDomain)
            {
                // ...this new exception may get thrown.  It is logically a re-throw, but
                //  physically a brand-new exception.  Since the stack trace is cleared
                //  on a new exception, the "_remoteStackTraceString" is provided to
                //  effectively import a stack trace from a "remote" exception.  So,
                //  move the _stackTraceString into the _remoteStackTraceString.  Note
                //  that if there is an existing _remoteStackTraceString, it will be
                //  preserved at the head of the new string, so everything works as
                //  expected.
                // Even if this exception is NOT thrown, things will still work as expected
                //  because the StackTrace property returns the concatenation of the
                //  _remoteStackTraceString and the _stackTraceString.
                //                _remoteStackTraceString += _stackTraceString;
                //                _stackTraceString = null;
            }
        }


    }

    internal class HResults
    {
        internal const int COR_E_SERIALIZATION = unchecked((int)0x8013150C);
    }
}
