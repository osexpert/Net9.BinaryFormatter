using System.ComponentModel;
using System.Reflection;

namespace Net9.BinaryFormatter
{
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
