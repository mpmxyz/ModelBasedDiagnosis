using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace AAI6
{
    internal class VariantsQueue
    {
        private static readonly EqualityComparer<uint[]> variantsEqualityComparer = new VariantsEqualityComparer();

        private class VariantsEqualityComparer : EqualityComparer<uint[]>
        {
            public override bool Equals(uint[]? x, uint[]? y)
            {
                if (x == y) return true;
                if (x == null || y == null) return false;
                if (x.Length != y.Length) return false;
                for (int i = 0; i < x.Length; i++)
                {
                    if (x[i] != y[i]) return false;
                }
                return true;
            }

            public override int GetHashCode([DisallowNull] uint[] obj)
            {
                int hashcode = 0;
                if (obj != null)
                {
                    foreach (var x in obj)
                    {
                        hashcode = HashCode.Combine(hashcode, x);
                    }
                }
                return hashcode;
            }
        }

        private readonly Graph templateGraph;
        private readonly PriorityQueue<uint[], (uint[], float)> queue = new(new VariantsComparer());
        private readonly HashSet<uint[]> knownVariants = new(variantsEqualityComparer);
        private readonly List<uint[]> allPreferred = [];

        public VariantsQueue(Graph initialGraph)
        {
            templateGraph = initialGraph;
            Enqueue(new uint[initialGraph.Components.Count()]);
        }

        public int KnownCount => knownVariants.Count;

        /// <summary>
        /// suppresses all descendants of the given list of variants<br/>
        /// This method must be called latest after dequeueing <paramref name="preferred"/> and before the next call of <see cref="Dequeue"/>
        /// </summary>
        /// <param name="preferred">preferred (and necessarily conflict free!) variants</param>
        public void AddPreferred(uint[] preferred)
        {
            allPreferred.Add(preferred);
        }

        public void Enqueue(uint[] variants)
        {
            if (knownVariants.Add(variants))
            {
                // Console.WriteLine($"added      {string.Join(", ", variants)}");
                queue.Enqueue(variants, (variants, templateGraph.Likelyhood(variants)));
            }
            else
            {
                //Console.WriteLine($"duplicated {string.Join(", ", variants)}");
            }
        }

        public void EnqueueDescendantsOf(uint[] variants, int[] conflictPattern)
        {
            int i = 0;
            foreach (var component in templateGraph.Components)
            {
                if (variants[i] + 1 < component.VariantCount && conflictPattern[i] != -1)
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
            var result = queue.Dequeue();
            DequeueNonPreferred();
            return result;
        }

        public bool HasItems()
        {
            return queue.Count > 0; 
        }

        public uint[] Peek()
        {
            return queue.Peek();
        }

        private void DequeueNonPreferred() {
            int x = 0;
            while (queue.Count > 0 && IsSuppressed(queue.Peek()))
            {
                //Console.WriteLine($"suppressed {string.Join(", ", queue.Peek())}");
                //make sure that decendants are also non-preferred
                queue.Dequeue();
            }
        }

        private bool IsSuppressed(uint[] variants)
        {
            return allPreferred.Any(preferredVariants => VariantsComparer.CompareVariants(preferredVariants, variants) < 0);
        }
    }
}
