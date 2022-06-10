using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {
    public class StartScreen : BorderedScreen {
        public StartScreen(ContentManager content) : base(content) {
            Font24 = content.Load<SpriteFont>("Interface/sds24");
            Font14 = content.Load<SpriteFont>("Interface/sds14");
        }

        public override Subscreen RespondToInput(Keys key, MouseHandler mouse) {
            if (key == Keys.Enter) { return null; }
            return this;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, MouseHandler mouseHandler) {
            base.Draw(gameTime, spriteBatch, mouseHandler);

            int x = Constants.ScreenWidth / 2;
            int y = Constants.ScreenHeight / 2 - 64;
            WriteCentered(spriteBatch, Font24, "MonoRogue", new Vector2(x, y), Color.White);
            y += 48;
            WriteCentered(spriteBatch, Font14, "Press Enter to start", new Vector2(x, y), Color.White);
        }
    }
}
