using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net9.BinaryFormatter
{
    public class GenericHashSetConverterFactory : BinaryConverter
    {
        // needed?
        //        Dictionary<Type, BaseSerializer> _cache = new();

        public override bool CanConvert(Type type)
        {
            var canHandle = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(HashSet<>);
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

        private BinaryConverter GetHandler(Type type)
        {
            //          if (!_cache.TryGetValue(type, out var handler))
            //          {
            var gt = type.GetGenericArguments();
            var gser = typeof(GenericHashSetConverter<>).MakeGenericType(gt);
            var handler = (BinaryConverter)Activator.CreateInstance(gser)!;
            //                _cache.TryAdd(type, handler); // worst case we waiste some mem?
            //            }

            return handler;
        }
    }

    public class GenericHashSetConverter<K> : BinaryConverter<HashSet<K>>
        where K : notnull
    {
        public override void Serialize(object obj, SerializationInfo info)
        {
            var dict = (HashSet<K>)obj;
            var arr = dict.ToArray();
            info.AddValue("Keys", arr);
        }

        public override object Deserialize(object obj, SerializationInfo info)
        {
    //        var dict = (HashSet<K>)obj;
            var keys = (K[])info.GetValue("Keys", typeof(K[]))!;
            //          foreach (var k in keys)
            //                dict.Add(k);
            var dict = new HashSet<K>(keys);
            return dict;
        }
    }
}
