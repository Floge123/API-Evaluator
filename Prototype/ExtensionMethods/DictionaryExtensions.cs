using System;
using System.Collections.Generic;
using System.Linq;

namespace Prototype.ExtensionMethods
{
    public static class IDictionaryExtensions
    {
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

        public static void Print<K, V>(this IDictionary<K, V> dic)
        {
            foreach (var entry in dic)
            {
                Console.WriteLine($"{entry.Key} : {entry.Value}");
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
