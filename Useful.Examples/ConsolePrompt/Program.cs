using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Useful.Prompt;

namespace ConsolePrompt
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Prompt.Build()
                .SetPopulatePromptAction(() => Task.FromResult(DateTime.Now.ToShortTimeString() + " > "))
                .SetLineHandler((line) =>
                {
                    switch (line)
                    {
                        case "hello":
                            Prompt.WriteLine("world");
                            break;
                    }
                    return Task.CompletedTask;
                })
                .Run();
        }
    }
}
