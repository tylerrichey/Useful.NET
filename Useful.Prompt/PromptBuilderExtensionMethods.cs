using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Useful.Prompt
{
    /// <summary>
    /// Building extension methods for a fluent Prompt configuration
    /// </summary>
    public static class PromptBuilderExtensionMethods
    {
        /// <summary>
        /// Override the default console writer to do things like use colors.
        /// </summary>
        /// <param name="promptBuilder"></param>
        /// <param name="console"></param>
        /// <returns></returns>
        public static PromptBuilder SetConsoleWriter(this PromptBuilder promptBuilder, IConsole console)
        {
            promptBuilder.ConsoleWriter = console;
            return promptBuilder;
        }

        /// <summary>
        /// Define an action to run after you initialize the Prompt, but before the user gets the prompt.
        /// </summary>
        /// <param name="promptBuilder"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static PromptBuilder SetOnStartupAction(this PromptBuilder promptBuilder, Func<Task> action)
        {
            promptBuilder.UseOnStartupAction = true;
            promptBuilder.OnStartupAction = action;
            return promptBuilder;
        }

        /// <summary>
        /// Define a function that returns the string that populates the prompt.
        /// </summary>
        /// <param name="promptBuilder"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static PromptBuilder SetPopulatePromptAction(this PromptBuilder promptBuilder, Func<Task<string>> action)
        {
            promptBuilder.PopulatePrompt = action;
            return promptBuilder;
        }

        /// <summary>
        /// Define a handler function for Console.ReadKey(). If you use keys, you can't define a line handler.
        /// </summary>
        /// <param name="promptBuilder"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static PromptBuilder SetKeyHandler(this PromptBuilder promptBuilder, Func<ConsoleKeyInfo, Task> action)
        {
            promptBuilder.UseKeyHandler = true;
            promptBuilder.UseLineHandler = false;
            promptBuilder.KeyHandler = action;
            return promptBuilder;
        }

        /// <summary>
        /// Override the default key for quitting the Prompt.
        /// </summary>
        /// <param name="promptBuilder"></param>
        /// <param name="consoleKey"></param>
        /// <returns></returns>
        public static PromptBuilder SetQuitKeyInfo(this PromptBuilder promptBuilder, ConsoleKey consoleKey)
        {
            promptBuilder.QuitKey = consoleKey;
            return promptBuilder;
        }

        /// <summary>
        /// Define a handler function for Console.ReadLine() inputs. If you use lines, you can't define a key handler.
        /// </summary>
        /// <param name="promptBuilder"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static PromptBuilder SetLineHandler(this PromptBuilder promptBuilder, Func<string, Task> action)
        {
            promptBuilder.UseKeyHandler = false;
            promptBuilder.UseLineHandler = true;
            promptBuilder.LineHandler = action;
            return promptBuilder;
        }

        /// <summary>
        /// Override the default line for quitting the Prompt.
        /// </summary>
        /// <param name="promptBuilder"></param>
        /// <param name="consoleKey"></param>
        /// <returns></returns>
        public static PromptBuilder SetQuitLine(this PromptBuilder promptBuilder, string quitLine)
        {
            promptBuilder.QuitLine = quitLine;
            return promptBuilder;
        }

        /// <summary>
        /// Define a timepsan to automatically update the prompt at an interval. Useful for including things like date/time in your prompts. Note, if the prompt is "locked" when the auto update occurs, it will just skip it instead of waiting since it will be updated on unlock.
        /// </summary>
        /// <param name="promptBuilder"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static PromptBuilder SetAutoPromptUpdateIfUnlockedTimeSpan(this PromptBuilder promptBuilder, TimeSpan timeSpan)
        {
            promptBuilder.AutomaticUpdatePromptTimeSpan = timeSpan;
            return promptBuilder;
        }
    }
}
