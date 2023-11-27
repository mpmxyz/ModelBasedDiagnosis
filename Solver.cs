namespace AAI6
{
    internal class Solver
    {
        void PasteValues(Graph graph, IValue[] values)
        {
            uint i = 0;
            foreach (var newValue in graph.Values)
            {
                newValue.TrySetFrom(values[i]);
                i++;
            }
        }

        public static IEnumerable<(uint[], IEnumerable<Graph>)> Execute(IEnumerable<Func<Graph>> graphGenerators, int maxCount = int.MaxValue)
        {
            var initialGraph = graphGenerators.First()();
            var variantsQueue = new VariantsQueue(initialGraph);
            var result = new List<(uint[], IEnumerable<Graph>)>();

            while (variantsQueue.HasItems())
            {
                var variants = variantsQueue.Dequeue();
                var graphs = new List<Graph>();
                bool valid = true;
                foreach (var newGraph in graphGenerators)
                {
                    valid &= FillGraph(newGraph, variants, out var graph);
                    if (valid)
                    {
                        graphs.Add(graph);
                    }
                    else
                    {
                        break;
                    }
                }
                if (valid)
                {
                    result.Add((variants, graphs));
                    if (result.Count >= maxCount)
                    {
                        break;
                    }
                }
                else
                {
                    //Console.WriteLine(string.Join(", ", variants) + " /// " + string.Join(", ", graph.Values.Select(x => x.ToString())) + " => XXX");
                    variantsQueue.EnqueueDescendantsOf(variants);
                }
            }
            //start with variants 0
            //try fill values
            //  apply components
            //    optimization: only touch components surrounding recent changes
            //  trying domain values -> CloneGraph
            //all noop -> valid solution found
            //conflict -> split to increment a variant each -> CloneVariants

            return result;
        }

        static bool FillGraph(Func<Graph> newGraph, uint[] variants, out Graph graph)
        {
            var processedValues = new HashSet<IValue>();
            var touchedValues = new HashSet<IValue>();
            var dirtyComponents = new HashSet<IComponent>();
            return FillGraph(newGraph, variants, out graph, processedValues, touchedValues, dirtyComponents);
        }

        static bool FillGraph(
            Func<Graph> newGraph,
            uint[] variants,
            out Graph graph,
            ISet<IValue> processedValues,
            ISet<IValue> touchedValues,
            ISet<IComponent> dirtyComponents
        )
        {
            graph = newGraph();

            foreach (var value in graph.Values)
            {
                if (value.Assigned)
                {
                    touchedValues.Add(value);
                }
                else if (value.DomainSize == 1)
                {
                    value.TrySetByDomain(0);
                }
            }

            while (touchedValues.Count > 0)
            {
                dirtyComponents.Clear();
                foreach (var value in touchedValues)
                {
                    if (value.Assigned && processedValues.Add(value))
                    {
                        foreach(var component in graph.GetComponentsWithValue(value))
                        {
                            dirtyComponents.Add(component);
                        }
                    }
                }
                touchedValues.Clear();
                foreach (var component in dirtyComponents)
                {
                    var result = component.Apply(variants[graph.IndexOf(component)]);
                    //Console.WriteLine(string.Join(", ", variants) + " /// " + string.Join(", ", graph.Values.Select(x => x.ToString())) + " => " + result + $"({graph.IndexOf(component)})");

                    switch (result)
                    {
                        case Result.OK:
                            foreach (var value in component.ConnectedValues)
                            {
                                touchedValues.Add(value);
                            }
                            break;
                        case Result.CONFLICT:
                            return false;
                    }
                }
            }


            //TODO: speculatively assign unknown values
            //optimization: focus on values connected to nodes with the least amount of unknowns
            //no unassigned values: success
            return true;
        }
    }
}
