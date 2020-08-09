using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;

namespace Useful.Prompt
{
    public interface IConsole
    {
        public void Write(string input);
        public void WriteLine(string input);
        public void WriteStyled(string input);
        public void WriteLineStyled(string input);
    }
}
