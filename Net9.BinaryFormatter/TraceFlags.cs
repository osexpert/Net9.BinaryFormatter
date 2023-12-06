using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net9.BinaryFormatter
{
    public static class TraceFlags
    {
        /// <summary>
        /// Check if different types regardless of primary type or not (non-arrays)
        /// commenting this line seems to fix https://github.com/dotnet/runtime/issues/90387
        /// </summary>
        public static bool Formatter_IConvertibleFix = false;

        /// <summary>
        /// 
        /// </summary>
        public static bool Formatter_IConvertibleArrayFix = false;

        /// <summary>
        /// Default in net8.0.0 was true. But changed to false because it is weird to try to go back in time.
        /// This would make System.String pretend it belong in mscorlib instead CoreLib
        /// </summary>
        public static bool Formatter_CheckTypeForwardedFromAttributeDuringAssemblyNameLookup = false;

        /// <summary>
        /// Use typeNameInfo._transmitTypeOnMember instead of memberNameInfo._transmitTypeOnMember in WriteMember
        /// Do not combine with IConvertibleFixArray!
        /// </summary>
        //public static bool IConvertibleFixArrayAlt2 = false;
    }
}
