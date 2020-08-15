using System;
using System.Collections.Generic;
using System.Text;

namespace Useful.Prompt
{
    public class DefaultConsole : IConsole
    {
        public void Write(string input) => Console.Write(input);

        public void WriteLine(string input) => Console.WriteLine(input);

        public void WriteLineStyled(string input) => Console.WriteLine(input);

        public void WriteStyled(string input) => Console.Write(input);
    }
}
