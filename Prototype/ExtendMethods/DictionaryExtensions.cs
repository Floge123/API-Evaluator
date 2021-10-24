using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.ExtendMethods
{
    public static class DictionaryExtensions
    {
        public static void AddOrCreate<K>(this Dictionary<K, double> dic, K key, double value)
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
    }
}
