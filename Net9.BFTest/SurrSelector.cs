/*
 * Based on code from Microsoft.Bot.Builder
 * https://github.com/CXuesong/BotBuilder.Standard
 * branch: netcore20+net45
 * BotBuilder.Standard/CSharp/Library/Microsoft.Bot.Builder/Fibers/NetStandardSerialization.cs
 * BotBuilder.Standard/CSharp/Library/Microsoft.Bot.Builder/Fibers/Serialization.cs
 */

using Net9.BinaryFormatter;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Permissions;

namespace FromGore
{
    /// <summary>
    /// Based on code from Microsoft.Bot.Builder on github
    /// </summary>
    public sealed class SafeSurrogateSelector : ISurrogateSelector
    {
        //private static readonly IList<ISerializationSurrogateEx> _providers = GetProviders();
        private BinarySerializerOptions _options;

//        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public SafeSurrogateSelector(ISurrogateSelector? next, BinarySerializerOptions options)
        {
            _options = options;
            if (next != null)
                throw new NotImplementedException("Next surrogate selector not allowed");
        }

        //[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        //static IList<ISerializationSurrogateEx> GetProviders()
        //{
        //	var providers = new List<ISerializationSurrogateEx>();

        //	// These 2 are about safety during deserialize (does not alter serialization)
        //	providers.Add(new DataSetSurrogate());
        //	providers.Add(new WindowsIdentitySurrogate());

        //	if (BinaryFormatterAdapter.NetCore)
        //	{
        //		// These are about things that are no longer Serializable in net6
        //		// There is a lot more that is not Serializable in net6 (CollectionBase etc.)
        //		// but for some things its easier\better to change to somethign else than adding support for it here.
        //		providers.Add(new TypeSurrogate());
        //		providers.Add(new CultureInfoSurrogate());
        //	}

        //	return providers;
        //}

        //[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISurrogateSelector.ChainSelector(ISurrogateSelector selector)
        {
            throw new NotImplementedException();
        }

        //[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        ISurrogateSelector ISurrogateSelector.GetNextSelector()
        {
            throw new NotImplementedException();
        }

//        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        ISerializationSurrogate ISurrogateSelector.GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
        {
            // Use a simpler logic at first, until we get conflicts (if ever)
            ISurrogate found = null;
            foreach (var surr in _options.Surrogates)
            {
                if (surr.Handles(type, context))
                {
                    if (found == null)
                        found = surr;
                    else
                        throw new Exception("Myltiple surrogates can handle the same type: " + type);
                }
            }

            if (found != null)
            {
                selector = this;
                return found;
            }
            else
            {
                selector = null;
                return null;
            }
        }
    }

    /// <summary>
    /// Extend <see cref="ISerializationSurrogate"/> with a "tester" method used by <see cref="SafeSurrogateSelector"/>.
    /// </summary>
    public interface ISurrogate : ISerializationSurrogate
    {
        /// <summary>
        /// Determine whether this surrogate provider handles this type.
        /// </summary>
        /// <param name="type">The query type.</param>
        /// <param name="context">The serialization context.</param>
        /// <returns>True if this provider handles this type, false otherwise.</returns>
        bool Handles(Type type, StreamingContext context);
    }

    public class BinarySerializerOptions
    {
        public bool NetCore { get; }

        public BinarySerializerOptions(bool netCore)
        {
            NetCore = netCore;
        }

//        public BinarySerializerConfig Config { get; set; } = null;

        public List<ISurrogate> Surrogates { get; } = new();

    }


    internal class GenDictSurrogate : ISurrogate
    {
        public bool Handles(Type type, StreamingContext context)
        {
            var canHandle = IsAssignableToGenericType(type, typeof(Dictionary<,>));
            return canHandle;
        }

        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = givenType.BaseType;
            if (baseType == null) return false;

            return IsAssignableToGenericType(baseType, genericType);
        }

        Dictionary<Type, BaseSerializer> _cache = new();

        //        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            //var ci = (CultureInfo)obj;
            //info.SetType(typeof(CultureInfoReference));
            //info.AddValue("Name", ci.Name);
            //info.AddValue("UseUserOverride", ci.UseUserOverride);

            var ot = obj.GetType();
            BaseSerializer handler = GetHandler(ot);

            handler.Serialize(obj, info);
        }

        private BaseSerializer GetHandler(Type ot)
        {
            if (!_cache.TryGetValue(ot, out var handler))
            {
                var gt = ot.GetGenericArguments();
                var gser = typeof(DictSerializer<,>).MakeGenericType(gt);
                handler = (BaseSerializer)Activator.CreateInstance(gser)!;
                _cache.TryAdd(ot, handler); // worst case we waiste some mem?
            }

            return handler;
        }

        //      [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector? selector)
        {
            var ot = obj.GetType();

            BaseSerializer handler = GetHandler(ot);

            return handler.DeSerialize(obj, info);
        }

        abstract class BaseSerializer
        {
            internal abstract void Serialize(object o, SerializationInfo si);
            internal abstract object DeSerialize(object o, SerializationInfo si);
        }

        class DictSerializer<K, V> : BaseSerializer
            where K : notnull
        {

            public DictSerializer()
            {
            }

            internal override object DeSerialize(object o, SerializationInfo si)
            {
                var d = (Dictionary<K, V>)o;

                var keyValues = (KeyValuePair<K, V>[]?)si.GetValue("KeyValues", typeof(KeyValuePair<K, V>[]))!;

                foreach (var kv in keyValues)
                    d.Add(kv.Key, kv.Value);

                return d;
            }

            internal override void Serialize(object o, SerializationInfo si)
            {
                var d = (Dictionary<K, V>)o;

                var arr = d.ToArray();

                si.AddValue("KeyValues", arr);
            }




        }

        //[Serializable]
        //internal sealed class CultureInfoReference : IObjectReference
        //{
        //    private readonly string Name;
        //    private readonly bool UseUserOverride;

        //    public object GetRealObject(StreamingContext context)
        //    {
        //        return new CultureInfo(Name, UseUserOverride);
        //    }
        //}
    }
}
