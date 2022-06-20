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

        public int OffsetX;
        public int OffsetY;

        // Keep track of all the tiles the player has seen
        public bool[,] HasSeen;

        private static Texture2D ExitLocked;
        private static Texture2D ExitOpen;

        // A way to cache the glyphs to not have to recalculate everything between key presses
        public WorldView(int widthInTiles, int heightInTiles) {
            Glyphs = new Texture2D[widthInTiles, heightInTiles];
            Colors = new Color[widthInTiles, heightInTiles];
            Width = widthInTiles;
            Height = heightInTiles;

            HasSeen = new bool[Constants.WorldWidth, Constants.WorldHeight];
        }

        public static void LoadContent(ContentManager content) {
            ExitLocked = content.Load<Texture2D>("Misc/ExitLocked");
            ExitOpen = content.Load<Texture2D>("Misc/ExitOpen");
        }

        public void Update(World world, Creature player) {
            // First prepare the grid of tiles in the world
            OffsetX = Max(0, Min(player.X - Width / 2, world.Width - Width));
            OffsetY = Max(0, Min(player.Y - Height / 2, world.Height - Height));

            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    int tileX = x + OffsetX;
                    int tileY = y + OffsetY;
                    Point point = new Point(tileX, tileY);

                    bool canSee = player.CanSee(tileX, tileY);
                    Texture2D tile = world.Tiles[tileX, tileY].Glyph;
                    Color color = world.Tiles[tileX, tileY].Color;

                    if (canSee) {
                        HasSeen[tileX, tileY] = true;
                        if (world.Items.ContainsKey(point)) {
                            tile = world.Items[point].Glyph;
                            color = world.Items[point].Color;
                        } else {
                            if (world.ColorOverlay[tileX, tileY] != Color.Transparent) {
                                color = world.ColorOverlay[tileX, tileY];
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

            // If the player is in range of the exit, draw it
            if (HasSeen[world.Exit.X, world.Exit.Y] && 
                world.Exit.X >= OffsetX && world.Exit.X < Width + OffsetX && world.Exit.Y >= OffsetY && world.Exit.Y < Height + OffsetY) {
                    Glyphs[world.Exit.X - OffsetX, world.Exit.Y - OffsetY] = player.HasKey ? ExitOpen : ExitLocked;
                    Colors[world.Exit.X - OffsetX, world.Exit.Y - OffsetY] = player.HasKey ? Color.SkyBlue : Color.Gold;
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

        public void Draw(SpriteBatch spriteBatch) {
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    if (Glyphs[x,y] == null) { continue; }
                    spriteBatch.Draw(Glyphs[x,y], new Vector2(x * 32, y * 32), Colors[x,y]);
                }
            }
        }
    }
}
