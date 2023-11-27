namespace AAI6
{
    internal interface IValue
    {
        bool Assigned { get; }
        void Clear();
        int DomainSize { get; }
        Result TrySetByDomain(int domainIndex);
        Result TrySetFrom(IValue other);
    }

    internal interface IValue<T> : IValue
    {
        bool TryGet(out T value);
        Result TrySet(T value);
        IEnumerable<T> Domain { get; }
    }
}
