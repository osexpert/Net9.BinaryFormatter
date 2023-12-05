using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
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

    public class AllowedTypesBinder : SerializationBinder
    {
        /// <summary>
        /// Key: (type.Assembly.FullName, type.Name)
        /// </summary>
        Dictionary<(string?, string), Type> AllowedTypes { get; }

        public AllowedTypesBinder(bool addDefaultTypes = true)
        {
            if (addDefaultTypes)
            {
                var ats = SerializeAllowedTypes.GetDefaultAllowedTypes();
                AllowedTypes = ats.ToDictionary(t => (t.Assembly.FullName, t.FullName!));
            }
            else
            {
                AllowedTypes = new();
            }
        }

        public override Type? BindToType(string assemblyName, string typeName)
        {
            // Problem: version is in the assembly name:
            // System.Private.CoreLib, Version = 9.0.0.0, Culture = neutral, PublicKeyToken = 7cec85d7bea7798e
            // So would need to to ignore the version??

            if (AllowedTypes.TryGetValue((assemblyName, typeName), out var type))
            {
                return type;
            }
            throw new Exception($"Not allowed to load type '{assemblyName}'.'{typeName}'");
        }

        public void AddAllowedType(Type t)
        {
            AllowedTypes[(t.Assembly.FullName, t.FullName!)] = t;
        }
    }

}
