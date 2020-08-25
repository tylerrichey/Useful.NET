using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Useful
{
    /// <summary>
    /// Configuration object for CSV transformations
    /// </summary>
    public class CsvConfig
    {
        public bool Header { get; internal set; }
        public string Seperator { get; internal set; }
        public bool QuoteQualified { get; internal set; }
        public string QuoteCharacter { get; internal set; }
        public Dictionary<Type, string> Filters { get; internal set; }
        public Dictionary<Type, IFormatProvider> FormatProviders { get; internal set; }
        public List<string> IgnoredProperties { get; internal set; }

        /// <summary>
        /// Default settings for CSV transformations
        /// </summary>
        public static CsvConfig Default => new CsvConfig
        {
            Header = true,
            Seperator = ",",
            QuoteQualified = true,
            QuoteCharacter = "\"",
            Filters = new Dictionary<Type, string>
            {
                {  typeof(DateTime), "yyyy-MM-dd HH:mm:ss" }
            },
            FormatProviders = new Dictionary<Type, IFormatProvider>(),
            IgnoredProperties = new List<string>()
        };

        public static CsvConfig Empty => new CsvConfig();

        internal CsvConfig()
        {
            Seperator = string.Empty;
            QuoteCharacter = string.Empty;
            Filters = new Dictionary<Type, string>();
            FormatProviders = new Dictionary<Type, IFormatProvider>();
            IgnoredProperties = new List<string>();
        }
    }

    /// <summary>
    /// Configuration building extension methods for fluent CSV transformations
    /// </summary>
    public static class CsvConfigBuilder
    {
        /// <summary>
        /// Whether or not to include a header
        /// </summary>
        /// <param name="config"></param>
        /// <param name="useHeader">default = true</param>
        /// <returns></returns>
        public static CsvConfig UseHeader(this CsvConfig config, bool useHeader = true)
        {
            config.Header = useHeader;
            return config;
        }

        /// <summary>
        /// Define the character or string to use a column seperator
        /// </summary>
        /// <param name="config"></param>
        /// <param name="seperator">default = ,</param>
        /// <returns></returns>
        public static CsvConfig UseSeperator(this CsvConfig config, string seperator)
        {
            config.Seperator = seperator;
            return config;
        }

        /// <summary>
        /// Determine whether or not to quote qualify columns, and also define the quote qualification character/string
        /// </summary>
        /// <param name="config"></param>
        /// <param name="quoteQualify">default = true</param>
        /// <param name="quoteCharacter">default = "</param>
        /// <returns></returns>
        public static CsvConfig UseQuoteQualification(this CsvConfig config, bool quoteQualify = true, string quoteCharacter = "\"")
        {
            config.QuoteQualified = quoteQualify;
            config.QuoteCharacter = quoteCharacter;
            return config;
        }

        /// <summary>
        /// Set a filter for a type that implements IFormattable
        /// </summary>
        /// <typeparam name="T">A type that implements IFormattable</typeparam>
        /// <param name="config"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static CsvConfig UseFilter<T>(this CsvConfig config, string filter) where T : IConvertible
        {
            config.Filters.Add(typeof(T), filter);
            return config;
        }

        /// <summary>
        /// Set a format provider for a type that implements IFormattable
        /// </summary>
        /// <typeparam name="T">A type that implements IFormattable</typeparam>
        /// <param name="config"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public static CsvConfig UseFormatProvider<T>(this CsvConfig config, IFormatProvider formatProvider) where T : IFormattable
        {
            config.FormatProviders.Add(typeof(T), formatProvider);
            return config;
        }

        /// <summary>
        /// Add a case-sensitive property name to exclude from the resulting CSV
        /// </summary>
        /// <param name="config"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static CsvConfig IgnoreProperty(this CsvConfig config, string propertyName)
        {
            config.IgnoredProperties.Add(propertyName);
            return config;
        }

        internal static string GetFilter<T>(this CsvConfig config, T obj)
        {
            var value = string.Empty;
            config.Filters.TryGetValue(obj.GetType(), out value);
            return value;
        }

        internal static IFormatProvider GetFormatProvider<T>(this CsvConfig config, T obj)
        {
            config.FormatProviders.TryGetValue(obj.GetType(), out IFormatProvider formatProvider);
            return formatProvider;
        }
    }

    /// <summary>
    /// Static class to hold various extension methods for CSV transformations
    /// </summary>
    public static class CSV
    {
        /// <summary>
        /// Transform an IEnumerable to a byte array CSV using the default settings.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task<byte[]> ToCsv<T>(this IEnumerable<T> data) => await data.ToCsv<T>(CsvConfig.Default);

        /// <summary>
        /// Transform an IEnumerable to a byte array CSV using custom settings.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="csvConfig"></param>
        /// <returns></returns>
        public static async Task<byte[]> ToCsv<T>(this IEnumerable<T> data, CsvConfig csvConfig)
        {
            using var ms = new MemoryStream();
            using var writer = new StreamWriter(ms);
            await data.ToCsv(writer, csvConfig);
            ms.Seek(0, SeekOrigin.Begin);
            return ms.ToArray();
        }

        /// <summary>
        /// Transform an IEnumerable to CSV and write it to a file using the default settings.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="outputFilename"></param>
        /// <returns></returns>
        public static async Task ToCsv<T>(this IEnumerable<T> data, string outputFilename) => await data.ToCsv<T>(outputFilename, CsvConfig.Default);

        /// <summary>
        /// Transform an IEnumerable to CSV and write it to a file using custom settings.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="outputFilename"></param>
        /// <param name="csvConfig"></param>
        /// <returns></returns>
        public static async Task ToCsv<T>(this IEnumerable<T> data, string outputFilename, CsvConfig csvConfig)
        {
            using var writer = new StreamWriter(outputFilename);
            await data.ToCsv(writer, csvConfig);
        }

        /// <summary>
        /// Transform an IEnumerable to a CSV and write it to a Stream using the default settings.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task ToCsv<T>(this IEnumerable<T> data, Stream stream) => await data.ToCsv<T>(stream, CsvConfig.Default);

        /// <summary>
        /// Transform an IEnumerable to a CSV and write it to a Stream using custom settings.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="stream"></param>
        /// <param name="csvConfig"></param>
        /// <returns></returns>
        public static async Task ToCsv<T>(this IEnumerable<T> data, Stream stream, CsvConfig csvConfig)
        {
            using var writer = new StreamWriter(stream);
            await data.ToCsv(writer, csvConfig);
        }

        /// <summary>
        /// Transform an IEnumerable to a CSV and write it to a StreamWriter using the default settings.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="streamWriter"></param>
        /// <returns></returns>
        public static async Task ToCsv<T>(this IEnumerable<T> data, StreamWriter streamWriter) => await data.ToCsv<T>(streamWriter, CsvConfig.Default);

        /// <summary>
        /// Transform an IEnumerable to a CSV and write it to a StreamWriter using custom settings.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="streamWriter"></param>
        /// <param name="csvConfig"></param>
        /// <returns></returns>
        public static async Task ToCsv<T>(this IEnumerable<T> data, StreamWriter streamWriter, CsvConfig csvConfig)
        {
            var seperator = csvConfig.Seperator;
            if (string.IsNullOrEmpty(seperator))
            {
                throw new FormatException("No seperator provided.");
            }
            else if (csvConfig.QuoteQualified && string.IsNullOrEmpty(csvConfig.QuoteCharacter))
            {
                throw new FormatException("QuoteQualified = true, but QuoteCharacter is empty.");
            }
            var props = typeof(T).GetProperties()
                .Where(x => !csvConfig.IgnoredProperties.Contains(x.Name));
            if (csvConfig.Header)
            {
                await streamWriter.WriteLineAsync(string.Join(seperator, props.Select(p => p.Name.Quoted(csvConfig))));
            }
            foreach (var record in data)
            {
                var row = new List<string>();
                foreach (var p in props)
                {
                    var value = p.GetValue(record);
                    var result = value switch
                    {
                        IFormattable f => f.ToString(csvConfig.GetFilter(f), csvConfig.GetFormatProvider(f)),
                        IConvertible c => c.ToString(csvConfig.GetFormatProvider(c)),
                        _ => value.ToString()
                    };
                    row.Add(result.Quoted(csvConfig));
                }
                await streamWriter.WriteLineAsync(string.Join(seperator, row));
            }
            await streamWriter.FlushAsync();
        }

        private static string Quoted(this string value, CsvConfig csvConfig) 
            => csvConfig.QuoteQualified ? csvConfig.QuoteCharacter + value + csvConfig.QuoteCharacter : value;
    }
}
