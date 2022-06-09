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
        private static List<Tile> Walls;
        private static List<Tile> Floors;
        public static void LoadTiles(ContentManager content) {
            Walls = new List<Tile>();
            for (int i = 0; i < Constants.NumberOfWallSprites; i++) {
                Walls.Add(new Tile(false, content.Load<Texture2D>($"Tiles/Wall{i}"), Color.White));
            }

            Floors = new List<Tile>();
            for (int i = 0; i < Constants.NumberOfFloorSprites; i++) {
                Floors.Add(new Tile(true, content.Load<Texture2D>($"Tiles/Floor{i}"), Color.Gray));
            }
        }

        public static Tile GetWall(System.Random random) {
            int i = random.Next(Walls.Count);
            return Walls[i];
        }
        public static Tile GetFloor(System.Random random) {
            int i = random.Next(Floors.Count);
            return Floors[i];
        }
    }
}
