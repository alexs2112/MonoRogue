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

        private static Texture2D InterfaceLine;
        private static Texture2D HeartsFull;
        private static Texture2D TileHighlight;
        private static SpriteFont Font14;
        private static SpriteFont Font12;

        // Cache messages so we aren't formatting all of the strings hundreds of times per second for no reason
        private List<string> Messages;

        public static void LoadTextures(ContentManager content) {
            InterfaceLine = content.Load<Texture2D>("Interface/InterfaceDivider");
            HeartsFull = content.Load<Texture2D>("Interface/Hearts");
            TileHighlight = content.Load<Texture2D>("Interface/TileHighlight");

            Font14 = content.Load<SpriteFont>("Interface/sds14");
            Font12 = content.Load<SpriteFont>("Interface/sds12");
        }


        public void DrawInterface(SpriteBatch spriteBatch) {
            spriteBatch.Draw(InterfaceLine, new Vector2(StartX, 0), Color.Gray);
        }

        public void DrawCreatureStats(SpriteBatch spriteBatch, Creature creature) {
            int x = StartX + 32;
            int y = 8;
            spriteBatch.DrawString(Font14, creature.Name, new Vector2(x, y), Color.White);
            y += 24;
            DrawHearts(spriteBatch, creature.MaxHP, creature.HP, x, y, Color.Red);
            if (creature.Armor == null) {
                y += 32;
                spriteBatch.DrawString(Font14, "No armor", new Vector2(x, y), Color.Gray);
            } else {
                y += 32;
                DrawHearts(spriteBatch, creature.Armor.MaxDefense, creature.Armor.Defense, x, y, Color.LightSkyBlue);
            }

            y += 40;
            (int Min, int Max) damage = creature.GetDamage();
            spriteBatch.DrawString(Font14, $"Damage: {damage.Min}-{damage.Max}", new Vector2(x, y), Color.White);
            y += 32;

            string s;
            Color c;
            if (creature.Weapon == null) { s = "None"; c = Color.Gray; }
            else { s = creature.Weapon.Name; c = Color.White; }
            spriteBatch.DrawString(Font14, $"Weapon: {s}", new Vector2(x, y), c);
        }

        private static Rectangle HeartEmpty = new Rectangle(0, 0, 32, 32);
        private static Rectangle HeartQuarter = new Rectangle(32, 0, 32, 32);
        private static Rectangle HeartHalf = new Rectangle(64, 0, 32, 32);
        private static Rectangle HeartThree = new Rectangle(96, 0, 32, 32);
        private static Rectangle HeartFull = new Rectangle(128, 0, 32, 32);
        public static void DrawHearts(SpriteBatch spriteBatch, int max, int health, int x, int y, Color color) {
            // Each heart counts as 4 HP
            int fullHearts = health / 4;
            int partialHearts = health % 4;
            int emptyHearts = (max + 3) / 4 - fullHearts;

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
                if (partialHearts == 1) {
                    partialSource = HeartQuarter;
                } else if (partialHearts == 2) {
                    partialSource = HeartHalf;
                } else {
                    partialSource = HeartThree;
                }
                spriteBatch.Draw(HeartsFull, partialVector, partialSource, color);
                x += 32;
            }

            // Then draw the empty hearts to show capacity
            for (int i = 0; i < emptyHearts; i++) {
                spriteBatch.Draw(HeartsFull, new Vector2(i * 32 + x, y), HeartEmpty, Color.DarkGray);
            }
        }

        public void DrawItemInfo(SpriteBatch spriteBatch, Item item) {
            int y = 8;
            int x = StartX + 32;
            spriteBatch.DrawString(Font14, item.Name, new Vector2(x, y), Color.White);
            y += 32;

            if (item.IsFood) {
                Food food = (Food)item;
                spriteBatch.DrawString(Font14, $"Food: {food.Value}", new Vector2(x, y), Color.White);
            } else if (item.IsArmor) {
                Armor armor = (Armor)item;
                spriteBatch.DrawString(Font14, $"Defense: {armor.Defense}/{armor.MaxDefense}", new Vector2(x, y), Color.White);
                if (armor.MovementPenalty != 0) {
                    y += 32;
                    spriteBatch.DrawString(Font14, $"Delay: {armor.MovementPenalty}", new Vector2(x, y), Color.White);
                }
            } else if (item.IsWeapon) {
                Weapon weapon = (Weapon)item;
                spriteBatch.DrawString(Font14, $"Damage: {weapon.Damage.Min}-{weapon.Damage.Max}", new Vector2(x, y), Color.White);
                if (weapon.AttackDelay != 0) {
                    y += 32;
                    spriteBatch.DrawString(Font14, $"Delay: {weapon.AttackDelay}", new Vector2(x, y), Color.White);
                }
            }
        }

        public void DrawTileHighlight(SpriteBatch spriteBatch, MouseHandler mouse, WorldView world) {
            Point p = mouse.GetViewTile(world);
            if (p.X == -1 || !world.HasSeen[p.X + world.OffsetX, p.Y + world.OffsetY]) { return; }

            spriteBatch.Draw(TileHighlight, new Vector2(p.X * 32, p.Y * 32), Color.White);
        }

        public void DrawMessages(SpriteBatch spriteBatch) {
            if (Messages == null || Messages.Count == 0) { return; }

            int lineHeight = 18;
            int y = Constants.ScreenHeight - Messages.Count * lineHeight;
            foreach (string m in Messages) {
                spriteBatch.DrawString(Font12, m, new Vector2(StartX + 32, y), Color.LightGray);
                y += lineHeight;
            }
        }

        public void UpdateMessages(List<string> messages) {
            Messages = new List<string>();
            foreach (string m in messages) {
                if (m.Length > MessageLineLength) {
                    string[] words = m.Split(' ');
                    Messages.AddRange(SplitMessage(words, MessageLineLength));
                } else {
                    Messages.Add(m);
                }
            }
        }

        public static List<string> SplitMessage(string[] words, int maxChars) {
            List<string> output = new List<string>();
            string current = "";
            foreach (string s in words) {
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
