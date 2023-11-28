﻿using System.Xml.Linq;

namespace AAI6
{
    internal class Adder(IValue<int> a, IValue<int> b, IValue<int> sum, string name = "") : IComponent
    {
        private readonly IValue<int> a = a;
        private readonly IValue<int> b = b;
        private readonly IValue<int> sum = sum;

        public IEnumerable<IValue> ConnectedValues => [a, b, sum];

        public uint VariantCount => 2;

        public string Name { get; } = name;


        public Result Apply(uint variant)
        {
            Result result = Result.Noop.Instance;
            switch(variant)
            {
                case 0:
                    int vA, vB, vSum;
                    if (a.TryGet(out vA) && b.TryGet(out vB))
                    {
                        result = result.CombineWith(sum.TrySet(vA + vB, ([a, b], this)));
                    }
                    if (a.TryGet(out vA) && sum.TryGet(out vSum))
                    {
                        result = result.CombineWith(b.TrySet(vSum - vA, ([sum, a], this)));
                    }
                    if (b.TryGet(out vB) && sum.TryGet(out vSum))
                    {
                        result = result.CombineWith(a.TrySet(vSum - vB, ([sum, b], this)));
                    }
                    break;
            }
            return result;
        }

        public float Likelyhood(uint variant)
        {
            return variant switch
            {
                0 => 1,
                _ => 0.5f,
            };
        }
    }
}
