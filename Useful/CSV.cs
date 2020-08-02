using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Useful
{
    public class CsvConfig
    {
        public bool Header { get; internal set; }
        public string Seperator { get; internal set; }
        public bool QuoteQualified { get; internal set; }
        public string QuoteCharacter { get; internal set; }
        public Dictionary<Type, string> Filters { get; internal set; }
        public Dictionary<Type, IFormatProvider> FormatProviders { get; internal set; }
        public List<string> IgnoredProperties { get; internal set; }

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

    public static class CsvConfigBuilder
    {
        public static CsvConfig UseHeader(this CsvConfig config, bool useHeader = true)
        {
            config.Header = useHeader;
            return config;
        }

        public static CsvConfig UseSeperator(this CsvConfig config, string seperator)
        {
            config.Seperator = seperator;
            return config;
        }

        public static CsvConfig UseQuoteQualification(this CsvConfig config, bool quoteQualify = true, string quoteCharacter = "\"")
        {
            config.QuoteQualified = quoteQualify;
            config.QuoteCharacter = quoteCharacter;
            return config;
        }
        
        public static CsvConfig UseFilter(this CsvConfig config, Type type, string filter)
        {
            config.Filters.Add(type, filter);
            return config;
        }

        public static CsvConfig UseFormatProvider(this CsvConfig config, Type type, IFormatProvider formatProvider)
        {
            config.FormatProviders.Add(type, formatProvider);
            return config;
        }

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

    public static class CSV
    {
        public static async Task<byte[]> ToCsv<T>(this IEnumerable<T> data) => await data.ToCsv<T>(CsvConfig.Default);

        public static async Task<byte[]> ToCsv<T>(this IEnumerable<T> data, CsvConfig csvConfig)
        {
            using var ms = new MemoryStream();
            using var writer = new StreamWriter(ms);
            await data.ToCsv(writer, csvConfig);
            ms.Seek(0, SeekOrigin.Begin);
            return ms.ToArray();
        }

        public static async Task ToCsv<T>(this IEnumerable<T> data, string outputFilename) => await data.ToCsv<T>(outputFilename, CsvConfig.Default);

        public static async Task ToCsv<T>(this IEnumerable<T> data, string outputFilename, CsvConfig csvConfig)
        {
            using var writer = new StreamWriter(outputFilename);
            await data.ToCsv(writer, csvConfig);
        }

        public static async Task ToCsv<T>(this IEnumerable<T> data, Stream stream) => await data.ToCsv<T>(stream, CsvConfig.Default);

        public static async Task ToCsv<T>(this IEnumerable<T> data, Stream stream, CsvConfig csvConfig)
        {
            using var writer = new StreamWriter(stream);
            await data.ToCsv(writer, csvConfig);
        }

        public static async Task ToCsv<T>(this IEnumerable<T> data, StreamWriter streamWriter) => await data.ToCsv<T>(streamWriter, CsvConfig.Default);

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
                    var result = string.Empty;
                    result = value switch
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
