using System.Collections;

namespace AAI6
{
    internal class VariantsQueue
    {
        private readonly Graph templateGraph;
        private readonly PriorityQueue<uint[], (uint[], float)> queue =
            new PriorityQueue<uint[], (uint[], float)>(new VariantsComparer());
        private readonly ISet<uint[]> known = new HashSet<uint[]>(
            EqualityComparer<uint[]>.Create(
                StructuralComparisons.StructuralEqualityComparer.Equals,
                StructuralComparisons.StructuralEqualityComparer.GetHashCode
            )
        );

        public VariantsQueue(Graph initialGraph)
        {
            templateGraph = initialGraph;
            Enqueue(new uint[initialGraph.Components.Count()]);
        }

        private class VariantsComparer : IComparer<(uint[] variants, float likelyhood)>
        {
            public int Compare((uint[] variants, float likelyhood) x, (uint[] variants, float likelyhood) y)
            {
                bool xLessOrEqual = true, yLessOrEqual = true;
                for (int i = 0; i < x.variants.Length; i++)
                {
                    uint xVar = x.variants[i], yVar = y.variants[i];
                    if (xVar > yVar)
                    {
                        xLessOrEqual = false;
                    }
                    else if (xVar < yVar)
                    {
                        yLessOrEqual = false;
                    }
                }
                if (xLessOrEqual && !yLessOrEqual)
                {
                    return -1;
                }
                else if (!xLessOrEqual && yLessOrEqual)
                {
                    return +1;
                }
                if (x.likelyhood > y.likelyhood)
                {
                    return -1; //x more likely than y -> x is prioritized to/smaller than y
                }
                else if (x.likelyhood < y.likelyhood)
                {
                    return +1; //y more likely than x -> y is prioritized to/smaller than x
                }
                return 0;
            }
        }


        public void Enqueue(uint[] variants)
        {
            if (known.Add(variants))
            {
                queue.Enqueue(variants, (variants, templateGraph.Likelyhood(variants)));
            }
        }

        public void EnqueueDescendantsOf(uint[] variants)
        {
            int i = 0;
            foreach (var component in templateGraph.Components)
            {
                if (variants[i] + 1 < component.VariantCount)
                {
                    var copy = (uint[]) variants.Clone();
                    copy[i]++;
                    Enqueue(copy);
                }
                i++;
            }
        }

        public uint[] Dequeue()
        {
            return queue.Dequeue();
        }

        public bool HasItems()
        {
            return queue.Count > 0; 
        }
    }
}
