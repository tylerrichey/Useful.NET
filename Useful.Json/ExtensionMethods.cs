using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Useful.Extension;

namespace Useful.Json
{
    public static class ExtensionMethods
    {
        public static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            },
            TypeNameHandling = TypeNameHandling.Objects,
            Converters = new List<JsonConverter>(new JsonSerializerSettings().Converters)
            {
                new StringEnumConverter()
            }
        };

        public static string SerializeObject(this object value)
        {
            return JsonConvert.SerializeObject(value, JsonSerializerSettings);
        }

        public static byte[] SerializeObjectToBytes(this object value)
        {
            return SerializeObject(value).ToBytes();
        }

        public static T DeserializeObject<T>(this string value)
        {
            return JsonConvert.DeserializeObject<T>(value, JsonSerializerSettings);
        }

        public static T DeserializeObjectFromBytes<T>(this byte[] value)
        {
            return value.GetString().DeserializeObject<T>();
        }

        public static object DeserializeObjectFromBytes(this byte[] value, Type type)
        {
            return JsonConvert.DeserializeObject(value.GetString(), type, JsonSerializerSettings);
        }

        /// <summary>
        /// Decompress, read and deserialize the JSON to specified type for a list of filenames. Used in conjunction with SerializeGzipToFile()
        /// </summary>
        /// <typeparam name="T">The type to deserialize</typeparam>
        /// <param name="files">An IEnumerable of filenames</param>
        /// <returns></returns>
        public static async IAsyncEnumerable<IEnumerable<T>> DeserializeManyGzipFiles<T>(this IEnumerable<string> files)
        {
            foreach (var f in files)
            {
                using var s = File.OpenRead(f);
                using var ms = new MemoryStream();
                using var gzip = new GZipStream(s, CompressionMode.Decompress);
                await gzip.CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var sr = new StreamReader(ms);
                using var json = new JsonTextReader(sr);
                yield return JsonSerializer.Create(JsonSerializerSettings)
                    .Deserialize<IEnumerable<T>>(json);
            }
        }

        /// <summary>
        /// Gzip and serialize an IEnumerable to a file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task SerializeGzipToFile<T>(this IEnumerable<T> input, string fileName)
        {
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);
            using var json = new JsonTextWriter(writer);
            using var file = File.Create(fileName);
            using var gzip = new GZipStream(file, CompressionLevel.Optimal);
            JsonSerializer.Create(JsonSerializerSettings)
                .Serialize(json, input);
            await json.FlushAsync();
            stream.Seek(0, SeekOrigin.Begin);
            await stream.CopyToAsync(gzip);
            await gzip.FlushAsync();
        }
    }
}
