using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MyDictionary
{
    public class MyDictionaryClass<TKey, TValue> : IDictionary<TKey, TValue>
    {

        private struct Entry
        {
            public int hashCode;
            public int next;
            public TKey key;
            public TValue value;
        }
        private int[] buckets;
        private Entry[] entries;
        private int version;
        private int freeList;
        private int freeCount;
        private int count;
        private readonly Dictionary<TKey, TValue>.KeyCollection keys;
        private Dictionary<TKey, TValue>.ValueCollection values;
        private IEqualityComparer<TKey> comparer;
        private IEqualityComparer<TKey> Comparer => this.comparer;

        public MyDictionaryClass()  : this(0, (IEqualityComparer<TKey>)null)
        {
        }

        /// <summary>Инициализирует новый пустой экземпляр класса <see cref="T:System.Collections.Generic.Dictionary`2" />, имеющий заданную начальную емкость и использующий функцию сравнения по умолчанию, проверяющую равенство для данного типа ключа.</summary>
        /// <param name="capacity">Начальное количество элементов, которое может содержать коллекция <see cref="T:System.Collections.Generic.Dictionary`2" />.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Значение параметра <paramref name="capacity" /> меньше 0.</exception>
        
        public MyDictionaryClass(int capacity)          : this(capacity, (IEqualityComparer<TKey>)null)
        {
        }

        /// <summary>Инициализирует новый пустой экземпляр класса <see cref="T:System.Collections.Generic.Dictionary`2" /> начальной емкостью по умолчанию, использующий указанную функцию сравнения <see cref="T:System.Collections.Generic.IEqualityComparer`1" />.</summary>
        /// <param name="comparer">Реализация <see cref="T:System.Collections.Generic.IEqualityComparer`1" />, которую следует использовать при сравнении ключей, или <see langword="null" />, если для данного типа ключа должна использоваться реализация <see cref="T:System.Collections.Generic.EqualityComparer`1" /> по умолчанию.</param>

        public MyDictionaryClass(IEqualityComparer<TKey> comparer)          : this(0, comparer)
        {
        }

        /// <summary>Инициализирует новый пустой экземпляр класса <see cref="T:System.Collections.Generic.Dictionary`2" /> заданной начальной емкостью, использующий указанную функцию сравнения <see cref="T:System.Collections.Generic.IEqualityComparer`1" />.</summary>
        /// <param name="capacity">Начальное количество элементов, которое может содержать коллекция <see cref="T:System.Collections.Generic.Dictionary`2" />.</param>
        /// <param name="comparer">Реализация <see cref="T:System.Collections.Generic.IEqualityComparer`1" />, которую следует использовать при сравнении ключей, или <see langword="null" />, если для данного типа ключа должна использоваться реализация <see cref="T:System.Collections.Generic.EqualityComparer`1" /> по умолчанию.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Значение параметра <paramref name="capacity" /> меньше 0.</exception>
 
        public MyDictionaryClass(int capacity, IEqualityComparer<TKey> comparer)
        {if (capacity > 0)
                this.Initialize(capacity);
            this.comparer = comparer ?? (IEqualityComparer<TKey>)EqualityComparer<TKey>.Default;
        }


        public TValue this[TKey key]
        {
            get
            {
                int entry = this.FindEntry(key);
                if (entry >= 0)
                    return this.entries[entry].value;
                return default;
            }
            set => this.Insert(key, value, false);
        }
        private int FindEntry(TKey key)
        {
            if ((object)key == null)
            if (this.buckets != null)
            {
                int num = this.Comparer.GetHashCode(key) & int.MaxValue;
                for (int index = this.buckets[num % this.buckets.Length]; index >= 0; index = this.entries[index].next)
                {
                    if (this.entries[index].hashCode == num && this.Comparer.Equals(this.entries[index].key, key))
                        return index;
                }
            }
            return -1;
        }
        private void Insert(TKey key, TValue value, bool add)
        {
            if (this.buckets == null)
                this.Initialize(0);
            int num1 = this.Comparer.GetHashCode(key) & int.MaxValue;
            int index1 = num1 % this.buckets.Length;
            int num2 = 0;
            for (int index2 = this.buckets[index1]; index2 >= 0; index2 = this.entries[index2].next)
            {
                if (this.entries[index2].hashCode == num1 && this.Comparer.Equals(this.entries[index2].key, key))
                {
                     this.entries[index2].value = value;
                    ++this.version;
                    return;
                }
                ++num2;
            }
            int index3;
            if (this.freeCount > 0)
            {
                index3 = this.freeList;
                this.freeList = this.entries[index3].next;
                --this.freeCount;
            }
            else
            {
                if (this.count == this.entries.Length)
                {
                    this.Resize();
                    index1 = num1 % this.buckets.Length;
                }
                index3 = this.count;
                ++this.count;
            }
            this.entries[index3].hashCode = num1;
            this.entries[index3].next = this.buckets[index1];
            this.entries[index3].key = key;
            this.entries[index3].value = value;
            this.buckets[index1] = index3;
            ++this.version;
            if (num2 <= 100 || !IsWellKnownEqualityComparer((object)this.Comparer))
                return;
            comparer = (IEqualityComparer<TKey>)GetRandomizedEqualityComparer((object)this.Comparer);
            this.Resize(this.entries.Length, true);
        }
        private void Resize() => this.Resize(ExpandPrime(this.count), false);
        private void Resize(int newSize, bool forceNewHashCodes)
        {
            int[] numArray = new int[newSize];
            for (int index = 0; index < numArray.Length; ++index)
                numArray[index] = -1;
            MyDictionaryClass<TKey, TValue>.Entry[] entryArray = new MyDictionaryClass<TKey, TValue>.Entry[newSize];
            Array.Copy((Array)this.entries, 0, (Array)entryArray, 0, this.count);
            if (forceNewHashCodes)
            {
                for (int index = 0; index < this.count; ++index)
                {
                    if (entryArray[index].hashCode != -1)
                        entryArray[index].hashCode = Comparer.GetHashCode(entryArray[index].key) & int.MaxValue;
                }
            }
            for (int index1 = 0; index1 < this.count; ++index1)
            {
                if (entryArray[index1].hashCode >= 0)
                {
                    int index2 = entryArray[index1].hashCode % newSize;
                    entryArray[index1].next = numArray[index2];
                    numArray[index2] = index1;
                }
            }
            this.buckets = numArray;
            this.entries = entryArray;
        }
        public static bool IsWellKnownEqualityComparer(object comparer) => comparer == null || comparer == EqualityComparer<string>.Default;
        public static int ExpandPrime(int oldSize)
        {
            int min = 2 * oldSize;
            return (uint)min > 2146435069U && 2146435069 > oldSize ? 2146435069 : GetPrime(min);
        }
        public static IEqualityComparer GetRandomizedEqualityComparer(object comparer)
        {
            if (comparer == null)
                return (IEqualityComparer)new RandomizedObjectEqualityComparer();
            if (comparer == EqualityComparer<string>.Default)
                return (IEqualityComparer)new RandomizedStringEqualityComparer();
            return comparer is IWellKnownStringEqualityComparer equalityComparer ? equalityComparer.GetRandomizedEqualityComparer() : (IEqualityComparer)null;
        }
        private void Initialize(int capacity)
        {
            int prime = GetPrime(capacity);
            this.buckets = new int[prime];
            for (int index = 0; index < this.buckets.Length; ++index)
                this.buckets[index] = -1;
            this.entries = new MyDictionaryClass<TKey, TValue>.Entry[prime];
            this.freeList = -1;
        }
        public static int GetPrime(int min)
        { for (int index = 0; index < primes.Length; ++index)
            {
                int prime = primes[index];
                if (prime >= min)
                    return prime;
            }
            for (int candidate = min | 1; candidate < int.MaxValue; candidate += 2)
            {
                if (IsPrime(candidate) && (candidate - 1) % 101 != 0)
                    return candidate;
            }
            return min;
        }
        public static bool IsPrime(int candidate)
        {
            if ((candidate & 1) == 0)
                return candidate == 2;
            int num = (int)Math.Sqrt((double)candidate);
            for (int index = 3; index <= num; index += 2)
            {
                if (candidate % index == 0)
                    return false;
            }
            return true;
        }
        public static readonly int[] primes = new int[72]
 {
      3,
      7,
      11,
      17,
      23,
      29,
      37,
      47,
      59,
      71,
      89,
      107,
      131,
      163,
      197,
      239,
      293,
      353,
      431,
      521,
      631,
      761,
      919,
      1103,
      1327,
      1597,
      1931,
      2333,
      2801,
      3371,
      4049,
      4861,
      5839,
      7013,
      8419,
      10103,
      12143,
      14591,
      17519,
      21023,
      25229,
      30293,
      36353,
      43627,
      52361,
      62851,
      75431,
      90523,
      108631,
      130363,
      156437,
      187751,
      225307,
      270371,
      324449,
      389357,
      467237,
      560689,
      672827,
      807403,
      968897,
      1162687,
      1395263,
      1674319,
      2009191,
      2411033,
      2893249,
      3471899,
      4166287,
      4999559,
      5999471,
      7199369
 };

        public ICollection<TKey> Keys => this.keys;

        public ICollection<TValue> Values => this.values;

        public int Count => count - freeCount;

        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value) => this.Insert(key, value, true);

        public void Add(KeyValuePair<TKey, TValue> item) => this.Add(item.Key, item.Value);

        public void Clear()
        {
            if (this.count <= 0)
                return;
            for (int index = 0; index < this.buckets.Length; ++index)
                this.buckets[index] = -1;
            Array.Clear((Array)this.entries, 0, this.count);
            this.freeList = -1;
            this.count = 0;
            this.freeCount = 0;
            ++this.version;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(TKey key) => this.FindEntry(key) >= 0;

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            int count = this.count;
            MyDictionaryClass<TKey, TValue>.Entry[] entries = this.entries;
            for (int index1 = 0; index1 < count; ++index1)
            {
                if (entries[index1].hashCode >= 0)
                    array[arrayIndex++] = new KeyValuePair<TKey, TValue>(entries[index1].key, entries[index1].value);
            }
        }
        public bool Remove(TKey key)
        {
           if (this.buckets != null)
            {
                int num = Comparer.GetHashCode(key) & int.MaxValue;
                int index1 = num % this.buckets.Length;
                int index2 = -1;
                for (int index3 = this.buckets[index1]; index3 >= 0; index3 = this.entries[index3].next)
                {
                    if (this.entries[index3].hashCode == num && Comparer.Equals(this.entries[index3].key, key))
                    {
                        if (index2 < 0)
                            this.buckets[index1] = this.entries[index3].next;
                        else
                            this.entries[index2].next = this.entries[index3].next;
                        this.entries[index3].hashCode = -1;
                        this.entries[index3].next = this.freeList;
                        this.entries[index3].key = default(TKey);
                        this.entries[index3].value = default(TValue);
                        this.freeList = index3;
                        ++this.freeCount;
                        ++this.version;
                        return true;
                    }
                    index2 = index3;
                }
            }
            return false;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            int entry = FindEntry(key);
            if (entry >= 0)
            {
                value = this.entries[entry].value;
                return true;
            }
            value = default(TValue);
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public class RandomizedObjectEqualityComparer : IEqualityComparer
        {
            private long _entropy;
            private Random rnd = new Random(1000);
            public RandomizedObjectEqualityComparer() => this._entropy = rnd.Next(1000);

            public bool Equals(object x, object y) => x != null ? y != null && x.Equals(y) : y == null;

            public int GetHashCode(object obj)
            {
                if (obj == null)
                    return 0;
                return obj is string s ? InternalMarvin32HashString(s, s.Length, this._entropy) : obj.GetHashCode();
            }

            public override bool Equals(object obj) => obj is RandomizedObjectEqualityComparer equalityComparer && this._entropy == equalityComparer._entropy;

            public override int GetHashCode() => this.GetType().Name.GetHashCode() ^ (int)(this._entropy & (long)int.MaxValue);

            private static extern int InternalMarvin32HashString(
              string s,
              int strLen,
              long additionalEntropy);

        }

        public class RandomizedStringEqualityComparer : IEqualityComparer<string>, IEqualityComparer, IWellKnownStringEqualityComparer
        {
            private long _entropy;
            private Random rnd = new Random(1000);

            public RandomizedStringEqualityComparer() => this._entropy = rnd.Next();

            public bool Equals(object x, object y)
            {
                if (x == y)
                    return true;
                if (x == null || y == null)
                    return false;
                if (x is string && y is string)
                    return this.Equals((string)x, (string)y);
                return false;
            }

            public bool Equals(string x, string y) => x != null ? y != null && x.Equals(y) : y == null;

            public int GetHashCode(string obj) => obj == null ? 0 : InternalMarvin32HashString(obj, obj.Length, this._entropy);

            public int GetHashCode(object obj)
            {
                if (obj == null)
                    return 0;
                return obj is string s ? InternalMarvin32HashString(s, s.Length, this._entropy) : obj.GetHashCode();
            }

            public override bool Equals(object obj) => obj is RandomizedStringEqualityComparer equalityComparer && this._entropy == equalityComparer._entropy;

            public override int GetHashCode() => this.GetType().Name.GetHashCode() ^ (int)(this._entropy & (long)int.MaxValue);

            IEqualityComparer IWellKnownStringEqualityComparer.GetRandomizedEqualityComparer() => (IEqualityComparer)new RandomizedStringEqualityComparer();

            IEqualityComparer IWellKnownStringEqualityComparer.GetEqualityComparerForSerialization() => (IEqualityComparer)EqualityComparer<string>.Default;

            private static extern int InternalMarvin32HashString(
            string s,
            int strLen,
            long additionalEntropy);
        }
        public interface IWellKnownStringEqualityComparer
        {
            IEqualityComparer GetRandomizedEqualityComparer();

            IEqualityComparer GetEqualityComparerForSerialization();
        }
    }
}
