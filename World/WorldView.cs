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

        // Keep track of all the tiles the player has seen
        public bool[,] HasSeen;

        // A way to cache the glyphs to not have to recalculate everything between key presses
        public WorldView(int widthInTiles, int heightInTiles) {
            Glyphs = new Texture2D[widthInTiles, heightInTiles];
            Colors = new Color[widthInTiles, heightInTiles];
            Width = widthInTiles;
            Height = heightInTiles;

            HasSeen = new bool[Constants.WorldWidth, Constants.WorldHeight];
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
                    int tileX = x + offsetX;
                    int tileY = y + offsetY;

                    Texture2D tile;
                    Color color;
                    bool canSee = player.CanSee(tileX, tileY);

                    if (canSee) {
                        HasSeen[tileX, tileY] = true;
                    } else if (!HasSeen[tileX, tileY]) { 
                        Glyphs[x, y] = null;
                        continue;
                    }

                    if (world.Tiles[tileX, tileY] == 0) { 
                        tile = FloorTexture;
                        color = Color.Gray;
                    } else { 
                        tile = WallTexture;
                        color = Color.White;
                    }

                    if (!canSee) {
                        color = Color.DarkSlateGray;
                    }

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
