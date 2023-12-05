
namespace AAI6
{
    internal class SparseVariantPatternSet
    {
        private readonly List<(List<(uint index, uint value)> compactedPattern, int[] fullPattern)> patterns = [];

        public SparseVariantPatternSet() { }

        public void Add(int[] pattern)
        {
            List<(uint index, uint value)> compactedPattern = [];
            uint patternIndex = 0;
            foreach (int patternValue in pattern) { 
                if (patternValue >= 0)
                {
                    compactedPattern.Add((patternIndex, (uint) patternValue));
                }
                patternIndex++;
            }
            patterns.Add((compactedPattern, pattern));
        }

        public int[] Get(uint[] variants)
        {
            foreach (var (compactedPattern, pattern) in patterns)
            {
                var valid = true;
                foreach ((uint index, uint value) in compactedPattern)
                {
                    if (variants[index] != value)
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                {
                    return pattern;
                }
            }
            return null;
        }
    }
}
