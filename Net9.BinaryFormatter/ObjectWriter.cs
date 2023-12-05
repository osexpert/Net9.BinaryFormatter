// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Net9.BinaryFormatter;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;


namespace Net9.BinaryFormatter
{
    internal sealed class ObjectWriter
    {
        private const string ObjectWriterUnreferencedCodeMessage = "ObjectWriter requires unreferenced code";

        private Queue<object>? _objectQueue;
        private ObjectIDGenerator? _idGenerator;
        private int _currentId;

        private readonly ISurrogateSelector? _surrogates;
        private readonly StreamingContext _context;
        private BinaryFormatterWriter? _serWriter;
        private readonly SerializationObjectManager _objectManager;

        private long _topId;

        private readonly InternalFE _formatterEnums;
        private readonly SerializationBinder? _binder;
        private readonly SerializationControl _control;

        private SerObjectInfoInit? _serObjectInfoInit;

        private IFormatterConverter? _formatterConverter;

#pragma warning disable 0649 // Field is never assigned to, and will always have its default value null
        internal object[]? _crossAppDomainArray;
#pragma warning restore 0649

        private object? _previousObj;
        private long _previousId;

        private Type? _previousType;
        private InternalPrimitiveTypeE _previousCode = InternalPrimitiveTypeE.Invalid;

        internal ObjectWriter(ISurrogateSelector? selector, StreamingContext context, InternalFE formatterEnums, 
            SerializationBinder? binder, SerializationControl control)
        {
            _currentId = 1;
            _surrogates = selector;
            _context = context;
            _binder = binder;
            _formatterEnums = formatterEnums;
            _objectManager = new SerializationObjectManager(context);
            _control = control;
        }

        [RequiresUnreferencedCode(ObjectWriterUnreferencedCodeMessage)]
        internal void Serialize(object graph, BinaryFormatterWriter serWriter)
        {
            ArgumentNullException.ThrowIfNull(graph);
            ArgumentNullException.ThrowIfNull(serWriter);

            _serWriter = serWriter;

            serWriter.WriteBegin();
            long headerId;
            object? obj;

            // allocations if methodCall or methodResponse and no graph
            _idGenerator = new ObjectIDGenerator();
            _objectQueue = new Queue<object>();
            _formatterConverter = new FormatterConverter();
            _serObjectInfoInit = new SerObjectInfoInit();

            _topId = InternalGetId(graph, false, null, out _);
            headerId = -1;
            WriteSerializedStreamHeader(_topId, headerId);

            _objectQueue.Enqueue(graph);
            while ((obj = GetNext(out long objectId)) != null)
            {
                WriteObjectInfo? objectInfo;

                // GetNext will return either an object or a WriteObjectInfo.
                // A WriteObjectInfo is returned if this object was member of another object
                if (obj is WriteObjectInfo)
                {
                    objectInfo = (WriteObjectInfo)obj;
                }
                else
                {
                    objectInfo = WriteObjectInfo.Serialize(obj, _surrogates, _context, _serObjectInfoInit, _formatterConverter, this, _binder, _control);
                    objectInfo._assemId = GetAssemblyId(objectInfo);
                }

                objectInfo._objectId = objectId;
                TypeInfo dataInfo = TypeToTypeInfo(objectInfo);
                Write(objectInfo, dataInfo, dataInfo);
                PutTypeInfo(dataInfo);
                objectInfo.ObjectEnd();
            }

            serWriter.WriteSerializationHeaderEnd();
            serWriter.WriteEnd();

            // Invoke OnSerialized Event
            _objectManager.RaiseOnSerializedEvent();
        }

        internal SerializationObjectManager ObjectManager => _objectManager;

        // Writes a given object to the stream.
        [RequiresUnreferencedCode(ObjectWriterUnreferencedCodeMessage)]
        private void Write(WriteObjectInfo objectInfo, TypeInfo memberInfo, TypeInfo dataInfo)
        {
            object? obj = objectInfo._obj;
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(objectInfo) + "." + nameof(objectInfo._obj));
            }
            Type? objType = objectInfo._objectType;
            long objectId = objectInfo._objectId;

            if (ReferenceEquals(objType, Converter.s_typeofString))
            {
                Debug.Assert(_serWriter != null);
                // Top level String
                memberInfo._objectId = objectId;
                _serWriter.WriteObjectString((int)objectId, obj.ToString());
            }
            else
            {
                if (objectInfo._isArray)
                {
                    WriteArray(objectInfo, memberInfo);
                }
                else
                {
                    objectInfo.GetMemberInfo(out string[]? memberNames, out Type[]? memberTypes, out object?[]? memberData);

                    // Only Binary needs to transmit types for ISerializable because the binary formatter transmits the types in URT format.
                    // Soap transmits all types as strings, so it is up to the ISerializable object to convert the string back to its URT type
                    if (objectInfo._isSi || CheckTypeFormat(_formatterEnums._typeFormat, FormatterTypeStyle.TypesAlways))
                    {
                        memberInfo._transmitTypeOnObject = true;
                        memberInfo._isParentTypeOnObject = true;
                        dataInfo._transmitTypeOnObject = true;
                        dataInfo._isParentTypeOnObject = true;
                    }

                    Debug.Assert(memberNames != null && memberTypes != null && memberData != null);
                    var memberObjectInfos = new WriteObjectInfo?[memberNames.Length];

                    // Get assembly information
                    // Binary Serializer, assembly names need to be
                    // written before objects are referenced.
                    // GetAssemId here will write out the
                    // assemblyStrings at the right Binary
                    // Serialization object boundary.
                    for (int i = 0; i < memberTypes.Length; i++)
                    {
                        Type type =
                            memberTypes[i] ?? (memberData[i] != null ? GetType(memberData[i]!) :
                            Converter.s_typeofObject);

                        InternalPrimitiveTypeE code = ToCode(type);
                        if ((code == InternalPrimitiveTypeE.Invalid) &&
                            (!ReferenceEquals(type, Converter.s_typeofString)))
                        {
                            Debug.Assert(_serObjectInfoInit != null && _formatterConverter != null);
                            if (memberData[i] != null)
                            {
                                memberObjectInfos[i] = WriteObjectInfo.Serialize(
                                    memberData[i]!,
                                    _surrogates,
                                    _context,
                                    _serObjectInfoInit,
                                    _formatterConverter,
                                    this,
                                    _binder,
                                    _control);
                                memberObjectInfos[i]!._assemId = GetAssemblyId(memberObjectInfos[i]!);
                            }
                            else
                            {
                                memberObjectInfos[i] = WriteObjectInfo.Serialize(
                                    memberTypes[i],
                                    _surrogates,
                                    _context,
                                    _serObjectInfoInit,
                                    _formatterConverter,
                                    _binder,
                                    _control);
                                memberObjectInfos[i]!._assemId = GetAssemblyId(memberObjectInfos[i]!);
                            }
                        }
                    }
                    Write(objectInfo, memberInfo, dataInfo, memberNames, memberTypes, memberData, memberObjectInfos);
                }
            }
        }

        // Writes a given object to the stream.
        [RequiresUnreferencedCode(ObjectWriterUnreferencedCodeMessage)]
        private void Write(WriteObjectInfo objectInfo,
                           TypeInfo memberInfo,
                           TypeInfo dataInfo,
                           string[] memberNames,
                           Type[] memberTypes,
                           object?[] memberData,
                           WriteObjectInfo?[] memberObjectInfos)
        {
            int numItems = memberNames.Length;

            Debug.Assert(_serWriter != null);
            if (memberInfo != null)
            {
                memberInfo._objectId = objectInfo._objectId;
                _serWriter.WriteObject(memberInfo, dataInfo, numItems, memberNames, memberTypes, memberObjectInfos);
            }
            else // if (!ReferenceEquals(objectInfo._objectType, Converter.s_typeofString))
            {
                throw new Exception("Impossible: memberNameInfo is never null");
//                typeNameInfo._objectId = objectInfo._objectId;
//                _serWriter.WriteObject(typeNameInfo, null, numItems, memberNames, memberTypes, memberObjectInfos);
            }


            Debug.Assert(memberInfo != null);
            if (memberInfo._isParentTypeOnObject)
            {
                memberInfo._transmitTypeOnObject = true;
                memberInfo._isParentTypeOnObject = false;
            }
            else
            {
                memberInfo._transmitTypeOnObject = false;
            }

            // Write members
            for (int i = 0; i < numItems; i++)
            {
                //var transmitType = res != null && res.Value.b && res.Value.Item2 != null ? res.Value.Item2[i] != BinaryTypeEnum.Primitive : false;
                WriteMemberSetup(objectInfo, memberInfo, memberNames[i], memberTypes[i], memberData[i], memberObjectInfos[i]);
            }

            if (memberInfo != null)
            {
                memberInfo._objectId = objectInfo._objectId;
                _serWriter.WriteObjectEnd();
            }
            else if (!ReferenceEquals(objectInfo._objectType, Converter.s_typeofString))
            {
                _serWriter.WriteObjectEnd();
            }
        }

        [RequiresUnreferencedCode(ObjectWriterUnreferencedCodeMessage)]
        private void WriteMemberSetup(WriteObjectInfo objectInfo,
                                      TypeInfo memberInfo,
                                      string memberName,
                                      Type memberType,
                                      object? memberData,
                                      WriteObjectInfo? memberObjectInfo)
        {
            TypeInfo newMemberNameInfo = MemberToTypeInfo(memberName); // newMemberNameInfo contains the member type

            if (memberObjectInfo != null)
            {
                newMemberNameInfo._assemId = memberObjectInfo._assemId;
            }
            newMemberNameInfo._type = memberType;

            // newDataInfo contains the data type
            TypeInfo newDataInfo;
            if (memberObjectInfo == null)
            {
                newDataInfo = TypeToTypeInfo(memberType);
            }
            else
            {
                newDataInfo = TypeToTypeInfo(memberObjectInfo);
            }

            newMemberNameInfo._transmitTypeOnObject = memberInfo._transmitTypeOnObject;
            newMemberNameInfo._isParentTypeOnObject = memberInfo._isParentTypeOnObject;
            //newMemberNameInfo._transmitTypeOnMember control everything
            WriteMembers(newMemberNameInfo, newDataInfo, memberData, objectInfo, memberObjectInfo);
            PutTypeInfo(newMemberNameInfo);
            PutTypeInfo(newDataInfo);
        }

        // Writes the members of an object
        [RequiresUnreferencedCode(ObjectWriterUnreferencedCodeMessage)]
        private void WriteMembers(TypeInfo memberInfo,
                                  TypeInfo dataInfo,
                                  object? memberData,
                                  WriteObjectInfo objectInfo,
                                  WriteObjectInfo? memberObjectInfo)
        {
            //memberNameInfo._transmitTypeOnMember control everything

            Type? memberType = memberInfo._type;
            bool assignUniqueIdToValueType = false;

            // Types are transmitted for a member as follows:
            // The member is of type object
            // The member object of type is ISerializable and
            //  Binary - Types always transmitted.

            // FIXME: why check if type object? why not check for all types that are nullable?
            if (ReferenceEquals(memberType, Converter.s_typeofObject) || Nullable.GetUnderlyingType(memberType!) != null)
            {
                dataInfo._transmitTypeOnMember = true;
                memberInfo._transmitTypeOnMember = true;
            }

            if (CheckTypeFormat(_formatterEnums._typeFormat, FormatterTypeStyle.TypesAlways) || (objectInfo._isSi))
            {
                dataInfo._transmitTypeOnObject = true;
                memberInfo._transmitTypeOnObject = true;
                memberInfo._isParentTypeOnObject = true;
            }

            if (CheckForNull(objectInfo, memberInfo, dataInfo, memberData))
            {
                return;
            }

            object outObj = memberData!;
            Type? outType = null;

            // If member type does not equal data type, transmit type on object.
            //            if (memberTypeNameInfo._primitiveTypeEnum == InternalPrimitiveTypeE.Invalid || TraceFlags.IConvertibleFix)
            // I think it make more sense to check that member is NonPrimitive than to check the object.
            // If member is primitive, the check can be skipped, nothing else can be assigned to it that the primitive itself
            // In any case, this is probably an optimization.
            if ((TraceFlags.IConvertibleFix ? memberInfo : dataInfo)._primitiveTypeEnum == InternalPrimitiveTypeE.Invalid)
            {
                outType = GetType(outObj);
                if (!ReferenceEquals(memberType, outType))
                {
                    dataInfo._transmitTypeOnMember = true;
                    memberInfo._transmitTypeOnMember = true;
                }
            }

            if (ReferenceEquals(memberType, Converter.s_typeofObject))
            {
                assignUniqueIdToValueType = true;
                memberType = GetType(memberData!);
                if (memberObjectInfo == null)
                {
                    TypeToTypeInfo(memberType, dataInfo);
                }
                else
                {
                    TypeToTypeInfo(memberObjectInfo, dataInfo);
                }
            }

            if (memberObjectInfo != null && memberObjectInfo._isArray)
            {
                // outObj is an array. It can never be a value type.
                long arrayId = Schedule(outObj, false, null, memberObjectInfo);
                if (arrayId > 0)
                {
                    // Array as object
                    memberInfo._objectId = arrayId;
                    WriteObjectRef(arrayId);
                }
                else
                {
                    Debug.Assert(_serWriter != null);
                    // Nested Array
                    _serWriter.WriteMemberNested();

                    memberObjectInfo._objectId = arrayId;
                    memberInfo._objectId = arrayId;
                    WriteArray(memberObjectInfo, memberInfo);
                    objectInfo.ObjectEnd();
                }
                return;
            }

            // memberInfo._transmitTypeOnMember control everything
            if (!WriteKnownValueClass(memberInfo, dataInfo, memberData!))
            {
                outType ??= GetType(outObj);

                long memberObjectId = Schedule(outObj, assignUniqueIdToValueType, outType, memberObjectInfo);
                if (memberObjectId < 0)
                {
                    Debug.Assert(memberObjectInfo != null);
                    // Nested object
                    memberObjectInfo._objectId = memberObjectId;
                    TypeInfo newDataInfo = TypeToTypeInfo(memberObjectInfo);
                    newDataInfo._objectId = memberObjectId;
                    Write(memberObjectInfo, memberInfo, newDataInfo);
                    PutTypeInfo(newDataInfo);
                    memberObjectInfo.ObjectEnd();
                }
                else
                {
                    // Object reference
                    memberInfo._objectId = memberObjectId;
                    WriteObjectRef(memberObjectId);
                }
            }
        }

        // Writes out an array
        [RequiresUnreferencedCode(ObjectWriterUnreferencedCodeMessage)]
        private void WriteArray(WriteObjectInfo objectInfo, TypeInfo memberInfo)
        {
            bool isAllocatedMemberNameInfo = false;
            if (memberInfo == null)
            {
                throw new Exception("impossible");
                memberInfo = TypeToTypeInfo(objectInfo);
                isAllocatedMemberNameInfo = true;
            }

            memberInfo._isArray = true;

            long objectId = objectInfo._objectId;
            memberInfo._objectId = objectInfo._objectId;

            // Get array type
            Array array = (Array)objectInfo._obj!;
            //Type arrayType = array.GetType();
            Type arrayType = objectInfo._objectType!;

            // Get type of array element
            Type arrayElemType = arrayType.GetElementType()!;
            WriteObjectInfo? arrayElemObjectInfo = null;
            if (!arrayElemType.IsPrimitive)
            {
                Debug.Assert(_serObjectInfoInit != null && _formatterConverter != null);
                arrayElemObjectInfo = WriteObjectInfo.Serialize(arrayElemType, _surrogates, _context, _serObjectInfoInit, _formatterConverter, _binder, _control);
                arrayElemObjectInfo._assemId = GetAssemblyId(arrayElemObjectInfo);
            }

            TypeInfo arrayElemInfo = arrayElemObjectInfo == null ?
                TypeToTypeInfo(arrayElemType) :
                TypeToTypeInfo(arrayElemObjectInfo);
            arrayElemInfo._isArray = arrayElemInfo._type!.IsArray;

            TypeInfo arrayInfo = memberInfo;
            arrayInfo._objectId = objectId;
            arrayInfo._isArray = true;
            arrayElemInfo._objectId = objectId;
            arrayElemInfo._transmitTypeOnMember = memberInfo._transmitTypeOnMember;
            arrayElemInfo._transmitTypeOnObject = memberInfo._transmitTypeOnObject;
            arrayElemInfo._isParentTypeOnObject = memberInfo._isParentTypeOnObject;
            // memberInfo not used after this

            // Get rank and length information
            int rank = array.Rank;
            int[] lengthA = new int[rank];
            int[] lowerBoundA = new int[rank];
            int[] upperBoundA = new int[rank];
            for (int i = 0; i < rank; i++)
            {
                lengthA[i] = array.GetLength(i);
                lowerBoundA[i] = array.GetLowerBound(i);
                upperBoundA[i] = array.GetUpperBound(i);
            }

            InternalArrayTypeE arrayEnum;
            if (arrayElemInfo._isArray)
            {
                arrayEnum = rank == 1 ? InternalArrayTypeE.Jagged : InternalArrayTypeE.Rectangular;
            }
            else if (rank == 1)
            {
                arrayEnum = InternalArrayTypeE.Single;
            }
            else
            {
                arrayEnum = InternalArrayTypeE.Rectangular;
            }
            arrayElemInfo._arrayEnum = arrayEnum;

            Debug.Assert(_serWriter != null);
            // Byte array
            if ((ReferenceEquals(arrayElemType, Converter.s_typeofByte)) && (rank == 1) && (lowerBoundA[0] == 0))
            {
                _serWriter.WriteObjectByteArray(arrayInfo, arrayElemObjectInfo, arrayElemInfo, lengthA[0], lowerBoundA[0], (byte[])array);
                return;
            }

            if (ReferenceEquals(arrayElemType, Converter.s_typeofObject) || Nullable.GetUnderlyingType(arrayElemType) != null)
            {
                arrayInfo._transmitTypeOnMember = true;
                arrayElemInfo._transmitTypeOnMember = true;
            }

            if (CheckTypeFormat(_formatterEnums._typeFormat, FormatterTypeStyle.TypesAlways))
            {
                arrayInfo._transmitTypeOnObject = true;
                arrayElemInfo._transmitTypeOnObject = true;
            }

            if (arrayEnum == InternalArrayTypeE.Single)
            {
                // Single Dimensional array

                // BinaryFormatter array of primitive types is written out in the WriteSingleArray statement
                // as a byte buffer
                _serWriter.WriteSingleArray(arrayInfo, arrayElemObjectInfo, arrayElemInfo, lengthA[0], lowerBoundA[0], array);

                if (!(Converter.IsWriteAsByteArray(arrayElemInfo._primitiveTypeEnum) && (lowerBoundA[0] == 0)))
                {
                    object[]? objectA = null;
                    if (!arrayElemType.IsValueType)
                    {
                        // Non-primitive type array
                        objectA = (object[])array;
                    }

                    int upperBound = upperBoundA[0] + 1;
                    for (int i = lowerBoundA[0]; i < upperBound; i++)
                    {
                        if (objectA == null)
                        {
                            WriteArrayMember(objectInfo, arrayElemInfo, array.GetValue(i));
                        }
                        else
                        {
                            WriteArrayMember(objectInfo, arrayElemInfo, objectA[i]);
                        }
                    }
                    _serWriter.WriteItemEnd();
                }
            }
            else if (arrayEnum == InternalArrayTypeE.Jagged)
            {
                // Jagged Array

                arrayInfo._objectId = objectId;

                _serWriter.WriteJaggedArray(arrayInfo, arrayElemObjectInfo, arrayElemInfo, lengthA[0], lowerBoundA[0]);

                var objectA = (Array)array;
                for (int i = lowerBoundA[0]; i < upperBoundA[0] + 1; i++)
                {
                    WriteArrayMember(objectInfo, arrayElemInfo, objectA.GetValue(i));
                }
                _serWriter.WriteItemEnd();
            }
            else
            {
                // Rectangle Array
                // Get the length for all the ranks

                arrayInfo._objectId = objectId;
                _serWriter.WriteRectangleArray(arrayInfo, arrayElemObjectInfo, arrayElemInfo, rank, lengthA, lowerBoundA);

                // Check for a length of zero
                bool bzero = false;
                for (int i = 0; i < rank; i++)
                {
                    if (lengthA[i] == 0)
                    {
                        bzero = true;
                        break;
                    }
                }

                if (!bzero)
                {
                    WriteRectangle(objectInfo, rank, lengthA, array, arrayElemInfo, lowerBoundA);
                }
                _serWriter.WriteItemEnd();
            }

            _serWriter.WriteObjectEnd();

            PutTypeInfo(arrayElemInfo);
            if (isAllocatedMemberNameInfo)
            {
                PutTypeInfo(arrayInfo);
            }
        }

        // Writes out an array element
        [RequiresUnreferencedCode(ObjectWriterUnreferencedCodeMessage)]
        private void WriteArrayMember(WriteObjectInfo objectInfo, TypeInfo arrayElemInfo, object? data)
        {
            arrayElemInfo._isArrayItem = true;

            if (CheckForNull(objectInfo, arrayElemInfo, arrayElemInfo, data))
            {
                return;
            }

            TypeInfo? actualTypeInfo;
            Type? dataType = null;
            bool isObjectOnMember = false;

            if (arrayElemInfo._transmitTypeOnMember)
            {
                isObjectOnMember = true;
            }

            if (!isObjectOnMember && !arrayElemInfo.IsSealed)
            {
                dataType = GetType(data!);
                if (!ReferenceEquals(arrayElemInfo._type, dataType))
                {
                    isObjectOnMember = true;
                }
            }

            if (isObjectOnMember)
            {
                // Object array, need type of member
                dataType ??= GetType(data!);

                actualTypeInfo = TypeToTypeInfo(dataType);
                actualTypeInfo._transmitTypeOnMember = true;
                actualTypeInfo._objectId = arrayElemInfo._objectId;
                actualTypeInfo._assemId = arrayElemInfo._assemId;
                actualTypeInfo._isArrayItem = true;
                if (TraceFlags.IConvertibleFixArray)
                {
                    // all other places always set _transmitTypeOnMember for both member and object, and this seems to fix it
                    arrayElemInfo._transmitTypeOnMember = true; // Net9 hack
                }
            }
            else
            {
                actualTypeInfo = arrayElemInfo;
                actualTypeInfo._isArrayItem = true;
            }

            if (!WriteKnownValueClass(arrayElemInfo, actualTypeInfo, data!))
            {
                object obj = data!;
                bool assignUniqueIdForValueTypes = false;
                if (ReferenceEquals(arrayElemInfo._type, Converter.s_typeofObject))
                {
                    assignUniqueIdForValueTypes = true;
                }

                long arrayId = Schedule(obj, assignUniqueIdForValueTypes, actualTypeInfo._type);
                arrayElemInfo._objectId = arrayId;
                actualTypeInfo._objectId = arrayId;
                if (arrayId < 1)
                {
                    Debug.Assert(_serObjectInfoInit != null && _formatterConverter != null);
                    WriteObjectInfo newObjectInfo = WriteObjectInfo.Serialize(obj, _surrogates, _context, _serObjectInfoInit, _formatterConverter, this, _binder, _control);
                    newObjectInfo._objectId = arrayId;
                    newObjectInfo._assemId = !ReferenceEquals(arrayElemInfo._type, Converter.s_typeofObject) && Nullable.GetUnderlyingType(arrayElemInfo._type!) == null ?
                        actualTypeInfo._assemId :
                        GetAssemblyId(newObjectInfo);
                    TypeInfo dataInfo = TypeToTypeInfo(newObjectInfo);
                    dataInfo._objectId = arrayId;
                    newObjectInfo._objectId = arrayId;
                    Write(newObjectInfo, actualTypeInfo, dataInfo);
                    newObjectInfo.ObjectEnd();
                }
                else
                {
                    Debug.Assert(_serWriter != null);
                    _serWriter.WriteItemObjectRef((int)arrayId);
                }
            }
            if (arrayElemInfo._transmitTypeOnMember)
            {
                PutTypeInfo(actualTypeInfo);
            }
        }

        // Iterates over a Rectangle array, for each element of the array invokes WriteArrayMember
        [RequiresUnreferencedCode(ObjectWriterUnreferencedCodeMessage)]
        private void WriteRectangle(WriteObjectInfo objectInfo, int rank, int[] maxA, Array array, TypeInfo arrayElemInfo, int[]? lowerBoundA)
        {
            int[] currentA = new int[rank];
            int[]? indexMap = null;
            bool isLowerBound = false;
            if (lowerBoundA != null)
            {
                for (int i = 0; i < rank; i++)
                {
                    if (lowerBoundA[i] != 0)
                    {
                        isLowerBound = true;
                    }
                }
            }
            if (isLowerBound)
            {
                indexMap = new int[rank];
            }

            bool isLoop = true;
            while (isLoop)
            {
                isLoop = false;
                if (isLowerBound)
                {
                    for (int i = 0; i < rank; i++)
                    {
                        indexMap![i] = currentA[i] + lowerBoundA![i];
                    }

                    WriteArrayMember(objectInfo, arrayElemInfo, array.GetValue(indexMap!));
                }
                else
                {
                    WriteArrayMember(objectInfo, arrayElemInfo, array.GetValue(currentA));
                }

                for (int irank = rank - 1; irank > -1; irank--)
                {
                    // Find the current or lower dimension which can be incremented.
                    if (currentA[irank] < maxA[irank] - 1)
                    {
                        // The current dimension is at maximum. Increase the next lower dimension by 1
                        currentA[irank]++;
                        if (irank < rank - 1)
                        {
                            // The current dimension and higher dimensions are zeroed.
                            for (int i = irank + 1; i < rank; i++)
                            {
                                currentA[i] = 0;
                            }
                        }
                        isLoop = true;
                        break;
                    }
                }
            }
        }

        // This gives back the next object to be serialized.  Objects
        // are returned in a FIFO order based on how they were passed
        // to Schedule.  The id of the object is put into the objID parameter
        // and the Object itself is returned from the function.
        private object? GetNext(out long objID)
        {
            bool isNew;

            //The Queue is empty here.  We'll throw if we try to dequeue the empty queue.
            if (_objectQueue!.Count == 0)
            {
                objID = 0;
                return null;
            }

            object obj = _objectQueue.Dequeue();

            // A WriteObjectInfo is queued if this object was a member of another object
            object? realObj = obj is WriteObjectInfo ? ((WriteObjectInfo)obj)._obj : obj;
            Debug.Assert(realObj != null);
            objID = _idGenerator!.HasId(realObj, out isNew);
            if (isNew)
            {
                throw new SerializationException(SR.Format(SR.Serialization_ObjNoID, realObj));
            }

            return obj;
        }

        // If the type is a value type, we dont attempt to generate a unique id, unless its a boxed entity
        // (in which case, there might be 2 references to the same boxed obj. in a graph.)
        // "assignUniqueIdToValueType" is true, if the field type holding reference to "obj" is Object.
        private long InternalGetId(object obj, bool assignUniqueIdToValueType, Type? type, out bool isNew)
        {
            if (obj == _previousObj)
            {
                // good for benchmarks
                isNew = false;
                return _previousId;
            }
            Debug.Assert(_idGenerator != null);
            _idGenerator._currentCount = _currentId;
            if (type != null && type.IsValueType)
            {
                if (!assignUniqueIdToValueType)
                {
                    isNew = false;
                    return -1 * _currentId++;
                }
            }
            _currentId++;
            long retId = _idGenerator.GetId(obj, out isNew);

            _previousObj = obj;
            _previousId = retId;
            return retId;
        }


        // Schedules an object for later serialization if it hasn't already been scheduled.
        // We get an ID for obj and put it on the queue for later serialization
        // if this is a new object id.

        private long Schedule(object obj, bool assignUniqueIdToValueType, Type? type) =>
            Schedule(obj, assignUniqueIdToValueType, type, null);

        private long Schedule(object obj, bool assignUniqueIdToValueType, Type? type, WriteObjectInfo? objectInfo)
        {
            long id = 0;
            if (obj != null)
            {
                bool isNew;
                id = InternalGetId(obj, assignUniqueIdToValueType, type, out isNew);
                if (isNew && id > 0)
                {
                    Debug.Assert(_objectQueue != null);
                    _objectQueue.Enqueue(objectInfo ?? obj);
                }
            }
            return id;
        }

        // Determines if a type is a primitive type, if it is it is written
        private bool WriteKnownValueClass(TypeInfo memberInfo, TypeInfo dataInfo, object data)
        {
            if (ReferenceEquals(dataInfo._type, Converter.s_typeofString))
            {
                WriteString(dataInfo, data);
            }
            else
            {
                if (dataInfo._primitiveTypeEnum == InternalPrimitiveTypeE.Invalid)
                {
                    return false;
                }
                else
                {
                    Debug.Assert(_serWriter != null);
                    if (dataInfo._isArray) // null if an array
                    {
                        _serWriter.WriteItem(memberInfo, dataInfo, data);
                    }
                    else
                    {
                        _serWriter.WriteMember(memberInfo, dataInfo, data);
                    }
                }
            }

            return true;
        }


        // Writes an object reference to the stream.
        private void WriteObjectRef(long objectId) =>
            _serWriter!.WriteMemberObjectRef((int)objectId);

        // Writes a string into the XML stream
        private void WriteString(TypeInfo dataInfo, object stringObject)
        {
            bool isFirstTime = true;

            long stringId = -1;

            if (!CheckTypeFormat(_formatterEnums._typeFormat, FormatterTypeStyle.XsdString))
            {
                stringId = InternalGetId(stringObject, false, null, out isFirstTime);
            }
            dataInfo._objectId = stringId;

            if ((isFirstTime) || (stringId < 0))
            {
                Debug.Assert(_serWriter != null);
                _serWriter.WriteMemberString(dataInfo, (string)stringObject);
            }
            else
            {
                WriteObjectRef(stringId);
            }
        }

        // Writes a null member into the stream
        private bool CheckForNull(WriteObjectInfo objectInfo, TypeInfo memberInfo, TypeInfo dataInfo, object? data)
        {
            bool isNull = data == null;

            // Optimization, Null members are only written for Binary
            if ((isNull) && (((_formatterEnums._serializerTypeEnum == InternalSerializerTypeE.Binary)) ||
                             memberInfo._isArrayItem ||
                             memberInfo._transmitTypeOnObject ||
                             memberInfo._transmitTypeOnMember ||
                             objectInfo._isSi ||
                             (CheckTypeFormat(_formatterEnums._typeFormat, FormatterTypeStyle.TypesAlways))))
            {
                Debug.Assert(_serWriter != null);
                if (dataInfo._isArrayItem)
                {
                    if (dataInfo._arrayEnum == InternalArrayTypeE.Single)
                    {
                        _serWriter.WriteDelayedNullItem();
                    }
                    else
                    {
                        _serWriter.WriteNullItem();
                    }
                }
                else
                {
                    _serWriter.WriteNullMember(memberInfo);
                }
            }

            return isNull;
        }


        // Writes the SerializedStreamHeader
        private void WriteSerializedStreamHeader(long topId, long headerId) =>
            _serWriter!.WriteSerializationHeader((int)topId, (int)headerId, 1, 0);

        // Transforms a type to the serialized string form. URT Primitive types are converted to XMLData Types
        private TypeInfo TypeToTypeInfo(Type? type, WriteObjectInfo? objectInfo, InternalPrimitiveTypeE code, TypeInfo? typeInfo)
        {
            if (typeInfo == null)
            {
                typeInfo = GetTypeInfo();
            }
            else
            {
                typeInfo.Init();
            }

            if (code == InternalPrimitiveTypeE.Invalid)
            {
                if (objectInfo != null)
                {
                    typeInfo.NIname = objectInfo.GetTypeFullName();
                    typeInfo._assemId = objectInfo._assemId;
                }
            }
            typeInfo._primitiveTypeEnum = code;
            typeInfo._type = type;

            return typeInfo;
        }

        private TypeInfo TypeToTypeInfo(Type type) =>
            TypeToTypeInfo(type, null, ToCode(type), null);

        private TypeInfo TypeToTypeInfo(WriteObjectInfo objectInfo) =>
            TypeToTypeInfo(objectInfo._objectType, objectInfo, ToCode(objectInfo._objectType), null);

        private TypeInfo TypeToTypeInfo(WriteObjectInfo objectInfo, TypeInfo nameInfo) =>
            TypeToTypeInfo(objectInfo._objectType, objectInfo, ToCode(objectInfo._objectType), nameInfo);

        private void TypeToTypeInfo(Type type, TypeInfo nameInfo) =>
            TypeToTypeInfo(type, null, ToCode(type), nameInfo);

        private TypeInfo MemberToTypeInfo(string name)
        {
            TypeInfo memberNameInfo = GetTypeInfo();
            memberNameInfo.NIname = name;
            return memberNameInfo;
        }

        internal InternalPrimitiveTypeE ToCode(Type? type)
        {
            if (ReferenceEquals(_previousType, type))
            {
                return _previousCode;
            }
            else
            {
                InternalPrimitiveTypeE code = Converter.ToCode(type);
                if (code != InternalPrimitiveTypeE.Invalid)
                {
                    _previousType = type;
                    _previousCode = code;
                }
                return code;
            }
        }

        private Dictionary<string, long>? _assemblyToIdTable;
        private long GetAssemblyId(WriteObjectInfo objectInfo)
        {
            //use objectInfo to get assembly string with new criteria
            _assemblyToIdTable ??= new Dictionary<string, long>();

            long assemId;
            string assemblyString = objectInfo.GetAssemblyString();

            string serializedAssemblyString = assemblyString;
            if (assemblyString.Length == 0)
            {
                assemId = 0;
            }
            else if (assemblyString.Equals(Converter.s_urtAssemblyString) || assemblyString.Equals(Converter.s_urtAlternativeAssemblyString))
            {
                // Urt type is an assemId of 0. No assemblyString needs to be sent
                // FIXME: this seems wrong....does not work with TimeOnly...OR maybe we are using the wrong Urt-assembly in the destination...
                assemId = 0;
            }
            else
            {
                // Assembly needs to be sent
                // Need to prefix assembly string to separate the string names from the
                // assemblyName string names. That is a string can have the same value
                // as an assemblyNameString, but it is serialized differently
                bool isNew;
                if (_assemblyToIdTable.TryGetValue(assemblyString, out assemId))
                {
                    isNew = false;
                }
                else
                {
                    assemId = InternalGetId("___AssemblyString___" + assemblyString, false, null, out isNew);
                    _assemblyToIdTable[assemblyString] = assemId;
                }

                Debug.Assert(_serWriter != null);
                _serWriter.WriteAssembly(serializedAssemblyString, (int)assemId, isNew);
            }
            return assemId;
        }

        private Type GetType(object obj) => obj.GetType();

        private readonly SerStack _niPool = new SerStack("NameInfo Pool");

        private TypeInfo GetTypeInfo()
        {
            TypeInfo typeInfo;

            if (!_niPool.IsEmpty())
            {
                typeInfo = (TypeInfo)_niPool.Pop()!;
                typeInfo.Init();
            }
            else
            {
                typeInfo = new TypeInfo();
            }

            return typeInfo;
        }

        private bool CheckTypeFormat(FormatterTypeStyle test, FormatterTypeStyle want) => (test & want) == want;

        private void PutTypeInfo(TypeInfo typeInfo) => _niPool.Push(typeInfo);
    }
}
