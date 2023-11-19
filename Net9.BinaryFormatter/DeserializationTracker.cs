﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Net9.BinaryFormatter
{
    // Tracks whether deserialization is currently in progress
    internal sealed class DeserializationTracker
    {
        // True if the thread this tracker applies to is currently deserializing
        // potentially untrusted data
        internal bool DeserializationInProgress { get; set; }
    }
}
