using System;
using Useful.Tiles;

namespace Useful.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            var tileManager = TileManager.Config()
                .Build();

            tileManager.Add(StringTile.FromString("hello"));
            tileManager.Add(StringTile.FromString("hello2"));
            tileManager.Add(StringTile.FromString("hello3"));


            Console.ReadKey();
        }
    }
}
