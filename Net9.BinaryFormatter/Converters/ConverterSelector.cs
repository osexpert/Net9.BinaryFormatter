
using System;

namespace Net9.BinaryFormatter
{
    public class ConverterSelector : ISurrogateSelector
    {
        public List<BinaryConverter> Converters { get; }

        public ConverterSelector(bool addDefaultConverters = true)
        {
            if (addDefaultConverters)
            {
                Converters = GetDefaultConverters();
            }
            else
            {
                Converters = new();
            }
        }

        /// <summary>
        /// A new instance of converters are made every time
        /// </summary>
        /// <returns></returns>
        public static List<BinaryConverter> GetDefaultConverters()
        {
            var res = new List<BinaryConverter>();
            res.Add(new GenericDictionaryConverterFactory());
            res.Add(new GenericHashSetConverterFactory());
            return res;
        }

        Dictionary<Type, BinaryConverter?> _cache = new();

        public ISerializationSurrogate? GetSurrogate(Type type, StreamingContext context)
        {
            if (_cache.TryGetValue(type, out var converter))
                return converter;

            converter = GetConverterUncached(type);
            _cache.Add(type, converter);
            return converter;
        }

        private BinaryConverter? GetConverterUncached(Type type)
        {
            foreach (var converter in Converters)
                if (converter.CanConvert(type))
                    return converter;

            return null;
        }
    }
}
