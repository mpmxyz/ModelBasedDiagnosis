namespace AAI6
{
    internal class Value<T> : IValue<T>
    {

        private T _value = default!;
        private readonly IList<T> _domain;
        private readonly string name;

        public bool Assigned
        {
            get;
            private set;
        } = false;

        public IEnumerable<T> Domain => _domain;

        public int DomainSize => Domain.Count();

        public Value(IEnumerable<T>? domain = null, string name = "")
        {
            Assigned = false;
            _domain = domain != null ? new List<T>(domain) : [];
            this.name = name;
        }

        public Value(T value, string name = "")
        {
            Assigned = true;
            _domain = new List<T>();
            _value = value;
            this.name = name;
        }

        public void Clear()
        {
            Assigned = false;
        }

        public bool TryGet(out T value)
        {
            value = _value;
            return Assigned;
        }

        public Result TrySet(T value)
        {
            if (Assigned)
            {
                if (EqualityComparer<T>.Default.Equals(value, _value))
                {
                    return Result.NOOP;
                }
                else
                {
                    return Result.CONFLICT;
                }
            }
            if (DomainSize > 0 && !_domain.Contains(value))
            {
                return Result.CONFLICT;
            }
            _value = value;
            Assigned = true;
            return Result.OK;
        }

        public Result TrySetByDomain(int domainIndex)
        {
            return TrySet(_domain[domainIndex]);
        }

        public Result TrySetFrom(IValue other)
        {
            if (other is IValue<T> o && o.TryGet(out var v)) {
                return TrySet(v);
            }
            return Result.NOOP;
        }

        public override string ToString()
        {
            if (name.Any())
            {
                return $"{name}={(Assigned ? _value : " / ")}";
            }
            else
            {
                return $"{(Assigned ? _value : " / ")}";
            }
        }
    }
}
