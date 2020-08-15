using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace Useful.Tiles
{

    public class TileManagerConfig
    {
        internal TileManagerConfig() { }

        public int TileWidth { get; internal set; }
        public int TileHeight { get; internal set; }
        public int ColumnCount { get; internal set; }
        public int MaxWindowHeight { get; internal set; }
        public int MaxWindowWidth { get; internal set; }
    }
}
