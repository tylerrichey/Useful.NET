using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Useful.Tiles;

namespace Useful.Tests
{
    [TestClass]
    [DoNotParallelize]
    public partial class TileTests
    {
        public void Nothing() { }

        [TestMethod]
        public void AddTile()
        {
            var tileManager = TileManager.EmptyConfig()
                .Build();
            var itemsToAdd = 4;
            Enumerable.Range(0, itemsToAdd)
                .ToList()
                .ForEach(i =>
                {
                    tileManager.Add(ActionTile.FromAction((l, h) => Nothing()));
                });
            Assert.AreEqual(2, tileManager.Tiles.Count, "Expected rows incorrect");
            Assert.AreEqual(itemsToAdd, tileManager.Tiles.CountValues(), "Expected items incorrect");
        }
    }
}
