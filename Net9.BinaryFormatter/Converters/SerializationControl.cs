using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Net9.BinaryFormatter
{
    public class SerializationControl
    {
        internal static readonly Type s_typeofISerializable = typeof(ISerializable);

        public static readonly SerializationControl Default = new();

        public virtual bool IsSerializable(Type type)
        {
            // Based on Type.IsSerializable()

            // Original code used TypeAttributes.Serializable
            if (type.GetCustomAttribute<SerializableAttribute>() != null)
                return true;

            //weird case?
            Type? underlyingType = type.UnderlyingSystemType;
            if (TypeHelper.IsRuntimeType(underlyingType))// is RuntimeType)
            {
                do
                {
                    // In all sane cases we only need to compare the direct level base type with
                    // System.Enum and System.MulticastDelegate. However, a generic parameter can
                    // have a base type constraint that is Delegate or even a real delegate type.
                    // Let's maintain compatibility and return true for them.
                    // SECURITY: made the delegate logic OPTIN
                    //if ((delegateIsSerializable && underlyingType == typeof(Delegate)) || underlyingType == typeof(Enum))
                    if (underlyingType == typeof(Enum))
                        return true;

                    underlyingType = underlyingType.BaseType;
                }
                while (underlyingType != null);
            }

            return false;
        }

        public virtual bool IsNotSerialized(FieldInfo field)
        {
            return field.GetCustomAttribute<NonSerializedAttribute>() != null;
        }

        public virtual bool IsISerializable(Type objectType)
        {
            return objectType.IsAssignableTo(s_typeofISerializable);
        }

        public virtual bool IsISerializable(object obj)
        {
            return obj is ISerializable;
        }

        public virtual void GetObjectData(object obj, SerializationInfo si, StreamingContext context)
        {
            ((ISerializable)obj).GetObjectData(si, context);
        }
    }

}
