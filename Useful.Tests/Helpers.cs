using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Useful.Tests
{
    public static class Helpers
    {
        public static StreamReader GetReader(this MemoryStream memoryStream)
        {
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new StreamReader(memoryStream);
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
