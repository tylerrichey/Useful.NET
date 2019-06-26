using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Useful.Tests
{
    [TestClass]
    public partial class Csv
    {
        List<TestClass> basicData = new List<TestClass>
        {
            new TestClass
            {
                TestBool = true,
                TestString = "test1",
                TestDate = new DateTime(1987, 11, 2, 4, 20, 0)
            }
        };

        string basicTruth(string dateOverride = "1987-11-02 04:20:00")
        {
            var truth = new StringBuilder(2);
            truth.AppendLine("\"TestString\",\"TestBool\",\"TestDate\",\"TestInt\"");
            truth.AppendLine("\"test1\",\"True\",\"" + dateOverride + "\",\"0\"");
            return truth.ToString();
        }

        [TestMethod]
        public async Task WriteCsvSeperatorException()
        {
            await Assert.ThrowsExceptionAsync<FormatException>(() => basicData.ToCsv(CsvConfig.Default.UseSeperator(string.Empty)));
        }

        [TestMethod]
        public async Task WriteCsvQuoteException()
        {
            await Assert.ThrowsExceptionAsync<FormatException>(() => basicData.ToCsv(CsvConfig.Default.UseQuoteQualification(true, string.Empty)));
        }

        [TestMethod]
        public async Task WriteCsvFormatProvider()
        {
            var config = CsvConfig.Default
                .UseFormatProvider(typeof(DateTime), new CultureInfo("fr-FR"));
            config.Filters.Clear();
            var result = await basicData.ToCsv(config);
            Assert.AreEqual(basicTruth("02/11/1987 04:20:00"), Encoding.ASCII.GetString(result));
        }

        [TestMethod]
        public async Task WriteCsvBasic()
        {
            var result = await basicData.ToCsv();
            Assert.AreEqual(basicTruth(), Encoding.ASCII.GetString(result));
        }

        [TestMethod]
        public async Task WriteCsvFile()
        {
            var fileName = Path.GetTempFileName();
            await basicData.ToCsv(fileName);
            Assert.AreEqual(basicTruth(), await File.ReadAllTextAsync(fileName));
            File.Delete(fileName);
        }

        [TestMethod]
        public async Task WriteCsvAdvanced()
        {
            var data = new List<TestClass>
            {
                new TestClass
                {
                    TestBool = true,
                    TestString = "test1",
                    TestDate = new DateTime(1987, 11, 2, 4, 20, 0),
                    TestInt = 1
                },
                new TestClass
                {
                    TestBool = false,
                    TestString = "test2",
                    TestDate = new DateTime(1981, 11, 18, 4, 20, 0),
                    TestInt = 2
                }
            };
            var config = CsvConfig.Empty
                .UseHeader(false)
                .UseQuoteQualification(false)
                .UseSeperator("|")
                .IgnoreProperty("TestBool")
                .UseFilter(typeof(DateTime), "MM/dd/yyyy")
                .UseFilter(typeof(int), "P");
            var result = await data.ToCsv(config);
            var truth = new StringBuilder(2);
            truth.AppendLine("test1|11/02/1987|100.00%");
            truth.AppendLine("test2|11/18/1981|200.00%");
            Assert.AreEqual(truth.ToString(), Encoding.ASCII.GetString(result));
        }
    }
}
