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
        private SpriteFont Font14;
        private SpriteFont Font12;

        // Cache messages so we aren't formatting all of the strings hundreds of times per second for no reason
        private List<string> Messages;

        public void LoadTextures(ContentManager content) {
            InterfaceLine = content.Load<Texture2D>("Interface/InterfaceDivider");

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
            spriteBatch.DrawString(Font14, $"HP: {creature.HP}/{creature.MaxHP}", new Vector2(StartX + 32, y), Color.White);
            y += 24;
            spriteBatch.DrawString(Font14, $"Damage: {creature.Damage}", new Vector2(StartX + 32, y), Color.White);
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
