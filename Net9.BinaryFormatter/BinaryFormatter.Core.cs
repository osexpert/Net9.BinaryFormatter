// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.VisualBasic.FileIO;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Net9.BinaryFormatter
{
    public sealed partial class BinaryFormatter : IFormatter
    {
#if false
        [RequiresDynamicCode(IFormatter.RequiresDynamicCodeMessage)]
        [RequiresUnreferencedCode(IFormatter.RequiresUnreferencedCodeMessage)]
        public T Deserialize<T>(Stream serializationStream)
        {
            // TODO: from the graph of types in T (object not allowed), set these types to the list of allowed types.
            // If a type not in the graph is in the stream, it will throw.
            // This can protect agains random streams with random types.
            // More:
            // - possibility to ignore other types? (ingore instead of failing)
            // - ignore members in stream but not in classes? (instead of failing)

            //List<Type> types = GetAllTypes<T>();
            

            Queue<Type> typeQ = new();
            HashSet<Type> types = new();

            typeQ.Enqueue(typeof(T));

            while (typeQ.Any())
            {
                var t = typeQ.Dequeue();

                // Base types
                var baseT = t.BaseType;
                while (baseT != null)
                {
                    if (types.Add(baseT))
                    {
                        // generics??
                        typeQ.Enqueue(baseT);
                    }
                    baseT = baseT.BaseType;
                }

                // Member types
                var res = FormatterServices.GetSerializableMembers(t, _context, _control);
                foreach (var mem in res)
                {
                    if (mem == null)
                        continue;

                    if (mem.MemberType == MemberTypes.Field)
                    {
                        var ft = (System.Reflection.FieldInfo)mem;
                        if (types.Add(ft.FieldType))
                            typeQ.Enqueue(ft.FieldType);
                    }
                    else if (mem.MemberType == MemberTypes.Property)
                    {
                        var ft = (System.Reflection.PropertyInfo)mem;
                        if (types.Add(ft.PropertyType))
                            typeQ.Enqueue(ft.PropertyType);

                    }
                    else throw new NotImplementedException("" + mem.MemberType);


                }

                //if (t.DeclaringType != null && types.Add(t.DeclaringType))
                //{
                //    // we added it so trace it
                   
                //}

                // generics
                if (t.IsGenericType)
                {
                    var gen_types = t.GetGenericArguments();
                    foreach (var gentype in gen_types)
                    if (types.Add(gentype))
                        typeQ.Enqueue(gentype);
                }
                else if (t.IsGenericTypeDefinition)
                {
                    var tt = t.GetGenericTypeDefinition();
                    if (types.Add(tt))
                        typeQ.Enqueue(tt);
                }
            }


            // KeyCollection\ValueCollection sku ikke vært der. den tar ikke hensyn til converters
            // men kanskje det ikke har noe å si?

            // OBS: dette vil ikke fange opp subklasser eller klasser som implementerer ifaces. så usikker på nytten

            return (T)Deserialize(serializationStream);
        }

//        private List<Type> GetAllTypes<T>()
//        {
//            HashSet<Type> ts = new();

//            var rootType = typeof(T);
//            ts.Add(rootType);

//            Queue<Type> typeQ = new();
//            typeQ.Enqueue(rootType);

//            while (typeQ.Any())
//            {
//                var t = typeQ.Dequeue();
//                t.

//            }

//// for a type: get all members, except NonSerialized
//// add type of member. if already added, done, else need to traverse further, call recursive with this type

//        }

        //public static IEnumerable<T> TopogicalSequenceDFS<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> deps)
        //{
        //    HashSet<T> yielded = new HashSet<T>();
        //    HashSet<T> visited = new HashSet<T>();
        //    Stack<Tuple<T, IEnumerator<T>>> stack = new Stack<Tuple<T, IEnumerator<T>>>();

        //    foreach (T t in source)
        //    {
        //        stack.Clear();
        //        if (visited.Add(t))
        //            stack.Push(new Tuple<T, IEnumerator<T>>(t, deps(t).GetEnumerator()));

        //        while (stack.Count > 0)
        //        {
        //            var p = stack.Peek();
        //            bool depPushed = false;
        //            while (p.Item2.MoveNext())
        //            {
        //                var curr = p.Item2.Current;
        //                if (visited.Add(curr))
        //                {
        //                    stack.Push(new Tuple<T, IEnumerator<T>>(curr, deps(curr).GetEnumerator()));
        //                    depPushed = true;
        //                    break;
        //                }
        //                else if (!yielded.Contains(curr))
        //                    throw new Exception("cycle");
        //            }

        //            if (!depPushed)
        //            {
        //                p = stack.Pop();
        //                if (!yielded.Add(p.Item1))
        //                    throw new Exception("bug");
        //                yield return p.Item1;
        //            }
        //        }
        //    }
        //}

#endif

        [RequiresDynamicCode(IFormatter.RequiresDynamicCodeMessage)]
        [RequiresUnreferencedCode(IFormatter.RequiresUnreferencedCodeMessage)]
        public object Deserialize(Stream serializationStream)
        {
            ArgumentNullException.ThrowIfNull(serializationStream);

            if (serializationStream.CanSeek && (serializationStream.Length == 0))
            {
                throw new SerializationException(SR.Serialization_Stream);
            }

            var formatterEnums = new InternalFE()
            {
                _typeFormat = _typeFormat,
                _serializerTypeEnum = InternalSerializerTypeE.Binary,
                _assemblyFormat = _assemblyFormat,
                _securityLevel = _securityLevel,
            };

            var reader = new ObjectReader(serializationStream, _surrogates, _context, formatterEnums, _binder, _control)
            {
                _crossAppDomainArray = _crossAppDomainArray
            };
            try
            {
                BinaryFormatterEventSource.Log.DeserializationStart();
                var parser = new BinaryParser(serializationStream, reader);
                return reader.Deserialize(parser);
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new SerializationException(SR.Serialization_CorruptedStream, e);
            }
            finally
            {
                BinaryFormatterEventSource.Log.DeserializationStop();
            }
        }

        [RequiresUnreferencedCode(IFormatter.RequiresUnreferencedCodeMessage)]
        public void Serialize(Stream serializationStream, object graph)
        {
            ArgumentNullException.ThrowIfNull(serializationStream);

            var formatterEnums = new InternalFE()
            {
                _typeFormat = _typeFormat,
                _serializerTypeEnum = InternalSerializerTypeE.Binary,
                _assemblyFormat = _assemblyFormat,
            };

            try
            {
                BinaryFormatterEventSource.Log.SerializationStart();
                var sow = new ObjectWriter(_surrogates, _context, formatterEnums, _binder, _control);
                BinaryFormatterWriter binaryWriter = new BinaryFormatterWriter(serializationStream, sow, _typeFormat);
                sow.Serialize(graph, binaryWriter);
                _crossAppDomainArray = sow._crossAppDomainArray;
            }
            finally
            {
                BinaryFormatterEventSource.Log.SerializationStop();
            }
        }
    }
}
