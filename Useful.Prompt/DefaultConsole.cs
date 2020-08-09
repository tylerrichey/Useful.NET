using System;
using System.Collections.Generic;
using System.Text;

namespace Useful.Prompt
{
    public class DefaultConsole : IConsole
    {
        public void Write(string input) => Console.Write(input);

        public void WriteLine(string input) => Console.WriteLine(input);

        public void WriteLineStyled(string input)
        {
            throw new NotImplementedException();
        }

        public void WriteStyled(string input)
        {
            throw new NotImplementedException();
        }
    }
}
