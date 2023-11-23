// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Net9.BinaryFormatter
{
    public abstract class SerializationBinder
    {
        /*
When overridden in a derived class, controls the binding of a serialized object to a type.
Parameters
serializedType    Type The type of the object the formatter creates a new instance of.
assemblyName    String Specifies the Assembly name of the serialized object.
typeName    String Specifies the Type name of the serialized object.
        */
        public virtual void BindToName(Type serializedType, out string? assemblyName, out string? typeName)
        {
            assemblyName = null;
            typeName = null;
        }

        public abstract Type? BindToType(string assemblyName, string typeName);
    }
}
