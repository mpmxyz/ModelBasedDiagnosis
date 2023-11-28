namespace AAI6
{
    /// <summary>
    /// assumptions:
    /// - fixed number of possible variants known for each component
    /// - all patterns are of the same length as variants
    /// - index 0 in node is "don't care"
    /// </summary>
    /// <param name="variantCounts"></param>
    internal class VariantsTrie(uint[] variantCounts)
    {
        private readonly uint[] VariantCounts = variantCounts;
        private readonly Node root = new(variantCounts[0]);

        public void Add(int[] pattern)
        {
            root.Add(pattern, VariantCounts);
        }

        public int[] Get(uint[] variants)
        {
            return root.Get(variants);
        }

        private class Node(uint dimSize)
        {
            private static readonly Node Leaf = new(0);

            private readonly Node[] nextNodes = new Node[dimSize + 1];

            public void Add(int[] pattern, uint[] dimSizes, uint dim = 0)
            {
                uint index = (uint) pattern[dim++] + 1;
                if (dim == dimSizes.Length)
                {
                    nextNodes[index] = Leaf;
                }
                else
                {
                    Node nextNode = nextNodes[index];
                    if (nextNode == null)
                    {
                        nextNode = new Node(dimSizes[dim]);
                        nextNodes[index] = nextNode;
                    }
                    nextNode.Add(pattern, dimSizes, dim);
                }
            }

            public int[] Get(uint[] variants, uint dim = 0)
            {
                if (this == Leaf)
                {
                    return new int[variants.Length];
                }
                uint index = variants[dim] + 1;
                int[]? matchingPattern = nextNodes[index]?.Get(variants, dim + 1);
                if (matchingPattern != null)
                {
                    matchingPattern[dim] = (int) variants[dim];
                }
                else
                {
                    matchingPattern = nextNodes[0]?.Get(variants, dim + 1); //don't care
                    if (matchingPattern != null)
                    {
                        matchingPattern[dim] = -1;
                    }
                }
                return matchingPattern;
            }
        }
    }

    
}
