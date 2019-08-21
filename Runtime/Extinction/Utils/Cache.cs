using System.Collections.Generic;
using System;

namespace Extinction.Utils
{
    public class Cache<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> cache = new Dictionary<TKey, TValue>();

        private readonly Func<TKey, TValue> generator;

        public Cache(System.Func<TKey, TValue> f)
        {
            this.generator = f;
        }

        public TValue At(TKey key)
        {
            TValue value;

            lock (cache)
            {

                if (cache.TryGetValue(key, out value))
                    return value;

                if (this.generator == null)
                    throw new ArgumentNullException();

                value = this.generator(key);
                cache[key] = value;
            }

            return value;
        }
    }
}
