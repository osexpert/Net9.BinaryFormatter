// See https://aka.ms/new-console-template for more information
using Net9.BinaryFormatter;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

public class Program
{
    //[Net9.BinaryFormatter.Serializable]
    public enum Lol42
    {
        Test = 5
    }

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

        var to = TimeOnly.FromDateTime(DateTime.Now);

        var cs = new ConverterSelector();

//        cs.Converters.Add(new GenericStackConverterFactory());
        //cs.Converters.Add(new Net9.BinaryFormatter.Converters.DateTimeConverter());

        bf.SurrogateSelector = new ConverterSelector();

        bf.Control.IsSerializableHandlers = new IsSerializableHandlers();
        bf.Control.IsSerializableHandlers.Handlers.OfType<SerializeAllowedTypes>().Single().AllowedTypes.Add(typeof(object));
        //bf.Control.IsSerializableHandlers.Handlers.OfType<SerializeAllowedTypes>().Single().AllowedTypes.Add(typeof(IConvertible));
        //bf.Control.IsSerializableHandlers.Handlers.OfType<SerializeAllowedTypes>().Single().AllowedTypes.Add(typeof(Test));
        var b = new AllowedTypesBinder();
        b.AddAllowedType(typeof(Test));
        b.AddAllowedType(typeof(IConvertible));
        b.AddAllowedType(typeof(IComparable));
        bf.Binder = b;

        //bf.Control = 
        //   bf.IsSerializable = new IsSerializableHandlers().IsSerializable;
        //        bf.Binder

        var hs= new HashSet<int>() { 5 };
        var sta = new Stack<int>();
        sta.Push(45);
        sta.Push(145);

        try
        {
            throw new Exception("lol");
        }
        catch (Exception e)
        {
            TraceFlags.Formatter_IConvertibleFix = true;
            TraceFlags.Formatter_IConvertibleArrayFix = true;

            var nt = new Test();
            var t = nt.dict.GetType();

            bf.Serialize(ms, nt);// list);
        }

        //var bf_desser = new BinaryFormatter();
        //bf_desser.SurrogateSelector = new ConverterSelector();
        //bf_desser.Control = new IsSerializableAlwaysTrue();


        ms.Position = 0;

        var g = bf.Deserialize(ms);//< List<Dictionary<int, string>>>(ms);

        //var sta2 = (Stack<int>)g;
        //var p1 = sta2.Pop();
        //var p2 = sta2.Pop();

        Console.WriteLine("Hello, World!");
    }
}

[Net9.BinaryFormatter.Serializable]
public class Test
{
    public int? nullable = null;
    public int? nullable2 = 42;
    public int i = 55;
    public IConvertible icon_null = null!;
    public object obj_4 = 4;
    public object[] objarr = new IConvertible[] { 5,5d,7M,77};
    public object[] objarr2 = new object[] { 5, 5d, 7M, 77, 98f };
    public IConvertible icon_4 = 4;
    public IConvertible[] iconarr = new IConvertible[] { 1, 2, 3, 4 };
    public int[] intarr = new int[] { 1, 2, 3, 4 };
    public TimeOnly to = new TimeOnly(42);
    public DateOnly don = new DateOnly(1, 2, 3);
    public IComparable comp = 4;
    public IComparable[] comparr = new IComparable[] { 1, 2, 3, 4 };
    public Dictionary<string, List<TimeSpan>> dict = new();
    public List<Dictionary<string, List<TimeSpan>>> dicts = new();

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

class IsSerializableAlwaysTrue : SerializationControl
{
    public override bool IsSerializable(Type type)
    {
        return true;
    }
}