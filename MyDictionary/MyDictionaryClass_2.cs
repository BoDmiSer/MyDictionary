using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MyDictionary
{
    class MyDictionaryClass_2<T, V> : IDictionary<T, V>
    {
        private List<int> keys;
        private List<Entry> values;
        private int capacity;

        private struct Entry
        {
            public int hashCode;
            public int next;
            public T key;
            public V value;
        }
        public MyDictionaryClass_2() : this(4)
        {
        }
        public MyDictionaryClass_2(int capacity)
        {
            this.capacity = capacity;
            keys = new List<int>(capacity);
            values = new List<Entry>(capacity);
            for (int i = 0; i < capacity; i++)
            {
                keys.Add(-1);
            }
        }

        public V this[T key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ICollection<T> Keys => throw new NotImplementedException();

        public ICollection<V> Values => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(T key, V value)
        {
            int index = key.GetHashCode() % capacity;
            if (keys[index] == -1)
            {
                keys[index] = key.GetHashCode() % capacity;
                values.Add(new Entry() { hashCode = key.GetHashCode() & 0x7fffffff % capacity, key = key, next = -1, value = value });
            }
            else
            {
                values.Add(new Entry() { hashCode = key.GetHashCode() & 0x7fffffff % capacity, key = key, next = keys[index], value = value });
            }
        }

        public void Add(KeyValuePair<T, V> item) => this.Add(item.Key, item.Value);

        public void Clear()
        {
            keys.Clear();
            for (int i = 0; i < capacity; i++)
            {
                keys.Add(-1);
            }
            values.Clear();
        }

        public bool Contains(KeyValuePair<T, V> item) => ContainsKey(item.Key);

        public bool ContainsKey(T key)
        {
            int index = key.GetHashCode() & 0x7fffffff % capacity;
            if (keys[index] != -1)
            {
                for (int index1 = index; index1 >= 0; index1 = values[index1].next)
                {
                    if (values[index1].hashCode == index1 && Equals(values[index1].key, key))
                        return true;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CopyTo(KeyValuePair<T, V>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<T, V>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(T key)
        {
            int index = key.GetHashCode() % capacity;
            if (keys[index] != -1)
            {
                for (int index1 = index; index1 >= 0; index1 = values[index1].next)
                {
                    if (values[index].hashCode == index1 && Equals(values[index1].key, key))
                    {
                        values.RemoveAt(index1);
                        return true;

                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Remove(KeyValuePair<T, V> item) => Remove(item.Key);

        public bool TryGetValue(T key, [MaybeNullWhen(false)] out V value)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

    }
}
