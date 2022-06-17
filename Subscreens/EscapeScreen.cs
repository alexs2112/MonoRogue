using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoRogue {
    public class EscapeScreen : BorderedScreen {
        private Main Main;
        private int Index;
        public EscapeScreen(Main main, ContentManager content) : base(content) { 
            Main = main;
            Index = 0;
        }

        public override Subscreen RespondToInput(Keys key, MouseHandler mouse) {
            switch(key) {
                case Keys.Enter:
                    if (Index == 0) { return null; }
                    else if (Index == 1) { Main.Exit(); }
                    break;

                // With only two options, it really only just toggles the index
                case Keys.Up:
                case Keys.Down:
                    if (Index == 0) { Index = 1; }
                    else if (Index == 1) { Index = 0; }
                    break;                
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
            WriteCentered(spriteBatch, Font16, "Quit", v, Index == 1 ? Color.LawnGreen : Color.White);

            v.Y = Constants.ScreenHeight - 64;
            WriteCentered(spriteBatch, Font14, $"Seed: {Main.seed}", v, Color.Gray);
        }
    }
}
