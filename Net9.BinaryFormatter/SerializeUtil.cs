using System.Reflection;

namespace Net9.BinaryFormatter
{
    internal class SerializeUtil
    {
        internal static bool IsSerializable(Type type)
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

#pragma warning disable SYSLIB0050 // Type or member is obsolete
            if ((type.Attributes & TypeAttributes.Serializable) != 0)
                return true;
#pragma warning restore SYSLIB0050 // Type or member is obsolete


            Type? underlyingType = type.UnderlyingSystemType;
            if (IsRuntimeType(underlyingType))// is RuntimeType)
            {
                do
                {
                    // In all sane cases we only need to compare the direct level base type with
                    // System.Enum and System.MulticastDelegate. However, a generic parameter can
                    // have a base type constraint that is Delegate or even a real delegate type.
                    // Let's maintain compatibility and return true for them.
                    if ((underlyingType == typeof(Delegate) && Hardening.DelegateIsSerializable) || underlyingType == typeof(Enum))
                        return true;



                    underlyingType = underlyingType.BaseType;
                }
                while (underlyingType != null);
            }

            return false;


            //_objectType.IsSerializable
            //throw new NotImplementedException();
        }

        internal static bool IsISerializable(object obj)
        {
            //obj is ISerializable
            //throw new NotImplementedException();
            return obj is ISerializable; // the Net9 variant
        }

        internal static void GetObjectData(object obj, SerializationInfo si, StreamingContext context)
        {
            ((ISerializable)obj).GetObjectData(si, context);
            //throw new NotImplementedException();
        }

        internal static bool IsNotSerialized(FieldInfo field)
        {
            //            [Obsolete(Obsoletions.LegacyFormatterMessage, DiagnosticId = Obsoletions.LegacyFormatterDiagId, UrlFormat = Obsoletions.SharedUrlFormat)]
            //public bool IsNotSerialized => (Attributes & FieldAttributes.NotSerialized) != 0;

            //if (!field.IsNotSerialized)
            //throw new NotImplementedException();
            //return (Attributes & FieldAttributes.NotSerialized) != 0;
            //     public bool IsNotSerialized => (Attributes & FieldAttributes.NotSerialized) != 0;

            return IsNotSerialized(field.Attributes);// field.IsNotSerialized;
        }

        internal static bool IsNotSerialized(FieldAttributes attributes)
        {
            //if ((fields[i].Attributes & FieldAttributes.NotSerialized) == FieldAttributes.NotSerialized)

            //throw new NotImplementedException();
#pragma warning disable SYSLIB0050 // Type or member is obsolete
            return ((attributes & FieldAttributes.NotSerialized) == FieldAttributes.NotSerialized);
#pragma warning restore SYSLIB0050 // Type or member is obsolete
        }

        static readonly Type _runtimeType = Type.GetType("System.RuntimeType") ?? throw new Exception("System.RuntimeType not found");
        internal static bool IsRuntimeType(Type type)
        {
            return type.IsAssignableFrom(_runtimeType);
        }
    }
}
