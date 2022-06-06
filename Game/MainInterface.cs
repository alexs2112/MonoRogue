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
        private SpriteFont Font14;
        private SpriteFont Font12;

        // Cache messages so we aren't formatting all of the strings hundreds of times per second for no reason
        private List<string> Messages;

        public void LoadTextures(ContentManager content) {
            InterfaceLine = content.Load<Texture2D>("Interface/InterfaceDivider");
            HeartsFull = content.Load<Texture2D>("Interface/Hearts");

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
            spriteBatch.DrawString(Font14, $"Damage: {creature.Damage}", new Vector2(StartX + 32, y), Color.White);
        }

        private static Rectangle HeartQuarter = new Rectangle(0, 0, 32, 32);
        private static Rectangle HeartHalf = new Rectangle(32, 0, 32, 32);
        private static Rectangle HeartThree = new Rectangle(64, 0, 32, 32);
        private static Rectangle HeartFull = new Rectangle(96, 0, 32, 32);
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
                spriteBatch.Draw(HeartsFull, new Vector2(i * 32 + x, y), HeartQuarter, Color.DarkGray);
            }
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
