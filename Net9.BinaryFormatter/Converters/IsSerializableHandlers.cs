using System.ComponentModel;
using System.Reflection;

namespace Net9.BinaryFormatter
{

    public class IsSerializableHandlers : IIsSerializable
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

        private static List<IIsSerializable> GetDefaultHandlers()
        {
            var res = new List<IIsSerializable>();
            res.Add(new SerializableByAttribute());
            res.Add(new SerializableGoodTypes());
            return res;
        }
    }


    public interface IIsSerializable
    {
        bool IsSerializable(Type type);
    }

    public class SerializableByAttribute : IIsSerializable
    {
        public bool DelegateIsSerializable { get; set; } = false;

        /// <summary>
        /// Orginal implementation in net8
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsSerializable(Type type)
        { 
            return IsSerializableStatic(type, DelegateIsSerializable);
        }

        internal static bool IsSerializableStatic(Type type, bool delegateIsSerializable)
        { 
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
                    if ((delegateIsSerializable && underlyingType == typeof(Delegate)) || underlyingType == typeof(Enum))
                        return true;

                    underlyingType = underlyingType.BaseType;
                }
                while (underlyingType != null);
            }

            return false;


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
        }
    }

    internal class DefaultIsSerializable : IIsSerializable
    {
        public static readonly DefaultIsSerializable Instance = new DefaultIsSerializable();

        //public bool DelegateIsSerializable { get; set; } = false;

        public bool IsSerializable(Type type)
        {
            return SerializableByAttribute.IsSerializableStatic(type, delegateIsSerializable: false);
        }
    }


    public class SerializableGoodTypes : IIsSerializable
    {
        HashSet<Type> _genericTypes = new();
        HashSet<Type> _types = new();

        //public bool DelegateIsSerializable { get; set; } = false;
        public void AddGoodType(Type type)
        {
            if (type.IsGenericType)
            {
                _genericTypes.Add(type);
            }
            else
            {
                _types.Add(type);
            }
        }

        public SerializableGoodTypes(bool addDefaultTypes = true)
        {
            if (addDefaultTypes)
            {
                _types.Add(typeof(Version));
                _types.Add(typeof(ValueType));
                _types.Add(typeof(DateTime));
                _types.Add(typeof(TimeOnly));
                _types.Add(typeof(DateOnly));

                _genericTypes.Add(typeof(List<>));
                _genericTypes.Add(typeof(Stack<>));
                _genericTypes.Add(typeof(KeyValuePair<,>));
            }
        }

        public bool IsSerializable(Type type)
        {
            if (type.IsGenericType)
            {
                if (_genericTypes.Contains(type) || _genericTypes.Contains(type.GetGenericTypeDefinition()))
                    return true;
            }
            else
            {
                if (_types.Contains(type))
                    return true;
            }

            return false;
        }
    }


}
