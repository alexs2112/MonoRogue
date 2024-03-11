using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {
    public class Tile {
        public bool Walkable;
        public bool SeeThrough;
        public bool CanOpen;
        public bool IsFeature;
        public Texture2D Glyph;
        public Color Color;

        protected Tile(bool walkable, bool seeThrough,Texture2D glyph, Color color) {
            Walkable = walkable;
            SeeThrough = seeThrough;
            Glyph = glyph;
            Color = color;
        }

        // Essentially an enum that stores the different tiles in the game
        private static List<Tile> DungeonWalls;
        private static List<Tile> DungeonFloors;
        private static List<Tile> CaveWalls;
        private static List<Tile> CaveFloors;
        private static Tile WallCrumbledTopRight;
        private static Tile WallCrumbledTopLeft;
        private static Tile WallCrumbledBotRight;
        private static Tile WallCrumbledBotLeft;

        public static void LoadTiles(ContentManager content) {
            DungeonWalls = new List<Tile>();
            for (int i = 0; i < Constants.NumberOfDungeonWallSprites; i++) {
                DungeonWalls.Add(new Tile(false, false, content.Load<Texture2D>($"Tiles/Wall{i}"), Color.White));
            }

            DungeonFloors = new List<Tile>();
            for (int i = 0; i < Constants.NumberOfDungeonFloorSprites; i++) {
                DungeonFloors.Add(new Tile(true, true, content.Load<Texture2D>($"Tiles/Floor{i}"), Color.LightGray));
            }

            CaveWalls = new List<Tile>();
            for (int i = 0; i < Constants.NumberOfCaveWallSprites; i++) {
                CaveWalls.Add(new Tile(false, false, content.Load<Texture2D>($"Tiles/CaveWall{i}"), Color.SaddleBrown));
            }

            CaveFloors = new List<Tile>();
            for (int i = 0; i < Constants.NumberOfCaveFloorSprites; i++) {
                CaveFloors.Add(new Tile(true, true, content.Load<Texture2D>($"Tiles/CaveFloor{i}"), Color.Tan));
            }

            WallCrumbledTopRight = new Tile(false, false, content.Load<Texture2D>("Tiles/CrumbledWall0"), Color.White);
            WallCrumbledTopLeft = new Tile(false, false, content.Load<Texture2D>("Tiles/CrumbledWall1"), Color.White);
            WallCrumbledBotRight = new Tile(false, false, content.Load<Texture2D>("Tiles/CrumbledWall2"), Color.White);
            WallCrumbledBotLeft = new Tile(false, false, content.Load<Texture2D>("Tiles/CrumbledWall3"), Color.White);
        }

        public static Tile GetDungeonWall() { return GetDungeonWall(new System.Random()); }
        public static Tile GetDungeonWall(System.Random random) {
            int i = random.Next(DungeonWalls.Count);
            return DungeonWalls[i];
        }
        public static Tile GetDungeonFloor() { return GetDungeonFloor(new System.Random()); }
        public static Tile GetDungeonFloor(System.Random random) {
            int i = random.Next(DungeonFloors.Count);
            return DungeonFloors[i];
        }
        public static Tile GetCaveWall() { return GetCaveWall(new System.Random()); }
        public static Tile GetCaveWall(System.Random random) {
            int i = random.Next(CaveWalls.Count);
            return CaveWalls[i];
        }
        public static Tile GetCaveFloor() { return GetCaveFloor(new System.Random()); }
        public static Tile GetCaveFloor(System.Random random) {
            int i = random.Next(CaveFloors.Count);
            return CaveFloors[i];
        }

        public static Tile GetCrumbledWall(World world, bool[,] isDungeon, Point p) {
            if (world.IsWall(p.X, p.Y + 1)) {
                if (!isDungeon[p.X + 1, p.Y]) { return WallCrumbledTopRight; }
                else { return WallCrumbledTopLeft; }
            } else if (world.IsWall(p.X, p.Y - 1)) {
                if (!isDungeon[p.X + 1, p.Y]) { return WallCrumbledBotRight; }
                else { return WallCrumbledBotLeft; }
            } else if (world.IsWall(p.X + 1, p.Y)) {
                if (!isDungeon[p.X, p.Y + 1]) { return WallCrumbledBotLeft; }
                else { return WallCrumbledTopLeft; }
            } else {
                if (!isDungeon[p.X, p.Y - 1]) { return WallCrumbledTopRight; }
                else { return WallCrumbledBotRight; }
            }
        }
    }
}
