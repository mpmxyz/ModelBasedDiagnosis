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

        public static IEnumerable<(uint[], IEnumerable<Graph>)> Execute(IEnumerable<Func<Graph>> graphGenerators, int maxCount = int.MaxValue, bool printProgress = false)
        {
            var initialGraph = graphGenerators.First()();
            var result = new List<(uint[], IEnumerable<Graph>)>();
            //var knownConflicts = new SparseVariantPatternSet()
            var knownConflicts = new VariantsTrie(initialGraph.VariantCounts);
            var variantsQueue = new VariantsQueue(initialGraph);

            int checkCount = 0, conflictCount = 0;

            void PrintProgress()
            {
                Console.CursorLeft = 0;
                Console.Write($"Found: {result!.Count}, Checked: {checkCount}, Known conflicts: {conflictCount}");
            }

            while (variantsQueue.HasItems())
            {
                var variants = variantsQueue.Dequeue();
                var graphs = new List<Graph>();
                int[]? conflictPattern = knownConflicts.Get(variants);
                checkCount++;
                if (conflictPattern == null)
                {
                    //Console.WriteLine($"conflict2  {string.Join(", ", variants)}");
                    foreach (var newGraph in graphGenerators)
                    {
                        conflictPattern = FillGraph(newGraph, variants, out var graph);
                        if (conflictPattern != null)
                        {
                            //Console.WriteLine($"conflict   {string.Join(", ", conflictPattern)}");
                            knownConflicts.Add(conflictPattern);
                            conflictCount++;
                            break;
                        }
                        else
                        {
                            graphs.Add(graph);
                        }
                    }
                }
                if (printProgress)
                {
                    PrintProgress();
                }
                if (conflictPattern == null)
                {
                    //Console.WriteLine($"conflict   {string.Join(", ", variants)}");

                    variantsQueue.AddPreferred(variants);
                    result.Add((variants, graphs));

                    if (printProgress)
                    {
                        PrintProgress();
                    }

                    if (result.Count >= maxCount)
                    {
                        break;
                    }
                }
                else
                {
                    variantsQueue.EnqueueDescendantsOf(variants, conflictPattern);
                }
            }
            //start with variants 0
            //try fill values
            //  apply components
            //    optimization: only touch components surrounding recent changes
            //  trying domain values -> CloneGraph
            //all noop -> valid solution found
            //conflict -> split to increment a variant each -> CloneVariants
            if (printProgress)
            {
                PrintProgress();
                Console.WriteLine(" Done!");
            }
            return result;
        }

        static int[]? FillGraph(
            Func<Graph> newGraph,
            uint[] variants,
            out Graph graph
        )
        {
            var processedValues = new HashSet<IValue>();
            var touchedValues = new HashSet<IValue>();
            var dirtyComponents = new HashSet<IComponent>();
            return FillGraph(
                newGraph,
                variants,
                out graph,
                processedValues,
                touchedValues,
                dirtyComponents
            );
        }

        static int[]? FillGraph(
            Func<Graph> newGraph,
            uint[] variants,
            out Graph graph,
            HashSet<IValue> processedValues,
            HashSet<IValue> touchedValues,
            HashSet<IComponent> dirtyComponents
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
                        case Result.Ok:
                            foreach (var value in component.ConnectedValues)
                            {
                                touchedValues.Add(value);
                            }
                            break;
                        case Result.Conflict conflict:
                            return GetConflictPattern(variants, graph, conflict);
                    }
                }
            }


            //TODO: speculatively assign unknown values
            //optimization: focus on values connected to nodes with the least amount of unknowns
            //no unassigned values: success
            return null;
        }

        private static int[] GetConflictPattern(uint[] variants, Graph graph, Result.Conflict conflict)
        {
            int[] conflictVariants = new int[graph.Components.Count()];
            for (int i = 0; i < conflictVariants.Length; i++)
            {
                conflictVariants[i] = -1;
            }
            HashSet<IValue> knownValues = [];
            List<(IValue[] values, IComponent? component)> sources = [conflict.Source, conflict.Source2];

            void AddComponentCandidate(IComponent? component)
            {
                if (component != null)
                {
                    uint index = graph.IndexOf(component);
                    conflictVariants[index] = (int) variants[index];
                }
            }
            void ProcessSource((IValue[] values, IComponent? component) source)
            {
                AddComponentCandidate(source.component);
                foreach (var value in source.values)
                {
                    var deps = value.Dependencies;
                    if (knownValues.Add(value) && deps != null)
                    {
                        sources.Add(((IValue[], IComponent?)) deps);
                    }
                }
            }

            for (int i = 0; i < sources.Count; i++)
            {
                ProcessSource(sources[i]);
            }

            return conflictVariants;
        }
    }
}
