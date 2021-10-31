using System;
using System.Collections.Generic;
using System.Linq;

namespace Prototype.ExtensionMethods
{
    public static class DictionaryExtensions
    {
        public static void CreateOrIncrease<K>(this Dictionary<K, double> dic, K key, double value)
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

        public static void CreateOrIncrement<K>(this Dictionary<K, int> dic, K key)
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

        public static void Print<K, V>(this Dictionary<K, V> dic)
        {
            foreach (var entry in dic)
            {
                Console.WriteLine($"{entry.Key} : {entry.Value}");
            }
        }

        public static void AddOrCreate<K, V>(this Dictionary<K, ICollection<V>> dic, K key, V value)
        {
            if (value is null) return;
            if (dic.ContainsKey(key))
            {
                dic[key].Add(value);
            }
            else
            {
                dic.Add(key, new V[] { value }.ToList());
            }
        }

        public static void CreateOrReplace<K, V>(this Dictionary<K, V> dic, K key, V value)
        {
            if (value is null) return;
            if (dic.ContainsKey(key))
            {
                dic[key] = value;
            }
            else
            {
                dic.Add(key, value);
            }
        }

        public static void AddOrCreate<K, V>(this Dictionary<K, ICollection<V>> dic, K key, ICollection<V> value)
        {
            if (value is null) return;
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
