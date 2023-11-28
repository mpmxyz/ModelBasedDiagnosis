namespace AAI6
{
    internal class Lamp(IValue<Voltage> a, IValue<Voltage> b, IValue<bool> lit, string name = "") : IComponent
    {
        private readonly IValue<Voltage> a = a;
        private readonly IValue<Voltage> b = b;
        private readonly IValue<bool> lit = lit;

        public IEnumerable<IValue> ConnectedValues => [a, b, lit];

        public uint VariantCount => 2;

        public string Name { get; } = name;

        public Result Apply(uint variant)
        {
            Voltage vA, vB;
            bool vLit;
            Result result = Result.Noop.Instance;
            switch (variant)
            {
                case 0:
                    if (a.TryGet(out vA) && b.TryGet(out vB))
                    {
                        result = result.CombineWith(lit.TrySet(
                            (vA == Voltage.PLUS && vB == Voltage.GROUND) || (vA == Voltage.GROUND && vB == Voltage.PLUS),
                            ([a,b],this)
                            ));
                    }
                    if (lit.TryGet(out vLit))
                    {
                        if (vLit)
                        {
                            if (a.TryGet(out vA))
                            {
                                result = result.CombineWith(
                                    b.TrySet((vA == Voltage.PLUS) ? Voltage.GROUND : Voltage.PLUS, ([lit, a], this))
                                );
                            }
                            if (b.TryGet(out vB))
                            {
                                result = result.CombineWith(
                                    a.TrySet((vB == Voltage.PLUS) ? Voltage.GROUND : Voltage.PLUS, ([lit, b], this))
                                );
                            }
                        }
                        else
                        {
                            if (a.TryGet(out vA))
                            {
                                result = result.CombineWith(
                                    b.TrySet((vA != Voltage.PLUS) ? Voltage.GROUND : Voltage.PLUS, ([lit, a], this))
                                );
                            }
                            if (b.TryGet(out vB))
                            {
                                result = result.CombineWith(
                                    a.TrySet((vB != Voltage.PLUS) ? Voltage.GROUND : Voltage.PLUS, ([lit, b], this))
                                );
                            }
                        }
                    }
                    break;
                case 1:
                    result = result.CombineWith(lit.TrySet(false, ([], this)));
                    break;
            }
            return result;
        }

        public float Likelyhood(uint variant)
        {
            return variant switch
            {
                0 => 0.9f,
                _ => 0.1f
            };
        }
    }
}
