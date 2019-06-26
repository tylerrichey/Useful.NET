using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Useful.ExtensionMethods;

namespace Useful.Json
{
    public static class ExtensionMethods
    {
        private static JsonSerializerSettings _jsonSerializerSettings;
        public static JsonSerializerSettings JsonSerializerSettings
        {
            get
            {
                if (_jsonSerializerSettings == null)
                {
                    _jsonSerializerSettings = new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        ContractResolver = new DefaultContractResolver
                        {
                            NamingStrategy = new CamelCaseNamingStrategy()
                        },
                        TypeNameHandling = TypeNameHandling.Objects
                    };
                    _jsonSerializerSettings.Converters.Add(new StringEnumConverter());
                }
                return _jsonSerializerSettings;
            }
        }

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

        public static IEnumerable<IEnumerable<T>> DeserializeManyGzipFiles<T>(this IEnumerable<string> files)
        {
            foreach (var f in files)
            {
                using (var s = File.OpenRead(f))
                using (var ms = new MemoryStream())
                using (var gzip = new GZipStream(s, CompressionMode.Decompress))
                {
                    gzip.CopyTo(ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    var sr = new StreamReader(ms);
                    using (var json = new JsonTextReader(sr))
                    {
                        yield return JsonSerializer.Create(JsonSerializerSettings)
                           .Deserialize<IEnumerable<T>>(json);
                    }
                }
            }
        }

        public static void SerializeGzipToFile<T>(this IEnumerable<T> input, string fileName)
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var json = new JsonTextWriter(writer))
            using (var file = File.Create(fileName))
            using (var gzip = new GZipStream(file, CompressionLevel.Optimal))
            {
                JsonSerializer.Create(JsonSerializerSettings)
                    .Serialize(json, input);
                json.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(gzip);
                gzip.Flush();
            }
        }
    }
}
