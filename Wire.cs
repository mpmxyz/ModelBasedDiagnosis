namespace AAI6
{
    internal class Wire(IValue<Voltage> a, IValue<Voltage> b, string name = "") : IComponent
    {
        private readonly IValue<Voltage> a = a;
        private readonly IValue<Voltage> b = b;

        public IEnumerable<IValue> ConnectedValues => [a, b];

        public uint VariantCount => 2;

        public string Name { get; } = name;

        public Result Apply(uint variant)
        {
            Voltage vA, vB;
            Result result = Result.Noop.Instance;
            switch (variant)
            {
                case 0:
                    if (a.TryGet(out vA))
                    {
                        result = result.CombineWith(b.TrySet(vA, ([a], this)));
                    }
                    if (b.TryGet(out vB))
                    {
                        result = result.CombineWith(a.TrySet(vB, ([b], this)));
                    }
                    break;
                case 1:
                    break;
            }
            return result;
        }

        public float Likelyhood(uint variant)
        {
            return variant switch
            {
                0 => 0.999f,
                _ => 0.001f
            };
        }
    }
}
