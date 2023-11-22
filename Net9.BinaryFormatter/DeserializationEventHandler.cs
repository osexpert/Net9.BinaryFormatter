// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Net9.BinaryFormatter
{
    internal delegate void DeserializationEventHandler(object? sender);

    public delegate void SerializationEventHandler(StreamingContext context);

    public interface IDeserializationCallback
    {
        void OnDeserialization(object? sender);
    }
}
