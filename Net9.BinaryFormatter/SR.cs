// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using System.Globalization;

namespace Net9.BinaryFormatter
{
    internal class SR
    {
        internal const string Argument_DataLengthDifferent = nameof(Argument_DataLengthDifferent);
        internal const string Serialization_NotISer = nameof(Serialization_NotISer);
        internal const string Serialization_ConstructorNotFound = nameof(Serialization_ConstructorNotFound);
        internal const string Serialization_SameNameTwice = nameof(Serialization_SameNameTwice);
        internal const string Argument_MustSupplyContainer = nameof(Argument_MustSupplyContainer);
        internal const string Argument_MemberAndArray = nameof(Argument_MemberAndArray);
        internal const string Argument_MustSupplyParent = nameof(Argument_MustSupplyParent);
        internal const string Serialization_NotCyclicallyReferenceableSurrogate = nameof(Serialization_NotCyclicallyReferenceableSurrogate);
        internal const string Serialization_ObjectNotSupplied = nameof(Serialization_ObjectNotSupplied);
        internal const string Serialization_ParentChildIdentical = nameof(Serialization_ParentChildIdentical);
        internal const string Serialization_IncorrectNumberOfFixups = nameof(Serialization_IncorrectNumberOfFixups);
        internal const string Serialization_InvalidType = nameof(Serialization_InvalidType);
        internal const string Serialization_InvalidFixupType = nameof(Serialization_InvalidFixupType);
        internal const string Serialization_IdTooSmall = nameof(Serialization_IdTooSmall);
        internal const string Serialization_TooManyReferences = nameof(Serialization_TooManyReferences);
        internal const string Serialization_ObjectTypeEnum = nameof(Serialization_ObjectTypeEnum);
        internal const string Serialization_Assembly = nameof(Serialization_Assembly);
        internal const string Serialization_OptionalFieldVersionValue = nameof(Serialization_OptionalFieldVersionValue);
        internal const string Serialization_MissingMember = nameof(Serialization_MissingMember);
        internal const string Serialization_SerMemberInfo = nameof(Serialization_SerMemberInfo);
        internal const string Serialization_ArrayType = nameof(Serialization_ArrayType);
        internal const string Serialization_ArrayTypeObject = nameof(Serialization_ArrayTypeObject);
        internal const string Serialization_Map = nameof(Serialization_Map);
        internal const string Serialization_CrossAppDomainError = nameof(Serialization_CrossAppDomainError);
        internal const string Serialization_TypeMissing = nameof(Serialization_TypeMissing);
        internal const string Serialization_StreamEnd = nameof(Serialization_StreamEnd);
        internal const string Serialization_BinaryHeader = nameof(Serialization_BinaryHeader);
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
        internal const string Serialization_NonSerType = nameof(Serialization_NonSerType);
        internal const string Serialization_UnknownMemberInfo = nameof(Serialization_UnknownMemberInfo);

        internal static string Format(string str, params object?[]  args)
        {
            //return string.Format(str, args);
            return str + string.Join(", ", new object[] { str }.Concat(args));
        }

        internal static string Format(IFormatProvider fp, string str, params object?[] args)
        {
            //return string.Format(fp, str, args);
            return str + string.Join(", ", new object[] { str }.Concat(args));
        }
    }
}