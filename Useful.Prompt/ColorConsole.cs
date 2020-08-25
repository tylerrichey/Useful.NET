using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Useful.Prompt
{

    public class ColorConsole : IConsole
    {
        private Dictionary<Regex, Color> _regexColors;
        private ConsoleColor _defaultColor = ConsoleColor.Blue;
        public ColorConsole(Dictionary<Regex, Color> regexColors)
        {
            _regexColors = regexColors;
            Console.ForegroundColor = _defaultColor;
        }
        public void Write(string input) => Console.Write(input);

        public void WriteLine(string input) => Console.WriteLine(input);

        public void WriteLineStyled(string input) => WriteStyled(input + '\n');

        public void WriteStyled(string input)
        {
            throw new NotImplementedException();

            //var arr = input.Split(' ');
            //for (var i = 0; i < arr.Length; i++)
            //{
            //    var w = arr[i];
            //    if (string.IsNullOrWhiteSpace(w))
            //    {
            //        Console.Write(w);
            //        continue;
            //    }
            //    try
            //    {
            //        //if (w[0] == '@')
            //        //{
            //        //    Console.ForegroundColor = ConsoleColor.Green;
            //        //}
            //        //else if (w[0] == '#')
            //        //{
            //        //    Console.ForegroundColor = ConsoleColor.Red;
            //        //}
            //        //else if (w.Contains("://"))
            //        //{
            //        //    Console.ForegroundColor = ConsoleColor.Cyan;
            //        //}
            //    }
            //    catch (Exception e)
            //    {
            //        throw e;
            //    }
            //    finally
            //    {
            //        var space = i == input.Length - 1 ? "" : " ";
            //        Console.Write(w + space);
            //        //Console.ForegroundColor = _defaultColor;
            //    }
            //}
        }
    }
}
