namespace AAI6
{
    abstract internal class Result
    {

        private Result() {
        }
        public abstract Result CombineWith(Result other);

        public sealed class Noop : Result
        {
            public static readonly Noop Instance = new();
            private Noop() { }

            public override Result CombineWith(Result other)
            {
                return other;
            }
        }
        public sealed class Ok : Result
        {
            public static readonly Ok Instance = new();
            private Ok() { }

            public override Result CombineWith(Result other)
            {
                return other is Conflict ? other : this;
            }
        }
        public sealed class Conflict : Result
        {
            private static readonly (IValue[], IComponent?) NoConflict = ([], null);

            public (IValue[] Values, IComponent? Component) Source
            {
                get;
            }
            public (IValue[] Values, IComponent? Component) Source2
            {
                get;
            }

            private Conflict((IValue[], IComponent?) source, (IValue[], IComponent?) source2)
            {
                Source = source;
                Source2 = source2;
            }

            public static Conflict Create((IValue[], IComponent?) source, (IValue[], IComponent?)? source2 = null)
            {
                return new Conflict(source, source2 ?? NoConflict);
            }

            public override Result CombineWith(Result other)
            {
                return this;
            }
        }
    }
}
