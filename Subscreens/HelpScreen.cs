using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {
    public class HelpScreen : BorderedScreen {
        private List<string> Text;
        private SpriteFont Font12;

        public HelpScreen(ContentManager content) : base(content) {
            Font12 = content.Load<SpriteFont>("Interface/sds12");
            string[] text = new string[] {
                "Controls:",
                " - Arrow keys, Left Click, Numpad, or Vi Keys to move and attack",
                " - Spacebar or click yourself to interact with items on the ground",
                " - Right click creatures or items to see their stats",
                " - [s] to see your own stats",
                " - [r] to rest and repair armor",
                " - [.] or [5] to wait one turn",
                " - [esc] to quit the game or exit subscreens",
                " - [/] or [?] to show this menu"
            };

            Text = SplitText(text, MaxScreenChars);
        }

        private List<string> SplitText(string[] text, int maxChars) {
            List<string> output = new List<string>();
            foreach (string line in text) {
                output.AddRange(MainInterface.SplitMessage(line.Split(' '), maxChars));
            }
            return output;
        }

        public override Subscreen RespondToInput(Keys key, MouseHandler mouse) {
            if (key == Keys.Escape || mouse.RightClicked()) { return null; }
            return this;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, MouseHandler mouseHandler) {
            base.Draw(gameTime, spriteBatch, mouseHandler);

            int x = 32;
            int y = 32;
            foreach (string s in Text) {
                spriteBatch.DrawString(Font14, s, new Vector2(x,y), Color.White);
                y += 32;
            }
        }
    }
}
