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
        internal const string Serialization_TypeExpected = nameof(Serialization_TypeExpected);
        internal const string Serialization_MissingObject = nameof(Serialization_MissingObject);
        internal const string Serialization_InvalidFixupDiscovered = nameof(Serialization_InvalidFixupDiscovered);
        internal const string Serialization_TypeLoadFailure = nameof(Serialization_TypeLoadFailure);
        internal const string Serialization_PartialValueTypeFixup = nameof(Serialization_PartialValueTypeFixup);
        internal const string Serialization_ValueTypeFixup = nameof(Serialization_ValueTypeFixup);
        internal const string ArgumentOutOfRange_ObjectID = nameof(ArgumentOutOfRange_ObjectID);
        internal const string Serialization_UnableToFixup = nameof(Serialization_UnableToFixup);
        internal const string Serialization_RegisterTwice = nameof(Serialization_RegisterTwice);
        internal const string Serialization_DangerousDeserialization_Switch = nameof(Serialization_DangerousDeserialization_Switch);
        internal const string Serialization_TopObjectInstantiate = nameof(Serialization_TopObjectInstantiate);
        internal const string Serialization_ParseError = nameof(Serialization_ParseError);
        internal const string Serialization_XMLElement = nameof(Serialization_XMLElement);
        internal const string Serialization_TopObject = nameof(Serialization_TopObject);
        internal const string Serialization_ISerializableTypes = nameof(Serialization_ISerializableTypes);
        internal const string Serialization_NoMemberInfo = nameof(Serialization_NoMemberInfo);
        internal const string Serialization_TypeCode = nameof(Serialization_TypeCode);
        internal const string Serialization_MemberInfo = nameof(Serialization_MemberInfo);
        internal const string Serialization_ISerializableMemberInfo = nameof(Serialization_ISerializableMemberInfo);
        internal const string Arg_HTCapacityOverflow = nameof(Arg_HTCapacityOverflow);
        internal const string Serialization_TooManyElements = nameof(Serialization_TooManyElements);
        internal const string Serialization_ObjNoID = nameof(Serialization_ObjNoID);
        internal const string InvalidOperation_EnumOpCantHappen = nameof(InvalidOperation_EnumOpCantHappen);
        internal const string Serialization_CorruptedStream = nameof(Serialization_CorruptedStream);
        internal const string Serialization_NotFound = nameof(Serialization_NotFound);
        internal const string Argument_MustBeRuntimeType = nameof(Argument_MustBeRuntimeType);
        internal const string SerializationException = nameof(SerializationException);
        internal const string Serialization_TypeRead = nameof(Serialization_TypeRead);
        internal const string Serialization_TypeWrite = nameof(Serialization_TypeWrite);
        internal const string Serialization_AssemblyId = nameof(Serialization_AssemblyId);
        internal const string Serialization_AssemblyNotFound = nameof(Serialization_AssemblyNotFound);
        internal const string Serialization_InvalidFormat = nameof(Serialization_InvalidFormat);
        internal const string IO_EOF_ReadBeyondEOF = nameof(IO_EOF_ReadBeyondEOF);
        internal const string ArgumentNull_NullMember = nameof(ArgumentNull_NullMember);
        internal const string Argument_InvalidFieldInfo = nameof(Argument_InvalidFieldInfo);
        internal const string BinaryFormatter_SerializationDisallowed = nameof(BinaryFormatter_SerializationDisallowed);
        internal const string Serialization_Stream = nameof(Serialization_Stream);
        internal const string Serialization_NeverSeen = nameof(Serialization_NeverSeen);
        internal const string Serialization_IORIncomplete = nameof(Serialization_IORIncomplete);
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