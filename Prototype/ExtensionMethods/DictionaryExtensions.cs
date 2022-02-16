using System;
using System.Collections.Generic;
using System.Linq;

namespace Prototype.ExtensionMethods
{
    public static class DictionaryExtensions
    {
        public static void Merge<TKey, TValue>(this IDictionary<TKey, IList<TValue>> dic, IDictionary<TKey, IList<TValue>> other)
        {
            foreach (var (key, value) in other)
            {
                if (value is null) return;
                lock (dic)
                {
                    if (dic.ContainsKey(key))
                    {
                        dic[key].AddAll(value);
                    }
                    else
                    {
                        dic.Add(key, value);
                    }
                }
            }
        }
        
        public static void CreateOrIncrease<K>(this IDictionary<K, double> dic, K key, double value)
        {
            lock (dic)
            {
                if (dic.ContainsKey(key))
                {
                    dic[key] += value;
                }
                else
                {
                    dic.Add(key, value);
                }
            }
        }

        public static void CreateOrIncrement<K>(this IDictionary<K, int> dic, K key)
        {
            lock (dic)
            {
                if (dic.ContainsKey(key))
                {
                    dic[key]++;
                }
                else
                {
                    dic.Add(key, 1);
                }
            }
        }

        public static void AddOrCreate<K, V>(this IDictionary<K, ICollection<V>> dic, K key, V value)
        {
            if (value is null) return;
            lock (dic)
            {
                if (dic.ContainsKey(key))
                {
                    dic[key].Add(value);
                }
                else
                {
                    dic.Add(key, new[] {value}.ToList());
                }
            }
        }

        public static void AddOrCreate<K, V>(this IDictionary<K, ICollection<V>> dic, K key, ICollection<V> value)
        {
            if (value is null) return;
            lock (dic)
            {
                if (dic.ContainsKey(key))
                {
                    dic[key].AddAll(value);
                }
                else
                {
                    dic.Add(key, value);
                }
            }
            
        }
    }
}
