using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net9.BinaryFormatter
{
    public class GenericDictionaryConverterFactory : BinaryConverter
    {
        // needed?
//        Dictionary<Type, BaseSerializer> _cache = new();

        public override bool CanConvert(Type type)
        {
            var canHandle = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
            return canHandle;

        }

        public override void Serialize(object obj, SerializationInfo info)
        {
            var handler = GetHandler(obj.GetType());
            handler.Serialize(obj, info);
        }

        public override object Deserialize(object obj, SerializationInfo info)
        {
            var handler = GetHandler(obj.GetType());
            return handler.Deserialize(obj, info);
        }


        //public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        //{
        //    var interfaceTypes = givenType.GetInterfaces();

        //    foreach (var it in interfaceTypes)
        //    {
        //        if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
        //            return true;
        //    }

        //    if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
        //        return true;

        //    Type baseType = givenType.BaseType;
        //    if (baseType == null) return false;

        //    return IsAssignableToGenericType(baseType, genericType);
        //}

        private BinaryConverter GetHandler(Type type)
        {
  //          if (!_cache.TryGetValue(type, out var handler))
  //          {
                var gt = type.GetGenericArguments();
                var gser = typeof(GenericDictionaryConverter<,>).MakeGenericType(gt);
                var handler = (BinaryConverter)Activator.CreateInstance(gser)!;
//                _cache.TryAdd(type, handler); // worst case we waiste some mem?
//            }

            return handler;
        }
    }

    public class GenericDictionaryConverter<K, V> : BinaryConverter<Dictionary<K, V>>
        where K : notnull
    {
        public override void Serialize(object obj, SerializationInfo info)
        {
            var dict = (Dictionary<K, V>)obj;
            var arr = dict.ToArray();
            info.AddValue("KeyValues", arr);
        }

        public override object Deserialize(object obj, SerializationInfo info)
        {
            //var dict = (Dictionary<K, V>)obj;
            var keyValues = (KeyValuePair<K, V>[]?)info.GetValue("KeyValues", typeof(KeyValuePair<K, V>[]))!;
            //            foreach (var kv in keyValues)
            //              dict.Add(kv.Key, kv.Value);
            var dict = new Dictionary<K, V>(keyValues);
            return dict;
        }

   

    }
}
