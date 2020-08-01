using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Useful.Tests
{
    public static class Helpers
    {
        public static async Task WriteKeyAsync(this MemoryStream memoryStream, string key)
        {
            await memoryStream.WriteAsync(Console.InputEncoding.GetBytes(key).AsMemory());
            await memoryStream.FlushAsync();
        }

        public static async Task WriteLineAsync(this MemoryStream memoryStream, string input)
        {
            await memoryStream.WriteAsync(Console.InputEncoding.GetBytes(input + '\n').AsMemory());
            await memoryStream.FlushAsync();
        }

        public static string CleanOutput(this StringWriter stringWriter)
        {
            var str = stringWriter.ToString();
            var strReplaced = str.Replace("\b", "");
            return strReplaced;
        }
    }
}
