using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {
    public class MainInterface {
        // Storing the methods to draw the interface on the right side of the main screen

        public static int InterfaceWidth;
        public static int StartX;

        public static Texture2D InterfaceDivider;
        public static Texture2D InterfaceLine;
        private static Texture2D HeartsFull;
        public static Texture2D TileHighlight;
        private static Texture2D DifficultyBadge;

        // A bunch of creature specific textures
        private static Texture2D FlameGlyph;
        private static Texture2D AttackDelayGlyph;
        private static Texture2D MoveDelayGlyph;
        private static Texture2D UnarmedGlyph;

        // A rectangle used to draw the border to the side
        private static Texture2D BorderRect;

        // Cache messages so we aren't formatting all of the strings hundreds of times per second for no reason
        private List<string> Messages;

        public MainInterface() {
            UpdateScreen();
        }

        public static void UpdateScreen() {
            InterfaceWidth = (Constants.ScreenWidth % 32) + 320;
            StartX = Constants.ScreenWidth - InterfaceWidth;
        }

        public static void LoadTextures(ContentManager content, GraphicsDevice graphics) {
            InterfaceDivider = content.Load<Texture2D>("Interface/InterfaceDivider");
            InterfaceLine = content.Load<Texture2D>("Interface/InterfaceLine");
            HeartsFull = content.Load<Texture2D>("Interface/Hearts");
            TileHighlight = content.Load<Texture2D>("Interface/TileHighlight");
            FlameGlyph = content.Load<Texture2D>("Interface/Flame");
            AttackDelayGlyph = content.Load<Texture2D>("Interface/AttackDelay");
            MoveDelayGlyph = content.Load<Texture2D>("Interface/MoveDelay");
            UnarmedGlyph = content.Load<Texture2D>("Interface/Unarmed");
            DifficultyBadge = content.Load<Texture2D>("Interface/Difficulty");

            BorderRect = new Texture2D(graphics, 1, 1);
            BorderRect.SetData(new[] { Color.White });
        }


        public void DrawInterface(SpriteBatch spriteBatch, Creature player, Creature mouseCreature, Item floorItem, Item mouseItem, Tile tile) {
            DrawBorder(spriteBatch);

            int y = DrawCreatures(spriteBatch, player, mouseCreature);
            y = DrawItems(spriteBatch, floorItem, mouseItem, y);
            if (tile != null) { y = DrawTileHeader(spriteBatch, tile, StartX + 24, y); }
            DrawMessages(spriteBatch, y);
        }

        public static void DrawBorder(SpriteBatch spriteBatch) {
            for (int i = 0; i < (Constants.ScreenHeight + 63) / 64; i++) {
                spriteBatch.Draw(InterfaceDivider, new Vector2(StartX, Constants.ScreenHeight - (i+1) * 64), Color.Gray);
            }
            spriteBatch.Draw(InterfaceLine, new Vector2(StartX + 8, 0), Color.Gray);
            spriteBatch.Draw(BorderRect, new Rectangle(Constants.ScreenWidth - 4, 0, Constants.ScreenWidth, Constants.ScreenHeight), Color.Gray);
            spriteBatch.Draw(InterfaceLine, new Vector2(StartX + 8, Constants.ScreenHeight - 4), Color.Gray);
        }

        public int DrawCreatures(SpriteBatch spriteBatch, Creature player, Creature mouseCreature) {
            int x = StartX + 24;
            int y = 0;
            y = DrawCreatureStats(spriteBatch, player, x, y);

            if (mouseCreature != null && mouseCreature != player) {
                y = DrawCreatureStats(spriteBatch, mouseCreature, x, y);
            } else if (player.AI.LastAttacked != null) {
                y = DrawCreatureStats(spriteBatch, player.AI.LastAttacked, x, y);
            }
            return y;
        }
        public static int DrawCreatureStats(SpriteBatch spriteBatch, Creature creature, int x, int y) {
            // Draw creature header, Icon, Name, Difficulty
            spriteBatch.Draw(InterfaceLine, new Vector2(StartX + 8, y), Color.Gray);
            y += 16;
            y = DrawCreatureHeader(spriteBatch, creature, x, y - 8);

            // Draw creature health and defense
            DrawHearts(spriteBatch, creature.MaxHP, creature.HP, x, y, Color.Red);
            DrawHearts(spriteBatch, creature.GetDefense().Max, creature.GetDefense().Current, x + 32 * ((creature.MaxHP + 3) / 4), y, Color.LightSkyBlue);

            // Write rest tooltip when the player is damaged
            if (creature.IsPlayer && creature.GetDefense().Current < creature.GetDefense().Max) {
                int width = (int)Font.Get(12).MeasureString("[r]").X;
                int restX = x + 32 * ((creature.MaxHP + 3) / 4) + 32 * ((creature.GetDefense().Max + 3) / 4) + 8;
                if (restX + width < Constants.ScreenWidth - 8) {
                    spriteBatch.DrawString(Font.Get(12), "[r]", new Vector2(restX, y + 8), Color.Gray);
                }
            }
            y += 32;

            // Draw creature damage, attack delay, movement delay
            Texture2D weaponGlyph = creature.Weapon != null ? creature.Weapon.Glyph : UnarmedGlyph;
            spriteBatch.Draw(weaponGlyph, new Vector2(x, y), Color.White);
            spriteBatch.DrawString(Font.Get(12), $"{creature.GetDamage().Min}-{creature.GetDamage().Max}", new Vector2(x + 36, y + 8), Color.White);

            int nx = x + 112;
            int delay = creature.GetAttackDelay();
            Color color = delay < 8 ? Color.LightSeaGreen : delay <= 12 ? Color.White : Color.LightSalmon;
            spriteBatch.Draw(AttackDelayGlyph, new Vector2(nx, y), Color.White);
            spriteBatch.DrawString(Font.Get(12), $"{delay}", new Vector2(nx + 36, y + 8), color);

            nx += 96;
            delay = creature.GetMovementDelay();
            color = delay < 8 ? Color.LightSeaGreen : delay <= 12 ? Color.White : Color.LightSalmon;
            spriteBatch.Draw(MoveDelayGlyph, new Vector2(nx, y), Color.White);
            spriteBatch.DrawString(Font.Get(12), $"{delay}", new Vector2(nx + 36, y + 8), color);

            // Draw the final line at the bottom
            y += 36;
            spriteBatch.Draw(InterfaceLine, new Vector2(StartX + 8, y), Color.Gray);
            return y;
        }

        public static int DrawCreatureHeader(SpriteBatch spriteBatch, Creature creature, int x, int y) {
            return DrawCreatureHeader(spriteBatch, creature, x, y, Color.White);
        }
        public static int DrawCreatureHeader(SpriteBatch spriteBatch, Creature creature, int x, int y, Color nameColor) {
            spriteBatch.Draw(creature.Glyph, new Vector2(x, y), creature.Color);
            spriteBatch.DrawString(Font.Get(14), creature.Name, new Vector2(x + 36, y + 8), nameColor);
            if (!creature.IsPlayer) {
                int skullX = x + 32 + (int)(Font.Size14.Width * creature.Name.Length);
                if (creature.Name.Length < 10) { skullX += 16; }
                for (int i = 0; i < creature.Difficulty; i += 2) {
                    spriteBatch.Draw(FlameGlyph, new Vector2(skullX, y), Color.Orange);
                    skullX += 24;
                }
            } else if (creature.HasKey) {
                int keyX = x + 48 + (int)(Font.Size14.Width * creature.Name.Length);
                spriteBatch.Draw(GoldenKey.KeyGlyph, new Vector2(keyX, y), Color.Yellow);
            }
            return y + 32;
        }

        private static Rectangle HeartEmpty = new Rectangle(0, 0, 32, 32);
        private static Rectangle HeartQuarter = new Rectangle(32, 0, 32, 32);
        private static Rectangle HeartHalf = new Rectangle(64, 0, 32, 32);
        private static Rectangle HeartThree = new Rectangle(96, 0, 32, 32);
        private static Rectangle HeartFull = new Rectangle(128, 0, 32, 32);
        private static Rectangle HalfHeartEmpty = new Rectangle(0, 0, 16, 32);
        private static Rectangle HalfHeartHalf = new Rectangle(32, 0, 16, 32);
        private static Rectangle HalfHeartFull = new Rectangle(128, 0, 16, 32);
        public static void DrawHearts(SpriteBatch spriteBatch, int max, int health, int x, int y, Color color) {
            if (max == 0) { return; }

            // Each heart counts as 4 HP
            int fullHearts = health / 4;
            int partialHearts = health % 4;
            int emptyHearts = (max + 3) / 4 - fullHearts;

            // If the last heart to draw is only half a heart, ie max HP is divisible by 2 but not 4
            bool lastIsHalf = max % 4 == 2;

            // Draw each undamaged heart
            for (int i = 0; i < fullHearts; i++) {
                spriteBatch.Draw(HeartsFull, new Vector2(i * 32 + x, y), HeartFull, color);
            }
            x += fullHearts * 32;

            // Draw the partial heart at the end
            if (partialHearts > 0) { 
                emptyHearts -= 1;   // Reduce the number of empty hearts you need to draw to accomodate the partial one
                Vector2 partialVector = new Vector2(x, y);
                Rectangle partialSource;

                if (lastIsHalf && health > max - 2) {
                    if (health == max) {
                        partialSource = HalfHeartFull;
                    } else {
                        partialSource = HalfHeartHalf;
                    }
                } else {
                    if (partialHearts == 1) {
                        partialSource = HeartQuarter;
                    } else if (partialHearts == 2) {
                        partialSource = HeartHalf;
                    } else {
                        partialSource = HeartThree;
                    }
                }
                spriteBatch.Draw(HeartsFull, partialVector, partialSource, color);
                x += 32;
            }

            // Then draw the empty hearts to show capacity
            for (int i = 0; i < emptyHearts - 1; i++) {
                spriteBatch.Draw(HeartsFull, new Vector2(i * 32 + x, y), HeartEmpty, Color.DarkGray);
            }
            if (emptyHearts > 0){
                if (lastIsHalf) {
                    spriteBatch.Draw(HeartsFull, new Vector2((emptyHearts - 1) * 32 + x, y), HalfHeartEmpty, Color.DarkGray);
                } else {
                    spriteBatch.Draw(HeartsFull, new Vector2((emptyHearts - 1) * 32 + x, y), HeartEmpty, Color.DarkGray);
                }
            }
        }

        public static int DrawItems(SpriteBatch spriteBatch, Item floorItem, Item mouseItem, int y) {
            int x = StartX + 24;
            if (floorItem == mouseItem) { mouseItem = null; }
            // Don't need to draw the top bar as there will always be one there from a creature
            if (floorItem != null && mouseItem == null) {
                // Draw the full floor item
                y = DrawItemInfo(spriteBatch, floorItem, x, y + 12);
                spriteBatch.Draw(InterfaceLine, new Vector2(StartX + 8, y), Color.Gray);
            } else if (floorItem != null) {
                y = DrawItemHeader(spriteBatch, floorItem, x, y + 12);
                spriteBatch.Draw(InterfaceLine, new Vector2(StartX + 8, y), Color.Gray);
            }
            if (mouseItem != null) {
                y = DrawItemInfo(spriteBatch, mouseItem, x, y + 12);
                spriteBatch.Draw(InterfaceLine, new Vector2(StartX + 8, y), Color.Gray);
            }
            return y;
        }

        private static int DrawItemInfo(SpriteBatch spriteBatch, Item item, int x, int y) {
            y = DrawItemHeader(spriteBatch, item, x, y);
            x += 8;

            if (item.IsFood) {
                Food food = (Food)item;
                spriteBatch.DrawString(Font.Get(12), $"Food:", new Vector2(x, y + 8), Color.White);
                DrawHearts(spriteBatch, food.Value, food.Value, x + 96, y, Color.Yellow);
                y += 32;
            } else if (item.IsArmor) {
                Armor armor = (Armor)item;
                spriteBatch.DrawString(Font.Get(12), $"Defense:", new Vector2(x, y + 8), Color.White);
                DrawHearts(spriteBatch, armor.MaxDefense, armor.Defense, x + 144, y, Color.LightSkyBlue);
                if (armor.Block > 0) {
                    y += 32;
                    spriteBatch.DrawString(Font.Get(12), $"Block: {armor.Block}", new Vector2(x, y), Color.White);
                    y -= 8;
                }
                y += 32;
                spriteBatch.DrawString(Font.Get(12), $"Weight: {armor.Weight}", new Vector2(x, y), Color.White);
                y += 24;
            } else if (item.IsWeapon) {
                Weapon weapon = (Weapon)item;
                spriteBatch.DrawString(Font.Get(12), $"Type: {weapon.ItemType}", new Vector2(x, y), Color.White);
                y += 24;
                spriteBatch.DrawString(Font.Get(12), $"Damage: {weapon.Damage.Min}-{weapon.Damage.Max}", new Vector2(x, y), Color.White);
                y += 24;
                spriteBatch.DrawString(Font.Get(12), $"Delay: {weapon.Delay}", new Vector2(x, y), Color.White);
                y += 24;
            }
            if (item.ItemInfo != null) {
                foreach(string s in item.ItemInfo) {
                    spriteBatch.DrawString(Font.Get(12), s, new Vector2(x - 8, y), Color.White);
                    y += 24;
                }
            }
            return y;
        }
        private static int DrawItemHeader(SpriteBatch spriteBatch, Item item, int x, int y) {
            int iconX = x + (int)(Font.Size14.Width * item.Name.Length) + 16;
            if (iconX > Constants.ScreenWidth - 40) { iconX = Constants.ScreenWidth - 40; }
            spriteBatch.DrawString(Font.Get(14), item.Name, new Vector2(x, y + 8), Color.White);
            spriteBatch.Draw(item.Glyph, new Vector2(iconX, y), item.Color);
            return y + 36;
        }

        public static int DrawTileHeader(SpriteBatch spriteBatch, Tile tile, int x, int y) {
            if (!tile.IsFeature) { return y; }
            spriteBatch.DrawString(Font.Get(14), ((Feature)tile).Name, new Vector2(x, y + 12), Color.LightGray);
            spriteBatch.Draw(InterfaceLine, new Vector2(StartX + 8, y + 40), Color.Gray);
            return y + 40;
        }

        public static void DrawTileHighlight(SpriteBatch spriteBatch, MouseHandler mouse, WorldView world, Color color) {
            Point p = mouse.GetViewTile(world);
            if (p.X == -1) { return; }

            p.X += world.OffsetX; p.Y += world.OffsetY;
            if (!world.InBounds(p)) { return; }
            if (!world.HasSeen[p.X, p.Y]) { return; }

            spriteBatch.Draw(TileHighlight, new Vector2((p.X - world.OffsetX) * 32, (p.Y - world.OffsetY) * 32), color);
        }

        public static void DrawLineToCreature(SpriteBatch spriteBatch, MouseHandler mouse, WorldView world, Creature start, Creature end, Color color) {
            Point p = mouse.GetViewTile(world);
            Point tile = new Point(p.X + world.OffsetX, p.Y + world.OffsetY);
            if (p.X == -1 || p.X + world.OffsetX >= Constants.WorldWidth ||
                p.Y + world.OffsetY >= Constants.WorldHeight || !world.HasSeen[tile.X, tile.Y]) { return; }

            List<Point> line = start.GetLineToPoint(new Point(end.X, end.Y));
            foreach (Point point in line) {
                spriteBatch.Draw(TileHighlight, new Vector2((point.X - world.OffsetX) * 32, (point.Y - world.OffsetY) * 32), color);
            }
        }

        public static void DrawRelativeCreatureDiffulty(SpriteBatch spriteBatch, WorldView wv, Creature player, World world) {
            int strength = 0;
            if (player.Weapon != null) { strength += player.Weapon.Strength; }
            if (player.Armor != null) { strength += player.Armor.Strength; }

            for (int x = 0; x < wv.Width; x++) {
                for (int y = 0; y < wv.Height; y++) {
                    Point tile = new Point(x + wv.OffsetX, y + wv.OffsetY);
                    if (!player.CanSee(tile)) { continue; }
                    Creature c = world.GetCreatureAt(tile);
                    if (c == null || c.IsPlayer) { continue; }

                    Color color = Color.White;
                    if (c.Difficulty == strength + 2) { color = Color.Yellow; }
                    if (c.Difficulty >= strength + 3) { color = Color.Red; }
                    if (color != Color.White) {
                        spriteBatch.Draw(DifficultyBadge, new Vector2(x * 32, y * 32), color);
                    }
                }
            }
        }

        public void DrawMessages(SpriteBatch spriteBatch, int maxY) {
            if (Messages == null || Messages.Count == 0) { return; }

            int lineHeight = 18;
            int y = Constants.ScreenHeight - Messages.Count * lineHeight - 8;
            foreach (string m in Messages) {
                if (y > maxY + 4) {
                    spriteBatch.DrawString(Font.Get(10), m, new Vector2(StartX + 24, y), Color.LightGray);
                }
                y += lineHeight;
            }
        }

        public void UpdateMessages(List<string> messages) {
            Messages = new List<string>();
            foreach (string m in messages) {
                if (m.Length * Font.Size10.Width > InterfaceWidth - 40) {
                    Messages.AddRange(Font.Size10.SplitString(m, InterfaceWidth - 40));
                } else {
                    Messages.Add(m);
                }
            }
        }
    }
}
