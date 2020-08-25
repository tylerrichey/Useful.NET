using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;

namespace Useful.Prompt
{
    /// <summary>
    /// This wraps Colorful.Console (https://github.com/tomakita/Colorful.Console) for use with Prompt.
    /// </summary>
    public class ColorfulConsole : IConsole
    {
        private Colorful.StyleSheet _styleSheet = new Colorful.StyleSheet(Color.Green);

        public ColorfulConsole() { }
        /// <summary>
        /// This wraps Colorful.Console's WriteStyled() functionality by allowing you to supply a dictionary of regular expressions and the color you'd like them displayed in.
        /// </summary>
        /// <param name="defaultColor">Default color of all text displayed</param>
        /// <param name="regexColors"></param>
        public ColorfulConsole(Color defaultColor, Dictionary<Regex, Color> regexColors)
        {
            _styleSheet = new Colorful.StyleSheet(defaultColor);
            foreach (var i in regexColors)
            {
                _styleSheet.AddStyle(i.Key.ToString(), i.Value);
            }
        }

        public void Update(Color defaultColor, Dictionary<Regex, Color> regexColors)
        {
            _styleSheet = new Colorful.StyleSheet(defaultColor);
            foreach (var i in regexColors)
            {
                _styleSheet.AddStyle(i.Key.ToString(), i.Value);
            }
        }

        public void Write(string input) => Colorful.Console.Write(input);

        public void WriteLine(string input) => Colorful.Console.WriteLine(input);

        public void WriteLineStyled(string input) => Colorful.Console.WriteLineStyled(input, _styleSheet);

        public void WriteStyled(string input) => Colorful.Console.WriteStyled(input, _styleSheet);
    }
}
