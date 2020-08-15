using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace Useful.Tiles
{
    public abstract class Tile 
    {
        public Guid Id { get; protected set; }
        public int Height { get; internal set; }
    }

    public class ActionTile : Tile
    {
        public Action<int, int> ConsoleDrawAction { get; private set; }

        /// <summary>
        /// A tile that provides an action that draws directly on the console. NOTE: You could theoretically write anywhere on the screen with this method.
        /// </summary>
        /// <param name="action">An action that takes two int parameters; the coordinates of the top left corner of the tile from the buffer.</param>
        /// <returns></returns>
        public static ActionTile FromAction(Action<int, int> action) => new ActionTile
        {
            Id = Guid.NewGuid(),
            ConsoleDrawAction = action
        };
    }

    public class StringTile : Tile
    {
        public string String { get; protected set; }

        public static StringTile FromString(string input) => new StringTile
        {
            String = input
        };
    }

    public class FormattedStringTile : StringTile 
    {
        public static FormattedStringTile FromFormattedString(string input) => new FormattedStringTile
        {
            String = input
        };
    }
}
