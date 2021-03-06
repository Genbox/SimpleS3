using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Genbox.SimpleS3.Core.Internals.Misc
{
    /// <summary>
    /// Represents an ordered collection of keys and values with the same performance as <see cref="Dictionary{TKey,TValue}" /> with O(1) lookups
    /// and adds but with O(n) inserts and removes.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    internal class OrderedDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        private const int _hashPrime = 101;
        private const int _maxPrimeArrayLength = 0x7FEFFFFD;

        // We want to initialize without allocating arrays. We also want to avoid null checks.
        // Array.Empty would give divide by zero in modulo operation. So we use static one element arrays.
        // The first add will cause a resize replacing these with real arrays of three elements.
        // Arrays are wrapped in a class to avoid being duplicated for each <TKey, TValue>
        private static readonly Entry[] _initialEntries = new Entry[1];

        private readonly IEqualityComparer<TKey> _comparer = EqualityComparer<TKey>.Default;

        private readonly int[] _primes =
        {
            3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
            1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
            17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
            187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
            1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369
        };

        // 1-based index into _entries; 0 means empty
        private int[] _buckets = new int[1];

        // remains contiguous and maintains order
        private Entry[] _entries = _initialEntries;

        public OrderedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> enumerable)
        {
            foreach (KeyValuePair<TKey, TValue> pair in enumerable)
            {
                Add(pair.Key, pair.Value);
            }
        }

        /// <summary>Gets the number of key/value pairs contained in the <see cref="OrderedDictionary{TKey, TValue}" />.</summary>
        /// <returns>The number of key/value pairs contained in the <see cref="OrderedDictionary{TKey, TValue}" />.</returns>
        public int Count { get; private set; }

        /// <summary>Gets or sets the value associated with the specified key as an O(1) operation.</summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>
        /// The value associated with the specified key. If the specified key is not found, a get operation throws a <see cref="KeyNotFoundException" />
        /// , and a set operation creates a new element with the specified key.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="KeyNotFoundException">The property is retrieved and <paramref name="key" /> does not exist in the collection.</exception>
        public TValue this[TKey key]
        {
            get
            {
                int index = IndexOf(key);
                if (index < 0)
                    throw new KeyNotFoundException();
                return _entries[index].Value;
            }
            set => TryInsert(null, key, value, InsertionBehavior.OverwriteExisting);
        }

        public IEnumerable<TKey> Keys => throw new NotSupportedException();
        public IEnumerable<TValue> Values => throw new NotSupportedException();

        /// <summary>Determines whether the <see cref="OrderedDictionary{TKey, TValue}" /> contains the specified key as an O(1) operation.</summary>
        /// <param name="key">The key to locate in the <see cref="OrderedDictionary{TKey, TValue}" />.</param>
        /// <returns>true if the <see cref="OrderedDictionary{TKey, TValue}" /> contains an element with the specified key; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        public bool ContainsKey(TKey key)
        {
            return IndexOf(key) >= 0;
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                Entry entry = _entries[i];
                yield return new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
            }
        }

        /// <summary>Returns an enumerator that iterates through the <see cref="OrderedDictionary{TKey, TValue}" />.</summary>
        /// <returns>An <see cref="OrderedDictionary{TKey, TValue}.Enumerator" /> structure for the <see cref="OrderedDictionary{TKey, TValue}" />.</returns>
        public IEnumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>Gets the value associated with the specified key as an O(1) operation.</summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">
        /// When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value
        /// for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.
        /// </param>
        /// <returns>true if the <see cref="OrderedDictionary{TKey, TValue}" /> contains an element with the specified key; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        public bool TryGetValue(TKey key, out TValue value)
        {
            int index = IndexOf(key);
            if (index >= 0)
            {
                value = _entries[index].Value;
                return true;
            }
            value = default!;
            return false;
        }

        private static bool IsPrime(int candidate)
        {
            if ((candidate & 1) != 0)
            {
                int limit = (int)Math.Sqrt(candidate);
                for (int divisor = 3; divisor <= limit; divisor += 2)
                {
                    if (candidate % divisor == 0)
                        return false;
                }
                return true;
            }
            return candidate == 2;
        }

        private int GetPrime(int min)
        {
            if (min < 0)
                throw new ArgumentException("Hashtable's capacity overflowed and went negative. Check load factor, capacity and the current size of the table.");

            for (int i = 0; i < _primes.Length; i++)
            {
                int prime = _primes[i];
                if (prime >= min)
                    return prime;
            }

            //outside of our predefined table. 
            //compute the hard way. 
            for (int i = min | 1; i < int.MaxValue; i += 2)
            {
                if (IsPrime(i) && (i - 1) % _hashPrime != 0)
                    return i;
            }
            return min;
        }

        /// <summary>Adds the specified key and value to the dictionary as an O(1) operation.</summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="OrderedDictionary{TKey, TValue}" />.</exception>
        public void Add(TKey key, TValue value)
        {
            TryInsert(null, key, value, InsertionBehavior.ThrowOnExisting);
        }

        /// <summary>
        /// Returns the zero-based index of the element with the specified key within the <see cref="OrderedDictionary{TKey, TValue}" /> as an O(1)
        /// operation.
        /// </summary>
        /// <param name="key">The key of the element to locate.</param>
        /// <returns>
        /// The zero-based index of the element with the specified key within the <see cref="OrderedDictionary{TKey, TValue}" />, if found; otherwise,
        /// -1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        public int IndexOf(TKey key)
        {
            return IndexOf(key, out _);
        }

        private Entry[] Resize(int newSize)
        {
            int[] newBuckets = new int[newSize];
            Entry[] newEntries = new Entry[newSize];

            int count = Count;
            Array.Copy(_entries, newEntries, count);
            for (int i = 0; i < count; ++i)
            {
                AddEntryToBucket(ref newEntries[i], i, newBuckets);
            }

            _buckets = newBuckets;
            _entries = newEntries;
            return newEntries;
        }

        private int IndexOf(TKey key, out uint hashCode)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            unchecked
            {
                hashCode = (uint)_comparer.GetHashCode(key);
            }

            int index = _buckets[(int)(hashCode % (uint)_buckets.Length)] - 1;
            if (index >= 0)
            {
                Entry[] entries = _entries;
                int collisionCount = 0;
                do
                {
                    Entry entry = entries[index];
                    if (entry.HashCode == hashCode && _comparer.Equals(entry.Key, key))
                        break;
                    index = entry.Next;
                    if (collisionCount >= entries.Length)
                    {
                        // The chain of entries forms a loop; which means a concurrent update has happened.
                        // Break out of the loop and throw, rather than looping forever.
                        throw new InvalidOperationException();
                    }
                    ++collisionCount;
                } while (index >= 0);
            }
            return index;
        }

        private bool TryInsert(int? index, TKey key, TValue value, InsertionBehavior behavior)
        {
            int i = IndexOf(key, out uint hashCode);
            if (i >= 0)
            {
                switch (behavior)
                {
                    case InsertionBehavior.OverwriteExisting:
                        _entries[i].Value = value;
                        return true;
                    case InsertionBehavior.ThrowOnExisting:
                        throw new ArgumentException();
                    default:
                        return false;
                }
            }

            AddInternal(index, key, value, hashCode);
            return true;
        }

        private int ExpandPrime(int oldSize)
        {
            int newSize = 2 * oldSize;

            // Allow the hashtables to grow to maximum possible size (~2G elements) before encountering capacity overflow.
            // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
            if ((uint)newSize > _maxPrimeArrayLength && _maxPrimeArrayLength > oldSize)
            {
                Debug.Assert(_maxPrimeArrayLength == GetPrime(_maxPrimeArrayLength), "Invalid MaxPrimeArrayLength");
                return _maxPrimeArrayLength;
            }

            return GetPrime(newSize);
        }

        private int AddInternal(int? index, TKey key, TValue value, uint hashCode)
        {
            Entry[] entries = _entries;

            // Check if resize is needed
            int count = Count;
            if (entries.Length == count || entries.Length == 1)
                entries = Resize(ExpandPrime(entries.Length));

            // Increment indices >= index;
            int actualIndex = index ?? count;
            for (int i = count - 1; i >= actualIndex; --i)
            {
                entries[i + 1] = entries[i];
                UpdateBucketIndex(i, 1);
            }

            ref Entry entry = ref entries[actualIndex];
            entry.HashCode = hashCode;
            entry.Key = key;
            entry.Value = value;
            AddEntryToBucket(ref entry, actualIndex, _buckets);
            ++Count;
            return actualIndex;
        }

        // Returns the index of the next entry in the bucket
        private static void AddEntryToBucket(ref Entry entry, int entryIndex, int[] buckets)
        {
            ref int b = ref buckets[(int)(entry.HashCode % (uint)buckets.Length)];
            entry.Next = b - 1;
            b = entryIndex + 1;
        }

        private void UpdateBucketIndex(int entryIndex, int incrementAmount)
        {
            Entry[] entries = _entries;
            Entry entry = entries[entryIndex];
            ref int b = ref _buckets[(int)(entry.HashCode % (uint)_buckets.Length)];

            // Bucket was pointing to entry. Increment the index by incrementAmount.
            if (b == entryIndex + 1)
                b += incrementAmount;
            else
            {
                // Start at the entry the bucket points to, and walk the chain until we find the entry with the index we want to increment.
                int i = b - 1;
                int collisionCount = 0;
                while (true)
                {
                    ref Entry e = ref entries[i];
                    if (e.Next == entryIndex)
                    {
                        e.Next += incrementAmount;
                        return;
                    }
                    i = e.Next;
                    if (collisionCount >= entries.Length)
                    {
                        // The chain of entries forms a loop; which means a concurrent update has happened.
                        // Break out of the loop and throw, rather than looping forever.
                        throw new InvalidOperationException();
                    }
                    ++collisionCount;
                }
            }
        }

        private enum InsertionBehavior
        {
            OverwriteExisting,
            ThrowOnExisting
        }

        private struct Entry
        {
            public uint HashCode;
            public TKey Key;
            public TValue Value;
            public int Next; // the index of the next item in the same bucket, -1 if last
        }

        /// <summary>Enumerates the elements of a <see cref="OrderedDictionary{TKey, TValue}" />.</summary>
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private readonly OrderedDictionary<TKey, TValue> _orderedDictionary;
            private int _index;

            /// <summary>Gets the element at the current position of the enumerator.</summary>
            /// <returns>The element in the <see cref="OrderedDictionary{TKey, TValue}" /> at the current position of the enumerator.</returns>
            public KeyValuePair<TKey, TValue> Current { get; private set; }

            object IEnumerator.Current => Current;

            internal Enumerator(OrderedDictionary<TKey, TValue> orderedDictionary)
            {
                _orderedDictionary = orderedDictionary;
                _index = 0;
            }

            /// <summary>Releases all resources used by the <see cref="OrderedDictionary{TKey, TValue}.Enumerator" />.</summary>
            public void Dispose() { }

            /// <summary>Advances the enumerator to the next element of the <see cref="OrderedDictionary{TKey, TValue}" />.</summary>
            /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
            /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
            public bool MoveNext()
            {
                if (_index < _orderedDictionary.Count)
                {
                    Entry entry = _orderedDictionary._entries[_index];
                    Current = new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
                    ++_index;
                    return true;
                }
                Current = default;
                return false;
            }

            void IEnumerator.Reset()
            {
                _index = 0;
                Current = default;
            }
        }
    }
}