using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Net9.BinaryFormatter
{

    public static class IsSerializableHandlers
    {
        public static List<IsSerializableHandler> Handlers { get; set; } = GetDefaultHandlers();

        internal static bool IsSerializable(Type type)
        {
            foreach (var h in Handlers)
                if (h.IsSerializable(type))
                    return true;

            return false;
        }


        private static List<IsSerializableHandler> GetDefaultHandlers()
        {
            var res = new List<IsSerializableHandler>();
            res.Add(new CommonSerializableTypes());
            res.Add(new SerializableAttributeTypes());
            return res;
        }
    }


    public abstract class IsSerializableHandler
    {
        public abstract bool IsSerializable(Type type);
    }

    public class SerializableAttributeTypes : IsSerializableHandler
    {
        public override bool IsSerializable(Type type)
        {
            return type.GetCustomAttribute<SerializableAttribute>() != null;
        }
    }

    public class CommonSerializableTypes : IsSerializableHandler
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

        public override bool IsSerializable(Type type)
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
                    if ((underlyingType == typeof(Delegate) && Net9Configuration.DelegateIsSerializable) || underlyingType == typeof(Enum))
                        return true;



                    underlyingType = underlyingType.BaseType;
                }
                while (underlyingType != null);
            }

            return false;
        }
    }


}
