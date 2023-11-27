using AAI6;

static Graph AdderMultiplierExample()
{
    var a = new Value<int>(2);
    var b = new Value<int>(3);
    var c = new Value<int>(2);
    var d = new Value<int>(3);
    var e = new Value<int>(2);
    var f = new Value<int>(3);

    var x = new Value<int>();
    var y = new Value<int>();
    var z = new Value<int>();

    var g = new Value<int>(11);
    var h = new Value<int>(11);

    var a1 = new Adder(x, y, g);
    var a2 = new Adder(y, z, h);

    var m1 = new Multiplier(a, b, x);
    var m2 = new Multiplier(c, d, y);
    var m3 = new Multiplier(e, f, z);

    return new Graph(
        new IValue[] { a, b, c, d, e, f, x, y, z, g, h },
        new IComponent[] { m1, m2, m3, a1, a2 }
    );
}

static Graph AssignmentCircuit(bool switch1, bool switch2, bool lamp)
{
    var bPlus = new Value<Voltage>(Voltage.PLUS, name: "bPlus");
    var s1a = new Value<Voltage>(name: "s1a");
    var s1state = new Value<bool>(switch1, name: "s1state");
    var s1bTrue = new Value<Voltage>(name: "s1bTrue");
    var s1bFalse = new Value<Voltage>(name: "s1bFalse");
    var s2bTrue = new Value<Voltage>(name: "s2bTrue");
    var s2bFalse = new Value<Voltage>(name: "s2bFalse");
    var s2state = new Value<bool>(switch2, name: "s2state");
    var s2a = new Value<Voltage>(name: "s2a");
    var lPlus = new Value<Voltage>(name: "lPlus");
    var lLit = new Value<bool>(lamp, name: "lLit");
    var lMinus = new Value<Voltage>(name: "lMinus");
    var bMinus = new Value<Voltage>(Voltage.MINUS, name: "bMinus");

    var c1 = new Wire(bPlus, s1a);
    var s1 = new Switch(s1a, s1bTrue, s1bFalse, s1state);
    var c2 = new Wire(s1bFalse, s2bFalse);
    var c3 = new Wire(s1bTrue, s2bTrue);
    var s2 = new Switch(s2a, s2bTrue, s2bFalse, s2state);
    var c4 = new Wire(s2a, lPlus);
    var l = new Lamp(lPlus, lMinus, lLit);
    var c5 = new Wire(lMinus, bMinus);

    return new Graph(
        [bPlus, s1a, s1state, s1bTrue, s1bFalse, s2bTrue, s2bFalse, s2state, s2a, lPlus, lLit, lMinus, bMinus],
        [c1, s1, c2, c3, s2, c4, l, c5]
    );
}

static Graph LampTest()
{
    var lPlus = new Value<Voltage>(Voltage.PLUS);
    var lLit = new Value<bool>(false);
    var lMinus = new Value<Voltage>(Voltage.MINUS);
    var l = new Lamp(lPlus, lMinus, lLit);
    return new Graph([lPlus, lLit, lMinus], [l]);
}

static Graph WireTest()
{
    var a = new Value<Voltage>(Voltage.PLUS);
    var b = new Value<Voltage>(Voltage.PLUS);
    var c = new Wire(a, b);
    return new Graph([a, b], [c]);
}

static Graph SwitchTest()
{
    var a = new Value<Voltage>(Voltage.PLUS, name: "a");
    var bTrue = new Value<Voltage>(name: "bTrue");
    var bFalse = new Value<Voltage>(name: "bFalse");
    var state = new Value<bool>(false);
    var s = new Switch(a, bTrue, bFalse, state);
    return new Graph([a, bTrue, bFalse, state], [s]);
}

IEnumerable<(uint[], IEnumerable<Graph>)> solutions;

Console.WriteLine("0: AdderMultiplier (Assignment 6.1)");
Console.WriteLine("1: Electric Circuit (Assignment 6.3)");
Console.WriteLine("2: Wire Test");
Console.WriteLine("3: Lamp Test");
Console.WriteLine("4: Switch Test");
Console.Write("Select your test: ");
switch(Console.ReadLine())
{
    case "0":
        Console.WriteLine("m1, m2, m3, a1, a2");
        solutions = Solver.Execute([AdderMultiplierExample]);
        break;
    case "1":
        Console.WriteLine("c1, s1, c2, c3, s2, c4, l, c5");
        solutions = Solver.Execute([
            () => AssignmentCircuit(false, false, false),
            () => AssignmentCircuit(true, false, false),
            () => AssignmentCircuit(true, true, true)
        ]);
        break;
    case "2":
        solutions = Solver.Execute([WireTest]);
        break;
    case "3":
        solutions = Solver.Execute([LampTest]);
        break;
    case "4":
        solutions = Solver.Execute([SwitchTest]);
        break;
    default:
        return;
}

foreach ((var variants, var graphs) in solutions)
{
    Console.WriteLine(string.Join(", ", variants) + " => " + graphs.First().Likelyhood(variants));
    foreach (var graph in graphs)
    {
        Console.WriteLine("        " + string.Join(", ", graph.Values.Select(x => x.ToString())));
    }
}
