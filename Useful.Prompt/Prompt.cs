using Colorful;
using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Useful.Prompt
{
    public class PromptBuilder
    {
        public StyleSheet StyleSheet { get; internal set; }
        public Func<Task> OnStartupAction { get; internal set; }
        public Func<string> PopulatePrompt { get; internal set; }
        public Action<ConsoleKeyInfo> KeyHandler { get; internal set; }
        public Action<string> LineHandler { get; internal set; }
        public ConsoleKey QuitKey { get; internal set; }
        public string QuitLine { get; internal set; }
    }

    public static class Prompt
    {
        private static int _lastPromptLength;
        public static SemaphoreSlim Lock = new SemaphoreSlim(1, 1);
        private static PromptBuilder _promptBuilder;

        public static PromptBuilder Build() => new PromptBuilder
        {
            PopulatePrompt = () => " > ",
            QuitKey = ConsoleKey.Q,
            QuitLine = "exit",
            StyleSheet = new StyleSheet(Color.AliceBlue)
        };

        public static PromptBuilder SetStyleSheet(this PromptBuilder promptBuilder, StyleSheet styleSheet)
        {
            promptBuilder.StyleSheet = styleSheet;
            return promptBuilder;
        }

        public static PromptBuilder SetOnStartupAction(this PromptBuilder promptBuilder, Func<Task> action)
        {
            promptBuilder.OnStartupAction = action;
            return promptBuilder;
        }

        public static PromptBuilder SetPopulatePromptAction(this PromptBuilder promptBuilder, Func<string> action)
        {
            promptBuilder.PopulatePrompt = action;
            return promptBuilder;
        }

        public static PromptBuilder SetKeyHandler(this PromptBuilder promptBuilder, Action<ConsoleKeyInfo> action)
        {
            promptBuilder.KeyHandler = action;
            return promptBuilder;
        }

        public static PromptBuilder SetQuitKeyInfo(this PromptBuilder promptBuilder, ConsoleKey consoleKey)
        {
            promptBuilder.QuitKey = consoleKey;
            return promptBuilder;
        }

        public static PromptBuilder SetLineHandler(this PromptBuilder promptBuilder, Action<string> action)
        {
            promptBuilder.LineHandler = action;
            return promptBuilder;
        }

        public static PromptBuilder SetQuitLine(this PromptBuilder promptBuilder, string quitLine)
        {
            promptBuilder.QuitLine = quitLine;
            return promptBuilder;
        }

        public static void Run(this PromptBuilder promptBuilder)
        {
            _promptBuilder = promptBuilder ?? throw new ArgumentException();
            UpdatePrompt();
            _promptBuilder.OnStartupAction?.Invoke().Wait();
            if (_promptBuilder.KeyHandler != null)
            {
                while (true)
                {
                    var key = System.Console.ReadKey(true);
                    if (_promptBuilder.QuitKey == key.Key)
                    {
                        return;
                    }
                    else
                    {
                        _promptBuilder.KeyHandler.Invoke(key);
                    }
                    UpdatePrompt();
                }
            }
            else
            {
                while (true)
                {
                    var line = System.Console.ReadLine();
                    if (_promptBuilder.QuitLine == line)
                    {
                        return;
                    }
                    else
                    {
                        _promptBuilder.LineHandler.Invoke(line);
                    }
                    UpdatePrompt();
                }
            }
        }

        public static void UpdatePrompt() => UpdatePrompt(false);

        private static void UpdatePrompt(bool isLocked)
        {
            if (!isLocked)
            {
                Lock.Wait();
            }

            ResetPromptPosition();
            var prompt = _promptBuilder.PopulatePrompt.Invoke();
            var paddedPrompt = prompt.PadRight(_lastPromptLength);
            var backPad = _lastPromptLength - prompt.Length < 0 ? 0 : paddedPrompt.Length + (_lastPromptLength - prompt.Length);
            Colorful.Console.WriteStyled(paddedPrompt.PadRight(backPad, '\b'), _promptBuilder.StyleSheet);
            _lastPromptLength = paddedPrompt.Length;
            Lock.Release();
        }

        private static void ResetPromptPosition() => Colorful.Console.Write(string.Concat(Enumerable.Range(0, _lastPromptLength).Select(i => "\b")));

        public static void WriteLine(string input, params object[] arg)
        {
            Lock.Wait();
            ResetPromptPosition();
            Colorful.Console.WriteLineStyled(string.Format(input, arg).PadRight(_lastPromptLength), _promptBuilder.StyleSheet);
            _lastPromptLength = 0;
            UpdatePrompt(true);
        }

        public static void WriteAtPosition(string input, int left, int fromTop)
        {
            try
            {
                Lock.Wait();
                var currentTop = System.Console.CursorTop;
                System.Console.SetCursorPosition(left, System.Console.CursorTop - fromTop);
                Colorful.Console.WriteStyled(input, _promptBuilder.StyleSheet);
                System.Console.SetCursorPosition(0, currentTop);
            }
            catch
            {
                throw;
            }
            finally
            {
                Lock.Release();
            }
        }
    }
}
