using static System.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoRogue {
    public class WorldView {
        public Texture2D[,] Glyphs;
        public Color[,] Colors;
        public int Width;
        public int Height;

        private Texture2D WallTexture;
        private Texture2D FloorTexture;

        // A way to cache the glyphs to not have to recalculate everything between key presses
        public WorldView(int widthInTiles, int heightInTiles) {
            Glyphs = new Texture2D[widthInTiles, heightInTiles];
            Colors = new Color[widthInTiles, heightInTiles];
            Width = widthInTiles;
            Height = heightInTiles;
        }

        public void LoadContent(ContentManager content) {
            WallTexture = content.Load<Texture2D>("Tiles/Wall");
            FloorTexture = content.Load<Texture2D>("Tiles/Floor");
        }

        public void Update(World world, Creature player) {
            // First prepare the grid of tiles in the world
            int offsetX = Max(0, Min(player.X - Width / 2, world.Width - Width));
            int offsetY = Max(0, Min(player.Y - Height / 2, world.Height - Height));

            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    Texture2D tile;
                    Color color;
                    if (world.Tiles[x + offsetX, y + offsetY] == 0) { tile = FloorTexture; color = Color.Gray; }
                    else { tile = WallTexture; color = Color.White; }

                    Glyphs[x,y] = tile;
                    Colors[x,y] = color;
                }
            }

            // Then overwrite tiles with creatures
            int px = player.X; int py = player.Y;
            Glyphs[px - offsetX, py - offsetY] = player.Glyph;
            Colors[px - offsetX, py - offsetY] = player.Color;
        }
    }
}
