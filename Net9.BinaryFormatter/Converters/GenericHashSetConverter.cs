
namespace Net9.BinaryFormatter
{
    public class GenericHashSetConverterFactory : BinaryConverter
    {
        public override bool CanConvert(Type type)
        {
            var canHandle = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(HashSet<>);
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
            var genType = typeof(GenericHashSetConverter<>).MakeGenericType(genArgs);
            return (BinaryConverter)Activator.CreateInstance(genType)!;
        }
    }

    public class GenericHashSetConverter<K> : BinaryConverter<HashSet<K>>
        where K : notnull
    {
        public override void Serialize(object obj, SerializationInfo info, StreamingContext context)
        {
            var hs = (HashSet<K>)obj;
            var arr = hs.ToArray();
            info.AddValue("Values", arr);
        }

        public override object Deserialize(object obj, SerializationInfo info, StreamingContext context)
        {
            //var keys = (K[])info.GetValue("Values", typeof(K[]))!;
            //var dict = new HashSet<K>(keys);
            //return dict;

            // We can't do anything with the keys and values until the entire graph has been deserialized
            // and we have a resonable estimate that GetHashCode is not going to fail.  For the time being,
            // we'll just cache this.  The graph is not valid until OnDeserialization has been called.
            //            HashHelpers.SerializationInfoTable.Add(obj, info);

            context.AddOnDeserialization(obj, info, OnDeserialization);
            return obj;
        }

        private void OnDeserialization(object obj, SerializationInfo info)
        {
            var keys = (K[])info.GetValue("Values", typeof(K[]))!;
            //var dict = new HashSet<K>(keys);
            var hs = (HashSet<K>)obj;
            foreach (var k in keys)
                hs.Add(k);

            hs.TrimExcess();
            //return dict;
        }
    }
}
