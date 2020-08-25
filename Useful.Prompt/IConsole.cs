using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;

namespace Useful.Prompt
{
    /// <summary>
    /// A basic interface wrapper for the existing System.Console write functions.
    /// </summary>
    public interface IConsole
    {
        public void Write(string input);
        public void WriteLine(string input);
        public void WriteStyled(string input);
        public void WriteLineStyled(string input);
    }
}
