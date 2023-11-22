using System.Reflection;

namespace Net9.BinaryFormatter
{

    public class IsSerializableHandlers
    {
        public static List<IIsSerializable> Handlers { get; set; } = GetDefaultHandlers();

        public static bool IsSerializable(Type type)
        {
            foreach (var h in Handlers)
                if (h.IsSerializable(type))
                    return true;

            return false;
        }


        private static List<IIsSerializable> GetDefaultHandlers()
        {
            var res = new List<IIsSerializable>();
            res.Add(new CommonSerializableTypes());
            res.Add(new SerializableAttributeTypes());
            return res;
        }
    }


    public interface IIsSerializable
    {
        bool IsSerializable(Type type);
    }

    public class SerializableAttributeTypes : IIsSerializable
    {
        public bool IsSerializable(Type type)
        {
            return type.GetCustomAttribute<SerializableAttribute>() != null;
        }
    }

    public class CommonSerializableTypes : IIsSerializable
    {
        HashSet<Type> _genTypes = new();
        HashSet<Type> _types = new();

        public CommonSerializableTypes()
        {
            _types.Add(typeof(Version));
            _types.Add(typeof(ValueType));
            _types.Add(typeof(DateTime));
            _types.Add(typeof(TimeOnly));
            _types.Add(typeof(DateOnly));

            _genTypes.Add(typeof(List<>));
            _genTypes.Add(typeof(Stack<>));
            _genTypes.Add(typeof(KeyValuePair<,>));
        }

        public bool IsSerializable(Type type)
        {
            if (type.IsGenericType)
            {
                if (_genTypes.Contains(type.GetGenericTypeDefinition()))
                    return true;
            }
            else
            {
                if (_types.Contains(type))
                    return true;
            }

            
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
                    if ((underlyingType == typeof(Delegate) && Net9Settings.DelegateIsSerializable) || underlyingType == typeof(Enum))
                        return true;



                    underlyingType = underlyingType.BaseType;
                }
                while (underlyingType != null);
            }

            return false;
        }
    }


}
