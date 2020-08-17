using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using Useful.Extension;
using System.IO;

namespace Useful.Tiles
{
    public class TileManager
    {
        private const int _defaultTileHeight = 5;
        private const int _defaultTileWidth = 40;
        private const char uRowChar = '\u254c';
        private const char uColChar = '\u250a';
        private const char uCorChar = '\u256a';

        private Mutex _mutex = new Mutex();
        private int _nextColumn = 1;
        private Guid _currentRowId = Guid.Empty;
        private ConcurrentDictionary<Guid, List<Tile>> _tiles = new ConcurrentDictionary<Guid, List<Tile>>();

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

        private TileManager() { }
        internal TileManager(TileManagerConfig tileManagerConfig) => TileManagerConfig = tileManagerConfig;

        public Dictionary<Guid, List<Tile>> GetTiles()
        {
            _mutex.WaitOne();
            var snapshot = new Dictionary<Guid, List<Tile>>(_tiles);
            _mutex.ReleaseMutex();
            return snapshot;
        }

        private void ClearWindowLines(int numLines)
        {
            Enumerable
                .Range(0, numLines - 1)
                .ToList()
                .ForEach(i => Console.WriteLine(GetColumnString()));
            Console.WriteLine(GetRoWithCornersString());
        }

        private string GetRoWithCornersString() => GetColumnString().PadRight(System.Console.BufferWidth)
                .Replace(' ', uRowChar)
                .Replace(uColChar, uCorChar);

        private string GetColumnString()
        {
            var columnString = string.Empty;
            Enumerable.Range(1, TileManagerConfig.ColumnCount - 1)
                .ToList()
                .ForEach(i => columnString += "".PadLeft(TileManagerConfig.TileWidth - 1) + uColChar);
            return columnString.PadRight(System.Console.BufferWidth);
        }

        public void Add(Tile tile)
        {
            _mutex.WaitOne();

            _currentRowId = _nextColumn > 1 ? _currentRowId : Guid.NewGuid();
            tile.Height = tile.Height > 0 ? tile.Height : _defaultTileHeight;
            if (_nextColumn == 1)
            {
                ClearWindowLines(tile.Height);
            }

            _tiles.AddOrUpdate(_currentRowId, new List<Tile> { tile }, (k, v) => v.AddAndReturn(tile));

            var leftPosition = _nextColumn == 1 ? 0 : _nextColumn * TileManagerConfig.TileWidth;
            try
            {
                Console.SetCursorPosition(leftPosition, Console.CursorTop);
            }
            catch (IOException)
            {
                //will throw during test runs
            }

            switch (tile)
            {
                case ActionTile actionTile:
                    actionTile.ConsoleDrawAction(leftPosition, 1);
                    break;
                case FormattedStringTile formattedStringTile:
                    Console.Write(formattedStringTile.String);
                    break;
                case StringTile stringTile:
                    foreach (var line in stringTile.String.BreakIntoLinesByLength(TileManagerConfig.TileWidth, TileManagerConfig.TileHeight))
                    {
                        Console.Write(line + '\n');
                    }
                    break;
                default:
                    throw new ApplicationException("Unknown tile type.");
            }

            _nextColumn = (_nextColumn + 1 > TileManagerConfig.ColumnCount) ? 1 : _nextColumn + 1;

            _mutex.ReleaseMutex();
        }

        public Dictionary<Guid, List<Tile>> GetVisibleTiles()
        {
            var result = new Dictionary<Guid, List<Tile>>();
            var count = 0;
            foreach (var i in GetTiles().Reverse())
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
