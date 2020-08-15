using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace Useful.Tiles
{

    public static class TileManagerConfigExtentions
    {
        public static TileManager Build(this TileManagerConfig tileManagerConfig) => new TileManager(tileManagerConfig);

        public static TileManagerConfig SetTileWidth(this TileManagerConfig tileManagerConfig, int tileWidth)
        {
            tileManagerConfig.TileWidth = tileWidth;
            return tileManagerConfig;
        }

        public static TileManagerConfig SetTileHeight(this TileManagerConfig tileManagerConfig, int tileHeight)
        {
            tileManagerConfig.TileHeight = tileHeight;
            return tileManagerConfig;
        }

        public static TileManagerConfig SetColumnCount(this TileManagerConfig tileManagerConfig, int columnCount)
        {
            tileManagerConfig.ColumnCount = columnCount;
            return tileManagerConfig;
        }

        public static TileManagerConfig SetMaxWindowHeight(this TileManagerConfig tileManagerConfig, int maxWindowHeight)
        {
            tileManagerConfig.MaxWindowHeight = maxWindowHeight;
            return tileManagerConfig;
        }

        public static TileManagerConfig SetMaxWindowWidth(this TileManagerConfig tileManagerConfig, int maxWindowWidth)
        {
            tileManagerConfig.MaxWindowWidth = maxWindowWidth;
            return tileManagerConfig;
        }
    }
}
