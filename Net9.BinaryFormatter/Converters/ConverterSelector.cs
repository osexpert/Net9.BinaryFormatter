
namespace Net9.BinaryFormatter
{
    public class ConverterSelector : ISurrogateSelector
    {
        public List<BinaryConverter> Converters { get; } = new();

        public ConverterSelector()//ISurrogateSelector? next)
        {
//            if (next != null)
  //              throw new NotImplementedException("Next surrogate selector not allowed");
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
