using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {
    public class MainInterface {
        // Storing the methods to draw the interface on the right side of the main screen
        
        // Where the interface starts from on the screen
        public static int StartX = 32 * Constants.WorldViewWidth;

        // How many characters fit per message line
        private static int MessageLineLength = 20;

        private static Texture2D InterfaceDivider;
        private static Texture2D InterfaceLine;
        private static Texture2D HeartsFull;
        private static Texture2D TileHighlight;
        private static Texture2D MousePointer;
        private static SpriteFont Font14;
        private static SpriteFont Font12;
        private static SpriteFont Font10;

        // Cache messages so we aren't formatting all of the strings hundreds of times per second for no reason
        private List<string> Messages;

        public static void LoadTextures(ContentManager content) {
            InterfaceDivider = content.Load<Texture2D>("Interface/InterfaceDivider");
            InterfaceLine = content.Load<Texture2D>("Interface/InterfaceLine");
            HeartsFull = content.Load<Texture2D>("Interface/Hearts");
            TileHighlight = content.Load<Texture2D>("Interface/TileHighlight");
            MousePointer = content.Load<Texture2D>("Interface/MousePointer");

            Font14 = content.Load<SpriteFont>("Interface/sds14");
            Font12 = content.Load<SpriteFont>("Interface/sds12");
            Font10 = content.Load<SpriteFont>("Interface/sds10");
        }


        public void DrawInterface(SpriteBatch spriteBatch) {
            spriteBatch.Draw(InterfaceDivider, new Vector2(StartX, 0), Color.Gray);
        }

        public void DrawCreatureStats(SpriteBatch spriteBatch, Creature creature) { DrawCreatureStats(spriteBatch, creature, null); }
        public void DrawCreatureStats(SpriteBatch spriteBatch, Creature creature, Creature player) {
            int x = StartX + 24;
            int y = 8;
            spriteBatch.DrawString(Font14, creature.Name, new Vector2(x, y), Color.White);
            if (creature.IsPlayer && creature.HasKey) {
                spriteBatch.Draw(GoldenKey.KeyGlyph, new Vector2(x, Constants.ScreenWidth - 48), Color.Yellow);
            }

            y += 24;
            DrawHearts(spriteBatch, creature.MaxHP, creature.HP, x, y, Color.Red);
            y += 32;
            if (creature.GetDefense().Max == 0) {
                spriteBatch.DrawString(Font14, "No armor", new Vector2(x, y + 8), Color.Gray);
            } else {
                DrawHearts(spriteBatch, creature.GetDefense().Max, creature.GetDefense().Current, x, y, Color.LightSkyBlue);
            }

            if (player != null) {
                y = 8;
                int width = (int)Font14.MeasureString(player.Name).X;
                int playerX = Constants.ScreenWidth - 16;
                spriteBatch.DrawString(Font14, player.Name, new Vector2(playerX - width, y), Color.White);
                y += 24;
                DrawHearts(spriteBatch, player.MaxHP, player.HP, playerX - 32 * ((player.MaxHP + 3) / 4), y, Color.Red);
                y += 32;
                if (player.GetDefense().Max > 0) {
                    DrawHearts(spriteBatch, player.GetDefense().Max, player.GetDefense().Current, playerX - 32 * ((player.GetDefense().Max + 3) / 4), y, Color.LightSkyBlue);
                }
            }

            y += 40;
            (int Min, int Max) damage = creature.GetDamage();
            spriteBatch.DrawString(Font14, $"Damage: {damage.Min}-{damage.Max}", new Vector2(x, y), Color.White);
            y += 32;

            string s;
            Color c;
            if (creature.Weapon == null) { s = "Unarmed"; c = Color.Gray; }
            else { s = creature.Weapon.Name; c = Color.White; }
            spriteBatch.DrawString(Font14, s, new Vector2(x, y), c);
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

        public void DrawItems(SpriteBatch spriteBatch, Item floorItem, Item mouseItem) {
            int x = StartX + 24;
            int y = 184;

            if (floorItem == mouseItem) { mouseItem = null; }
            if (floorItem != null && mouseItem == null) {
                // Draw the full floor item
                spriteBatch.Draw(InterfaceLine, new Vector2(StartX + 8, y - 16), Color.Gray);
                y = DrawItemInfo(spriteBatch, floorItem, x, y);
                spriteBatch.Draw(InterfaceLine, new Vector2(StartX + 8, y), Color.Gray);
            } else if (floorItem != null) {
                spriteBatch.Draw(InterfaceLine, new Vector2(StartX + 8, y - 16), Color.Gray);
                y = DrawItemHeader(spriteBatch, floorItem, x, y, false);
                spriteBatch.Draw(InterfaceLine, new Vector2(StartX + 8, y), Color.Gray);
                y += 16;
            }
            if (mouseItem != null) {
                spriteBatch.Draw(InterfaceLine, new Vector2(StartX + 8, y - 16), Color.Gray);
                y = DrawItemInfo(spriteBatch, mouseItem, x, y, true);
                spriteBatch.Draw(InterfaceLine, new Vector2(StartX + 8, y), Color.Gray);
            }
        }

        public int DrawItemInfo(SpriteBatch spriteBatch, Item item, int x, int y) { return DrawItemInfo(spriteBatch, item, x, y, false); }
        private int DrawItemInfo(SpriteBatch spriteBatch, Item item, int x, int y, bool mousePointer) {
            y = DrawItemHeader(spriteBatch, item, x, y, mousePointer);

            if (item.IsFood) {
                Food food = (Food)item;
                spriteBatch.DrawString(Font12, $"Food:", new Vector2(x, y), Color.White);
                DrawHearts(spriteBatch, food.Value, food.Value, x + 96, y - 8, Color.Yellow);
            } else if (item.IsArmor) {
                Armor armor = (Armor)item;
                spriteBatch.DrawString(Font12, $"Defense:", new Vector2(x, y), Color.White);
                DrawHearts(spriteBatch, armor.Defense, armor.MaxDefense, x + 144, y - 8, Color.LightSkyBlue);
                y += 24;
                spriteBatch.DrawString(Font12, $"Weight: {armor.Weight}", new Vector2(x, y), Color.White);
            } else if (item.IsWeapon) {
                Weapon weapon = (Weapon)item;
                spriteBatch.DrawString(Font12, $"Damage: {weapon.Damage.Min}-{weapon.Damage.Max}", new Vector2(x, y), Color.White);
                y += 24;
                spriteBatch.DrawString(Font12, $"Delay: {weapon.Delay}", new Vector2(x, y), Color.White);
            }
            if (item.ItemInfo != null) {
                y -= 24;
                foreach(string s in item.ItemInfo) {
                    y += 24;
                    spriteBatch.DrawString(Font12, s, new Vector2(x, y), Color.White);
                }
            }
            return y + 24;
        }
        private int DrawItemHeader(SpriteBatch spriteBatch, Item item, int x, int y, bool mousePointer) {
            int iconX = x + (int)Font14.MeasureString(item.Name).X + 16;
            if (mousePointer) {
                spriteBatch.Draw(MousePointer, new Vector2(x - 4, y - 4), Color.White);
                spriteBatch.DrawString(Font14, item.Name, new Vector2(x + 16, y), Color.White);
                iconX += 16;
            } else {
                spriteBatch.DrawString(Font14, item.Name, new Vector2(x, y), Color.White);
            }
            spriteBatch.Draw(item.Glyph, new Vector2(iconX, y - 8), item.Color);
            return y + 32;
        }

        public void DrawTileHighlight(SpriteBatch spriteBatch, MouseHandler mouse, WorldView world, Color color) {
            Point p = mouse.GetViewTile(world);
            if (p.X == -1 || !world.HasSeen[p.X + world.OffsetX, p.Y + world.OffsetY]) { return; }

            spriteBatch.Draw(TileHighlight, new Vector2(p.X * 32, p.Y * 32), color);
        }

        public void DrawLineToCreature(SpriteBatch spriteBatch, MouseHandler mouse, WorldView world, Creature start, Creature end, Color color) {
            Point p = mouse.GetViewTile(world);
            Point tile = new Point(p.X + world.OffsetX, p.Y + world.OffsetY);
            if (p.X == -1 || !world.HasSeen[tile.X, tile.Y]) { return; }

            List<Point> line = start.GetLineToPoint(new Point(end.X, end.Y));
            foreach (Point point in line) {
                spriteBatch.Draw(TileHighlight, new Vector2((point.X - world.OffsetX) * 32, (point.Y - world.OffsetY) * 32), color);
            }
        }

        public void DrawMessages(SpriteBatch spriteBatch) {
            if (Messages == null || Messages.Count == 0) { return; }

            int lineHeight = 18;
            int y = Constants.ScreenHeight - Messages.Count * lineHeight;
            foreach (string m in Messages) {
                spriteBatch.DrawString(Font10, m, new Vector2(StartX + 32, y), Color.LightGray);
                y += lineHeight;
            }
        }

        public void UpdateMessages(List<string> messages) {
            Messages = new List<string>();
            foreach (string m in messages) {
                if (m.Length > MessageLineLength) {
                    Messages.AddRange(SplitMessage(m, MessageLineLength));
                } else {
                    Messages.Add(m);
                }
            }
        }

        public static List<string> SplitMessage(string message, int maxChars) {
            List<string> output = new List<string>();
            string current = "";
            foreach (string s in message.Split(' ')) {
                if (current.Length + s.Length > maxChars) {
                    output.Add(current);
                    current = s + " ";
                } else {
                    current += s + " ";
                }
            }
            if (current.Length > 0) {
                output.Add(current);
            }
            return output;
        }
    }
}
