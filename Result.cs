namespace AAI6
{
    internal static class ResultExtensions
    {
        internal static Result CombineWith(this Result a, Result b)
        {
            return a > b ? a : b;
        }
    }

    internal enum Result
    {
        NOOP, OK, CONFLICT
    }
}
