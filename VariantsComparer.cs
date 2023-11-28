namespace AAI6
{
    internal class VariantsComparer : IComparer<(uint[] variants, float likelyhood)>
    {
        public static int CompareVariants(uint[] x, uint[] y)
        {
            bool xLessOrEqual = true, yLessOrEqual = true;
            for (int i = 0; i < x.Length; i++)
            {
                uint xVar = x[i], yVar = y[i];
                if (xVar > yVar)
                {
                    xLessOrEqual = false;
                    if (!yLessOrEqual)
                    {
                        break;
                    }
                }
                else if (xVar < yVar)
                {
                    yLessOrEqual = false;
                    if (!xLessOrEqual)
                    {
                        break;
                    }
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
            return 0;
        }

        public int Compare((uint[] variants, float likelyhood) x, (uint[] variants, float likelyhood) y)
        {
            int tupleCompare = CompareVariants(x.variants, y.variants);
            if (tupleCompare != 0)
            {
                return tupleCompare;
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
}
