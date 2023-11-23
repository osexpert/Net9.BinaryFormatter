using System.ComponentModel;
using System.Reflection;

namespace Net9.BinaryFormatter
{
    internal class SerializeHelper
    {
        internal static bool IsISerializable(object obj)
        {
            //obj is ISerializable
            return obj is ISerializable; // the Net9 variant
        }

        internal static void GetObjectData(object obj, SerializationInfo si, StreamingContext context)
        {
            ((ISerializable)obj).GetObjectData(si, context);
        }

        internal static bool IsNotSerialized(FieldInfo field)
        {
            //public bool IsNotSerialized => (Attributes & FieldAttributes.NotSerialized) != 0;

            //if (!field.IsNotSerialized)
            //return (Attributes & FieldAttributes.NotSerialized) != 0;
            //     public bool IsNotSerialized => (Attributes & FieldAttributes.NotSerialized) != 0;

            //return IsNotSerialized(field.Attributes);// field.IsNotSerialized;
            return field.GetCustomAttribute<NonSerializedAttribute>() != null;
        }
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

  

}
