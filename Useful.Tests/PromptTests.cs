using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Useful.Prompt;
using WindowsInput;
using WindowsInput.Native;
using cmd = Useful.Prompt.Prompt;

namespace Useful.Tests
{
    [TestClass]
    [DoNotParallelize]
    public partial class PromptTests
    {
        [TestCleanup]
        public void TestCleanup()
        {
            var standardOutput = new StreamWriter(Console.OpenStandardOutput())
            {
                AutoFlush = true
            };
            Console.SetOut(standardOutput);
            var standardInput = new StreamReader(Console.OpenStandardInput());
            Console.SetIn(standardInput);
            cmd.Reset();
        }

        private StringReader InputCommand(string input)
        {
            var writer = new StringWriter();
            writer.WriteLine(input);
            return new StringReader(writer.ToString());
        }

        [TestMethod]
        public async Task Defaults()
        {
            var defaults = cmd.Build();
            var outWriter = new StringWriter();
            var inReader = InputCommand("exit");
            Console.SetOut(outWriter);
            Console.SetIn(inReader);
            await defaults.SetLineHandler(async (k) => await Task.CompletedTask)
                .Run();

            Assert.AreEqual(await defaults.PopulatePrompt(), outWriter.CleanOutput());
        }

        [TestMethod]
        public async Task OnStartup()
        {
            var prompt = cmd.Build();
            var outWriter = new StringWriter();
            var inReader = InputCommand("exit");
            Console.SetOut(outWriter);
            Console.SetIn(inReader);
            prompt.SetLineHandler(async (k) => await Task.CompletedTask);
            var didRun = false;
            await prompt.SetOnStartupAction(async () =>
            {
                didRun = true;
                await Task.CompletedTask;
            }).Run();

            Assert.AreEqual(await prompt.PopulatePrompt(), outWriter.CleanOutput());
            Assert.IsTrue(didRun);
        }

        [TestMethod]
        public async Task CustomPrompt()
        {
            var prompt = cmd.Build();
            var outWriter = new StringWriter();
            var inReader = InputCommand("exit");
            Console.SetOut(outWriter);
            Console.SetIn(inReader);
            var newPrompt = "hello > ";
            await prompt.SetLineHandler(async (k) => await Task.CompletedTask)
                .SetPopulatePromptAction(async () => await Task.FromResult(newPrompt))
                .Run();

            Assert.AreEqual(newPrompt, outWriter.CleanOutput());
        }

        [TestMethod]
        public async Task LineHandlerAndWriteLine()
        {
            var prompt = cmd.Build();
            var outWriter = new StringWriter();
            Console.SetOut(outWriter);

            var inStream = new MemoryStream();
            await inStream.WriteLineAsync("test");
            await inStream.WriteLineAsync("exit");
            inStream.Seek(0, SeekOrigin.Begin);
            var inReader = new StreamReader(inStream);
            Console.SetIn(inReader);
            
            const string line = "testline";
            await prompt.SetLineHandler(async (k) =>
            {
                cmd.WriteLine(line);
                await Task.CompletedTask;
            }).Run();
            var p = await prompt.PopulatePrompt();
            Assert.AreEqual(p + line + "\r\n" + p + p, outWriter.CleanOutput());
        }

        [TestMethod]
        public async Task QuitLine()
        {
            var prompt = cmd.Build();
            var outWriter = new StringWriter();
            var inReader = InputCommand("quit");
            Console.SetOut(outWriter);
            Console.SetIn(inReader);
            await prompt.SetLineHandler(async (k) => await Task.CompletedTask)
                .SetQuitLine("quit")
                .Run();

            Assert.AreEqual(await prompt.PopulatePrompt(), outWriter.CleanOutput());
        }
    }
}
