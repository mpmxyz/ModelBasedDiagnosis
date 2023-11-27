namespace AAI6
{
    internal class Switch(
        IValue<Voltage> a,
        IValue<Voltage> bTrue,
        IValue<Voltage> bFalse,
        IValue<bool> state
    ) : IComponent
    {
        private readonly IValue<Voltage> a = a;
        private readonly IValue<Voltage> bTrue = bTrue;
        private readonly IValue<Voltage> bFalse = bFalse;
        private readonly IValue<bool> state = state; //true = up

        public IEnumerable<IValue> ConnectedValues => [a, bTrue, bFalse, state];

        public uint VariantCount => 4;

        public Result Apply(uint variant)
        {
            Voltage vA, vBTrue, vBFalse;
            bool vState;
            Result result = Result.NOOP;
            switch (variant)
            {
                case 0:
                    if (state.TryGet(out vState))
                    {
                        if (vState)
                        {
                            if (a.TryGet(out vA))
                            {
                                result = result.CombineWith(bTrue.TrySet(vA));
                            }
                            if (bTrue.TryGet(out vBTrue))
                            {
                                result = result.CombineWith(a.TrySet(vBTrue));
                            }
                        }
                        else
                        {
                            if (a.TryGet(out vA))
                            {
                                result = result.CombineWith(bFalse.TrySet(vA));
                            }
                            if (bFalse.TryGet(out vBFalse))
                            {
                                result = result.CombineWith(a.TrySet(vBFalse));
                            }
                        }
                    }
                    else
                    {
                        if (bTrue.TryGet(out vBTrue) && bFalse.TryGet(out vBFalse) && vBTrue == vBFalse)
                        {
                            result = result.CombineWith(a.TrySet(vBTrue));
                        }
                        if (a.TryGet(out vA) && bTrue.TryGet(out vBTrue) && bFalse.TryGet(out vBFalse))
                        {
                            if (vBTrue != vBFalse)
                            {
                                result = result.CombineWith(state.TrySet(vA == vBTrue));
                            }
                        }
                    }
                    break;
                case 1:
                    if (a.TryGet(out vA))
                    {
                        result = result.CombineWith(bTrue.TrySet(vA));
                    }
                    if (bTrue.TryGet(out vBTrue))
                    {
                        result = result.CombineWith(a.TrySet(vBTrue));
                    }
                    break;
                case 2:
                    if (a.TryGet(out vA))
                    {
                        result = result.CombineWith(bTrue.TrySet(vA));
                    }
                    if (bFalse.TryGet(out vBFalse))
                    {
                        result = result.CombineWith(a.TrySet(vBFalse));
                    }
                    break;
                case 3:

                    break;
            }
            return result;
        }

        public float Likelyhood(uint variant)
        {
            return variant switch
            {
                0 => 0.95f,
                1 => 0.02f,
                2 => 0.02f,
                _ => 0.01f
            };
        }
    }
}
