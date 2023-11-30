
namespace Net9.BinaryFormatter
{
    public class GenericDictionaryConverterFactory : BinaryConverter
    {
        public override bool CanConvert(Type type)
        {
            var canHandle = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
            return canHandle;
        }

        public override void Serialize(object obj, SerializationInfo info, StreamingContext context)
        {
            var handler = GetHandler(obj.GetType());
            handler.Serialize(obj, info, context);
        }

        public override object Deserialize(object obj, SerializationInfo info, StreamingContext context)
        {
            var handler = GetHandler(obj.GetType());
            return handler.Deserialize(obj, info, context);
        }

        private BinaryConverter GetHandler(Type type)
        {
            var genArgs = type.GetGenericArguments();
            var genType = typeof(GenericDictionaryConverter<,>).MakeGenericType(genArgs);
            var res = (BinaryConverter)Activator.CreateInstance(genType)!;
            //if (res is IDeserializationCallback dc)
            //{
            //    OnDeserializationEvent += dc.OnDeserialization;
            //}
            return res;
        }

        //public event Action<object?>? OnDeserializationEvent;

        //public void OnDeserialization(object? sender)
        //{
        //    OnDeserializationEvent?.Invoke(sender);
        //}
    }

    public class GenericDictionaryConverter<K, V> : BinaryConverter<Dictionary<K, V>>
        where K : notnull
    {
        public override void Serialize(object obj, SerializationInfo info, StreamingContext context)
        {
            var dict = (Dictionary<K, V>)obj;
            var arr = dict.ToArray();
            info.AddValue("KeyValues", arr);
        }

        public override object Deserialize(object obj, SerializationInfo info, StreamingContext context)
        {
            // We can't do anything with the keys and values until the entire graph has been deserialized
            // and we have a resonable estimate that GetHashCode is not going to fail.  For the time being,
            // we'll just cache this.  The graph is not valid until OnDeserialization has been called.
            //            HashHelpers.SerializationInfoTable.Add(obj, info);
            // Well...it depends very much on the type of the key. If the key is primitive, its not a problem.


            context.AddOnDeserialization(obj, info, OnDeserialization);

            return obj;
        }

        public void OnDeserialization(object obj, SerializationInfo info)
        {
            //HashHelpers.SerializationInfoTable.TryGetValue(obj, out SerializationInfo? info);

            //if (info == null)
            //{
            //    // We can return immediately if this function is called twice.
            //    // Note we remove the serialization info from the table at the end of this method.
            //    return;
            //}

            var keyValues = (KeyValuePair<K, V>[]?)info.GetValue("KeyValues", typeof(KeyValuePair<K, V>[]))!;

            var dict = (Dictionary<K, V>)obj;

            foreach (var kv in keyValues)
                dict.Add(kv.Key, kv.Value);

            dict.TrimExcess();
            //   var dict = new Dictionary<K, V>(keyValues);



            //int realVersion = siInfo.GetInt32(VersionName);
            //int hashsize = siInfo.GetInt32(HashSizeName);
            //_comparer = (IEqualityComparer<TKey>)siInfo.GetValue(ComparerName, typeof(IEqualityComparer<TKey>))!; // When serialized if comparer is null, we use the default.

            //if (hashsize != 0)
            //{
            //    Initialize(hashsize);

            //    KeyValuePair<TKey, TValue>[]? array = (KeyValuePair<TKey, TValue>[]?)
            //        siInfo.GetValue(KeyValuePairsName, typeof(KeyValuePair<TKey, TValue>[]));

            //    if (array == null)
            //    {
            //        ThrowHelper.ThrowSerializationException(ExceptionResource.Serialization_MissingKeys);
            //    }

            //    for (int i = 0; i < array.Length; i++)
            //    {
            //        if (array[i].Key == null)
            //        {
            //            ThrowHelper.ThrowSerializationException(ExceptionResource.Serialization_NullKey);
            //        }

            //        Add(array[i].Key, array[i].Value);
            //    }
            //}
            //else
            //{
            //    _buckets = null;
            //}

            //_version = realVersion;
     //       HashHelpers.SerializationInfoTable.Remove(this);

            //return dict;
        }
    }
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