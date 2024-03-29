using static System.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoRogue {
    public class WorldView {
        public Texture2D[,] Glyphs;
        public Color[,] Colors;
        public int Width;
        public int Height;

        public int OffsetX;
        public int OffsetY;

        // Keep track of all the tiles the player has seen
        public bool[,] HasSeen;

        // A way to cache the glyphs to not have to recalculate everything between key presses
        public WorldView() {
            UpdateScreenSize();

            HasSeen = new bool[Constants.WorldWidth, Constants.WorldHeight];
        }

        public void Update(World world, Creature player) {
            Update(world, player, player.X, player.Y);
        }
        public void Update(World world, Creature player, int sx, int sy) {
            // First prepare the grid of tiles in the world
            OffsetX = sx - Width / 2;
            OffsetY = sy - Height / 2;

            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    int tileX = x + OffsetX;
                    int tileY = y + OffsetY;
                    Point point = new Point(tileX, tileY);

                    if (!world.InBounds(point)) {
                        Glyphs[x, y] = null;
                        continue;
                    }

                    bool canSee = player.CanSee(tileX, tileY);
                    Texture2D tile = world.Tiles[tileX, tileY].Glyph;
                    Color color = world.Tiles[tileX, tileY].Color;

                    if (world.Items.ContainsKey(point)) { tile = world.Items[point].Glyph; }

                    if (canSee) {
                        HasSeen[tileX, tileY] = true;
                        if (world.Items.ContainsKey(point)) {
                            color = world.Items[point].Color;
                        } else {
                            if (world.Bloodstains[tileX, tileY]) {
                                color = Color.DarkRed;
                            }
                        }
                    } else if (!HasSeen[tileX, tileY]) { 
                        // Overwrite the tile with null if we have never seen it
                        Glyphs[x, y] = null;
                        continue;
                    } else {
                        color = Color.DarkSlateGray;
                    }

                    Glyphs[x,y] = tile;
                    Colors[x,y] = color;
                }
            }

            // Overwrite the tiles that a creature is standing on
            foreach (Creature c in world.Creatures) {
                if (c.X >= OffsetX && c.X < Width + OffsetX && c.Y >= OffsetY && c.Y < Height + OffsetY) {
                    if (player.CanSee(c.X, c.Y)) {
                        Glyphs[c.X - OffsetX, c.Y - OffsetY] = c.Glyph;
                        Colors[c.X - OffsetX, c.Y - OffsetY] = c.Color;
                    }
                }
            }
        }

        public bool InBounds(Point p) { return InBounds(p.X, p.Y); }
        public bool InBounds(int x, int y) { return x >= 0 && x < Constants.WorldWidth && y >= 0 && y < Constants.WorldHeight; }

        public void Draw(SpriteBatch spriteBatch) {
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    if (Glyphs[x,y] == null) { continue; }
                    spriteBatch.Draw(Glyphs[x,y], new Vector2(x * 32, y * 32), Colors[x,y]);
                }
            }
        }

        public void UpdateScreenSize() {
            Width = (Constants.ScreenWidth - MainInterface.InterfaceWidth) / 32;
            Height = (Constants.ScreenHeight) / 32;
            Glyphs = new Texture2D[Width, Height];
            Colors = new Color[Width, Height];
        }
    }
}
