using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Useful.Prompt;
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
        }

        private async Task<StreamReader> InputCommand(string input)
        {
            var inStream = new MemoryStream();
            await inStream.WriteLineAsync(input);
            return inStream.GetReader();
        }

        [TestMethod]
        public async Task Defaults()
        {
            var defaults = cmd.Build();
            var outWriter = new StringWriter();
            Console.SetOut(outWriter);
            Console.SetIn(await InputCommand("exit"));
            await defaults.SetLineHandler(async (k) => await Task.CompletedTask)
                .Run();

            Assert.AreEqual(await defaults.PopulatePrompt(), outWriter.CleanOutput());
        }

        [TestMethod]
        public async Task MissingHandler()
        {
            var prompt = cmd.Build();
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => prompt.Run());
        }

        [TestMethod]
        public async Task OnStartup()
        {
            var prompt = cmd.Build();
            var outWriter = new StringWriter();
            Console.SetOut(outWriter);
            Console.SetIn(await InputCommand("exit"));
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
            Console.SetOut(outWriter);
            Console.SetIn(await InputCommand("exit"));
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
            Console.SetIn(inStream.GetReader());
            
            const string line = "testline";
            await prompt.SetLineHandler(async (k) =>
            {
                cmd.WriteLine(line);
                await Task.CompletedTask;
            }).Run();
            var p = await prompt.PopulatePrompt();
            Assert.AreEqual(p + line + "\r\n" + p + p + p, outWriter.CleanOutput());
        }

        [TestMethod]
        public async Task QuitLine()
        {
            var prompt = cmd.Build();
            var outWriter = new StringWriter();
            Console.SetOut(outWriter);
            Console.SetIn(await InputCommand("quit"));
            await prompt.SetLineHandler(async (k) => await Task.CompletedTask)
                .SetQuitLine("quit")
                .Run();

            Assert.AreEqual(await prompt.PopulatePrompt(), outWriter.CleanOutput());
        }

        [TestMethod]
        public async Task LineHandlerUnhandledException()
        {
            var prompt = cmd.Build();
            var outWriter = new StringWriter();
            Console.SetOut(outWriter);
            
            var inStream = new MemoryStream();
            await inStream.WriteLineAsync("test");
            await inStream.WriteLineAsync("exit");
            Console.SetIn(inStream.GetReader());
            const string exceptionMessage = "test exception";
            await prompt.SetLineHandler((_) => throw new Exception(exceptionMessage))
                .Run();
            var p = await prompt.PopulatePrompt();
            Assert.AreEqual(p + "Unhandled Exception: Useful.Tests - " + exceptionMessage + "\r\n" + p + p + p, outWriter.CleanOutput());
        }
    }
}
