# Useful.NET

[![Build status](https://ci.appveyor.com/api/projects/status/q2c8v5n8t7bus9hw?svg=true)](https://ci.appveyor.com/project/tylerrichey/useful-net)
[![NuGet](https://img.shields.io/nuget/v/Useful.NET.svg)](https://www.nuget.org/packages/Useful.NET/)

There's a bunch of extension methods, and here's some of the CSV stuff:

```c#
var data = await GetListOfObjects();
var fileName = Path.GetTempFileName();
await basicData.ToCsv(fileName);
```
```c#
var data = new List<TestClass>
{
    new TestClass { TestBool = true, TestString = "test1", TestDate = new DateTime(1987, 11, 2, 4, 20, 0), TestInt = 1 },
    new TestClass { TestBool = false, TestString = "test2", TestDate = new DateTime(1981, 11, 18, 4, 20, 0), TestInt = 2 }
};
var config = CsvConfig.Empty
    .UseHeader(false)
    .UseQuoteQualification(false)
    .UseSeperator("|")
    .IgnoreProperty("TestBool")
    .UseFilter(typeof(DateTime), "MM/dd/yyyy")
    .UseFilter(typeof(int), "P");
byte[] csvData = await data.ToCsv(config);
/*
test1|11/02/1987|100.00%
test2|11/18/1981|200.00%
*/
```

# Useful.Prompt

[![NuGet](https://img.shields.io/nuget/v/Useful.Prompt.svg)](https://www.nuget.org/packages/Useful.Prompt/)

```c#
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

/*
2:41 PM > hello
world
2:41 PM > exit
*/
```
```c#
await Prompt.Build()
    .SetConsoleWriter(new ColorfulConsole(Color.AliceBlue, new Dictionary<Regex, Color>()))
    .SetPopulatePromptAction(async () =>
    {
        var weather = await GetWeatherData();
        return weather.Temperature + " > ";
    })
    .SetAutoPromptUpdateIfUnlockedTimeSpan(TimeSpan.FromMinutes(10))
    .SetOnStartupAction(async () => await StartDataStream())
    .SetKeyHandler(ConsoleKeyHandler)
    .SetQuitKeyInfo(ConsoleKey.X)
    .Run();
```

# Useful.Json

[![NuGet](https://img.shields.io/nuget/v/Useful.Json.svg)](https://www.nuget.org/packages/Useful.Json/)

This is pretty much extension methods around an opinionated JsonSerializerSettings, but there is also:

```c#
var data = GetIEnumerableOfDataToSave();
await data.SerializeGzipToFile(saveDataFilename);
```
```c#
var saveDataFilenames = Directory.GetFiles(appDirectory, ".json");
await foreach (var data in saveDataFilenames.DeserializeManyGzipFiles<SavedData>())
{
    var savedDataList = data.ToList();
}
```