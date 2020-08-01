﻿using Colorful;
using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Useful.Prompt
{
    public static class Prompt
    {
        public static SemaphoreSlim Lock = new SemaphoreSlim(1, 1);

        private static int _lastPromptLength;
        private static PromptBuilder _promptBuilder;

        public static PromptBuilder Build() => new PromptBuilder
        {
            PopulatePrompt = async () => await Task.FromResult(" > "),
            QuitKey = ConsoleKey.Q,
            QuitLine = "exit",
            StyleSheet = new StyleSheet(Color.Green)
        };

        public static void Reset()
        {
            _lastPromptLength = 0;
            _promptBuilder = null;
        }

        public static async Task Run(this PromptBuilder promptBuilder)
        {
            _promptBuilder = promptBuilder ?? throw new ArgumentException();
            await UpdatePrompt();
            if (_promptBuilder.OnStartupAction != null)
            {
                await _promptBuilder.OnStartupAction.Invoke();
            }
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
                        await _promptBuilder.KeyHandler.Invoke(key);
                    }
                    await UpdatePrompt();
                }
            }
            else if (_promptBuilder.LineHandler != null)
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
                        await _promptBuilder.LineHandler.Invoke(line);
                    }
                    await UpdatePrompt();
                }
            }
            else
            {
                throw new ArgumentException("No line handler or key handler supplied.");
            }
        }

        public static async Task UpdatePrompt() => await UpdatePrompt(false);

        private static async Task UpdatePrompt(bool isLocked)
        {
            if (!isLocked)
            {
                Lock.Wait();
            }

            ResetPromptPosition();
            var prompt = await _promptBuilder.PopulatePrompt.Invoke();
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
            UpdatePrompt(true).Wait();
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
