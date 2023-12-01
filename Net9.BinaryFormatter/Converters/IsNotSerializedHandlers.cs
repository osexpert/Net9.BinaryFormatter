using System.Reflection;

namespace Net9.BinaryFormatter
{
    public class IsNotSerializedHandlers
    {
        public List<IIsNotSerialized> Handlers { get; }

        public bool IsNotSerialized(FieldInfo fi)
        {
            foreach (var h in Handlers)
                if (h.IsNotSerialized(fi))
                    return true;

            return false;
        }

        public IsNotSerializedHandlers(bool addDefaultHandlers = true)
        {
            if (addDefaultHandlers)
            {
                Handlers = GetDefaultHandlers();
            }
            else
            {
                Handlers = new();
            }
        }

        /// <summary>
        /// Return a list of new default instances of the the default handlers (new instances made every time GetDefaultHandlers is called)
        /// </summary>
        /// <returns></returns>
        public static List<IIsNotSerialized> GetDefaultHandlers()
        {
            var res = new List<IIsNotSerialized>();
            res.Add(new NotSerializedByAttribute());
            return res;
        }
    }


    public interface IIsNotSerialized
    {
        bool IsNotSerialized(FieldInfo fi);
    }

    public class NotSerializedByAttribute : IIsNotSerialized
    {
        public bool IsNotSerialized(FieldInfo fi)
            => IsNotSerializedStatic(fi);

        public static bool IsNotSerializedStatic(FieldInfo field)
        {
            return field.GetCustomAttribute<NonSerializedAttribute>() != null;
        }
    }

    public class NotSerializedByRuntimeAttribute : IIsNotSerialized
    {
        public bool IsNotSerialized(FieldInfo fi)
        {
#pragma warning disable SYSLIB0050 // Type or member is obsolete
            return fi.IsNotSerialized;
#pragma warning restore SYSLIB0050 // Type or member is obsolete
        }
    }

}