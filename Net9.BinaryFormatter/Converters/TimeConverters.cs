//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Net9.BinaryFormatter.Converters
//{
//    public class DateTimeConverter : BinaryConverter
//    {
//        public override bool CanConvert(Type type)
//        {
//            return type == typeof(DateTime);
//        }

//        public override object Deserialize(object obj, SerializationInfo info)
//        {
//            return DateTime.FromBinary(info.GetInt64("dateData"));
//        }

//        public override void Serialize(object obj, SerializationInfo info)
//        {
//            info.AddValue("dateData", ((DateTime)obj).ToBinary());
//        }
//    }
//}
