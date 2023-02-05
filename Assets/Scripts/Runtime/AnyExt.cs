using System;

namespace TeamShrimp.GGJ23
{
    public static class AnyExt
    {
        public static U Then<T, U>(this T t, Func<T, U> f) => f(t);
    }
}