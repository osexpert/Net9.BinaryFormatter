
using System;

namespace Net9.BinaryFormatter
{
    public class ConverterSelector : ISurrogateSelector
    {
        public List<BinaryConverter> Converters { get; } = new();

        public ConverterSelector(bool addDefaultConverters = true)//ISurrogateSelector? next)
        {
            if (addDefaultConverters)
            {
                Converters = GetDefaultConverters();
            }
            else
            {
                Converters = new();
            }

//            if (next != null)
  //              throw new NotImplementedException("Next surrogate selector not allowed");
        }

        private List<BinaryConverter> GetDefaultConverters()
        {
            var res = new List<BinaryConverter>();
            res.Add(new GenericDictionaryConverterFactory());
            res.Add(new GenericHashSetConverterFactory());
            return res;
        }

        //public void ChainSelector(ISurrogateSelector selector)
        //{
        //    throw new NotImplementedException();
        //}

        //public ISurrogateSelector? GetNextSelector()
        //{
        //    throw new NotImplementedException();
        //}

        public ISerializationSurrogate? GetSurrogate(Type type, StreamingContext context)
        {
            foreach (var converter in Converters)
                if (converter.CanConvert(type))
                    return converter;

            return null;
        }
    }
}
