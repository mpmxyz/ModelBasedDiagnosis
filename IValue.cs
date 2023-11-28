using System.Diagnostics.CodeAnalysis;

namespace AAI6
{
    internal interface IValue
    {
        public static readonly (IValue[], IComponent?) NoDependency = ([], null);

        [MemberNotNullWhen(true, nameof(Dependencies))]
        bool Assigned { get; }
        int DomainSize { get; }
        (IValue[], IComponent?)? Dependencies { get; }
        string Name { get; }

        void Clear();
        Result TrySetByDomain(int domainIndex);
        Result TrySetFrom(IValue other);
    }

    internal interface IValue<T> : IValue
    {
        bool TryGet(out T value);
        Result TrySet(T value, (IValue[], IComponent?)? dependencies);
        IEnumerable<T> Domain { get; }
    }
}
