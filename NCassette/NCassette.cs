using System;
using NCassetteLib.Common;

namespace NCassetteLib
{
    public static class NCassette
    {
        public static NRecord<T> Record<T>(Func<T> func)
        {
            return new NRecord<T>(func);
        }
    }
}
