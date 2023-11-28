namespace AAI6
{
    internal class Multiplier(IValue<int> a, IValue<int> b, IValue<int> product, string name = "") : IComponent
    {
        private readonly IValue<int> a = a;
        private readonly IValue<int> b = b;
        private readonly IValue<int> product = product;

        public IEnumerable<IValue> ConnectedValues => [a, b, product];

        public uint VariantCount => 2;

        public string Name { get; } = name;

        public Result Apply(uint variant)
        {
            Result result = Result.Noop.Instance;
            switch(variant)
            {
                case 0:
                    int vA, vB, vProduct;
                    if (a.TryGet(out vA) && b.TryGet(out vB))
                    {
                        result = result.CombineWith(product.TrySet(vA * vB, ([a, b], this)));
                    }
                    if (a.TryGet(out vA) && product.TryGet(out vProduct))
                    {
                        if (vProduct % vA == 0)
                        {
                            result = result.CombineWith(b.TrySet(vProduct / vA, ([a, product], this)));
                        }
                        else
                        {
                            result = Result.Conflict.Create(([a, product], this));
                        }
                    }
                    if (b.TryGet(out vB) && product.TryGet(out vProduct))
                    {
                        if (vProduct % vB == 0)
                        {
                            result = result.CombineWith(a.TrySet(vProduct / vB, ([b, product], this)));
                        }
                        else
                        {
                            result = Result.Conflict.Create(([b, product], this));
                        }
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
