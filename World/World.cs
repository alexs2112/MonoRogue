using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoRogue {
    public class World {
        public int[,] Tiles { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public List<Creature> Creatures;

        public World(int width, int height, int[,] tiles) {
            Width = width;
            Height = height;
            Tiles = tiles;
            Creatures = new List<Creature>();
        }

        public bool InBounds(int x, int y) { return x >= 0 && x < Width && y >= 0 && y < Height; }
        public bool IsFloor(int x, int y) { return InBounds(x, y) && Tiles[x,y] == 0; }
        public bool IsWall(int x, int y) { return !InBounds(x, y) || Tiles[x,y] == 1; }

        public Point GetRandomFloor(System.Random random) {
            int x;
            int y;

            do {
                x = random.Next(Width);
                y = random.Next(Height);
            } while (!IsFloor(x, y));
            return new Point(x, y);
        }
        public Point GetEmptyFloor(System.Random random) {
            int x;
            int y;

            do {
                x = random.Next(Width);
                y = random.Next(Height);
            } while (!IsFloor(x, y) || GetCreatureAt(x, y) != null);
            return new Point(x, y);
        }

        public Creature GetCreatureAt(int x, int y) {
            foreach (Creature c in Creatures) {
                if (c.X == x && c.Y == y) { return c; }
            }
            return null;
        }

        // Each alive creature takes their turn, each dead creature is removed from creatures oncec everone is done
        public void TakeTurns() {
            List<Creature> dead = new List<Creature>();
            foreach (Creature c in Creatures) {
                if (c.IsDead()) { dead.Add(c); continue; }
                c.TakeTurn(this);
            }
            foreach(Creature c in dead) {
                Creatures.Remove(c);
            }
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
