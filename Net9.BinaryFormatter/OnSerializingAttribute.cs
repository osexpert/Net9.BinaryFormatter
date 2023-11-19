// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Net9.BinaryFormatter
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class OnSerializingAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class OnSerializedAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class OnDeserializingAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class OnDeserializedAttribute : Attribute
    {
    }
}
