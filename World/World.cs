using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoRogue {
    public class World {
        public Tile[,] Tiles { get; private set; }
        public Color[,] ColorOverlay { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Dictionary<Point, Item> Items;

        public List<Creature> Creatures;

        public World(int width, int height,Tile[,] tiles) {
            Width = width;
            Height = height;
            Tiles = tiles;
            Creatures = new List<Creature>();
            Items = new Dictionary<Point, Item>();

            // To account for bloodstains on the floor, possibly some future usage
            ColorOverlay = new Color[width, height];
        }

        public bool InBounds(int x, int y) { return x >= 0 && x < Width && y >= 0 && y < Height; }
        public bool IsFloor(int x, int y) { return InBounds(x, y) && Tiles[x,y].Walkable; }
        public bool IsWall(int x, int y) { return !InBounds(x, y) || !Tiles[x,y].Walkable; }

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
            } while (!IsFloor(x, y) || GetCreatureAt(x, y) != null || GetItemAt(x, y) != null);
            return new Point(x, y);
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

        public void SpawnObjects(System.Random random, CreatureFactory creatureFactory, EquipmentFactory equipmentFactory) {
            for (int i = 0; i < 6; i++) {
                Point tile = GetEmptyFloor(random);
                creatureFactory.NewRat(this, tile.X, tile.Y);
            }
            for (int i = 0; i < 8; i++) {
                Point tile = GetEmptyFloor(random);
                creatureFactory.NewPig(this, tile.X, tile.Y);
            }
            for (int i = 0; i < 5; i++) {
                Point tile = GetEmptyFloor(random);
                creatureFactory.NewFarmer(this, tile.X, tile.Y);
            }
            
            for (int i = 0; i < 5; i++) {
                Point tile = GetEmptyFloor(random);
                Item item = equipmentFactory.RandomWeapon(random);
                Items.Add(tile, item);
            }

            for (int i = 0; i < 4; i++) {
                Point tile = GetEmptyFloor(random);
                Item item = equipmentFactory.RandomArmor(random);
                Items.Add(tile, item);
            }

            for (int i = 0; i < 6; i++) {
                Point tile = GetEmptyFloor(random);
                Food food = Food.RandomFood(random);
                Items.Add(tile, food);
            }
        }

        public void PrintToTerminal() {
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    char c;
                    if (Tiles[x,y].Walkable) { c = '.'; }
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
