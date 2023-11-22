// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Net9.BinaryFormatter
{
    internal class SR
    {
        internal const string Argument_DataLengthDifferent = "Parameters 'members' and 'data' must have the same length.";
        internal const string Serialization_NotISer = "The given object does not implement the ISerializable interface.";
        internal const string Serialization_ConstructorNotFound = "The constructor to deserialize an object of type '{0}' was not found.";
        internal const string Serialization_SameNameTwice = "Cannot add the same member twice to a SerializationInfo object.";
        internal const string Argument_MustSupplyContainer = "When supplying a FieldInfo for fixing up a nested type, a valid ID for that containing object must also be supplied.";
        internal const string Argument_MemberAndArray = "Cannot supply both a MemberInfo and an Array to indicate the parent of a value type.";
        internal const string Argument_MustSupplyParent = "When supplying the ID of a containing object, the FieldInfo that identifies the current field within that object must also be supplied.";
        internal const string Serialization_NotCyclicallyReferenceableSurrogate = "{0}.SetObjectData returns a value that is neither null nor equal to the first parameter. Such Surrogates cannot be part of cyclical reference.";
        internal const string Serialization_ObjectNotSupplied = "The object with ID {0} was referenced in a fixup but does not exist.";
        internal const string Serialization_ParentChildIdentical = "The ID of the containing object cannot be the same as the object ID.";
        internal const string Serialization_IncorrectNumberOfFixups = "The ObjectManager found an invalid number of fixups. This usually indicates a problem in the Formatter.";
        internal const string Serialization_InvalidType = "Only system-provided types can be passed to the GetUninitializedObject method. '{0}' is not a valid instance of a type.";
        internal const string Serialization_InvalidFixupType = "A member fixup was registered for an object which implements ISerializable or has a surrogate. In this situation, a delayed fixup must be used.";
        internal const string Serialization_IdTooSmall = "Object IDs must be greater than zero.";
        internal const string Serialization_TooManyReferences = "The implementation of the IObjectReference interface returns too many nested references to other objects that implement IObjectReference.";
        internal const string Serialization_ObjectTypeEnum = "Invalid ObjectTypeEnum {0}.";
        internal const string Serialization_Assembly = "No assembly information is available for object on the wire, '{0}'.";
        internal const string Serialization_OptionalFieldVersionValue = "Version value must be positive.";
        internal const string Serialization_MissingMember = "Member '{0}' in class '{1}' is not present in the serialized stream and is not marked with {2}.";
        internal const string Serialization_SerMemberInfo = "MemberInfo type {0} cannot be serialized.";
        internal const string Serialization_ArrayType = "Invalid array type '{0}'.";
        internal const string Serialization_ArrayTypeObject = "Array element type is Object, 'dt' attribute is null.";
        internal const string Serialization_Map = "No map for object '{0}'.";
        internal const string Serialization_CrossAppDomainError = "Cross-AppDomain BinaryFormatter error; expected '{0}' but received '{1}'.";
        internal const string Serialization_TypeMissing = "Type is missing for member of type Object '{0}'.";
        internal const string Serialization_StreamEnd = "End of Stream encountered before parsing was completed.";
        internal const string Serialization_BinaryHeader = "Binary stream '{0}' does not contain a valid BinaryHeader. Possible causes are invalid stream or object version change between serialization and deserialization.";
        internal const string Serialization_TypeExpected = "Invalid expected type.";
        internal const string Serialization_MissingObject = "The object with ID {0} was referenced in a fixup but has not been registered.";
        internal const string Serialization_InvalidFixupDiscovered = "A fixup on an object implementing ISerializable or having a surrogate was discovered for an object which does not have a SerializationInfo available.";
        internal const string Serialization_TypeLoadFailure = "Unable to load type {0} required for deserialization.";
        internal const string Serialization_PartialValueTypeFixup = "Fixing up a partially available ValueType chain is not implemented.";
        internal const string Serialization_ValueTypeFixup = "ValueType fixup on Arrays is not implemented.";
        internal const string ArgumentOutOfRange_ObjectID = "objectID cannot be less than or equal to zero.";
        internal const string Serialization_UnableToFixup = "Cannot perform fixup.";
        internal const string Serialization_RegisterTwice = "An object cannot be registered twice.";
       // internal const string Serialization_DangerousDeserialization_Switch = nameof(Serialization_DangerousDeserialization_Switch);
        internal const string Serialization_TopObjectInstantiate = "Top object cannot be instantiated for element '{0}'.";
        internal const string Serialization_ParseError = "Parse error. Current element is not compatible with the next element, {0}.";
        internal const string Serialization_XMLElement = "Invalid element '{0}'.";
        internal const string Serialization_TopObject = "No top object.";
        internal const string Serialization_ISerializableTypes = "Types not available for ISerializable object '{0}'.";
        internal const string Serialization_NoMemberInfo = "No MemberInfo for Object {0}.";
        internal const string Serialization_TypeCode = "Invalid type code in stream '{0}'.";
        internal const string Serialization_MemberInfo = "MemberInfo cannot be obtained for ISerialized Object '{0}'.";
        internal const string Serialization_ISerializableMemberInfo = "MemberInfo requested for ISerializable type.";
        internal const string Arg_HTCapacityOverflow = "Hashtable's capacity overflowed and went negative. Check load factor, capacity and the current size of the table.";
        internal const string Serialization_TooManyElements = "The internal array cannot expand to greater than Int32.MaxValue elements.";
        internal const string Serialization_ObjNoID = "Object {0} has never been assigned an objectID.";
        internal const string InvalidOperation_EnumOpCantHappen = "Enumeration has either not started or has already finished.";
        internal const string Serialization_CorruptedStream = "Invalid BinaryFormatter stream.";
        internal const string Serialization_NotFound = "Member '{0}' was not found.";
        internal const string Argument_MustBeRuntimeType = "Type must be a runtime Type object.";
        internal const string SerializationException = "Serialization error.";
        internal const string Serialization_TypeRead = "Invalid read type request '{0}'.";
        internal const string Serialization_TypeWrite = "Invalid write type request '{0}'.";
        internal const string Serialization_AssemblyId = "No assembly ID for object type '{0}'.";
        internal const string Serialization_AssemblyNotFound = "Unable to find assembly '{0}'.";
        internal const string Serialization_InvalidFormat = "The input stream is not a valid binary format. The starting contents (in bytes) are: {0} ...";
        internal const string IO_EOF_ReadBeyondEOF = "Unable to read beyond the end of the stream.";
        internal const string ArgumentNull_NullMember = "Member at position {0} was null.";
        internal const string Argument_InvalidFieldInfo = "The FieldInfo object is not valid.";
//        internal const string BinaryFormatter_SerializationDisallowed = nameof(BinaryFormatter_SerializationDisallowed);
        internal const string Serialization_Stream = "Attempting to deserialize an empty stream.";
        internal const string Serialization_NeverSeen = "A fixup is registered to the object with ID {0}, but the object does not appear in the graph.";
        internal const string Serialization_IORIncomplete = "The object with ID {0} implements the IObjectReference interface for which all dependencies cannot be resolved. The likely cause is two instances of IObjectReference that have a mutual dependency on each other.";
        internal const string Serialization_NonSerType = "Type '{0}' in Assembly '{1}' is not marked as serializable.";
        internal const string Serialization_UnknownMemberInfo = "Only FieldInfo, PropertyInfo, and SerializationMemberInfo are recognized.";

        internal static string Format(string str, params object?[]  args)
        {
            return string.Format(str, args);
            //return str + string.Join(", ", new object[] { str }.Concat(args));
        }

        internal static string Format(IFormatProvider fp, string str, params object?[] args)
        {
            return string.Format(fp, str, args);
            //return str + string.Join(", ", new object[] { str }.Concat(args));
        }
    }
}