using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {
    public class HelpScreen : BorderedScreen {
        private List<string> Text;

        public HelpScreen(ContentManager content) : base(content) {
            string[] text = new string[] {
                "Controls:",
                " - Arrow keys, Left Click, Numpad, or Vi Keys to move and attack",
                " - Spacebar or click yourself to interact with items on the ground",
                " - Right click creatures or items to see their stats",
                " - [s] to see your own stats",
                " - [r] to rest and repair armor",
                " - [.] or [5] to wait one turn",
                " - [f] to fire a ranged weapon",
                " - [m] to view your world map",
                " - [esc] to quit the game or exit menus",
                " - [/] or [?] to show this menu"
            };

            Text = SplitText(text, MaxScreenChars);
        }

        private List<string> SplitText(string[] text, int maxChars) {
            List<string> output = new List<string>();
            foreach (string line in text) {
                output.AddRange(MainInterface.SplitMessage(line, maxChars));
            }
            return output;
        }

        public override Subscreen RespondToInput(Keys key, MouseHandler mouse) {
            return base.RespondToInput(key, mouse);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, MouseHandler mouseHandler) {
            base.Draw(gameTime, spriteBatch, mouseHandler);

            int x = 32;
            int y = 24;
            foreach (string s in Text) {
                spriteBatch.DrawString(Font14, s, new Vector2(x,y), Color.White);
                y += 32;
            }
        }
    }
}
