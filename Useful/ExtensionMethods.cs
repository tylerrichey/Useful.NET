using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Useful.ExtensionMethods
{
    public static class ExtensionMethods
    {
        //using System.ComponentModel.DataAnnotations;
        //public static bool IsValid(this object input) => Validator.TryValidateObject(input, new ValidationContext(input), new List<ValidationResult>(), true);

        public static DateTime NearestFutureDayOfWeek(this DateTime sourceDate, DayOfWeek dayOfWeek) => DateTime.Today.AddDays(dayOfWeek - sourceDate.DayOfWeek < 0 ? (dayOfWeek - sourceDate.DayOfWeek) + 7 : dayOfWeek - sourceDate.DayOfWeek);
        public static DateTime NearestFriday(this DateTime sourceDate) => sourceDate.NearestFutureDayOfWeek(DayOfWeek.Friday);
		
		public static bool ContainsWord(this string input, string word) => Regex.IsMatch(input, @"\b" + word + @"\b", RegexOptions.IgnoreCase);

        public static bool ContainsWord(this string input, string[] words)
        {
            foreach (var w in words)
            {
                if (input.ContainsWord(w))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool EqualsList<T>(this List<T> input, List<T> compare) => !input.Except(compare).Any() && !compare.Except(input).Any();

        public static bool ContainsAny<T>(this IEnumerable<T> input, IEnumerable<T> compare)
        {
            foreach (var c in compare)
            {
                if (input.Contains(c))
                {
                    return true;
                }
            }
            return false;
        }
		
		public static byte[] ToBytes(this string value) => Encoding.UTF8.GetBytes(value);

        public static string GetString(this byte[] value) => Encoding.UTF8.GetString(value);

        public static string ToCamelCase(this string value) => string.IsNullOrEmpty(value) || value.Length < 2 ? value : char.ToLowerInvariant(value[0]) + value.Substring(1);
		
		public static IEnumerable<string[]> GetStringArrayEnumerable(this StreamReader sr, int numberOfLinesPerArray)
        {
            while (!sr.EndOfStream)
            {
                yield return Enumerable.Range(0, numberOfLinesPerArray)
                                       .Select(i => sr.ReadLine())
                                       .ToArray();
            }
            yield break;
        }
		
		public static string ToShortHandString(this TimeSpan timeSpan)
        {
            var ts = timeSpan.TotalSeconds;
            var secs = Convert.ToInt32(ts % 60);
            var mins = Convert.ToInt32((ts / 60) % 60);
            var hrs = Convert.ToInt32((ts / 3600) % 24);
            var days = Convert.ToInt32(ts / 86400);
            var showZeroes = false;
            var result = string.Empty;

            if (days > 0)
            {
                result = days + (days == 1 ? " day" : " days");
                showZeroes = true;
            }
            if (showZeroes || hrs > 0)
            {
                result += (showZeroes ? ", " : "") + hrs + (hrs == 1 ? " hr" : " hrs");
                showZeroes = true;
            }
            if (showZeroes || mins > 0)
            {
                result += (showZeroes ? ", " : "") + mins + (mins == 1 ? " min" : " mins");
                showZeroes = true;
            }
            result += (showZeroes ? ", " : "") + secs + (secs == 1 ? " sec" : " secs");

            return result;
        }
		
		public static void ExceptionToConsole(this Exception e)
		{
			Console.WriteLine($"Exception: {e.Message} {(e.InnerException != null ? Environment.NewLine + e.InnerException.Message : string.Empty)}");
            Console.WriteLine($"Stack trace:{Environment.NewLine}{e.StackTrace}");
		}

        //using Microsoft.Extensions.Configuration;
        //public static KeyValuePair<string, string> ToKeyValuePair(this IConfigurationSection configurationSection)
        //{
        //    return new KeyValuePair<string, string>(configurationSection.Key, configurationSection.Value);
        //}

        //public static List<KeyValuePair<string, string>> ToKeyValuePairList(this IEnumerable<IConfigurationSection> configurationSection)
        //{
        //    return configurationSection.Select(ToKeyValuePair)
        //                               .ToList();
        //}

        public static string EnumerableStringToString(this IEnumerable<string> list)
        {
            var sb = new StringBuilder();
            foreach (var s in list)
            {
                sb.AppendLine(s);
            }
            return sb.ToString();
        }
		
        public static List<T> EmptyBagToList<T>(this ConcurrentBag<T> bag)
        {
            var list = new List<T>();
            while (!bag.IsEmpty)
            {
                if (bag.TryTake(out T item))
                {
                    list.Add(item);
                }
            }
            return list;
        }

        public static Dictionary<TKey, TValue> EmptyConcurrentDictionaryToDictionary<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> concurrentDict)
        {
            var dict = new Dictionary<TKey, TValue>();
            foreach (var k in concurrentDict.Keys.ToList())
            {
                if (concurrentDict.TryRemove(k, out TValue value))
                {
                    dict.Add(k, value);
                }
            }
            return dict;
        }

        public static IEnumerable<string> WholeChunks(this string str, int chunkSize)
        {
            for (int i = 0; i < str.Length; i += chunkSize)
            {
                yield return str.Substring(i, chunkSize);
            }
        }

        public static string BytesToString(this int bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB" };
            int i;
            double dblSByte = bytes;
            for (i = 0; i < Suffix.Length - 1 && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }

            return string.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
        }

        public static string ResolveIpToHostname(this string ip)
        {
            try
            {
                return Dns.GetHostEntry(ip).HostName;
            }
            catch
            {
                return ip;
            }
        }

        public static string BytesToBitsPsToString(this long bytes, TimeSpan ts)
        {
            double bits = bytes.BytesToBitsPs(ts);
            string[] Suffix = { "bps", "Kbps", "Mbps", "Gbps" };
            int i;
            double dblSBits = bits;
            for (i = 0; i < Suffix.Length - 1 && bits >= 1024; i++, bits /= 1024)
            {
                dblSBits = bits / 1024.0;
            }

            return string.Format("{0:0.##} {1}", dblSBits, Suffix[i]);
        }

        public static double BytesToBitsPs(this long bytes, TimeSpan ts)
        {
            return bytes * 8 / ts.TotalSeconds;
        }
    }
}
