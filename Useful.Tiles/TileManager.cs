using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace Useful.Tiles
{
    public class TileManager
    {
        private const int _defaultTileHeight = 5;
        private const int _defaultTileWidth = 40;

        private Mutex _mutex = new Mutex();
        private int _nextColumn = 1;
        private Guid _currentRowId = Guid.Empty;

        public static TileManagerConfig Config() => new TileManagerConfig
        {
            TileWidth = _defaultTileWidth,
            TileHeight = _defaultTileHeight,
            ColumnCount = Console.WindowWidth / _defaultTileWidth,
            MaxWindowHeight = Console.WindowHeight,
            MaxWindowWidth = Console.WindowWidth
        };

        public static TileManagerConfig EmptyConfig() => new TileManagerConfig
        {
            TileWidth = _defaultTileWidth,
            TileHeight = _defaultTileHeight,
            ColumnCount = 2,
            MaxWindowHeight = 120,
            MaxWindowWidth = 80
        };

        public TileManagerConfig TileManagerConfig { get; private set; }

        public ConcurrentDictionary<Guid, List<Tile>> Tiles = new ConcurrentDictionary<Guid, List<Tile>>();

        private TileManager() { }
        internal TileManager(TileManagerConfig tileManagerConfig) => TileManagerConfig = tileManagerConfig;

        public void Add(Tile tile)
        {
            _mutex.WaitOne();

            _currentRowId = _nextColumn > 1 ? _currentRowId : Guid.NewGuid();
            tile.Height = tile.Height > 0 ? tile.Height : _defaultTileHeight;

            Tiles.AddOrUpdate(_currentRowId, new List<Tile> { tile }, (k, v) => v.AddAndReturn(tile));

            var leftPosition = _nextColumn == 1 ? 0 : _nextColumn * TileManagerConfig.TileWidth;
            Console.SetCursorPosition(leftPosition, Console.CursorTop);

            switch (tile)
            {
                case ActionTile actionTile:
                    actionTile.ConsoleDrawAction(leftPosition, 1);
                    break;
                case FormattedStringTile formattedStringTile:
                    Console.Write(formattedStringTile.String);
                    break;
                case StringTile stringTile:
                    Console.Write(stringTile.String.BreakIntoLinesByLength(TileManagerConfig.TileWidth, TileManagerConfig.TileHeight));
                    break;
            }

            _nextColumn = (_nextColumn + 1 > TileManagerConfig.ColumnCount) ? 1 : _nextColumn + 1;

            _mutex.ReleaseMutex();
        }

        private Dictionary<Guid, List<Tile>> VisibleTiles()
        {
            var result = new Dictionary<Guid, List<Tile>>();
            var count = 0;
            foreach (var i in Tiles.Reverse())
            {
                var height = i.Value.Max(t => t.Height);
                if (count + height > TileManagerConfig.MaxWindowHeight)
                {
                    return result;
                }
                else
                {
                    count += height;
                    result.Add(i.Key, i.Value);
                }
            }
            return result;
        }
    }
}
