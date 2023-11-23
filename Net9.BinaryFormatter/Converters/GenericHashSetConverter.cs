
namespace Net9.BinaryFormatter
{
    public class GenericHashSetConverterFactory : BinaryConverter
    {
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
            var gt = type.GetGenericArguments();
            var gser = typeof(GenericHashSetConverter<>).MakeGenericType(gt);
            var handler = (BinaryConverter)Activator.CreateInstance(gser)!;

            return handler;
        }
    }

    public class GenericHashSetConverter<K> : BinaryConverter<HashSet<K>>
        where K : notnull
    {
        public override void Serialize(object obj, SerializationInfo info)
        {
            var hs = (HashSet<K>)obj;
            var arr = hs.ToArray();
            info.AddValue("Keys", arr);
        }

        public override object Deserialize(object obj, SerializationInfo info)
        {
            var keys = (K[])info.GetValue("Keys", typeof(K[]))!;
            var dict = new HashSet<K>(keys);
            return dict;
        }
    }
}
