namespace AAI6
{
    internal class Wire(IValue<Voltage> a, IValue<Voltage> b) : IComponent
    {
        private readonly IValue<Voltage> a = a;
        private readonly IValue<Voltage> b = b;

        public IEnumerable<IValue> ConnectedValues => [a, b];

        public uint VariantCount => 2;

        public Result Apply(uint variant)
        {
            Voltage vA, vB;
            Result result = Result.NOOP;
            switch (variant)
            {
                case 0:
                    if (a.TryGet(out vA))
                    {
                        result = result.CombineWith(b.TrySet(vA));
                    }
                    if (b.TryGet(out vB))
                    {
                        result = result.CombineWith(a.TrySet(vB));
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
