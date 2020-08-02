using Colorful;
using System;
using System.Threading.Tasks;

namespace Useful.Prompt
{
    public static class PromptBuilderExtensionMethods
    {
        public static PromptBuilder SetStyleSheet(this PromptBuilder promptBuilder, StyleSheet styleSheet)
        {
            promptBuilder.StyleSheet = styleSheet;
            return promptBuilder;
        }

        public static PromptBuilder SetOnStartupAction(this PromptBuilder promptBuilder, Func<Task> action)
        {
            promptBuilder.UseOnStartupAction = true;
            promptBuilder.OnStartupAction = action;
            return promptBuilder;
        }

        public static PromptBuilder SetPopulatePromptAction(this PromptBuilder promptBuilder, Func<Task<string>> action)
        {
            promptBuilder.PopulatePrompt = action;
            return promptBuilder;
        }

        public static PromptBuilder SetKeyHandler(this PromptBuilder promptBuilder, Func<ConsoleKeyInfo, Task> action)
        {
            promptBuilder.UseKeyHandler = true;
            promptBuilder.UseLineHandler = false;
            promptBuilder.KeyHandler = action;
            return promptBuilder;
        }

        public static PromptBuilder SetQuitKeyInfo(this PromptBuilder promptBuilder, ConsoleKey consoleKey)
        {
            promptBuilder.QuitKey = consoleKey;
            return promptBuilder;
        }

        public static PromptBuilder SetLineHandler(this PromptBuilder promptBuilder, Func<string, Task> action)
        {
            promptBuilder.UseKeyHandler = false;
            promptBuilder.UseLineHandler = true;
            promptBuilder.LineHandler = action;
            return promptBuilder;
        }

        public static PromptBuilder SetQuitLine(this PromptBuilder promptBuilder, string quitLine)
        {
            promptBuilder.QuitLine = quitLine;
            return promptBuilder;
        }
    }
}
