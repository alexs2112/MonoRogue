using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoRogue {
    public class World {
        public int[,] Tiles { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public World(int width, int height, int[,] tiles) {
            Width = width;
            Height = height;
            Tiles = tiles;
        }

        public bool InBounds(int x, int y) { return x >= 0 && x < Width && y >= 0 && y < Height; }
        public bool IsFloor(int x, int y) { return InBounds(x, y) && Tiles[x,y] == 0; }
        public bool IsWall(int x, int y) { return !InBounds(x, y) || Tiles[x,y] == 1; }

        public Point GetStartTile() {
            // For now, just return the most northwest floor tile
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    if (Tiles[x, y] == 0) { return new Point(x, y); }
                }
            }

            throw new System.Exception("Could not find a starting tile for the player!");
        }

        public void PrintToTerminal() {
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    System.Console.Write(Tiles[x,y]);
                }
                System.Console.Write("\n");
            }
        }
    }
}
