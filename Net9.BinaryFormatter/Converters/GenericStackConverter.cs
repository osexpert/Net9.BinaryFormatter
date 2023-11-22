#if false
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net9.BinaryFormatter
{
    public class GenericStackConverterFactory : BinaryConverter
    {
        // needed?
        //        Dictionary<Type, BaseSerializer> _cache = new();

        public override bool CanConvert(Type type)
        {
            var canHandle = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Stack<>);
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
            var gser = typeof(GenericStackConverter<>).MakeGenericType(gt);
            var handler = (BinaryConverter)Activator.CreateInstance(gser)!;
            //                _cache.TryAdd(type, handler); // worst case we waiste some mem?
            //            }

            return handler;
        }
    }

    public class GenericStackConverter<K> : BinaryConverter<Stack<K>>
        where K : notnull
    {
        public override void Serialize(object obj, SerializationInfo info)
        {
            var dict = (Stack<K>)obj;
            var arr = dict.ToArray();
            Array.Reverse(arr);
            info.AddValue("Keys", arr);
        }

        public override object Deserialize(object obj, SerializationInfo info)
        {
            var keys = (K[])info.GetValue("Keys", typeof(K[]))!;
            //var dict = (Stack<K>)obj; // nullref, internal _array not set
            var dict = new Stack<K>(keys);

  //          foreach (var k in keys)
//                dict.Push(k);
            return dict;
        }

        //public K[] ToSaneArray(Stack<K> s)
        //{
        //    if (s.Count == 0)
        //        return Array.Empty<K>();

        //    K[] objArray = new K[s.Count];
        //    int i = 0;
        //    while (i < s.Count)
        //    {
        //        objArray[i] = s[i];
        //        i++;
        //    }
        //    return objArray;
        //}

    }
}
#endif