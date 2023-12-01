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
        public static bool IConvertibleFix = false;

        /// <summary>
        /// 
        /// </summary>
        public static bool IConvertibleFixArray = false;

        /// <summary>
        /// Use typeNameInfo._transmitTypeOnMember instead of memberNameInfo._transmitTypeOnMember in WriteMember
        /// Do not combine with IConvertibleFixArray!
        /// </summary>
        //public static bool IConvertibleFixArrayAlt2 = false;
    }
}
