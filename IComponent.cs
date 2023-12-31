﻿namespace AAI6
{
    internal interface IComponent
    {
        IEnumerable<IValue> ConnectedValues { get; }
        uint VariantCount { get; }
        string Name { get; }

        Result Apply(uint variant);
        float Likelyhood(uint variant);
    }
}
