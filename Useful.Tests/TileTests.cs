using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Useful.Tiles;
using Useful.Extension;

namespace Useful.Tests
{
    [TestClass]
    [DoNotParallelize]
    public partial class TileTests
    {
        public void Nothing() { }

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

        [TestMethod]
        public void AddTile()
        {
            Console.SetIn(new StringReader(""));
            Console.SetOut(new StringWriter());
            var tileManager = TileManager.EmptyConfig()
                .Build();
            var itemsToAdd = 4;
            Enumerable.Range(0, itemsToAdd)
                .ToList()
                .ForEach(i =>
                {
                    tileManager.Add(ActionTile.FromAction((l, h) => Nothing()));
                });
            var tiles = tileManager.GetTiles();
            Assert.AreEqual(2, tiles.Count, "Expected rows incorrect");
            Assert.AreEqual(itemsToAdd, tiles.CountValues(), "Expected items incorrect");
        }
    }
}
