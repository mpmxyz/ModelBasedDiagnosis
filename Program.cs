using AAI6;
using System;

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
    var bMinus = new Value<Voltage>(Voltage.GROUND, name: "bMinus");

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
    var lMinus = new Value<Voltage>(Voltage.GROUND);
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

static Graph BigTest(
    int nWireSegments = 10,
    int nLines = 10,
    int nLit = 5
)
{
    

    var wires = new Wire[nWireSegments, nLines];
    var lamps = new Lamp[nLines];

    var lamps_lit = new Value<bool>[nLines];
    var lampWireVoltages = new Value<Voltage>[nWireSegments, nLines];

    int iValue = 0;
    var allValues = new IValue[2 + lamps_lit.Length + lampWireVoltages.Length];
    int iComponent = 0;
    var allComponents = new IComponent[wires.Length + lamps.Length];

    var batP = new Value<Voltage>(Voltage.PLUS, name: "batP");
    var batM = new Value<Voltage>(Voltage.GROUND, name: "batM");
    allValues[iValue++] = batP;
    allValues[iValue++] = batM;

    for (int l = 0; l < nLines; l++)
    {
        lamps_lit[l] = new(l < nLit, name: $"l{l}");
        allValues[iValue++] = lamps_lit[l];

        for (int x = 0; x < nWireSegments; x++)
        {
            lampWireVoltages[x, l] = new(name: $"U{x}_{l}");
            allValues[iValue++] = lampWireVoltages[x, l];
        }
        wires[0, l] = new(batP, lampWireVoltages[0, l], name: $"w{0}_{l}");
        allComponents[iComponent++] = wires[0, l];
        for (int x = 1; x < nWireSegments; x++)
        {
            wires[x, l] = new(lampWireVoltages[x - 1, l], lampWireVoltages[x, l], name: $"w{x}_{l}");
            allComponents[iComponent++] = wires[x, l];
        }
        lamps[l] = new(lampWireVoltages[nWireSegments - 1, l], batM, lamps_lit[l], name: $"l{l}");
        allComponents[iComponent++] = lamps[l];
    }

    return new Graph(allValues, allComponents);
}

IEnumerable<Func<Graph>> graphGenerators;

Console.WriteLine("0: AdderMultiplier (Assignment 6.1)");
Console.WriteLine("1: Electric Circuit (Assignment 6.3)");
Console.WriteLine("2: Wire Test");
Console.WriteLine("3: Lamp Test");
Console.WriteLine("4: Switch Test");
Console.WriteLine("5: Large scale test of parallel lamps with multiple wire segments in series");
Console.Write("Select your test: ");

int MAX_DISPLAYED_TOTAL_COUNT = 1000;
switch (Console.ReadLine())
{
    case "0":
        Console.WriteLine("m1, m2, m3, a1, a2");
        graphGenerators = [AdderMultiplierExample];
        break;
    case "1":
        Console.WriteLine("c1, s1, c2, c3, s2, c4, l, c5");
        graphGenerators = [
            () => AssignmentCircuit(false, false, false),
            () => AssignmentCircuit(true, false, false),
            () => AssignmentCircuit(true, true, true)
        ];
        break;
    case "2":
        graphGenerators = [WireTest];
        break;
    case "3":
        graphGenerators = [LampTest];
        break;
    case "4":
        graphGenerators = [SwitchTest];
        break;
    case "5":
        int nWireSegments, nLines, nLit;
        do
            Console.Write("# of wire segments per line: ");
        while (!int.TryParse(Console.ReadLine(), out nWireSegments) || nWireSegments <= 0);
        do
            Console.Write("# of parallel lines: ");
        while (!int.TryParse(Console.ReadLine(), out nLines) || nLines <= 0);
        do
            Console.Write("# of lit lamps: ");
        while (!int.TryParse(Console.ReadLine(), out nLit) || nLit < 0 || nLit > nLines);
        MAX_DISPLAYED_TOTAL_COUNT = 0;
        graphGenerators = [() => BigTest(nWireSegments, nLines, nLit)];
        break;
    default:
        return;
}
int maxCount;
do
    Console.Write("Max # of diagnoses: ");
while (!int.TryParse(Console.ReadLine(), out maxCount) || maxCount < 0);


var stopwatch = new System.Diagnostics.Stopwatch();
stopwatch.Start();

var solutions = Solver.Execute(graphGenerators, maxCount: maxCount, printProgress: true);
stopwatch.Stop();

Console.WriteLine($"Runtime: {stopwatch.ElapsedMilliseconds}ms");

var totalCount = Solver.Execute(graphGenerators, maxCount: MAX_DISPLAYED_TOTAL_COUNT).Count();
string totalText;
if (solutions.Count() < maxCount)
{
    totalText = $"{solutions.Count()}";
}
else if (totalCount >= MAX_DISPLAYED_TOTAL_COUNT)
{
    totalText = "many";
}
else
{
    totalText = $"{totalCount}";
}
Console.WriteLine($"Showing {solutions.Count()}/{totalText} solutions");

var componentNames = graphGenerators.First()().Components.Select(c => c.Name).ToArray();

Console.WriteLine(string.Join("; ", componentNames));
foreach ((var variants, var graphs) in solutions)
{
    Console.WriteLine(string.Join("; ", variants) + " => " + graphs.First().Likelyhood(variants));
    foreach (var graph in graphs)
    {
        Console.WriteLine("        " + string.Join("; ", graph.Values.Select(x => x.ToString())));
    }
}

Console.WriteLine("Press any key to continue...");
Console.ReadKey();
