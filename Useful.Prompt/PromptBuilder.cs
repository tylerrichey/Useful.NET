using Colorful;
using System;
using System.Threading.Tasks;

namespace Useful.Prompt
{
    public class PromptBuilder
    {
        public StyleSheet StyleSheet { get; internal set; }
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
    }
}
