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
        /// <summary>
        /// If set, ovverides the default IsSerializable.
        /// </summary>
        public IsSerializableHandlers? IsSerializableHandlers { get; set; }

        /// <summary>
        /// If set, ovverides the default IsNotSerialized.
        /// </summary>
        public IsNotSerializedHandlers? IsNotSerializedHandlers { get; set; }

        //internal static readonly Type s_typeofISerializable = typeof(ISerializable);

        public static readonly SerializationControl Default = new();

        public virtual bool IsSerializable(Type type)
        {
            if (IsSerializableHandlers != null)
                return IsSerializableHandlers.IsSerializable(type);
            else
                return SerializeByAttribute.IsSerializableStatic(type);
        }

        public virtual bool IsNotSerialized(FieldInfo field)
        {
            if (IsNotSerializedHandlers != null)
                return IsNotSerializedHandlers.IsNotSerialized(field);
            else
                return NotSerializedByAttribute.IsNotSerializedStatic(field);
        }

    }

}
