// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace Net9.BinaryFormatter
{
    public sealed partial class BinaryFormatter : IFormatter
    {
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

            var reader = new ObjectReader(serializationStream, _surrogates, _context, formatterEnums, _binder, _isSerializable, _isNotSerialized)
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
                var sow = new ObjectWriter(_surrogates, _context, formatterEnums, _binder, _isSerializable, _isNotSerialized);
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
