using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoRogue {
    public class WorldView {
        public Texture2D[,] Glyphs;
        public Color[,] Colors;
        public int width;
        public int height;

        private Texture2D wallTexture;
        private Texture2D floorTexture;

        // A way to cache the glyphs to not have to recalculate everything between key presses
        public WorldView(int widthInTiles, int heightInTiles) {
            Glyphs = new Texture2D[widthInTiles, heightInTiles];
            Colors = new Color[widthInTiles, heightInTiles];
            width = widthInTiles;
            height = heightInTiles;
        }

        public void LoadContent(ContentManager content) {
            wallTexture = content.Load<Texture2D>("Tiles/Wall");
            floorTexture = content.Load<Texture2D>("Tiles/Floor");
        }

        public void Update(World world, Creature player) {
            // First draw the grid of tiles in the world
            for (int x = 0; x < world.Width; x++) {
                for (int y = 0; y < world.Height; y++) {
                    Texture2D tile;
                    Color color;
                    if (world.Tiles[x,y] == 0) { tile = floorTexture; color = Color.Gray; }
                    else { tile = wallTexture; color = Color.White; }

                    Glyphs[x,y] = tile;
                    Colors[x,y] = color;
                }
            }

            // Then overwrite tiles with creatures
            int px = player.X; int py = player.Y;
            Glyphs[px, py] = player.Glyph;
            Colors[px, py] = player.Color;
        }
    }
}
