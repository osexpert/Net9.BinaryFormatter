using System.ComponentModel;
using System.Reflection;

namespace Net9.BinaryFormatter
{
    internal class SerializeUtil
    {
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

            //return IsNotSerialized(field.Attributes);// field.IsNotSerialized;
            return field.GetCustomAttribute<NonSerializedAttribute>() != null;
        }

        //internal static bool IsNotSerialized(FieldAttributes attributes)
        //{
        //    //if ((fields[i].Attributes & FieldAttributes.NotSerialized) == FieldAttributes.NotSerialized)

        //    //throw new NotImplementedException();
        //    //#pragma warning disable SYSLIB0050 // Type or member is obsolete
        //    //          return ((attributes & FieldAttributes.NotSerialized) == FieldAttributes.NotSerialized);
        //    //#pragma warning restore SYSLIB0050 // Type or member is obsolete
        //    return attributes.GetCustomAttribute<NonSerializedAttribute>() != null;
        //}


    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate, Inherited = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class SerializableAttribute : Attribute
    {
        public SerializableAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class NonSerializedAttribute : Attribute
    {
        public NonSerializedAttribute()
        {
        }
    }

    public static class Net9Settings
    {
        //public static readonly Net9Settings Default = new Net9Settings();

        //public Net9Settings()
        //{
        //   // SetUrtAssemblies();
        //}

        //private void SetUrtAssemblies()
        //{
        //    s_urtAssembly = Assembly.Load("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
        //    s_urtAssemblyString = s_urtAssembly.FullName!;

        //    s_urtAlternativeAssembly = typeof(string).Assembly;
        //    s_urtAlternativeAssemblyString = s_urtAlternativeAssembly.FullName!;
        //}

        public static bool DelegateIsSerializable { get; set; } = false;

        //internal bool IsAnyUrtAssembly(string assemblyString)
        //{
        //    return assemblyString.Equals(s_urtAssemblyString) || assemblyString.Equals(s_urtAlternativeAssemblyString);
        //}

        //internal bool IsMainUrtAssembly(string assemblyString)
        //{
        //    return assemblyString.Equals(s_urtAssemblyString);
        //}

        //internal bool IsMainUrtAssembly(Assembly assembly)
        //{
        //    return assembly == s_urtAssembly;
        //}

        //internal BinaryAssemblyInfo GetMainUrtBinaryAssemblyInfo()
        //{
        //    return new BinaryAssemblyInfo(s_urtAssemblyString, s_urtAssembly);
        //}


        ////        public static bool EnableTypeGoodList { get; set; } = true;


        //// In .NET Framework the default assembly is mscorlib.dll --> typeof(string).Assembly.
        //// In Core type string lives in System.Private.Corelib.dll which doesn't
        //// contain all the types which are living in mscorlib in .NET Framework. Therefore we
        //// use our mscorlib facade which also contains manual type forwards for deserialization.
        //internal Assembly s_urtAssembly;// = Assembly.Load("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
        //////        internal static readonly Assembly s_urtAssembly = typeof(string).Assembly;// Assembly.Load("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
        //internal string s_urtAssemblyString;// = s_urtAssembly.FullName!;

        //internal Assembly s_urtAlternativeAssembly;// = s_typeofString.Assembly;
        //internal string s_urtAlternativeAssemblyString;// = s_urtAlternativeAssembly.FullName!;

    }

}
