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
    /// <summary>
    /// A static class that provides a quick, fluent way to get a command prompt based console application up and running.
    /// </summary>
    public static class Prompt
    {
        /// <summary>
        /// A <see cref="SemaphoreSlim"/> used to restrict console writes. If using one of the "Unlocked" methods, you need to manage this yourself to stop user-input or auto-updating prompts causing display issues.
        /// </summary>
        public static SemaphoreSlim Lock = new SemaphoreSlim(1, 1);

        private static int _lastPromptLength;
        private static PromptBuilder _promptBuilder;
        private static Timer _promptUpdateTimer;

        /// <summary>
        /// Default settings for a Prompt
        /// </summary>
        /// <returns></returns>
        public static PromptBuilder Build() => new PromptBuilder
        {
            PopulatePrompt = async () => await Task.FromResult(" > "),
            QuitKey = ConsoleKey.Q,
            QuitLine = "exit",
            AutomaticUpdatePromptTimeSpan = TimeSpan.Zero,
            ConsoleWriter = new DefaultConsole()
        };

        /// <summary>
        /// Run the prompt. Remember that this will call <see cref="Console.ReadKey"/> or <see cref="Console.ReadLine"/> which will block the main thread.
        /// </summary>
        /// <param name="promptBuilder"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Force an update to the prompt. This is automatically called if you use <see cref="WriteLine(string, object[])"/> or <see cref="WriteLineUnlocked(string, object[])"/>, but you'll have to call it yourself if writing at specific positions, i.e.: <see cref="WriteAtPosition(string, int, int)"/>
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Writes a line to the console output along with managing the <see cref="Lock"/> and updating the prompt after. Before writing a line, the cursor is reset to the bottom left corner.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="arg"></param>
        public static void WriteLine(string input, params object[] arg)
        {
            Lock.Wait();
            WriteLineUnlocked(input, arg);
            UpdatePrompt(true).Wait();
        }

        /// <summary>
        /// Writes a line to the console output unlocked; also updates prompt after.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="arg"></param>
        public static void WriteLineUnlocked(string input, params object[] arg)
        {
            ResetPromptPosition();
            _promptBuilder.ConsoleWriter.WriteLineStyled(string.Format(input, arg).PadRight(_lastPromptLength));
            _lastPromptLength = 0;
            UpdatePromptUnlocked().Wait();
        }

        /// <summary>
        /// Writes a string to a specific position in the console window while manging the <see cref="Lock"/>. This is not a "safe" function and it will throw an exception if you input invalid coordinates, see: <see cref="Console.SetCursorPosition(int, int)"/>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="left">The column to write in, starting at 0 from the left</param>
        /// <param name="fromTop">The row starting from the bottom of the console to write in. For example, to write on the row above the prompt, you'd supply 1</param>
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

        /// <summary>
        /// Writes a string to a specific position in the console window unlocked. This is not a "safe" function and it will throw an exception if you input invalid coordinates, see: <see cref="Console.SetCursorPosition(int, int)"/>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="left">The column to write in, starting at 0 from the left</param>
        /// <param name="fromTop">The row starting from the bottom of the console to write in. For example, to write on the row above the prompt, you'd supply 1</param>
        public static void WriteAtPositionUnlocked(string input, int left, int fromTop)
        {
            var currentTop = System.Console.CursorTop;
            WriteAtPositionUnlockedNoReset(input, left, fromTop);
            System.Console.SetCursorPosition(0, currentTop);
        }

        /// <summary>
        /// Writes a string to a specific position in the console window unlocked, and does not reset the cursor position after writing. Can be helpful if performing multiple writes under the same lock.
        /// This is not a "safe" function and it will throw an exception if you input invalid coordinates, see: <see cref="Console.SetCursorPosition(int, int)"/>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="left">The column to write in, starting at 0 from the left</param>
        /// <param name="fromTop">The row starting from the bottom of the console to write in. For example, to write on the row above the prompt, you'd supply 1</param>
        public static void WriteAtPositionUnlockedNoReset(string input, int left, int fromTop)
        {
            System.Console.SetCursorPosition(left, System.Console.CursorTop - fromTop);
            _promptBuilder.ConsoleWriter.WriteStyled(input);
        }

        /// <summary>
        /// Resets the cursor position to "default" at the prompt.
        /// </summary>
        /// <returns></returns>
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
