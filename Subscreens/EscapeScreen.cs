using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {
    public class EscapeScreen : BorderedScreen {
        private Main Main;
        private ContentManager Content;
        private int Index;
        public EscapeScreen(Main main, ContentManager content) : base(content) { 
            Main = main;
            Content = content;
            Index = 0;
        }

        public override Subscreen RespondToInput(Keys key, MouseHandler mouse) {
            switch(key) {
                case Keys.Escape: base.CloseSubscreen(); return null;
                case Keys.Enter:
                    if (Index == 0) { base.CloseSubscreen(); return null; }
                    else if (Index == 1) { return new WindowResizeScreen(Content, Main, this); }
                    else if (Index == 2) { Main.Exit(); }
                    break;

                // With only two options, it really only just toggles the index
                case Keys.Up: if (Index > 0) { Index--; } break;
                case Keys.Down: if (Index < 2) { Index++; } break;
            }
            return this;
        }
        
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, MouseHandler mouseHandler) {
            base.Draw(gameTime, spriteBatch, mouseHandler);

            Vector2 v = new Vector2(Constants.ScreenWidth / 2, 128);
            WriteCentered(spriteBatch, Font24, "Paused", v, Color.White);
            v.Y += 64;
            WriteCentered(spriteBatch, Font16, "Continue", v, Index == 0 ? Color.LawnGreen : Color.White);
            v.Y += 32;
            WriteCentered(spriteBatch, Font16, "Settings", v, Index == 1 ? Color.LawnGreen : Color.White);
            v.Y += 32;
            WriteCentered(spriteBatch, Font16, "Quit", v, Index == 2 ? Color.LawnGreen : Color.White);

            v.Y = Constants.ScreenHeight - 64;
            WriteCentered(spriteBatch, Font14, $"Seed: {Main.Seed}", v, Color.Gray);
        }
    }
}
