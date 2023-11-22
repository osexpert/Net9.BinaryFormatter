using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net9.BinaryFormatter
{
    public abstract class BinaryConverter : ISerializationSurrogate
    {
        /// <summary>
        /// If a converter can convert the type, there is no need to have a IsSerializable handler in addition.
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public abstract bool CanConvert(Type type);

        void ISerializationSurrogate.GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Serialize(obj, info);
        }

        object ISerializationSurrogate.SetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            return Deserialize(obj, info);
        }

        public abstract void Serialize(object obj, SerializationInfo info);
        public abstract object Deserialize(object obj, SerializationInfo info);
    }

    public abstract class BinaryConverter<T> : BinaryConverter
    {
        public override bool CanConvert(Type type)
        {
            return type == typeof(T);
        }
    }
}
