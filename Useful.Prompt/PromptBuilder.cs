using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Useful.Prompt
{
    /// <summary>
    /// This object is used to hold the configuration for the Prompt
    /// </summary>
    public class PromptBuilder
    {
        public Func<Task> OnStartupAction { get; internal set; }
        public Func<Task<string>> PopulatePrompt { get; internal set; }
        public Func<ConsoleKeyInfo, Task> KeyHandler { get; internal set; }
        public Func<string, Task> LineHandler { get; internal set; }
        public ConsoleKey QuitKey { get; internal set; }
        public string QuitLine { get; internal set; }
        public bool UseLineHandler { get; internal set; }
        public bool UseKeyHandler { get; internal set; }
        public bool UseOnStartupAction { get; internal set; }
        public TimeSpan AutomaticUpdatePromptTimeSpan { get; internal set; }
        public IConsole ConsoleWriter { get; internal set; }
    }
}
