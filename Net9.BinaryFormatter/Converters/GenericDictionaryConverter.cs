
namespace Net9.BinaryFormatter
{
    public class GenericDictionaryConverterFactory : BinaryConverter
    {
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

        private BinaryConverter GetHandler(Type type)
        {
            var genArgs = type.GetGenericArguments();
            var genType = typeof(GenericDictionaryConverter<,>).MakeGenericType(genArgs);
            return (BinaryConverter)Activator.CreateInstance(genType)!;
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
            var keyValues = (KeyValuePair<K, V>[]?)info.GetValue("KeyValues", typeof(KeyValuePair<K, V>[]))!;
            var dict = new Dictionary<K, V>(keyValues);
            return dict;
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