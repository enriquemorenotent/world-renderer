using System.Collections.Generic;
using System;

namespace Extinction.Utils
{
    public class Cache<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> cache = new Dictionary<TKey, TValue>();

        private readonly Func<TKey, TValue> generator;

        public Cache(Func<TKey, TValue> _generator)
        {
            generator = _generator;
        }

        public TValue At(TKey key)
        {
            TValue value;

            lock (cache)
            {

                if (cache.TryGetValue(key, out value))
                    return value;

                if (generator == null)
                    throw new ArgumentNullException();

                value = generator(key);
                cache[key] = value;
            }

            return value;
        }
    }
}
