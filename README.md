# Net9.BinaryFormatter
BinaryFormatter for net9 and later.

BinaryFormatter will possibly be removed in net9. This code is copied from net8.0.0
Everything is copied so it is completely independent.

Even these types are copied and are independent:
```
SerializationException
SerializationInfo
StreamingContext
SerializableAttribute
NonSerializedAttribute
ISerializable
```
It also does not use `FieldAttributes.NotSerialized` or `Type.IsSerializable (TypeAttributes.Serializable)`.

Since it is independent, it also means it can not do anything out of the box, since no types is attributed `[Net9.BinaryFormatter.Serializable]` or implementing `Net9.BinaryFormatter.ISerializable`.
So you would need to change your code:
````
from [Serializable] -> [Net9.BinaryFormatter.Serializable]
from [NonSerialized] -> [Net9.BinaryFormatter.NonSerialized]
from ISerializable -> Net9.BinaryFormatter.ISerializable
````
etc.

After this you should be able to serialize your own types. But what about the ones in the runtime? Yes, this is WIP, but some types have been added and you can enable like this:
```
var bf = new BinaryFormatter();
bf.SurrogateSelector = new ConverterSelector(); // add default converters, currently Dictionary<,>, HashSet<>.
bf.IsSerializable = new IsSerializableHandlers(); // add default IsSerializable handlers, currently List<>, Stack<>, DateTime, KeyValuePair<,>, etc.
```
So this is very far from a drop in replacement and require a lot of manual work to switch over. So when switching, might just as well switch to `System.Text.Json`. But alternatives are good, and who knows, in some very special cases, switching to `System.Text.Json` may not be possible.

