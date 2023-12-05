using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

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
        Dictionary<string, Type> AllowedTypes { get; } = new();
        Dictionary<string, Assembly> AllowedAssemblies { get; } = new();

        private bool _runtimeSpecialAccess;

        public AllowedTypesBinder(bool addDefaultTypes = true, bool runtimeSpecialAccess = true)
        {
            _runtimeSpecialAccess = runtimeSpecialAccess;

            if (addDefaultTypes)
            {
                var ats = SerializeAllowedTypes.GetDefaultAllowedTypes();

                foreach (var at in ats)
                    AddAllowedType(at);
                //                AllowedAssemblies = ats.Select(t => t.Assembly).Distinct().ToDictionary(a => a.GetName().Name!);
                //              AllowedTypes = ats.ToDictionary(t => t.FullName!);

                AddAllowedType(typeof(Dictionary<,>));

//                AllowedAssemblies[Converter.s_urtAlternativeAssembly.GetName().Name!] = Converter.s_urtAlternativeAssembly;
            }
            else
            {
                //AllowedTypes = new();
                //AllowedAssemblies = new();
            }
        }

        public override Type? BindToType(string assemblyName, string typeName)
        {
            // Problem: version is in the assembly name:
            // System.Private.CoreLib, Version = 9.0.0.0, Culture = neutral, PublicKeyToken = 7cec85d7bea7798e
            // So would need to to ignore the version??

            var assembly_qualified_name = $"{typeName}, {assemblyName}";

            var t = Type.GetType(assembly_qualified_name, ResolveAsm, ResolveType);

            //if (AllowedTypes.TryGetValue((assemblyName, typeName), out var type))
            //{
            //    return type;
            //}
            if (t == null) // shoul dnever get here?
                throw new Exception($"Not allowed to load type '{assembly_qualified_name}'");

            return t;
        }

        private Type? ResolveType(Assembly? assembly, string typeName, bool caseInsentitive)
        {
            if (caseInsentitive)
                throw new Exception("CI not supported");

            if (assembly == null)
                throw new Exception("Assembly is null?");

            if (AllowedTypes.TryGetValue(typeName, out var t))
            {
                if (t.Assembly == assembly)
                    return t;

                if (_runtimeSpecialAccess)
                {
                    var asm_runtime = assembly == Converter.s_urtAssembly || assembly == Converter.s_urtAlternativeAssembly;
                    var t_asm_runtime = t.Assembly == Converter.s_urtAssembly || t.Assembly == Converter.s_urtAlternativeAssembly;
                    if (asm_runtime && t_asm_runtime)
                        return t;
                }
            }

            throw new Exception($"Not allowed to load assembly '{typeName}'");
        }

        private Assembly? ResolveAsm(AssemblyName name)
        {
            if (AllowedAssemblies.TryGetValue(name.Name!, out var asm))
                return asm;

            if (_runtimeSpecialAccess)
            {
                if (name.Name == Converter.s_urtAssembly.GetName().Name)
                    return Converter.s_urtAssembly;
                if (name.Name == Converter.s_urtAlternativeAssembly.GetName().Name)
                    return Converter.s_urtAlternativeAssembly;
            }

            throw new Exception($"Not allowed to load assembly '{name}'");
        }

        public void AddAllowedType(Type t)
        {
            AllowedTypes[t.FullName!] = t;
            AllowedAssemblies[t.Assembly.GetName().Name!] = t.Assembly;
        }
    }

}
