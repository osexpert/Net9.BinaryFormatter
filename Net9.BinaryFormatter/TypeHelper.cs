
namespace Net9.BinaryFormatter
{
    public static class TypeHelper
    {
//        static readonly Type _runtimeType = Type.GetType("System.RuntimeType") ?? throw new Exception("System.RuntimeType not found");
        public static bool IsRuntimeType(Type type)
        {
            // Example of what would not be a runtime type: new TypeDelegator(typeof(int))


            return type.GetType() == typeof(void).GetType();
            //  return type == _runtimeType || type.GetType() == _runtimeType;
            //            if (type.IsPointer) return true;
            //if (IsExtends(type, typeof(object))) return false;
            //          return false;


            //            //https://stackoverflow.com/a/10183678/2671330
            //          return !typeof(Type).IsAssignableFrom(type);

            // does not work
            //return type.IsAssignableFrom(_runtimeType);
        }
    }
}

