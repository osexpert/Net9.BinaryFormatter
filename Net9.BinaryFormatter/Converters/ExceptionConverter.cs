using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Net9.BinaryFormatter.Converters
{
    public class ExceptionConverter : BinaryConverter
    {
        public override bool CanConvert(Type type)
        {
            return type.IsAssignableTo(typeof(Exception));
        }

        public override void Serialize(object obj, SerializationInfo info, StreamingContext context)
        {
            var e = (Exception)obj;
            //return obj;
            info.AddValue("Message", e.Message);
        }

        public override object Deserialize(object obj, SerializationInfo info, StreamingContext context)
        {
            //throw new NotImplementedException();
            var e = (Exception)obj;

            ExceptionDispatchInfo.SetRemoteStackTrace(e, "lolzs");

            var f = typeof(Exception).GetField("_message", System.Reflection.BindingFlags.NonPublic| System.Reflection.BindingFlags.Instance)!;
            var m = info.GetString("Message");
            f.SetValue(e, m);

            return obj;

        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void SerializationException(SerializationInfo info, StreamingContext context)
    //        : base(
    //info.GetString("Message"),
    //(Exception?)(info.GetValue("InnerException", typeof(Exception))))
        {

            Exception e = null!;


            //ArgumentNullException.ThrowIfNull(info);

            //_message = info.GetString("Message"); // Do not rename (binary serialization)
            //_data = (IDictionary?)(info.GetValueNoThrow("Data", typeof(IDictionary))); // Do not rename (binary serialization)
            //_innerException = (Exception?)(info.GetValue("InnerException", typeof(Exception))); // Do not rename (binary serialization)
            //_helpURL = info.GetString("HelpURL"); // Do not rename (binary serialization)
            //_stackTraceString = info.GetString("StackTraceString"); // Do not rename (binary serialization)
            var ss = info.GetString("StackTraceString");
            if (ss != null)
                ExceptionDispatchInfo.SetRemoteStackTrace(e, ss);

            //_remoteStackTraceString = info.GetString("RemoteStackTraceString"); // Do not rename (binary serialization)
            //_HResult = info.GetInt32("HResult"); // Do not rename (binary serialization)
            //_source = info.GetString("Source"); // Do not rename (binary serialization)

            //RestoreRemoteStackTrace(info, context);

            ArgumentNullException.ThrowIfNull(info);

            //            Message = info.GetString("Message"); // Do not rename (binary serialization)
            var data = (IDictionary?)(info.GetValueNoThrow("Data", typeof(IDictionary))); // Do not rename (binary serialization)
            if (data != null)
            {
                var en = data.GetEnumerator();
                while (en.MoveNext())
                {
                    e.Data.Add(en.Key, en.Value);
                }
            }

            //            InnerException = (Exception?)(info.GetValue("InnerException", typeof(Exception))); // Do not rename (binary serialization)
            e.HelpLink = info.GetString("HelpURL"); // Do not rename (binary serialization)
                                                    //            _stackTraceString = info.GetString("StackTraceString"); // Do not rename (binary serialization)
                                                    //            _remoteStackTraceString = info.GetString("RemoteStackTraceString"); // Do not rename (binary serialization)
            e.HResult = info.GetInt32("HResult"); // Do not rename (binary serialization)
            e.Source = info.GetString("Source"); // Do not rename (binary serialization)

            RestoreRemoteStackTrace(info, context);
        }


        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Exception e = null!;

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
            info.AddValue("Message", e.Message, typeof(string)); // Do not rename (binary serialization)
            info.AddValue("Data", e.Data, typeof(IDictionary)); // Do not rename (binary serialization)
            info.AddValue("InnerException", e.InnerException, typeof(Exception)); // Do not rename (binary serialization)
            info.AddValue("HelpURL", e.HelpLink, typeof(string)); // Do not rename (binary serialization)

            //            ExceptionDispatchInfo.SetCurrentStackTrace
            info.AddValue("StackTraceString", e.StackTrace, typeof(string));
            //          info.AddValue("StackTraceString", SerializationStackTraceString, typeof(string)); // Do not rename (binary serialization)
            //info.AddValue("RemoteStackTraceString", _remoteStackTraceString, typeof(string)); // Do not rename (binary serialization)

            info.AddValue("RemoteStackIndex", 0, typeof(int)); // Do not rename (binary serialization)
            info.AddValue("ExceptionMethod", null, typeof(string)); // Do not rename (binary serialization)
            info.AddValue("HResult", e.HResult); // Do not rename (binary serialization)
            info.AddValue("Source", e.Source, typeof(string)); // Do not rename (binary serialization)
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
}
