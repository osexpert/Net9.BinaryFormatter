
// See https://aka.ms/new-console-template for more information
using System.Runtime.Serialization.Formatters.Binary;

Console.WriteLine("Hello, World!");

#pragma warning disable SYSLIB0011 // Type or member is obsolete



var bf = new BinaryFormatter();
#pragma warning restore SYSLIB0011 // Type or member is obsolet

var ms = new MemoryStream();
bf.Serialize(ms, 42);

Console.WriteLine("Hello, World!");
