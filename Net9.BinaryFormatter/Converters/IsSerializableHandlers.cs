using System;
using System.Buffers.Text;
using System.ComponentModel;
using System.Reflection;

namespace Net9.BinaryFormatter
{

    public class IsSerializableHandlers //: IIsSerializable
    {
        public List<IIsSerializable> Handlers { get; }

        public bool IsSerializable(Type type)
        {
            foreach (var h in Handlers)
                if (h.IsSerializable(type))
                    return true;

            return false;
        }

        public IsSerializableHandlers(bool addDefaultHandlers = true)
        {
            if (addDefaultHandlers)
            {
                Handlers = GetDefaultHandlers();
            }
            else
            {
                Handlers = new();
            }
        }

        /// <summary>
        /// Return a list of new default instances of the the default handlers (new instances made every time GetDefaultHandlers is called)
        /// </summary>
        /// <returns></returns>
        public static List<IIsSerializable> GetDefaultHandlers()
        {
            var res = new List<IIsSerializable>();
            res.Add(new SerializePrimitiveTypes());
            res.Add(new SerializeAllowedTypes());
            res.Add(new SerializeByAttribute());
       //     res.Add(new SerializeExceptions());
            return res;
        }
    }


    public interface IIsSerializable
    {
        bool IsSerializable(Type type);
    }

    public class SerializeByAttribute : IIsSerializable
    {
        /*
         * No point:
         * Delegates are unsupported anyways
         *    public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new PlatformNotSupportedException(SR.Serialization_DelegatesNotSupported);
        }
         * 
         */
        //        public bool DelegateIsSerializable { get; set; } = false;

        /// <summary>
        /// Orginal implementation in net8
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsSerializable(Type type)
        {
            return IsSerializableStatic(type);//, DelegateIsSerializable);
        }

        /// <summary>
        /// Based on Type.IsSerializable()
        /// </summary>
        /// <param name="type"></param>
        /// <param name="delegateIsSerializable"></param>
        /// <returns></returns>
        public static bool IsSerializableStatic(Type type)//, bool delegateIsSerializable)
        {
            // Based on Type.IsSerializable()

            // Original code used TypeAttributes.Serializable
            if (type.GetCustomAttribute<SerializableAttribute>() != null)
                return true;

            //weird case?
            // FIXME: any reason to not use Type.IsAssignableTo? Why walk bases manually?
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
    }

    //public class DefaultIsSerializable //: IIsSerializable
    //{
    //    //public static readonly DefaultIsSerializable Instance = new DefaultIsSerializable();

    //    //public bool DelegateIsSerializable { get; set; } = false;

    //    //        bool IIsSerializable.IsSerializable(Type type) => IsSerializable(type);

    //    public static bool IsSerializable(Type type)
    //    {
    //        return SerializableByAttribute.IsSerializableStatic(type);//, delegateIsSerializable: false);
    //    }
    //}


    public class SerializePrimitiveTypes : IIsSerializable
    {
        public bool IsSerializable(Type type) => type.IsPrimitive;
    }

    public class SerializeAllowedTypes : IIsSerializable
    {
        public HashSet<Type> AllowedTypes { get; }

        public SerializeAllowedTypes(bool addDefaultTypes = true)
        {
            if (addDefaultTypes)
            {
                AllowedTypes = GetDefaultAllowedTypes();
            }
            else
            {
                AllowedTypes = new();
            }
        }

        public static HashSet<Type> GetDefaultAllowedTypes()
        {
            HashSet<Type> res = new();
            res.Add(typeof(Version));
            res.Add(typeof(ValueType));
            res.Add(typeof(DateTime));
            res.Add(typeof(DateTimeOffset));
            res.Add(typeof(TimeSpan));
            res.Add(typeof(TimeOnly));
            res.Add(typeof(DateOnly));
            res.Add(typeof(string));

            res.Add(typeof(List<>));
            res.Add(typeof(Stack<>));
            res.Add(typeof(KeyValuePair<,>));
            res.Add(typeof(Nullable<>));

            res.Add(typeof(ValueTuple<>));
            res.Add(typeof(ValueTuple<,>));
            res.Add(typeof(ValueTuple<,,>));
            res.Add(typeof(ValueTuple<,,,>));
            res.Add(typeof(ValueTuple<,,,,>));
            res.Add(typeof(ValueTuple<,,,,,>));
            res.Add(typeof(ValueTuple<,,,,,,>));
            res.Add(typeof(ValueTuple<,,,,,,,>));

            res.Add(typeof(Tuple<>));
            res.Add(typeof(Tuple<,>));
            res.Add(typeof(Tuple<,,>));
            res.Add(typeof(Tuple<,,,>));
            res.Add(typeof(Tuple<,,,,>));
            res.Add(typeof(Tuple<,,,,,>));
            res.Add(typeof(Tuple<,,,,,,>));
            res.Add(typeof(Tuple<,,,,,,,>));

            return res;
        }

        public bool IsSerializable(Type type)
        {
            //if (type.IsPrimitive)
            //    return true;

            if (AllowedTypes.Contains(type))
                return true;

            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                if (AllowedTypes.Contains(type.GetGenericTypeDefinition()))
                    return true;
            }

            return false;
        }
    }


    //public class SerializeExceptions : IIsSerializable
    //{
    //    public bool IsSerializable(Type type)
    //    {
    //        return type.IsAssignableTo(typeof(Exception));
    //    }
    //}

    //public class DefaultIsNotSerialized
    //{
    //    //public static readonly DefaultIsNotSerialized Instance = new DefaultIsNotSerialized();

    //    public static bool IsNotSerialized(FieldInfo field)
    //    {
    //        return field.GetCustomAttribute<NonSerializedAttribute>() != null;
    //    }
    //}

}
