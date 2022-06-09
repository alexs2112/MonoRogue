using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {
    public class MainInterface {
        // Storing the methods to draw the interface on the right side of the main screen
        
        // Where the interface starts from on the screen
        private static int StartX = 32 * Constants.WorldViewWidth;

        // How many characters fit per message line
        private static int MessageLineLength = 20;

        private Texture2D InterfaceLine;
        private Texture2D HeartsFull;
        private Texture2D TileHighlight;
        private SpriteFont Font14;
        private SpriteFont Font12;

        // Cache messages so we aren't formatting all of the strings hundreds of times per second for no reason
        private List<string> Messages;

        public void LoadTextures(ContentManager content) {
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
            int y = 8;
            spriteBatch.DrawString(Font14, creature.Name, new Vector2(StartX + 32, y), Color.White);
            y += 24;
            DrawCreatureHealth(spriteBatch, creature, StartX + 32, y);
            y += 40;
            (int Min, int Max) damage = creature.GetDamage();
            spriteBatch.DrawString(Font14, $"Damage: {damage.Min}-{damage.Max}", new Vector2(StartX + 32, y), Color.White);
            y += 32;
            string s;
            Color c;
            if (creature.Armor == null) { s = "None"; c = Color.Gray; }
            else { s = creature.Armor.Name; c = Color.White; }
            spriteBatch.DrawString(Font14, $"Armor: {s}", new Vector2(StartX + 32, y), c);
            
            y += 32;
            if (creature.Weapon == null) { s = "None"; c = Color.Gray; }
            else { s = creature.Weapon.Name; c = Color.White; }
            spriteBatch.DrawString(Font14, $"Weapon: {s}", new Vector2(StartX + 32, y), c);
        }

        private static Rectangle HeartEmpty = new Rectangle(0, 0, 32, 32);
        private static Rectangle HeartQuarter = new Rectangle(32, 0, 32, 32);
        private static Rectangle HeartHalf = new Rectangle(64, 0, 32, 32);
        private static Rectangle HeartThree = new Rectangle(96, 0, 32, 32);
        private static Rectangle HeartFull = new Rectangle(128, 0, 32, 32);
        private void DrawCreatureHealth(SpriteBatch spriteBatch, Creature creature, int x, int y) {
            // Each heart counts as 4 HP
            int fullHearts = creature.HP / 4;
            int partialHearts = creature.HP % 4;
            int emptyHearts = (creature.MaxHP + 3) / 4 - fullHearts;

            for (int i = 0; i < fullHearts; i++) {
                spriteBatch.Draw(HeartsFull, new Vector2(i * 32 + x, y), HeartFull, Color.Red);
            }
            x += fullHearts * 32;
            
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
                spriteBatch.Draw(HeartsFull, partialVector, partialSource, Color.Red);
                x += 32;
            }

            for (int i = 0; i < emptyHearts; i++) {
                spriteBatch.Draw(HeartsFull, new Vector2(i * 32 + x, y), HeartEmpty, Color.DarkGray);
            }
        }

        public void DrawItemInfo(SpriteBatch spriteBatch, Item item) {
            int y = 8;
            spriteBatch.DrawString(Font14, item.Name, new Vector2(StartX + 32, y), Color.White);
            y += 32;

            if (item.IsFood) {
                Food food = (Food)item;
                spriteBatch.DrawString(Font14, $"Food: {food.Value}", new Vector2(StartX + 32, y), Color.White);
            }
        }

        public void DrawTileHighlight(SpriteBatch spriteBatch, MouseHandler mouse, WorldView world) {
            Point p = mouse.GetViewTile(world);
            if (p.X == -1) { return; }

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
                    Messages.AddRange(SplitMessage(words));
                } else {
                    Messages.Add(m);
                }
            }
        }

        private static List<string> SplitMessage(string[] words) {
            List<string> output = new List<string>();
            string current = "";
            foreach (string s in words) {
                if (current.Length + s.Length > MessageLineLength) {
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
