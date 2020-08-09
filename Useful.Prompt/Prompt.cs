using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Useful.Prompt
{
    public static class Prompt
    {
        public static SemaphoreSlim Lock = new SemaphoreSlim(1, 1);

        private static int _lastPromptLength;
        private static PromptBuilder _promptBuilder;
        private static Timer _promptUpdateTimer;

        public static PromptBuilder Build() => new PromptBuilder
        {
            PopulatePrompt = async () => await Task.FromResult(" > "),
            QuitKey = ConsoleKey.Q,
            QuitLine = "exit",
            AutomaticUpdatePromptTimeSpan = TimeSpan.Zero,
            ConsoleWriter = new DefaultConsole()
        };

        public static async Task Run(this PromptBuilder promptBuilder)
        {
            _lastPromptLength = 0;
            _promptBuilder = promptBuilder;
            await UpdatePrompt();
            
            if (_promptBuilder.UseOnStartupAction)
            {
                await _promptBuilder.OnStartupAction.Invoke();
            }

            if (_promptBuilder.AutomaticUpdatePromptTimeSpan != TimeSpan.Zero)
            {
                _promptUpdateTimer = new Timer((s) =>
                {
                    if (Lock.CurrentCount > 0)
                    {
                        UpdatePrompt().Wait();
                    }
                }, null, _promptBuilder.AutomaticUpdatePromptTimeSpan, _promptBuilder.AutomaticUpdatePromptTimeSpan);
            }

            if (_promptBuilder.UseKeyHandler)
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
                        try
                        {
                            await _promptBuilder.KeyHandler.Invoke(key);
                        }
                        catch (Exception e)
                        {
                            WriteLine("Unhandled Exception: {0} - {1}", e.Source, e.Message);
                        }
                    }
                    await UpdatePrompt();
                }
            }
            else if (_promptBuilder.UseLineHandler)
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
                        try
                        {
                            await _promptBuilder.LineHandler.Invoke(line);
                        }
                        catch (Exception e)
                        {
                            WriteLine("Unhandled Exception: {0} - {1}", e.Source, e.Message);
                        }
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
                await Lock.WaitAsync();
            }

            await UpdatePromptUnlocked();
            Lock.Release();
        }

        private static async Task UpdatePromptUnlocked()
        {
            ResetPromptPosition();
            var prompt = await _promptBuilder.PopulatePrompt.Invoke();
            var paddedPrompt = prompt.PadRight(_lastPromptLength);
            var backPad = _lastPromptLength - prompt.Length < 0 ? 0 : paddedPrompt.Length + (_lastPromptLength - prompt.Length);
            _promptBuilder.ConsoleWriter.WriteStyled(paddedPrompt.PadRight(backPad, '\b'));
            _lastPromptLength = paddedPrompt.Length;
        }

        private static void ResetPromptPosition() => _promptBuilder.ConsoleWriter.Write(string.Concat(Enumerable.Range(0, _lastPromptLength).Select(i => "\b")));

        public static void WriteLine(string input, params object[] arg)
        {
            Lock.Wait();
            WriteLineUnlocked(input, arg);
            UpdatePrompt(true).Wait();
        }
        public static void WriteLineUnlocked(string input, params object[] arg)
        {
            ResetPromptPosition();
            _promptBuilder.ConsoleWriter.WriteLineStyled(string.Format(input, arg).PadRight(_lastPromptLength));
            _lastPromptLength = 0;
            UpdatePromptUnlocked().Wait();
        }

        public static void WriteAtPosition(string input, int left, int fromTop)
        {
            try
            {
                Lock.Wait();
                WriteAtPositionUnlocked(input, left, fromTop);
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

        public static void WriteAtPositionUnlocked(string input, int left, int fromTop)
        {
            var currentTop = System.Console.CursorTop;
            WriteAtPositionUnlockedNoReset(input, left, fromTop);
            System.Console.SetCursorPosition(0, currentTop);
        }

        public static void WriteAtPositionUnlockedNoReset(string input, int left, int fromTop)
        {
            System.Console.SetCursorPosition(left, System.Console.CursorTop - fromTop);
            _promptBuilder.ConsoleWriter.WriteStyled(input);
        }

        public static async Task ResetPositionUnlocked()
        {
            try
            {
                System.Console.SetCursorPosition(System.Console.WindowLeft, System.Console.BufferHeight - 1);
                await UpdatePrompt(Lock.CurrentCount == 0);
            }
            catch
            {
                if (Lock.CurrentCount == 0)
                {
                    Lock.Release();
                }
                throw;
            }
        }
    }
}
