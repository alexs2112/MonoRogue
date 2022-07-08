using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoRogue {
    public class World {
        public Tile[,] Tiles;
        public bool[,] Bloodstains;
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Dictionary<Point, Item> Items;

        public Creature Player;
        public List<Creature> Creatures;

        public List<Projectile> Projectiles;

        public Point Exit;

        // How hard this given tile should be displayed, calculated by region and depth
        public int[,] Difficulty;

        public World(int width, int height,Tile[,] tiles) {
            Width = width;
            Height = height;
            Tiles = tiles;
            Creatures = new List<Creature>();
            Items = new Dictionary<Point, Item>();
            Projectiles = new List<Projectile>();
            Bloodstains = new bool[width, height];
        }

        public bool InBounds(Point p) { return InBounds(p.X, p.Y); }
        public bool InBounds(int x, int y) { return x >= 0 && x < Width && y >= 0 && y < Height; }
        public bool IsFloor(Point p) { return IsFloor(p.X, p.Y); }
        public bool IsFloor(int x, int y) { return InBounds(x, y) && Tiles[x,y].Walkable; }
        public bool IsWall(Point p) { return IsWall(p.X, p.Y); }
        public bool IsWall(int x, int y) { return !InBounds(x, y) || !Tiles[x,y].Walkable; }
        public bool BlockSight(Point p) { return BlockSight(p.X, p.Y); }
        public bool BlockSight(int x, int y) { return !Tiles[x,y].SeeThrough; }
        public bool IsDoor(Point p) { return IsDoor(p.X, p.Y); }
        public bool IsDoor(int x, int y) { return Tiles[x,y].Breakable; }

        public Point GetRandomFloor(System.Random random) {
            int x;
            int y;

            do {
                x = random.Next(Width);
                y = random.Next(Height);
            } while (!IsFloor(x, y) || GetCreatureAt(x, y) != null || GetItemAt(x, y) != null);
            return new Point(x, y);
        }
        
        public Tile GetTile(Point p) { return GetTile(p.X, p.Y); }
        public Tile GetTile(int x, int y) {
            if (x == -1) { return null; }
            return Tiles[x,y];
        }

        public Creature GetCreatureAt(Point p) { return GetCreatureAt(p.X, p.Y); }
        public Creature GetCreatureAt(int x, int y) {
            foreach (Creature c in Creatures) {
                if (c.X == x && c.Y == y) { return c; }
            }
            return null;
        }

        public Item GetItemAt(int x, int y) { return GetItemAt(new Point(x, y)); }
        public Item GetItemAt(Point p) {
            if (Items.ContainsKey(p)) {
                return Items[p];
            }
            return null;
        }
        public void EatFoodAt(Creature c, int x, int y) { EatFoodAt(c, new Point(x, y)); }
        public void EatFoodAt(Creature c, Point p) {
            Item item = GetItemAt(p);
            if (item == null) { return; }
            
            if (!item.IsFood) { return; }
            Food food = (Food)item;
            if (food.Eat(c)) { Items.Remove(p); }
        }

        public void OpenDoor(int x, int y) {
            if (Tiles[x,y].Breakable) {
                Tiles[x,y] = Feature.GetOpenDoor();
            }
        }

        // Each alive creature takes their turn, each dead creature is removed from creatures once everyone is done
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

        public void UpdateProjectiles(System.TimeSpan elapsed, MainInterface mainInterface, WorldView worldView) {
            if (!Constants.AllowAnimations) { return; }

            bool updateDisplay = false;
            for (int i = 0; i < Projectiles.Count; i++) {
                Projectile p = Projectiles[i];
                p.Update(elapsed);
                if (p.Finished) {
                    Projectiles.RemoveAt(i);
                    i--;
                    updateDisplay = true;
                }
            }
            if (updateDisplay) {
                mainInterface.UpdateMessages(Player.AI.GetMessages());
                worldView.Update(this, Player);
            }
        }

        public void EndProjectiles() {
            foreach (Projectile p in Projectiles) {
                p.EndEarly();
            }
            Projectiles.Clear();
        }

        public void PrintToTerminal() {
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    char c;
                    if (Tiles[x,y].Walkable) { c = '.'; }
                    else if (Tiles[x,y].Breakable) { c = '+'; }
                    else { c = '#'; }
                    System.Console.Write(c);
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
