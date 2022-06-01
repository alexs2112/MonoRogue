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

        // Bresenham's Line Algorithm: https://en.wikipedia.org/wiki/Bresenham's_line_algorithm
        public static List<Point> GetLine(int sx, int sy, int dx, int dy) {
            return GetLine(new Point(sx, sy), new Point(dx, dy));
        }
        public static List<Point> GetLine(Point source, Point dest) {
            List<Point> points = new List<Point>();

            int sx = source.X < dest.X ? 1 : -1;
            int sy = source.Y < dest.Y ? 1 : -1;
            int dx = System.Math.Abs(dest.X - source.X);
            int dy = System.Math.Abs(dest.Y - source.Y);
            int err = dx - dy;

            while (true) {
                points.Add(source);
                if (source == dest) { break; }

                int e2 = err * 2;
                if (e2 > -dy) {
                    err -= dy;
                    source.X += sx;
                } 
                if (e2 < dx) {
                    err += dx;
                    source.Y += sy;
                }
            }
            return points;
        }
    }
}
