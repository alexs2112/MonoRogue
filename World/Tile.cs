using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {
    public class Tile {
        public bool Walkable;
        public Texture2D Glyph;
        public Color Color;

        private Tile(bool walkable, Texture2D glyph, Color color) {
            Walkable = walkable;
            Glyph = glyph;
            Color = color;
        }

        // Essentially an enum that stores the different tiles in the game
        private static List<Tile> DungeonWalls;
        private static List<Tile> DungeonFloors;
        private static List<Tile> CaveWalls;
        private static List<Tile> CaveFloors;
        public static void LoadTiles(ContentManager content) {
            DungeonWalls = new List<Tile>();
            for (int i = 0; i < Constants.NumberOfDungeonWallSprites; i++) {
                DungeonWalls.Add(new Tile(false, content.Load<Texture2D>($"Tiles/Wall{i}"), Color.White));
            }

            DungeonFloors = new List<Tile>();
            for (int i = 0; i < Constants.NumberOfDungeonFloorSprites; i++) {
                DungeonFloors.Add(new Tile(true, content.Load<Texture2D>($"Tiles/Floor{i}"), Color.Gray));
            }

            CaveWalls = new List<Tile>();
            for (int i = 0; i < Constants.NumberOfCaveWallSprites; i++) {
                CaveWalls.Add(new Tile(false, content.Load<Texture2D>($"Tiles/CaveWall{i}"), Color.SaddleBrown));
            }

            CaveFloors = new List<Tile>();
            for (int i = 0; i < Constants.NumberOfCaveFloorSprites; i++) {
                CaveFloors.Add(new Tile(true, content.Load<Texture2D>($"Tiles/CaveFloor{i}"), Color.SandyBrown));
            }
        }

        public static Tile GetDungeonWall(System.Random random) {
            int i = random.Next(DungeonWalls.Count);
            return DungeonWalls[i];
        }
        public static Tile GetDungeonFloor(System.Random random) {
            int i = random.Next(DungeonFloors.Count);
            return DungeonFloors[i];
        }
        public static Tile GetCaveWall(System.Random random) {
            int i = random.Next(CaveWalls.Count);
            return CaveWalls[i];
        }
        public static Tile GetCaveFloor(System.Random random) {
            int i = random.Next(CaveFloors.Count);
            return CaveFloors[i];
        }
    }
}
