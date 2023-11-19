using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Net9.BinaryFormatter
{
    internal class SerializeUtil
    {
        internal static bool IsSerializable(Type objectType)
        {
        //    [Obsolete(Obsoletions.LegacyFormatterMessage, DiagnosticId = Obsoletions.LegacyFormatterDiagId, UrlFormat = Obsoletions.SharedUrlFormat)]
        //    public virtual bool IsSerializable
        //{
        //    get
        //    {
        //        if ((GetAttributeFlagsImpl() & TypeAttributes.Serializable) != 0)
        //            return true;

        //        Type? underlyingType = UnderlyingSystemType;
        //        if (underlyingType is RuntimeType)
        //        {
        //            do
        //            {
        //                // In all sane cases we only need to compare the direct level base type with
        //                // System.Enum and System.MulticastDelegate. However, a generic parameter can
        //                // have a base type constraint that is Delegate or even a real delegate type.
        //                // Let's maintain compatibility and return true for them.
        //                if (underlyingType == typeof(Delegate) || underlyingType == typeof(Enum))
        //                    return true;

        //                underlyingType = underlyingType.BaseType;
        //            }
        //            while (underlyingType != null);
        //        }

        //        return false;
        //    }
        //}


            //_objectType.IsSerializable
            throw new NotImplementedException();
        }

        internal static bool IsISerializable(object obj)
        {
            //obj is ISerializable
            throw new NotImplementedException();
        }

        internal static void GetObjectData(object obj, SerializationInfo si, StreamingContext context)
        {
            //((ISerializable)obj).GetObjectData(_si, context);
            throw new NotImplementedException();
        }

        internal static bool IsNotSerialized(FieldInfo field)
        {
//            [Obsolete(Obsoletions.LegacyFormatterMessage, DiagnosticId = Obsoletions.LegacyFormatterDiagId, UrlFormat = Obsoletions.SharedUrlFormat)]
            //public bool IsNotSerialized => (Attributes & FieldAttributes.NotSerialized) != 0;

            //if (!field.IsNotSerialized)
            throw new NotImplementedException();
        }

        internal static bool IsNotSerialized(FieldAttributes attributes)
        {
            //if ((fields[i].Attributes & FieldAttributes.NotSerialized) == FieldAttributes.NotSerialized)

            throw new NotImplementedException();
        }
    }
}
