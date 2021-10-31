using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.ExtensionMethods
{
    public static class CollectionExtensions
    {
        public static void AddAll<V>(this ICollection<V> collection, ICollection<V> newItems)
        {
            foreach (var item in newItems)
            {
                collection.Add(item);
            }
        }

        public static string ValuesToString<V>(this ICollection<V> collection)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var entry in collection)
            {
                sb.Append($"{entry},");
            }
            return sb.ToString();
        }
    }
}
