// See https://aka.ms/new-console-template for more information
using FromGore;
using Net9.BinaryFormatter;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public class Program
{
    public static void Main()
    {

        Console.WriteLine("Hello, World!");

        var bf = new BinaryFormatter();
        var ms = new MemoryStream();

        var d = new Dictionary<int, string>();
        d.Add(4, "dd");
        d.Add(5, "dd");
        d.Add(6, "dd");

        var d2 = new Dictionary<int, string>();
        d2.Add(4, "dd2");
        d2.Add(5, "dd2");
        d2.Add(6, "dd2");

        var list = new List<Dictionary<int, string>>();
        list.Add(d);
        list.Add(d2);


        
        var kv = new KeyValuePair2<int, int>(1, 2);

        var to = TimeOnly.FromDateTime(DateTime.Now); // FIXME

        BinarySerializerOptions opt = new BinarySerializerOptions(true);
        opt.Surrogates.Add(new GenDictSurrogate());
        bf.SurrogateSelector = new SafeSurrogateSelector(bf.SurrogateSelector, opt);

        bf.Serialize(ms, list);


        // hvorfor funker det med keyvalue pair??
        //hva med andre structs?
        //KeyValuePair<

        ms.Position = 0;

        var g = bf.Deserialize(ms);

        Console.WriteLine("Hello, World!");
    }
}

[Net9.BinaryFormatter.Serializable]
public readonly struct KeyValuePair2<TKey, TValue>
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly TKey key; // Do not rename (binary serialization)
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly TValue value; // Do not rename (binary serialization)

    public KeyValuePair2(TKey key, TValue value)
    {
        this.key = key;
        this.value = value;
    }

    public TKey Key => key;

    public TValue Value => value;

  
}