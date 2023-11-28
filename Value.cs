using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace AAI6
{
    internal class Value<T> : IValue<T>
    {
        private static readonly (IValue[], IComponent?) NoDependencies = ([], null);
        private static readonly IList<T> EmptyDomain = ImmutableList.Create<T>();

        private T _value = default!;
        private readonly IList<T> _domain;
        public string Name { get; }

        public (IValue[], IComponent?)? Dependencies
        {
            get;
            private set;
        }

        [MemberNotNullWhen(true, nameof(Dependencies))]
        public bool Assigned => Dependencies != null;

        public IEnumerable<T> Domain => _domain;

        public int DomainSize => Domain.Count();

        public Value(IEnumerable<T>? domain = null, string name = "")
        {
            Dependencies = null;
            _domain = domain != null ? new List<T>(domain) : EmptyDomain;
            Name = name;
        }

        public Value(T value, string name = "")
        {
            Dependencies = NoDependencies;
            _domain = EmptyDomain;
            _value = value;
            Name = name;
        }

        public void Clear()
        {
            Dependencies = null;
        }

        [MemberNotNullWhen(true, nameof(Dependencies))]
        public bool TryGet(out T value)
        {
            value = _value;
            return Assigned;
        }

        public Result TrySet(T value, (IValue[], IComponent?)? dependencies)
        {
            if (Assigned)
            {
                if (EqualityComparer<T>.Default.Equals(value, _value))
                {
                    return Result.Noop.Instance;
                }
                else
                {
                    return Result.Conflict.Create(Dependencies ?? NoDependencies, dependencies);
                }
            }
            if (DomainSize > 0 && !_domain.Contains(value))
            {
                return Result.Conflict.Create(dependencies ?? NoDependencies);
            }
            _value = value;
            Dependencies = dependencies ?? NoDependencies;
            return Result.Ok.Instance;
        }

        public Result TrySetByDomain(int domainIndex)
        {
            return TrySet(_domain[domainIndex], NoDependencies);
        }

        public Result TrySetFrom(IValue other)
        {
            if (other is IValue<T> o && o.TryGet(out var v)) {
                return TrySet(v, other.Dependencies);
            }
            return Result.Noop.Instance;
        }

        public override string ToString()
        {
            if (Name.Length != 0)
            {
                return $"{Name}={(Assigned ? _value : " / ")}";
            }
            else
            {
                return $"{(Assigned ? _value : " / ")}";
            }
        }
    }
}
