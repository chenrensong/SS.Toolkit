using System.Collections.Generic;

namespace SS.Toolkit.Generic
{
    public sealed class SafeDictionary<TKey, TValue>
    {
        private readonly object _lock = new object();
        private readonly Dictionary<TKey, TValue> _dictionary;

        public SafeDictionary(int capacity)
        {
            _dictionary = new Dictionary<TKey, TValue>(capacity);
        }

        public SafeDictionary()
        {
            _dictionary = new Dictionary<TKey, TValue>();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (_lock)
            {
                return _dictionary.TryGetValue(key, out value);
            }
        }

        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _dictionary.Count;
                }
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                lock (_lock)
                {
                    return _dictionary[key];
                }
            }
            set
            {
                lock (_lock)
                {
                    _dictionary[key] = value;
                }
            }
        }

        public void Add(TKey key, TValue value)
        {
            lock (_lock)
            {
                if (_dictionary.ContainsKey(key) == false)
                {
                    _dictionary.Add(key, value);
                }
            }
        }
    }
}
