using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace Useful.Tiles
{
    public static class ExtensionMethods
    {
        public static List<T> AddAndReturn<T>(this List<T> list, T item)
        {
            list.Add(item);
            return list;
        }

        //public static int CountValues<TKey, TValue>(this ICollection<KeyValuePair<TKey, ICollection<TValue>>> keyValuePairs)
        public static int CountValues(this ConcurrentDictionary<Guid, List<Tile>> keyValuePairs)
        {
            var count = 0;
            foreach (var k in keyValuePairs)
            {
                count += k.Value.Count;
            }
            return count;
        }

        public static IEnumerable<string> BreakIntoLinesByLength(this string input, int length, int maxLines = int.MaxValue)
        {
            var pieces = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var currentLine = string.Empty;
            var lineCount = 0;
            foreach (var p in pieces)
            {
                if (currentLine.Length + p.Length + 1 > length)
                {
                    yield return currentLine.Trim();
                    lineCount++;
                    if (lineCount >= maxLines)
                    {
                        break;
                    }
                    currentLine = string.Empty;
                }
                currentLine += p + " ";
            }
            if (lineCount <= maxLines)
            {
                yield return currentLine;
            }
        }
    }
}
